using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Tiles.Furniture;
using Terraria;
using AncientGod.Items.Placeables.SpaceBase;

namespace AncientGod.Items.Placeables.Furniture
{
    public class BaseLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Base Lantern");
            Item.ResearchUnlockCount = 1;
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
            Item.createTile = ModContent.TileType<Tiles.Furniture.BaseLantern>();
            Item.width = 12;
            Item.height = 12;
            Item.rare = 0;
        }

        /*public override void AddRecipes()
        {
            Mod calamityMod = ModLoader.GetMod("CalamityMod");
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(calamityMod.ItemType("CosmiliteLantern"));
                recipe.AddIngredient(ModContent.ItemType<Items.Tiles.FurnitureSets.Bloodstone.BloodstoneLanternItem>());
                recipe.AddIngredient(calamityMod.ItemType("BotanicLantern"));
                recipe.AddIngredient(calamityMod.ItemType("SilvaLantern"));
                recipe.AddIngredient(calamityMod.ItemType("AuricBar"), 5);
                recipe.AddTile(ModContent.TileType<AuricManufacturerPlaced>());
                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }*/
        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddIngredient(ModContent.ItemType<BaseEnclosure>(), 6);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
