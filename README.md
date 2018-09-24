# Introduction
This is a Server Manager for the Empyrion Dedicated Server.  It includes the original code for NCMR by Michael Ghoulding from this repository (https://github.com/MichaelGoulding/EmpyrionNetworkConnectedMods/blob/master/README.md).  We have added a GUI on top of the existing code and expanded the scope of his project to include Managing the Server.  Please do keep in mind that this is one of only 3 C# projects I have been involved with. Im just asking that you all be patient while I get used to all of this :)

# Major Contributors

1.  Plleg - Code involving the Empyrion API, GUI Interaction, NCMR Improvements.
2.  Chilimeat - Initial Interface Design, File system stuff, Some of the code behind the GUI
3.  Mortlath - NCMR Code base
4.  Avreon - NCMR Code Base

# Getting Started

## Build
1.  Clone the code (recursively to include submodules).  Don't just download a .zip of the code.
2.  Load up in Visual Studio 2017
3.  Build all.
4.  Correct any errors that may occur as file locations may differ.

 ## Run
After building, the \empyrionServerManager\bin\Release (or Debug) folder will contain the .exe which will load an run any *Mod.dll found in the Extensions folder (or 1 folder deeper).
Make sure you configure the Settings.yaml file to point to the API port used by EAH (Should be correct by default)

# Contribute
Send bugs or send pull requests with any improvements, modules, or documentation.

# Included Extensions
| Module | Description |
|:-----------|:-----------|
| FactionStorageMod | Lets players in the same faction access a shared storage area. |
| NoKillZonesMod | Lets admins jail and release players from a configured jail location.  Also puts players in jail who kill other players in "protected" areas. |
| PlayfieldStructureRegenMod | Regenerates POIs or asteriods when a playfield is loaded. |
| ServerPlaytimeRewardsMod | Gives a configured amount of XP points to every player every configured amount of minutes while they are logged in. |
| ShipBuyingMod | Lets players buy ships for credits. |
| StarterShipMod | Grants each player a one-time ship. |
| StructureOwnershipMod | It gives periodic rewards to any faction that captures the core of configured ships/buildings. You then use /income to take out the items you've earned. The idea is that it gives purpose in PVP to take over bases in space/planet. It also reduce the need to use autominers as you could capture a "steel block" factory or something. |
| VotingRewardMod | Calls empyrion-servers.com REST API to give configured rewards to players every day they vote. |
| SellToServer | (Deprecated as it's a part of game now) Lets you configure an area where you can type /sell and sell items back to the server for credits.  Prices are configured in yaml (including a default price if you want to accept any item). |
| DiscordBotMod | (Deprecated as it's a part of EAH now) Connects in-game general chat to a specific channel in your Discord server.  Two-way communication. |
| FactionPlayfieldKickerMod| (Deprecated as it's a part of EAH now) Keeps people not belonging to a specifc faction out of a playfield if they try to warp in. |
