using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;

namespace AncientGod.Items.Materials
{
    internal class Shadow_Iceland : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Orange;
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Shadow_Iceland>());
        }
        public override void AddRecipes()
        {
            Recipe new_recipe = CreateRecipe();
            new_recipe.AddIngredient(ItemID.BorealWood, 10);
            new_recipe.AddIngredient(ItemID.SnowBlock, 20);
            new_recipe.AddIngredient(ItemID.ShiverthornSeeds, 1);
            new_recipe.AddIngredient<MoonDevoration>(30);
            new_recipe.AddIngredient<Shadow>(2);
            new_recipe.AddTile(TileID.WorkBenches);
            new_recipe.Register();

            Recipe new_recipe2 = CreateRecipe();
            new_recipe2.AddIngredient(ItemID.BorealWood, 10);
            new_recipe2.AddIngredient(ItemID.IceBlock, 20);
            new_recipe2.AddIngredient(ItemID.ShiverthornSeeds, 1);
            new_recipe2.AddIngredient<MoonDevoration>(30);
            new_recipe2.AddIngredient<Shadow>(2);
            new_recipe2.AddTile(TileID.WorkBenches);
            new_recipe2.Register();

            Recipe.Create(ItemID.SnowBlock, 5)//雪
                .AddTile<Tiles.Shadow_Iceland>()
                .AddIngredient<Materials.MoonDevoration>(20)
                .Register();
            Recipe.Create(ItemID.IceBlock, 5)//冰
                .AddTile<Tiles.Shadow_Iceland>()
                .AddIngredient<Materials.MoonDevoration>(20)
                .Register();
            Recipe.Create(ItemID.SlushBlock, 1)//雪泥
                .AddTile<Tiles.Shadow_Iceland>()
                .AddIngredient<Materials.MoonDevoration>(40)
                .Register();
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
