using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class UIElementButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Sprite size
        private float _scale = 1.3f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 28;
        protected override int FrameHeight => 24;

        // Sprite animation
        protected override int FrameCount => 4;
        protected override int FrameSpeed => 10;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Open the log panel
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            List<BasePanel> panels = sys.mainState.AllPanels;
            BasePanel uiElementPanel = panels.FirstOrDefault(p => p is UIElementPanel);
            uiElementPanel?.SetActive(!uiElementPanel.GetActive()); // Set the log panel active if it exists

            // Toggle state of parentActive
            ParentActive = !ParentActive;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            // Toggle all elements
            UIElementSystem elementSystem = ModContent.GetInstance<UIElementSystem>();
            UIElementState elementState = elementSystem.debugState;
            elementState.ToggleShowAll();
        }
    }
}
