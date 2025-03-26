﻿using System;
using System.ComponentModel;
using System.Reflection;
using EliteTestingMod.Helpers;
using EliteTestingMod.UI;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace EliteTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]
        [DefaultValue("EliteTestingMod")]
        public string ModToReload = "EliteTestingMod";

        [DefaultValue(false)]
        public bool SaveWorldOnReload = false;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        [Header("UI")]

        [OptionStrings(["left", "bottom"])]
        [DefaultValue("bottom")]
        public string ButtonsPosition;

        [Range(50f, 80f)]
        [Increment(5f)]
        [DefaultValue(70)]
        public float ButtonSize = 70;

        [Range(0.7f, 1.1f)]
        [Increment(0.1f)]
        [DefaultValue(0.9f)]
        public float ButtonTextSize = 0.9f;

        [Range(300, 700f)]
        [Increment(50f)]
        [DefaultValue(550f)]
        public float PanelWidth = 400f;

        [Range(300, 700f)]
        [Increment(50f)]
        [DefaultValue(500f)]
        public float PanelHeight = 600;

        [DefaultValue(false)]
        public bool DraggablePanels = false;

        [Header("Game")]
        [DefaultValue(false)]
        public bool EnterWorldSuperMode = false;

        [DefaultValue(true)]
        public bool KeepRunning = true;

        [DefaultValue(true)]
        public bool AlwaysShowTextOnTop = true;

        [Header("Logging")]
        [DefaultValue(true)]
        public bool LogToLogFile = true;

        [DefaultValue(true)]
        public bool LogToChat = true;

        [Header("NPCSpawner")]

        [DefaultValue(null)]
        public NPCSpawnerConfig NPCSpawner = new();

        public override void OnChanged()
        {
            base.OnChanged();

            // Get mainstate
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null)
            {
                Log.Info("MainSystem is null in Config.OnChanged()");
                return;
            }
            MainState mainState = sys.mainState;

            // Delete all buttons and re-add them
            mainState.AreButtonsShowing = true;
            mainState.AllButtons.Clear();
            mainState.RemoveAllChildren();
            mainState.AddEverything();

            // expand so we can see the changes
            mainState.collapse.SetCollapsed(false);

            Log.Info("Config.OnChanged() ran successfully3333");
        }
    }

    public class NPCSpawnerConfig
    {
        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 SpawnOffset = new Vector2(0, 0);
    }

    internal static class Conf
    {
        // NOTE: Stolen from CalamityMod
        // https://github.com/CalamityTeam/CalamityModPublic/blob/e0838b30b8fdf86aeb4037931c8123703acd7c7e/CalamityMod.cs#L550
        #region Force ModConfig save (Reflection)
        internal static void ForceSaveConfig(Config cfg)
        {
            // There is no current way to manually save a mod configuration file in tModLoader.
            // The method which saves mod config files is private in ConfigManager, so reflection is used to invoke it.
            try
            {
                MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                if (saveMethodInfo is not null)
                    saveMethodInfo.Invoke(null, [cfg]);
            }
            catch
            {
                Log.Error("An error occurred while manually saving ModConfig!.");
            }
        }
        #endregion

        // Instance
        public static Config C => ModContent.GetInstance<Config>();

        // Reload header
        public static string ModToReload => C.ModToReload;
        public static bool SaveWorldOnReload => C.SaveWorldOnReload;
        public static bool ClearClientLogOnReload => C.ClearClientLogOnReload;

        // UI
        public static float TextSize => C.ButtonTextSize;
        public static float ButtonSize => C.ButtonSize;
        public static string ButtonsPosition => C.ButtonsPosition;
        public static bool DraggablePanels => C.DraggablePanels;
        public static float PanelWidth => C.PanelWidth;
        public static float PanelHeight => C.PanelHeight;

        // Game
        public static Vector2 NPCSpawnLocation => C.NPCSpawner.SpawnOffset;
        public static bool EnterWorldSuperMode => C.EnterWorldSuperMode;

        // Keep Game Running
        public static bool KeepRunning => C.KeepRunning;
        public static bool AlwaysShowTextOnTop => C.AlwaysShowTextOnTop;

        // Logging
        public static bool LogToLogFile => C.LogToLogFile;
        public static bool LogToChat => C.LogToChat;
    }
}
