using Terraria.ModLoader;
using Terraria;
using System.Diagnostics;
using System;

namespace SquidTestingMod.Commands
{
    public class JoinMPCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "mp";
        public override string Description => "Join multiplayer server";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            WorldGen.SaveAndQuit(() =>
            {
                try
                {
                    ProcessStartInfo a = new(@"C:\Program Files (x86)\Steam\steamapps\common\tModLoader\_START_SERVER") { UseShellExecute = true };
                    Process.Start(a);
                }
                catch (Exception e)
                {
                    caller.Reply("Failed to start server launcher: " + e.Message);
                }

                // Main.menuMode = 10; // go to main menu
                // Netplay.SetRemoteIP("localhost");
                // Netplay.ListenPort = 7777;
                // Netplay.StartTcpClient(); // enter the server as a client
            });
        }
    }
}