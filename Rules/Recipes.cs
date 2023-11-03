using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod;

namespace AncientGod.Rules
{
    public class Recipes :ModSystem
    {
        public void AddRecipe()
        {
            //雨天拿空桶合成水桶
            Recipe.Create(ItemID.WaterBucket, 1)
                .AddCondition(Condition.InRain)
                .AddIngredient(ItemID.EmptyBucket)
                .Register();
        }
    }
}
