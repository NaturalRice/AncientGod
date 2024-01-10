using AncientGod.Items.Placeables.SpaceBase;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace AncientGod.Items.Placeables.Furniture
{
    public class BaseChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantowax Chest");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 500;
            Item.createTile = ModContent.TileType<Tiles.Furniture.BaseChest>();
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModLoader.GetMod("CalamityMod").ItemType("Phantoplasm"), 4);
            recipe.AddIngredient(ModContent.ItemType<PhantowaxBlock>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.IronBar, 2);//加入材料（1火把）
            recipe.AddIngredient(ModContent.ItemType<BaseEnclosure>(), 8);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
