using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;

namespace AncientGod.Items.Placeables
{
    internal class Shadow_Jungle : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Orange;
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Shadow_Jungle>());
        }
        public override void AddRecipes()
        {
            Recipe new_recipe = CreateRecipe();
            new_recipe.AddIngredient(ItemID.RichMahogany, 10);
            new_recipe.AddIngredient(ItemID.MudBlock, 20);
            new_recipe.AddIngredient(ItemID.JungleGrassSeeds, 1);
            new_recipe.AddIngredient<MoonDevoration>(30);
            new_recipe.AddIngredient<Shadow>(2);
            new_recipe.AddTile(TileID.WorkBenches);
            new_recipe.Register();
        }
    }
}
