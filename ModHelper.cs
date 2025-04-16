using System.IO;
using Microsoft.Build.Locator;
using ModHelper.PacketHandlers;
using Terraria.ModLoader;

namespace ModHelper
{
    // If no Autoload(Side) is provided, it will default to Both (which is wanted in this case)
    // [Autoload(Side = ModSide.Client)]
    // [Autoload(Side = ModSide.Both)]
    public class ModHelper : Mod
    {
        public static ModHelper Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            MSBuildLocator.RegisterDefaults();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}