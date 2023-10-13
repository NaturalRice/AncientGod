using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;

namespace AncientGod.Items.Materials
{
    internal class Shadow_Jungle : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 100;
            Item.height = 100;
        }
        public override void AddRecipes()
        {
            Recipe new_recipe = CreateRecipe();
            new_recipe.AddIngredient(ItemID.RichMahogany, 10);
            new_recipe.AddIngredient(ItemID.MudBlock, 20);
            new_recipe.AddIngredient(ItemID.JungleGrassSeeds, 1);
            new_recipe.AddIngredient<MoonDevoration>(10);
            new_recipe.AddTile(TileID.WorkBenches);
            new_recipe.Register();
        }
    }
}
