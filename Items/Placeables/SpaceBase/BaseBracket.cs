using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Items.Placeables.SpaceBase
{
    public class BaseBracket : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.DefaultToPlaceableTile(ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>());
            Item.width = 16;
            Item.height = 16;
        }

        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ModContent.ItemType<Nanomaterial>(), 4);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.NanometerFurnace>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
