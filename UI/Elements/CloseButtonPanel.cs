using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class CloseButtonPanel : UIPanel
    {
        public Asset<Texture2D> closeTexture;

        public CloseButtonPanel()
        {
            // closeTexture = Assets.X;
            Left.Set(12f, 0f);
            Top.Set(-12f, 0f);
            Width.Set(35, 0f);
            Height.Set(35, 0f);
            MaxWidth.Set(35, 0f);
            MaxHeight.Set(35, 0f);
            HAlign = 1f;

            // create a UIText
            UIText text = new UIText("X", 0.4f, true);
            text.HAlign = 0.5f;
            text.VAlign = 0.5f;
            Append(text);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            BorderColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            BorderColor = Color.Black;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Find the parent BasePanel containing this close button
            UIElement current = Parent;
            while (current != null && current is not BasePanel)
            {
                current = current.Parent;
            }

            // If we found the parent panel, deactivate it
            if (current is BasePanel panel && panel.GetActive())
            {
                Log.Info("CloseButtonPanel: Deactivated panel with name: " + panel.GetType().Name);
                panel.SetActive(false);

                // Also update the parentActive property of the button
                // The parent is BasePanel, and BaseButton is something else entirely.
                // Maybe use Mainstate.AllButtons to find the button that opened this panel?
                // Or just set the parentActive to false here, since we are closing the panel anyway.
                // This is a bit hacky, but it works for now.

                // Find the button that opened this panel
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                List<BaseButton> buttons = sys?.mainState?.AllButtons;
                foreach (var button in buttons)
                {
                    if (button is BaseButton baseButton && baseButton.Active)
                    {
                        baseButton.ParentActive = false;
                        break; // Exit the loop once we find the button
                    }
                }
            }
        }
    }
}