using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;

namespace AncientGod.Items.Materials
{
    internal class Refrigerant : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.White;
        }
        public override void AddRecipes()
        {
            //水边合成冰块雪块
            Recipe.Create(ItemID.IceBlock)
                .AddCondition(Condition.NearWater)
                .AddIngredient<Refrigerant>(1)
                .Register();
            Recipe.Create(ItemID.SnowBlock)
                .AddCondition(Condition.InRain)
                .AddIngredient<Refrigerant>(3)
                .Register();
        }
    }
}
