using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items;

namespace AncientGod.Items.Placeables
{
    internal class Rubbish : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Rubbish>());
            Item.width = 16;
            Item.height = 16;
        }

        /*public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddTile(TileID.Campfire);//加入合成站（这里为了有趣我改成了篝火）
            recipe.Register();
        }*/
    }
}
