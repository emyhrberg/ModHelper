using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set button image size
        private float _scale = 0.6f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 55;
        protected override int FrameHeight => 70;

        // Set button image animation
        protected override int FrameCount => 10;
        protected override int FrameSpeed => 4;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Open the log panel
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            List<BasePanel> panels = sys.mainState.AllPanels;
            BasePanel modsPanel = panels.FirstOrDefault(p => p is ModsPanel);
            modsPanel?.SetActive(!modsPanel.GetActive()); // Set the log panel active if it exists

            // Toggle state of parentActive
            ParentActive = !ParentActive;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            WorldGen.JustQuit();
            Main.menuMode = 10001;
        }
    }
}