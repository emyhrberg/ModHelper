﻿using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Terraria;
using SkipSelect.MainCode.Other;

namespace SkipSelect.MainCode.UI
{
    public class MySystem : ModSystem
    {
        private UserInterface userInterface;
        private MyState myState;

        public override void Load()
        {
            if (!Main.dedServ) // ensure that this is only run on the client
            {
                var config = ModContent.GetInstance<Config>();
                if (config.EnableRefresh)
                {
                    userInterface = new UserInterface();
                    myState = new MyState();
                    userInterface.SetState(myState);
                }
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {

            // check if toggle
            Config c = ModContent.GetInstance<Config>();
            if (c.EnableRefresh)
            {
                userInterface?.Update(gameTime);
            }
        }

        // boilerplate code to draw the UI
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SkipSelect: MyState",
                    delegate
                    {
                        Config config = ModContent.GetInstance<Config>();
                        if (config.EnableRefresh)
                        {
                            userInterface?.Draw(Main.spriteBatch, new GameTime()); // actual draw
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
