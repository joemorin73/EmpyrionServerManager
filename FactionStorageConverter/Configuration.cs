using EmpyrionModApi;
using System.Collections.Generic;

namespace FactionStorageConverterMod
{
    public class Configuration
    {
        public string FactionStorageConverterCommand { get; set; }
        public string DedicatedServerPath { get; set; }
        public string AdminSteamID { get; set; }

        public Configuration()
        {
            FactionStorageConverterCommand = "/fss";
            DedicatedServerPath = $"D:\\Program Files (x86)\\Steam\\steamapps\\common\\Empyrion - Dedicated Server\\Content\\";
            AdminSteamID = "76561198024931077";
        }
    }
}
