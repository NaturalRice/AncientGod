using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Items.Placeables.Furniture
{
    public class BaseBed : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantowax Bed");
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
            Item.createTile = ModContent.TileType<Tiles.Furniture.BaseBed>();
            Item.width = 32;
            Item.height = 33;
            Item.rare = 0;
        }

        /*public override void AddRecipes()
        {
            Mod CalValEX = ModLoader.GetMod("CalamityMod");
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(ModLoader.GetMod("CalamityMod").ItemType("Phantoplasm"), 5);
                recipe.AddIngredient(ModContent.ItemType<PhantowaxBlock>(), 15);
                recipe.AddTile(TileID.LunarCraftingStation);
                recipe.SetResult(this);
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
