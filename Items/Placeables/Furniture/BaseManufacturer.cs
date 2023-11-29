using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Placeables.Furniture;
using Terraria;

namespace AncientGod.Items.Placeables.Furniture
{
    public class BaseManufacturer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Auric Manufacturer");
            // Tooltip.SetDefault("Used for special crafting");
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
            Item.createTile = ModContent.TileType<Tiles.Furniture.BaseManufacturer>();
            Item.width = 46;
            Item.height = 24;
            Item.rare = 0;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            Mod calamityMod = ModLoader.GetMod("CalamityMod");
            if (calamityMod != null)
            {
                recipe.AddIngredient(calamityMod.ItemType("AuricBar"), 10);
                recipe.AddIngredient(calamityMod.ItemType("AscendantSpiritEssence"), 4);
                recipe.AddTile(calamityMod.TileType("CosmicAnvil"));
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
