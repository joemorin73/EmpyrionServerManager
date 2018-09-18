using EmpyrionModApi;
using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactionStorageConverterMod
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerConfigItem
    {
        public Int16 ItemID;
        public string Name;

        public ServerConfigItem()
        {
            ItemID = -1;
            Name = "";
        }

        public ServerConfigItem(Int16 _itemID, string _name)
        {
            ItemID = _itemID;
            Name = _name;
        }
    }



    [System.ComponentModel.Composition.Export(typeof(IGameMod))]
    public class FactionStorageConverterMod : IGameMod
    {
        static readonly string k_versionString = EmpyrionModApi.Helpers.GetVersionString(typeof(FactionStorageConverterMod));

        static readonly string k_saveStateFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "FactionStorageConverterMod_SaveState.yaml";

        private TraceSource _traceSource = new TraceSource("FactionStorageConverterMod");
        private IGameServerConnection _gameServerConnection;
        Dictionary<int, ServerConfigItem> ServerConfigItems = new Dictionary<int, ServerConfigItem>();
        private Configuration _config;
        private SaveState _saveState;
        private HashSet<int> _factionStorageScreensOpen = new HashSet<int>(); // key is the faction id
        private string _fileStoragePath;


        public void Start(IGameServerConnection gameServerConnection)
        {
            _traceSource.TraceInformation("Starting up...");
            var configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "FactionStorageConverterMod_Settings.yaml";
            _fileStoragePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\SavedData";

            _gameServerConnection = gameServerConnection;
            _config = BaseConfiguration.GetConfiguration<Configuration>(configFilePath);

            //--- Load Server Config Items
            LoadServerConfigItems();

            _saveState = SaveState.Load(k_saveStateFilePath);

            _gameServerConnection.AddVersionString(k_versionString);
            _gameServerConnection.Event_ChatMessage += OnEvent_ChatMessage;
        }

        public void Stop()
        {
            lock (_saveState)
            {
                _saveState.Save(k_saveStateFilePath);
            }

            _traceSource.TraceInformation("Stopping...");
            _traceSource.Flush();
            _traceSource.Close();
        }

        private void OnEvent_ChatMessage(ChatType chatType, string msg, Player player)
        {
            if (msg == _config.FactionStorageConverterCommand)
            {
                if (player.SteamId.Equals(_config.AdminSteamID))
                {
                    Console.WriteLine("FSS activated");

                    var task = (
                    OnUseFactionStorage(player)
                    );
                }
                else
                {
                    Console.WriteLine("*** IMPOSTER !!");
                }
            }
        }

        private async Task OnUseFactionStorage(Player player)
        {
            await _gameServerConnection.RefreshFactionList();
            await player.RefreshInfo();

            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(_fileStoragePath,"FactionArchive"));            

            foreach (KeyValuePair<int,ItemStacks> storage in _saveState.FactionIdToItemStacks)
            {
                var items = storage.Value.ToArray();
                var faction = _gameServerConnection.GetFaction(storage.Key);
                if (faction != null)
                {
                    //Eleon.Modding.FactionInfo factionInfo = new Eleon.Modding.FactionInfo(0, 111, "name", "AAA");
                    string factionArchiveFile = System.IO.Path.Combine(_fileStoragePath, "FactionArchive", $"{storage.Key}.txt");
                    using (var writer = System.IO.File.CreateText(factionArchiveFile))
                    {
                        int slot = 0;
                        await writer.WriteLineAsync($"FactionID ({storage.Key})...FactionInit ({faction.Initials})...FactionName ({faction.Name})");
                        await writer.WriteLineAsync($"Slot,ItemID,Count,Ammo,Decay,ItemName");
                        foreach (var anItem in items)
                        {
                            var itemName = "Unknown";
                            if (ServerConfigItems.ContainsKey(anItem.Id))
                            {
                                itemName = ServerConfigItems[anItem.Id].Name;
                            }
                            await writer.WriteLineAsync($"{slot},{anItem.Id},{anItem.Amount},{0},{0},{itemName}");
                            slot++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("*** Cannot locate Info for Faction: {0}", storage.Key);
                }
            }
            Console.WriteLine("Faction Storage Conversion complete");
        }


        /// <summary>
        /// from Config.ecf, load all items and blocks.
        /// used for adding descriptive names to the faction bank save file
        /// </summary>
        private void LoadServerConfigItems()
        {
            int counter = 0;
            Console.WriteLine("loading server config items: {_config.DedicatedServerPath}");
            using (var reader = System.IO.File.OpenText(System.IO.Path.Combine(_config.DedicatedServerPath, $"Configuration\\Config.ecf")))
            {
                var configLine = "";
                while ((configLine = reader.ReadLine()) != null)
                {
                    counter++;
                    //if (counter < 100) Console.WriteLine(String.Format("[{0}]", configLine));
                    if (configLine.Contains("{ Item Id:") || configLine.Contains("{ Block Id:"))
                    {
                        //{ Block Id: 257, Name: CockpitMS01
                        string[] configLineSplit = configLine.Split(' ');
                        var item = new ServerConfigItem(Convert.ToInt16(configLineSplit[3].TrimEnd(',')), configLineSplit[5]);
                        ServerConfigItems.Add(item.ItemID, item);
                    }
                }
                //Console.WriteLine(String.Format("Config Items Loaded:{0}", ServerConfigItems.Count));
            }

        }


    }
}
