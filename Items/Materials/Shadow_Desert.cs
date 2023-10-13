using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;

namespace AncientGod.Items.Materials
{
    internal class Shadow_Desert : ModItem
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
            new_recipe.AddIngredient(ItemID.Cactus, 10);
            new_recipe.AddIngredient(ItemID.Scorpion, 5);
            new_recipe.AddIngredient(ItemID.SandBlock, 20);
            new_recipe.AddIngredient<MoonDevoration>(10);
            new_recipe.AddTile(TileID.WorkBenches);
            new_recipe.Register();
        }
    }
}
