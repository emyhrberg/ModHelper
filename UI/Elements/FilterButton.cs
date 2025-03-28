using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A button that can be clicked to filter items in the ItemSpawner and NPCSpawner panels.
    /// </summary>
    public class FilterButton : UIImageButton
    {
        // Variables
        private readonly Asset<Texture2D> icon;
        private readonly string HoverText = "";
        public bool Active = true; // whether this filter is currently active

        public FilterButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            // size and position
            Width.Set(21f, 0f);
            Height.Set(21f, 0f);
            MaxWidth.Set(21f, 0f);
            MaxHeight.Set(21f, 0f);
            MinWidth.Set(21f, 0f);
            MinHeight.Set(21f, 0f);
            Top.Set(50, 0f);

            // init stuff
            icon = texture;
            HoverText = hoverText;
            SetImage(icon);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (icon != null && icon.Value != null)
            {
                // Get rectangle dimensions
                Rectangle drawRect = GetDimensions().ToRectangle();
                float opacity = IsMouseHovering ? 1f : 0.7f;

                // If active, draw the active background
                if (Active)
                    spriteBatch.Draw(Ass.FilterBGActive.Value, drawRect, Color.White);
                else
                    spriteBatch.Draw(Ass.FilterBG.Value, drawRect, Color.White);

                // Always draw the icon on top with full opacity if active,
                // otherwise draw with the opacity value.
                if (Active)
                    spriteBatch.Draw(icon.Value, drawRect, Color.White);
                else
                    spriteBatch.Draw(icon.Value, drawRect, Color.White * opacity);

                // Draw tooltip text if hovering.
                if (IsMouseHovering)
                {
                    UICommon.TooltipMouseText(HoverText);
                }
                // Main.hoverItemName = HoverText;
            }

        }
    }
}