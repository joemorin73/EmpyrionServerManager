using Eleon.Modding;
using EmpyrionModApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace EmpyrionNCMRModHost
{
    public class ModHost : ModInterface
    {
        public CompositionContainer _container { get; private set; }
        public GameServerConnection _gameServerConnection { get; private set; }
        public ModGameAPI GameAPI { get; private set; }

#pragma warning disable 0649
        [ImportMany]
        IEnumerable<IGameMod> _gameMods;
#pragma warning restore 0649

        public void Game_Event(CmdId eventId, ushort seqNr, object data)
        {
        }

        public void Game_Exit()
        {
        }

        public void Game_Start(ModGameAPI dediAPI)
        {
            GameAPI = dediAPI;

            var configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "Settings.yaml";
            var config = Configuration.GetConfiguration<Configuration>(configFilePath);

            var modPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Extensions";

            using (var catalog = new AggregateCatalog(
                new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()),
                new DirectoryCatalog(modPath, "*Mod.dll")))
            {
                // iterate over all directories in .\Plugins dir and add all Plugin* dirs to catalogs
                foreach (var path in System.IO.Directory.EnumerateDirectories(modPath, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    catalog.Catalogs.Add(new DirectoryCatalog(path, "*Mod.dll"));
                }

                _container = new CompositionContainer(catalog);

                try
                {
                    this._container.ComposeParts(this);

                    //using (_gameServerConnection = new GameServerConnection(config))
                    //{
                    _gameServerConnection = new GameServerConnection(config);

                    foreach (var gameMod in _gameMods)
                    {
                        gameMod.Start(_gameServerConnection);
                    }

                    _gameServerConnection.Connect();

                    //                        // wait until the user presses Enter.
                    //                        string input = Console.ReadLine();
                    //
                    //                        foreach (var gameMod in _gameMods)
                    //                        {
                    //                            gameMod.Stop();
                    //                        }

                    //                    }
                }
                catch (CompositionException compositionException)
                {
                    GameAPI.Console_Write($"NCMR ModHost: {compositionException}");
                }
            }

        }

        public void Game_Update()
        {
        }
    }
}
