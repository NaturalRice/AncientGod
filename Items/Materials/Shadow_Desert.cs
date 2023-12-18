using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;
using AncientGod.Items.Tiles;

namespace AncientGod.Items.Materials
{
    internal class Shadow_Desert : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Orange;
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Shadow_Desert>());
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cactus, 10)
                .AddIngredient(ItemID.Scorpion, 5)
                .AddIngredient(ItemID.SandBlock, 20)
                .AddIngredient<MoonDevoration>(30)
                .AddIngredient<Shadow>(2)
                .AddTile(TileID.WorkBenches)
                .Register();

            Recipe.Create(ItemID.SandBlock, 5)
                .AddTile<Tiles.Shadow_Desert>()
                .AddIngredient<MoonDevoration>(20)
                .Register();
            Recipe.Create(ItemID.HardenedSand, 5)
                .AddTile<Tiles.Shadow_Desert>()
                .AddIngredient<MoonDevoration>(20)
                .Register();
            Recipe.Create(ItemID.Sandstone, 5)
                .AddTile<Tiles.Shadow_Desert>()
                .AddIngredient<MoonDevoration>(20)
                .Register();
            Recipe.Create(ItemID.SandstoneWallUnsafe, 1)
                .AddTile<Tiles.Shadow_Desert>()
                .AddCondition(Condition.InGraveyard)
                .AddIngredient<MoonDevoration>(50)
                .Register();
            Recipe.Create(ItemID.HardenedSandWallUnsafe, 1)
                .AddTile<Tiles.Shadow_Desert>()
                .AddCondition(Condition.InGraveyard)
                .AddIngredient<MoonDevoration>(50)
                .Register();
        }
    }
}
