using AncientGod.Items.Mounts.InfiniteFlight.AlmightyMecha;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AncientGod.Utilities.UI.MechaChoose
{
    // 关于 UI 的更多信息，你可以查看 https://github.com/tModLoader/tModLoader/wiki/Basic-UI-Element 和 https://github.com/tModLoader/tModLoader/wiki/Advanced-guide-to-custom-UI 
    internal class MechaChooseUIState : UIState
    {
        public MechaChooseDraggableUIPanel CoinCounterPanel;
        private AlmightyMechaKey mechaKey;
        public static event Action<int> ChooseChanged; // 声明一个静态事件
        //这里卡了好几天总算搞出来了，通过事件来进行通信，而不是直接在类之间引用彼此。这样可以有效地解耦两个类，使它们之间的关系更清晰。

        public static int Choose { get; private set; } // 添加静态属性 Choose
        //存在一个问题，即 choose 变量没有被传递到其他类中。要解决这个问题，你可以通过在 ExampleCoinsUIState 类中添加一个静态属性来访问 choose 变量，并且在其他地方使用该属性来获取选择的值。

        // 在 OnInitialize 中，我们将各种 UIElement 放置到 UIState（这个类）上。
        // UIState 类的宽度和高度等于整个屏幕的大小，因此通常我们首先定义一个 UIElement，它将作为我们 UI 的容器。
        // 然后，我们将各种其他 UIElement 放置到容器 UIElement 上，相对于容器 UIElement 定位。
        public override void OnInitialize()
        {
            mechaKey = new AlmightyMechaKey();
            // 在这里，我们定义我们的容器 UIElement。在 DraggableUIPanel.cs 中，你可以看到 DraggableUIPanel 是一个带有一些附加功能的 UIPanel。
            Asset<Texture2D> BackgroundTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/BackgroundTexture");
            Asset<Texture2D> BorderTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/BorderTexture");
            CoinCounterPanel = new MechaChooseDraggableUIPanel(BorderTexture, BackgroundTexture);
            CoinCounterPanel.SetPadding(0);
            // 我们需要将这个 UIElement 放置在它的父元素中。稍后我们将调用 `base.Append(coinCounterPanel);`。 
            // 这意味着这个类，ExampleCoinsUI，将是我们的父元素。由于 ExampleCoinsUI 是一个 UIState，左和顶部相对于屏幕的左上角。
            // SetRectangle 方法帮助我们设置 UIElement 的位置和大小
            SetRectangle(CoinCounterPanel, left: 800f, top: 400f, width: 300f, height: 346f);//CoinCounterPanel 的左上角位于屏幕的 (400, 100) 像素位置，宽度为 170 像素，高度为 70 像素。
            CoinCounterPanel.BackgroundColor = new Color(255, 255, 255);

            // 接下来，我们创建另一个要放置的 UIElement。由于我们将调用 `coinCounterPanel.Append(playButton);`，所以左和顶部相对于 coinCounterPanel 的左上角。
            // 通过正确嵌套 UIElements，我们可以轻松地相对于彼此定位事物。
            Asset<Texture2D> buttonZeroTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseZero");
            MechaChooseUIHoverImageButton playButton = new MechaChooseUIHoverImageButton(buttonZeroTexture, "万能机甲");
            SetRectangle(playButton, left: 100f, top: 10f, width: 88f, height: 88f);
            // 当点击按钮时，UIHoverImageButton 不会执行任何操作。在这里，我们分配一个希望在按钮被点击时调用的方法。
            playButton.OnLeftClick += new MouseEvent(ChooseZero);
            CoinCounterPanel.Append(playButton);

            Asset<Texture2D> buttonOneTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseOne");
            MechaChooseUIHoverImageButton closeButton = new MechaChooseUIHoverImageButton(buttonOneTexture, Language.GetTextValue("万能蠕虫")); // Localized text for "Close"
            SetRectangle(closeButton, left: 200f, top: 60f, width: 88f, height: 88f);
            closeButton.OnLeftClick += new MouseEvent(ChooseOne);
            CoinCounterPanel.Append(closeButton);

            Asset<Texture2D> buttonTwoTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseTwo");
            MechaChooseUIHoverImageButton twoButton = new MechaChooseUIHoverImageButton(buttonTwoTexture, Language.GetTextValue("万能眼球")); // Localized text for "Close"
            SetRectangle(twoButton, left: 0f, top: 60f, width: 88f, height: 88f);
            twoButton.OnLeftClick += new MouseEvent(ChooseTwo);
            CoinCounterPanel.Append(twoButton);

            Asset<Texture2D> buttonThreeTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseThree");
            MechaChooseUIHoverImageButton threeButton = new MechaChooseUIHoverImageButton(buttonThreeTexture, Language.GetTextValue("万能飞虫")); // Localized text for "Close"
            SetRectangle(threeButton, left: 0f, top: 160f, width: 88f, height: 88f);
            threeButton.OnLeftClick += new MouseEvent(ChooseThree);
            CoinCounterPanel.Append(threeButton);

            Asset<Texture2D> buttonFourTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseFour");
            MechaChooseUIHoverImageButton fourButton = new MechaChooseUIHoverImageButton(buttonFourTexture, Language.GetTextValue("万能??")); // Localized text for "Close"
            SetRectangle(fourButton, left: 200f, top: 160f, width: 88f, height: 88f);
            fourButton.OnLeftClick += new MouseEvent(ChooseFour);
            CoinCounterPanel.Append(fourButton);

            Asset<Texture2D> buttonFiveTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseFive");
            MechaChooseUIHoverImageButton fiveButton = new MechaChooseUIHoverImageButton(buttonFiveTexture, Language.GetTextValue("万能??")); // Localized text for "Close"
            SetRectangle(fiveButton, left: 100f, top: 210f, width: 88f, height: 88f);
            fiveButton.OnLeftClick += new MouseEvent(ChooseFive);
            CoinCounterPanel.Append(fiveButton);

            Asset<Texture2D> buttonSixTexture = ModContent.Request<Texture2D>("AncientGod/Utilities/UI/MechaChoose/ChooseSix");
            MechaChooseUIHoverImageButton sixButton = new MechaChooseUIHoverImageButton(buttonSixTexture, Language.GetTextValue("万能??")); // Localized text for "Close"
            SetRectangle(sixButton, left: 100f, top: 110f, width: 88f, height: 88f);
            sixButton.OnLeftClick += new MouseEvent(ChooseSix);
            CoinCounterPanel.Append(sixButton);

            Append(CoinCounterPanel);
            // 总结一下，ExampleCoinsUI 是一个 UIState，意味着它覆盖了整个屏幕。我们将 CoinCounterPanel 附加到 ExampleCoinsUI，距离屏幕的左上角一段距离。
            // 然后，我们将 playButton、closeButton 和 MoneyDisplay 放置在 CoinCounterPanel 上，以便我们可以轻松地相对于 CoinCounterPanel 放置这些 UIElement。
            // 由于 CoinCounterPanel 将移动，这种正确的组织将在 CoinCounterPanel 移动时适当移动 playButton、closeButton 和 MoneyDisplay。
        }

        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        private void OnChooseChanged(int newChoose)
        {
            Choose = newChoose;
            mechaKey.HandleSelection(newChoose);
        }

        public void ChooseZero(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(0);
        }
        public void ChooseOne(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(1);
        }
        public void ChooseTwo(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(2);
        }
        public void ChooseThree(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(3);
        }
        public void ChooseFour(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(4);
        }
        public void ChooseFive(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(5);
        }
        public void ChooseSix(UIMouseEvent evt, UIElement listeningElement)
        {
            // 触发事件并传递选择的值
            ChooseChanged?.Invoke(6);
        }
    }
}