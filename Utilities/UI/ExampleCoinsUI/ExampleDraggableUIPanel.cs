using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AncientGod.Utilities.UI.ExampleCoinsUI
{
    // 这个 DraggableUIPanel 类继承自 UIPanel
    // 继承是 UI 设计的一个很好的工具。通过继承，我们可以从 UIPanel 中免费获得背景绘制
    // 我们添加了一些代码，以使面板可以被拖动
    // 我们还添加了一些代码，以确保面板在被拖出界或屏幕大小改变时会弹回边界
    // UIPanel 不会阻止玩家在点击鼠标时使用物品，因此我们也添加了该功能
    public class ExampleDraggableUIPanel : UIPanel
    {
        // 存储从 UIPanel 的左上角开始拖动的偏移量
        private Vector2 offset;
        // 一个标志，用于检查面板当前是否正在被拖动
        private bool dragging;

        public ExampleDraggableUIPanel(Asset<Texture2D> texture1, Asset<Texture2D> texture2) : base(texture1, texture2, 4)
        {
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // 当你重写 UIElement 方法时，不要忘记调用基本方法
            // 这有助于保持 UIElement 的基本行为
            base.DrawSelf(spriteBatch);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            // 当你覆盖 UIElement 方法时，不要忘记调用基本方法
            // 这有助于保持 UIElement 的基本行为
            base.LeftMouseDown(evt);
            // 当鼠标按钮按下时，我们开始拖动
            DragStart(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            // 当鼠标按钮弹起时，我们停止拖动
            DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            // 偏移量变量有助于记住相对于鼠标位置的面板位置
            // 因此，无论你从哪里开始拖动面板，它都会平稳移动
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 endMousePosition = evt.MousePosition;
            dragging = false;

            Left.Set(endMousePosition.X - offset.X, 0f);
            Top.Set(endMousePosition.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 检查 ContainsPoint 然后将 mouseInterface 设置为 true 是非常常见的
            // 这使得在此 UIElement 上点击不会导致玩家使用当前物品
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X 和 Main.mouseX 是一样的
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // 在这里，我们检查 DraggableUIPanel 是否超出了父 UIElement 矩形的范围
            // （在我们的示例中，父级将是 ExampleCoinsUI，一个 UIState。这意味着我们正在检查 DraggableUIPanel 是否超出了整个屏幕）
            // 通过这样做和一些简单的数学，我们可以在用户调整窗口大小或以其他方式改变分辨率时，将面板弹回屏幕上
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                // Recalculate 强制 UI 系统重新执行定位数学计算。
                Recalculate();
            }
        }
    }
}