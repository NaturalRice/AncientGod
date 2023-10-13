using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;
using Terraria.DataStructures;

namespace AncientGod.Items.Materials
{
    internal class Shadow : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 50;
            Item.height = 50;
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 9));//这里使得贴图具有动画效果
            //这里使用了 Main.RegisterItemAnimation 方法，指定了物品的类型 (Item.type) 和一个 DrawAnimationVertical 对象。
            //DrawAnimationVertical 对象定义了动画的帧速率（5）（越低越快）和垂直帧数（9），这会使物品以垂直方向的动画效果播放
            Item.ResearchUnlockCount = 1;
        }

        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddTile(TileID.Campfire);//加入合成站（这里为了有趣我改成了篝火）
            recipe.Register();
        }
    }
}
