using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using ModHelper.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ModHelper.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]

        [DefaultValue(true)]
        public bool Reload = true;

        [DefaultValue("")]
        public string LatestModToReload = "";

        [DefaultValue(true)]
        public bool SaveWorldOnReload = true;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        [Header("Game")]

        [DefaultValue(false)]
        public bool EnterWorldSuperMode = false;

        [DefaultValue(true)]
        public bool GameKeepRunning = true;

        [DefaultValue(true)]
        public bool ShowGameKeepRunningText = true;

        [DefaultValue(true)]
        public bool ShowDebugText = true;

        [DefaultValue(true)]
        public bool SuperModeGlow = true;

        [Header("Logging")]
        [DefaultValue(true)]
        public bool LogToLogFile = true;

        [DefaultValue(true)]
        public bool LogToChat = true;

        [Header("MainMenu")]
        [DefaultValue(true)]
        public bool CreateMainMenuButtons = true;

        [Header("NPCSpawner")]

        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 SpawnOffset = new Vector2(0, 0);
    }

    internal static class Conf
    {
        // Reference:
        // https://github.com/CalamityTeam/CalamityModPublic/blob/1.4.4/CalamityMod.cs#L550
        internal static void Save()
        {
            // There is no current way to manually save a mod configuration file in tModLoader.
            // The method which saves mod config files is private in ConfigManager, so reflection is used to invoke it.
            try
            {
                MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                if (saveMethodInfo is not null)
                    saveMethodInfo.Invoke(null, [C]);
            }
            catch
            {
                Log.Error("An error occurred while manually saving ModConfig!.");
            }
        }

        // Instance (singleton) of the Config class
        // This is used to access the configuration values from other parts of the mod.
        // Example: Conf.C.Reload = false;
        public static Config C => ModContent.GetInstance<Config>();
    }
}
