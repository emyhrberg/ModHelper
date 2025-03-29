using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A parent panel
    /// for Player, Debug, World panels
    /// </summary>
    public abstract class BasePanel : UIPanel
    {
        // Variables
        protected UIList uiList;
        protected UIScrollbar scrollbar;
        protected bool scrollbarEnabled = true;

        private int panelPadding = 12;

        // Panel values
        protected bool Active = false; // draw and update when true
        protected Color darkBlue = new(73, 85, 186);
        public string Header = "";
        public CustomTitlePanel TitlePanel;

        // Size
        protected const float PANEL_WIDTH = 350f;
        protected const float PANEL_HEIGHT = 500f;

        #region Constructor
        public BasePanel(string header)
        {
            // panel settings
            Width.Set(PANEL_WIDTH, 0f);
            Height.Set(PANEL_HEIGHT, 0f);
            Top.Set(-70, 0f);
            Left.Set(-20, 0f);
            HAlign = 1.0f; // right aligned
            VAlign = 1.0f; // bottom aligned
            BackgroundColor = darkBlue;
            Header = header;

            // Create all content in the panel
            TitlePanel = new(header);
            CloseButtonPanel closeButtonPanel = new();

            // Add all content in the panel
            Append(TitlePanel);
            Append(closeButtonPanel);

            // Create a new list
            uiList = new UIList
            {
                MaxWidth = { Percent = 1f, Pixels = panelPadding * 2 },
                Width = { Percent = 1f, Pixels = panelPadding * 2 },
                MaxHeight = { Percent = 1f, Pixels = -20 },
                Height = { Percent = 1f, Pixels = -20 },
                HAlign = 0.5f,
                VAlign = 0f,
                Top = { Pixels = 24 },
                Left = { Pixels = 0 },
                ListPadding = 0f, // 0 or 5f
                ManualSortMethod = (e) => { }
            };

            // Create a new scrollbar
            scrollbar = new()
            {
                Height = { Percent = 1f, Pixels = -35 - 12 },
                HAlign = 1f,
                VAlign = 0f,
                Left = { Pixels = 5 }, // scrollbar has 20 width
                Top = { Pixels = 35 + 12 },
            };

            // Set the scrollbar to the list
            Append(uiList);
            if (scrollbarEnabled) uiList.SetScrollbar(scrollbar);
            if (scrollbarEnabled) Append(scrollbar);
        }
        #endregion

        #region Add UI Elements
        protected SliderPanel AddSlider(string title, float min, float max, float defaultValue, Action<float> onValueChanged = null, float? increment = null, float textSize = 1f, string hover = "", Action leftClickText = null, Action rightClickText = null, Func<float, string> valueFormatter = null)
        {
            SliderPanel sliderPanel = new(title, min, max, defaultValue, onValueChanged, increment, textSize, hover, leftClickText, rightClickText, valueFormatter);
            uiList.Add(sliderPanel);
            AddPadding(3);
            return sliderPanel;
        }

        protected OptionElement AddOption(string text, Action leftClick, string hover = "", Action rightClick = null, float padding = 3f)
        {
            OptionElement option = new(leftClick, text, hover, rightClick);
            uiList.Add(option);
            AddPadding(padding);
            return option;
        }

        protected ActionOption AddAction(Action leftClick, string text, string hover, Action rightClick = null, float textSize = 0.4f, float padding = 5f)
        {
            ActionOption actionOption = new(leftClick, text, hover, rightClick);
            uiList.Add(actionOption);
            AddPadding(padding);
            return actionOption;
        }

        protected HeaderElement AddHeader(string title, Action onLeftClick = null, string hover = "")
        {
            HeaderElement headerElement = new(title, hover);
            headerElement.OnLeftClick += (mouseEvent, element) => onLeftClick?.Invoke();
            uiList.Add(headerElement);
            return headerElement;
        }

        /// <summary>
        /// Add padding to the panel with a blank header with the given panel element height
        /// </summary>
        protected HeaderElement AddPadding(float padding = 20f)
        {
            // Create a blank UIElement to act as a spacer.
            HeaderElement paddingElement = new("");
            paddingElement.Height.Set(padding, 0f);
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            base.Draw(spriteBatch);
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            base.Update(gameTime);
        }
        #region Reset position

        public bool GetActive() => Active;

        // When we click on a button, we toggle the active state of the panel.
        // This method is called to reset the position of the panel when it is toggled (when the panel is shown again).
        public bool SetActive(bool active)
        {
            if (active)
            {
                // Reset panel position
                Top.Set(-70, 0f);
                Left.Set(-20, 0f);
                Recalculate();
            }

            Active = active;
            return Active;
        }
        #endregion
    }
}
