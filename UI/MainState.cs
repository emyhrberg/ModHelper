using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI
{
    public class MainState : UIState
    {
        // Panels
        public LogPanel logPanel;
        public ModsPanel modsPanel;
        public UIElementPanel uiPanel;
        public List<BasePanel> AllPanels = [];

        // Buttons
        public Collapse collapse;
        public bool AreButtonsShowing = true; // flag to toggle all buttons on/off using the toggle button
        public float ButtonSize = 70f;
        public float TextSize = 0.9f;
        public float offset = 0; // START offset for first button position relative to center
        public List<BaseButton> AllButtons = [];
        public ReloadSPButton reloadSPButton;

        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState() => AddEverything();

        public void AddEverything()
        {
            // Set offset for first button position relative to center
            offset = -ButtonSize * 4;
            offset -= 20; // 20 is CUSTOM CUSTOM CUSTOM offset, see collapse also. this is to avoid the collapse button colliding with heros mod

            // Add buttons
            AddButton<LaunchButton>(Ass.ButtonSecond, "Launch", "Start additional tML client");
            AddButton<LogButton>(Ass.ButtonDebug, "Log", "Customize logging", hoverTextDescription: "Right click to open log");
            AddButton<UIElementButton>(Ass.ButtonUI, "UI", "View and edit UI elements", hoverTextDescription: "Right click to toggle all UI elements hitboxes");
            AddButton<ModsButton>(Ass.ButtonMods, "Mods", "View list of mods", hoverTextDescription: "Right click to go to mod sources");
            reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", $"Reload {Conf.C.LatestModToReload}");

            // Add collapse button on top
            collapse = new(Ass.CollapseDown, Ass.CollapseUp, Ass.CollapseLeft, Ass.CollapseRight);
            Append(collapse);

            // Add the panels (invisible by default)
            logPanel = AddPanel<LogPanel>();
            modsPanel = AddPanel<ModsPanel>();
            uiPanel = AddPanel<UIElementPanel>();

            // Add the keep game running text
            if (Main.netMode == NetmodeID.SinglePlayer && Conf.C.ShowGameKeepRunningText)
            {
                string onOff = Conf.C.ShowGameKeepRunningText ? "ON" : "OFF";
                KeepGameRunningText topText = new($"Keep Game Running: {onOff}");
                Append(topText);
            }

            // Add the debug text
            if (Conf.C.ShowDebugText)
            {
                DebugText debugText = new(text: "", TextSize);
                Append(debugText);
            }
        }

        private T AddPanel<T>() where T : BasePanel, new()
        {
            // Create a new panel using reflection
            T panel = new();

            // Add to list
            AllPanels.Add(panel);

            // Add to MainState
            Append(panel);
            return panel;
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

            // Button dimensions
            float size = ButtonSize;
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);
            button.MaxWidth = new StyleDimension(size, 0);
            button.MaxHeight = new StyleDimension(size, 0);
            button.MinWidth = new StyleDimension(size, 0);
            button.MinHeight = new StyleDimension(size, 0);
            button.Recalculate();
            button.VAlign = 1.0f;
            button.HAlign = 0.5f;

            // set x pos with offset
            button.Left.Set(pixels: offset, precent: 0f);

            // increase offset for next button, except MPbutton
            offset += ButtonSize;

            // Add the button to the list of all buttons and append it to the MainState
            AllButtons.Add(button);
            Append(button);

            return button;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw everything in the main state
            base.Draw(spriteBatch);

            // last, draw the tooltips
            // this is to avoid the tooltips being drawn over the buttons
            foreach (var button in AllButtons)
            {
                if (button.IsMouseHovering && button.HoverText != null)
                {
                    // Draw the tooltip
                    DrawHelper.DrawTooltipPanel(button, button.HoverText, button.HoverTextDescription);
                }
            }
        }
    }
}