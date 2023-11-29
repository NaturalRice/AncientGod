using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Placeables.Furniture;
using Terraria;

namespace AncientGod.Items.Placeables.Furniture
{
    public class BasePlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chiseled Auric Platform");
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.BasePlatform>();
            Item.width = 12;
            Item.height = 12;
            Item.rare = 0;
        }

        /*public override void AddRecipes()
        {
            Mod calamityMod = ModLoader.GetMod("CalamityMod");
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(calamityMod.ItemType("CosmilitePlatform"), 30);
                recipe.AddIngredient(ModContent.ItemType<Items.Tiles.FurnitureSets.Bloodstone.BloodstonePlatformItem>(), 30);
                recipe.AddIngredient(calamityMod.ItemType("BotanicPlatform"), 30);
                recipe.AddIngredient(calamityMod.ItemType("SilvaPlatform"), 30);
                recipe.AddIngredient(calamityMod.ItemType("AuricBar"), 1);
                recipe.AddTile(ModContent.TileType<AuricManufacturerPlaced>());
                recipe.SetResult(this, 30);
                recipe.AddRecipe();
            }
        }*/
        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddTile(TileID.Campfire);//加入合成站（这里为了有趣我改成了篝火）
            recipe.Register();
        }
    }
}
