using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.AbstractElements
{
    /// <summary>
    /// A parent panel
    /// for Player, Debug, World panels
    /// </summary>
    public abstract class BasePanelConfig : UIPanel
    {
        // Variables
        // 35 is the customtitlepanel height
        // 12 is minus the padding of a panel
        protected float currentTop = 35 - 12;

        public UIList uiList;
        protected UIScrollbar scrollbar;

        // Panel values
        public string Header = "";
        public TitlePanel TitlePanel;


        #region Constructor
        public BasePanelConfig(string header, bool scrollbarEnabled = true)
        {
            // panel settings
            Width.Set(-20, 1f);
            Height.Set(-30, 1f);
            Top.Set(0, 0);
            Left.Set(0, 0);
            VAlign = 1f;
            HAlign = 0.5f;
            BackgroundColor = ColorHelper.SuperDarkBluePanel;

            Header = header;

            // Create a new list
            uiList = new UIList
            {
                MaxWidth = { Percent = 1f },
                Width = { Percent = 1f },
                MaxHeight = { Percent = 1f },
                Height = { Percent = 1f },
                HAlign = 0.5f,
                VAlign = 0f,
                Top = { Pixels = 0 },
                Left = { Pixels = 0 },
                ListPadding = 0f, // 0 or 5f
                ManualSortMethod = (e) => { }
            };

            // Create a new scrollbar
            if (scrollbarEnabled)
            {
                scrollbar = new()
                {
                    // MaxHeight = something // may be needed at some point
                    Height = { Percent = 1f }, // -35 for header, -12 for padding, -35 for resize icon
                    HAlign = 1f,
                    VAlign = 0f,
                    Left = { Pixels = 5 }, // scrollbar has 20 width
                    Top = { Pixels = 5 },
                };
            }


            // Set the scrollbar to the list
            Append(uiList);

            if (scrollbarEnabled) uiList.SetScrollbar(scrollbar);
            if (scrollbarEnabled) Append(scrollbar);

            Recalculate();
            scrollbar.Recalculate();
            uiList.Recalculate();
        }
        #endregion

        #region add stuff

        public UIElement AddPadding(float padding)
        {
            // Create a basic UIElement to act as a spacer instead of using HeaderElement
            UIElement paddingElement = new();
            paddingElement.Height.Set(padding, 0f);
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }

        #endregion
    }
}
