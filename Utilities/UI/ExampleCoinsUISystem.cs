﻿/*using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AncientGod.Utilities.UI
{
    [Autoload(Side = ModSide.Client)]
    public class ExampleCoinsUISystem : ModSystem
    {
        private UserInterface exampleCoinUserInterface;
        internal ExampleCoinsUIState exampleCoinsUI;

        // These two methods will set the state of our custom UI, causing it to show or hide
        public void ShowMyUI()
        {
            exampleCoinUserInterface?.SetState(exampleCoinsUI);
        }

        public void HideMyUI()
        {
            exampleCoinUserInterface?.SetState(null);
        }

        public override void Load()
        {
            // Create custom interface which can swap between different UIStates
            exampleCoinUserInterface = new UserInterface();
            // Creating custom UIState
            exampleCoinsUI = new ExampleCoinsUIState();

            // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
            exampleCoinsUI.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Here we call .Update on our custom UI and propagate it to its state and underlying elements
            if (exampleCoinUserInterface?.CurrentState != null)
            {
                exampleCoinUserInterface?.Update(gameTime);
            }
        }

        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
        // Setting the InterfaceScaleType to UI for appropriate UI scaling
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "ExampleMod: Coins Per Minute",
                    delegate {
                        if (exampleCoinUserInterface?.CurrentState != null)
                        {
                            exampleCoinUserInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
*/