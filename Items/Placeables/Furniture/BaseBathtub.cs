using AncientGod.Items.Placeables.SpaceBase;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Items.Placeables.Furniture
{
    public class BaseBathtub : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantowax Bathtub");
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
            Item.createTile = ModContent.TileType<Tiles.Furniture.BaseBathtub>();
            Item.width = 12;
            Item.height = 12;
            Item.rare = 0;
        }

        /*public override void AddRecipes()
        {
            Mod CalValEX = ModLoader.GetMod("CalamityMod");
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(ModContent.ItemType<PhantowaxBlock>(), 14);
                recipe.AddTile(TileID.LunarCraftingStation);
                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }*/
        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddIngredient(ModContent.ItemType<BaseEnclosure>(), 14);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
