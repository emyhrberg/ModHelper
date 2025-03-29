using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ModHelper.UI.Elements
{
    public class KeepGameRunningText : UIText
    {
        private bool Active = true;

        public KeepGameRunningText(string text, float scale = 0.4f, bool large = true) : base(text, scale, large)
        {
            TextColor = Color.White;
            VAlign = 0.8f;
            HAlign = 0.02f;

            float w = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).X;
            float h = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).Y;
            Width.Set(w, 0);
            Height.Set(h, 0);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            KeepGameRunning.KeepRunning = !KeepGameRunning.KeepRunning;

            if (KeepGameRunning.KeepRunning)
            {
                ChatHelper.NewText("Keep Game Running: ON");
            }
            else
            {
                ChatHelper.NewText("Keep Game Running: OFF");
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Active = !Active;

            Conf.C.ShowGameKeepRunningText = !Conf.C.ShowGameKeepRunningText;
            Conf.Save();

            if (Active)
            {
                ChatHelper.NewText("Show Keep Game Running Text");
            }
            else
            {
                ChatHelper.NewText("Hide Keep Game Running Text");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true; // disable item use if the button is hovered
                UICommon.TooltipMouseText("Right click to toggle");
            }
        }
    }

}