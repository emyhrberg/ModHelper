﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ModHelper.Helpers
{
    public static class DrawHelper
    {
        /// <summary>
        /// Draw a tooltip panel at a hardcoded position in the main menu
        /// </summary>
        public static void DrawMainMenuTooltipPanel(string text)
        {
            // Hardcoded panel position and size.
            // (108, Main.screenHeight/2 + 60) is used as the base text position.
            // The panel is 200x50, centered on that point.
            Vector2 basePos = new Vector2(113, Main.screenHeight / 2f);
            int width = 300;
            int height = 70;
            int offsetY = -35; // move it below the text
            Rectangle tooltipRect = new Rectangle((int)basePos.X - 100, (int)basePos.Y + offsetY, width, height);

            // Draw background panel.
            //Color darkBlue = UICommon.DefaultUIBlue;
            Color darkBlue = ColorHelper.DarkBluePanel;
            Utils.DrawInvBG(Main.spriteBatch, tooltipRect, darkBlue);

            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float scale = 0.9f;

            // Set horizontal padding (total available width is panel width minus twice the padding)
            int pad = 5;
            float maxTextWidth = tooltipRect.Width - pad * 2;

            // Wrap the text into at most two lines.
            // If the whole text fits within the maxTextWidth, it stays on one line.
            // Otherwise, a simple word wrap moves extra words to the next line.
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                // If adding the next word exceeds the available width, push the line.
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                if (ChatManager.GetStringSize(font, testLine, Vector2.One).X * scale > maxTextWidth)
                {
                    // If nothing is in currentLine, force the word in (in case a word is too long)
                    if (string.IsNullOrEmpty(currentLine))
                        currentLine = word;
                    lines.Add(currentLine);
                    currentLine = word;
                    if (lines.Count == 2) // Limit to two lines.
                        break;
                }
                else
                {
                    currentLine = testLine;
                }
            }
            if (lines.Count < 2 && !string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            // If more words remain and we've already created two lines, you might want to append ellipsis.
            if (lines.Count == 2 && words.Length > lines[0].Split(' ').Length + lines[1].Split(' ').Length)
            {
                lines[1] = lines[1].TrimEnd() + "...";
            }

            // Draw each line centered horizontally in the panel.
            // We'll vertically center the text block inside the panel.
            float totalTextHeight = 0;
            List<Vector2> sizes = new List<Vector2>();
            foreach (string line in lines)
            {
                Vector2 size = ChatManager.GetStringSize(font, line, Vector2.One) * scale;
                sizes.Add(size);
                totalTextHeight += size.Y;
            }

            // If there are two lines, add a small gap.
            int gap = lines.Count > 1 ? 2 : 0;
            totalTextHeight += gap;

            // Starting Y position so the text block is vertically centered in the panel.
            float startY = tooltipRect.Y + (tooltipRect.Height - totalTextHeight) / 2f;
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 size = sizes[i];
                // Center the line horizontally within the panel.
                float xPos = tooltipRect.X + (tooltipRect.Width - size.X) / 2f;
                float yPos = startY;
                Utils.DrawBorderString(Main.spriteBatch, lines[i], new Vector2(xPos, yPos), Color.White, scale);
                startY += size.Y + gap;
            }
        }

        /// <summary>
        /// Draws a tooltip panel just above a BaseButton element.
        /// </summary>
        /*public static void DrawMainMenuTooltipPanel(this UIElement element, string text, string tooltip, int yOffset = 0)
        {
            int pad = 6;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float nameWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            float tipWidth = ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f;
            float width = Math.Max(nameWidth, tipWidth) + pad * 4;
            float height = ChatManager.GetStringSize(font, $"{text}\n", Vector2.One).Y + pad * 2 - 4;

            if (!string.IsNullOrEmpty(tooltip))
            {
                height = ChatManager.GetStringSize(font, $"{text}\n{tooltip}", Vector2.One).Y + pad * 2 - 4;
            }

            CalculatedStyle dims = element.GetDimensions();

            // Move Y position up by decreasing the Y value of the dimensions
            dims.Y -= 80f;
            dims.Y -= yOffset;

            float tooltipX = dims.X + (dims.Width - width) / 2f;
            float tooltipY = dims.Y - height;
            Rectangle rect = new((int)tooltipX, (int)tooltipY, (int)width, (int)height);

            // draw bg panel
            Color darkBlue = new Color(22, 22, 55) * 0.925f;
            Utils.DrawInvBG(Main.spriteBatch, rect, darkBlue);

            // Calculate center X of panel
            float centerX = tooltipX + width / 2f;

            // draw main header text (centered)
            float headerTextWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            Vector2 headerPos = new Vector2(centerX - headerTextWidth / 2f, tooltipY + pad);
            Utils.DrawBorderString(Main.spriteBatch, text, headerPos, Color.White);

            // draw tooltip description text (centered)
            // NOTE: not really centered.
            if (!string.IsNullOrEmpty(tooltip))
            {
                float tooltipTextWidth = ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f;
                Vector2 tooltipPos = new Vector2(centerX - tooltipTextWidth / 2f, headerPos.Y + ChatManager.GetStringSize(font, text, Vector2.One).Y + 4);
                Utils.DrawBorderString(Main.spriteBatch, tooltip, tooltipPos, Color.LightGray, 0.9f);
            }
        }
        */
        /// <summary>
        /// Draws a tooltip panel just above a BaseButton element.
        /// </summary>
        public static void DrawTooltipPanel(this UIElement element, string text, string tooltip)
        {
            if (element == null || string.IsNullOrWhiteSpace(text))
                return;

            int pad = 6;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            // Get string sizes
            float nameWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            float tipWidth = !string.IsNullOrEmpty(tooltip)
                ? ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f
                : 0f;
            float width = Math.Max(nameWidth, tipWidth) + pad * 4;
            float height = ChatManager.GetStringSize(font, $"{text}\n", Vector2.One).Y + pad * 2 - 4;
            if (!string.IsNullOrEmpty(tooltip))
            {
                height = ChatManager.GetStringSize(font, $"{text}\n{tooltip}", Vector2.One).Y + pad * 2 - 4;
            }

            CalculatedStyle dims = element.GetDimensions();

            // Calculate centered tooltip position above element
            float tooltipX = dims.X + (dims.Width - width) / 2f;
            float tooltipY = dims.Y - height;
            // Clamp tooltip position so it never goes offscreen
            tooltipX = Math.Max(tooltipX, 0);
            tooltipY = Math.Max(tooltipY, 0);

            Rectangle rect = new Rectangle((int)tooltipX, (int)tooltipY, (int)width, (int)height);

            // Draw background panel
            Color darkBlue = new Color(22, 22, 55) * 0.925f;
            Utils.DrawInvBG(Main.spriteBatch, rect, darkBlue);

            // Center text
            float centerX = tooltipX + width / 2f;
            float headerTextWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            Vector2 headerPos = new Vector2(centerX - headerTextWidth / 2f, tooltipY + pad);
            Utils.DrawBorderString(Main.spriteBatch, text, headerPos, Color.White);

            // Draw description text if provided
            if (!string.IsNullOrEmpty(tooltip))
            {
                float tooltipTextWidth = ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f;
                Vector2 tooltipPos = new Vector2(centerX - tooltipTextWidth / 2f,
                    headerPos.Y + ChatManager.GetStringSize(font, text, Vector2.One).Y + 4);
                Utils.DrawBorderString(Main.spriteBatch, tooltip, tooltipPos, Color.LightGray, 0.9f);
            }
        }

        /// <summary>
        /// Draws a texture at the proper scale to fit within the given UI element.
        /// /// </summary>
        public static void DrawProperScale(SpriteBatch spriteBatch, UIElement element, Texture2D tex, float scale = 1.0f, float opacity = 1.0f, bool active = false)
        {
            if (tex == null || element == null)
            {
                Log.SlowInfo("Failed to find texture to draw. Skipping draw.", seconds: 5);
            }

            // Get the UI element's dimensions
            CalculatedStyle dims = element.GetDimensions();

            // Compute a scale that makes it fit within the UI element
            float scaleX = dims.Width / tex.Width;
            float scaleY = dims.Height / tex.Height;
            float drawScale = Math.Min(scaleX, scaleY) * scale;

            // Top-left anchor: just place it at dims.X, dims.Y
            Vector2 drawPosition = new Vector2(dims.X, dims.Y);

            float actualOpacity = opacity;
            if (active)
            {
                actualOpacity = 1f;
            }

            // Draw the texture anchored at top-left with the chosen scale
            spriteBatch.Draw(
                tex,
                drawPosition,
                null,
                Color.White * actualOpacity,
                0f,
                Vector2.Zero,
                drawScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
