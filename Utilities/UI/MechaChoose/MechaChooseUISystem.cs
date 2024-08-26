using AncientGod.Items.Mounts.InfiniteFlight.AlmightyMecha;
using AncientGod.Projectiles.Mounts.InfiniteFlight.AlmightyMecha;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Drawing.Printing;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AncientGod.Utilities.UI.MechaChoose
{
    [Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
    public class MechaChooseUISystem : ModSystem
    {
        private UserInterface exampleCoinUserInterface;
        internal MechaChooseUIState exampleCoinsUI;
        private AlmightyMechaBody mechaBody;
        private AlmightyMechaKey mechaKey;

        public bool active;//判断是否拥有万能机甲

        // These two methods will set the state of our custom UI, causing it to show or hide
        public void ShowMyUI()
        {
            exampleCoinUserInterface?.SetState(exampleCoinsUI);
        }

        public void HideMyUI()
        {
            exampleCoinUserInterface?.SetState(null);
        }

        public override void Load()//必须在此实例化对象，不然会导致空引用异常
        {
            // Create custom interface which can swap between different UIStates
            exampleCoinUserInterface = new UserInterface();
            // Creating custom UIState
            exampleCoinsUI = new MechaChooseUIState();

            // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
            exampleCoinsUI.Activate();
            exampleCoinUserInterface.SetState(exampleCoinsUI);
            // Instantiate AlmightyMechaBody object
            mechaBody = new AlmightyMechaBody();
            mechaKey = new AlmightyMechaKey();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.LocalPlayer != null)
            {
                AncientGodPlayer player = Main.LocalPlayer.GetModPlayer<AncientGodPlayer>();
                active = player.AlmightyMecha || player.AlmightyMechaDigger;
                //目前这里的效果是，当玩家未召唤出万能机甲时无法显示，一旦召唤了第一次，后面就无论如何都可以显示（退出世界后重置）
                if (active)
                {
                    ShowMyUI(); // 如果 active 为真，则显示 UI
                }
                else
                {
                    HideMyUI(); // 如果 active 为假，则隐藏 UI
                }
            }
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
                    delegate
                    {
                        if (exampleCoinUserInterface?.CurrentState != null && Main.mouseRight && !Main.mouseRightRelease)
                        {
                            // 不需要在这里检查 active，因为在 UpdateUI 方法中已经处理了
                            exampleCoinUserInterface.Draw(Main.spriteBatch, new GameTime());//此处最终显示UI
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}