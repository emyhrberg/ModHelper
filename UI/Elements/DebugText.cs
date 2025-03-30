using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class DebugText : UIText
    {
        private bool Active = true;

        public DebugText(string text, float textScale = 1.0f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 0.9f;
            HAlign = 0.02f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(200, 0);
            Height.Set(20 * 4, 0); // 20 * 3 for 3 lines of text
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Active = !Active;

            if (Active)
            {
                ChatHelper.NewText("Showing debug text", Color.White);
            }
            else
            {
                ChatHelper.NewText("Hiding debug text", Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // update the text to show playername, whoAmI, and FPS
            string playerName = Main.LocalPlayer.name;
            int whoAmI = Main.myPlayer;
            int fps = Main.frameRate;
            int ups = Main.updateRate;

            string netmode = Main.netMode switch
            {
                NetmodeID.SinglePlayer => "Singleplayer",
                NetmodeID.MultiplayerClient => "Multiplayer",
                _ => "Unknown"
            };

            string fileName = Path.GetFileName(Logging.LogPath);
            string text = $"{playerName} ({whoAmI})" +
                $"\n{netmode} ({fileName})" +
                $"\n{fps}fps {ups}ups ({Main.upTimerMax:0.0}ms)";

            SetText(text);
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
                UICommon.TooltipMouseText("Right click to toggle visibility");
            }
        }
    }
}