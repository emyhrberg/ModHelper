using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class LogButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        protected override float Scale => 0.5f;
        protected override int FrameCount => 16;
        protected override int FrameSpeed => 4;
        protected override int FrameWidth => 74;
        protected override int FrameHeight => 78;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle the log panel status
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            List<BasePanel> panels = sys.mainState.AllPanels;
            BasePanel logPanel = panels.FirstOrDefault(p => p is LogPanel);
            logPanel?.SetActive(!logPanel.GetActive());

            // Toggle state of parentActive
            ParentActive = !ParentActive;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // Open client log instantly
            Log.OpenClientLog();
        }
    }
}