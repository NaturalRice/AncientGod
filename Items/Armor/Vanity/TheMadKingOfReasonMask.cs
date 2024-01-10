using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class TheMadKingOfReasonMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 28;

            // Common values for every boss mask
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);
            Item.vanity = true;
            Item.maxStack = 1;
        }

        /*public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddTile(TileID.Campfire);//加入合成站（这里为了有趣我改成了篝火）
            recipe.Register();
        }*/
    }
}
