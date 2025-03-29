using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static ModHelper.UI.Elements.OptionElement;

namespace ModHelper.UI.Elements
{
    public class ModEnabledText : UIText
    {
        private State state;
        private Color red = new(226, 57, 39);
        private string internalModName;
        // private Action leftClick;

        public ModEnabledText(string text, string internalModName = "", Action leftClick = null) : base(text)
        {
            // text and size and position
            float def = -65f;
            TextColor = Color.Green;
            VAlign = 0.5f;
            Left.Set(def, 1f);

            this.internalModName = internalModName;
            // this.leftClick = leftClick;
        }

        public void SetTextState(State state)
        {
            this.state = state;
            TextColor = state == State.Enabled ? Color.Green : red;
            SetText(state == State.Enabled ? "Enabled" : "Disabled");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                if (state == State.Enabled)
                {
                    UICommon.TooltipMouseText("Click to disable");
                }
                else
                {
                    UICommon.TooltipMouseText("Click to enable");
                }
            }
        }
    }
}