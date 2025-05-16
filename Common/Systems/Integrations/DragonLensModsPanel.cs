using AssGen;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolSystem;
using DragonLens.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ModReloader.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensModsPanel : Tool
    {
        public override string IconKey => "Mods";

        public override string DisplayName => "Mods List";

        public override string Description => Conf.C.AddBloat ? "This interface allows the user to programmatically control the runtime inclusion state of modular content extensions (commonly referred to as 'mods'). Through toggling specific flags, one can determine whether a given mod should be initialized, loaded into memory, and allowed to influence the game's behavior during execution, or instead be excluded from the active mod pipeline until re-enabled." : $"Enable or disable mods";

        public override void OnActivate()
        {
            Log.Info("DLModsPanel activated");
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel modsPanel = sys.mainState.modsPanel;

            if (modsPanel is null)
            {
                Log.Error("ModsPanel is null");
                return;
            }

            if (modsPanel.GetActive())
            {
                modsPanel.SetActive(false);
            }
            else
            {
                modsPanel.SetActive(true);

                // bring to front …
                if (modsPanel.Parent is not null)
                {
                    UIElement parent = modsPanel.Parent;
                    modsPanel.Remove();
                    parent.Append(modsPanel);
                }
            }
        }

        public override void DrawIcon(SpriteBatch spriteBatch, Rectangle position)
        {
            base.DrawIcon(spriteBatch, position);
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel modsPanel = sys.mainState.modsPanel;

            if (modsPanel.GetActive())
            {
                GUIHelper.DrawOutline(spriteBatch, new Rectangle(position.X - 4, position.Y - 4, 46, 46), ThemeHandler.ButtonColor.InvertColor());

                Texture2D tex = DragonLensAssets.Misc.GlowAlpha.Value;
                Color color = new Color(255, 215, 150);
                color.A = 0;
                var target = new Rectangle(position.X, position.Y, 38, 38);

                spriteBatch.Draw(tex, target, color);
            }
        }
    }
}
