using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;

namespace AncientGod.Items.Materials
{
    internal class Shadow_Forest : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Orange;
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Shadow_Forest>());
        }
        public override void AddRecipes()
        {
            Recipe new_recipe = CreateRecipe();
            new_recipe.AddIngredient(ItemID.Wood, 10);
            new_recipe.AddIngredient(ItemID.DirtBlock, 20);
            new_recipe.AddIngredient(ItemID.Acorn, 15);
            new_recipe.AddIngredient(ItemID.GrassSeeds, 1);
            new_recipe.AddIngredient<MoonDevoration>(30);
            new_recipe.AddIngredient<Shadow>(2);
            new_recipe.AddTile(TileID.WorkBenches);
            new_recipe.Register();
            Recipe.Create(ItemID.DirtBlock, 5)
                .AddTile<Tiles.Shadow_Forest>()
                .AddIngredient<MoonDevoration>(20)
                .Register();
            Recipe.Create(ItemID.StoneBlock, 5)
                .AddTile<Tiles.Shadow_Forest>()
                .AddIngredient<MoonDevoration>(20)
                .Register();

        }
    }
}
