using DragonLens.Core.Systems.ToolSystem;
using Microsoft.CodeAnalysis;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using static Terraria.ModLoader.Exceptions.LevenshteinDistance;
using System.Diagnostics;
using System;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReload : Tool
    {
        public override string IconKey => "Reload";

        public override string DisplayName => "Reload";

        public override string Description => Conf.C.AddBloat ? $"Initiates a targeted runtime reinitialization sequence for the following specified mod assemblies: {string.Join(", ", Conf.C.ModsToReload)}.\nNote: Invoking this action via a right-click gesture will bypass the standard recompilation step, thereby triggering a reload process that omits the build phase and utilizes the most recently cached binary outputs." : $"Reloads {string.Join(", ", Conf.C.ModsToReload)}\n" +
            $"Right click will reload mods without building any";

        public override bool HasRightClick => true;
        public override async void OnActivate()
        {
            await ReloadUtilities.SinglePlayerReload();
        }

        public override async void OnRightClick()
        {
            ReloadUtilities.forceJustReload = true;
            await ReloadUtilities.SinglePlayerReload();
        }
    }
}
