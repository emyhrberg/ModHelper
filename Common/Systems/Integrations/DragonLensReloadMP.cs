using DragonLens.Core.Systems.ToolSystem;
using ModReloader.Common.Configs;
using ModReloader.Helpers;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReloadMP : Tool
    {
        public override string IconKey => "ReloadMP";

        public override string DisplayName => "Reload MP";

        public override string Description => Conf.C.AddBloat ? $"Initiates a targeted runtime reinitialization sequence for the following specified mod assemblies: {string.Join(", ", Conf.C.ModsToReload)}." : $"Reloads {string.Join(", ", Conf.C.ModsToReload)}";

        public override async void OnActivate()
        {
            await ReloadUtilities.MultiPlayerMainReload();
        }
    }
}
