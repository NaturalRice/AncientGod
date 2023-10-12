using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AcientGod.Items;

namespace AcientGod.Items.Materials
{
    internal class MoonDevoration : ModItem
    {
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 16;
            Item.height = 16;
        }
    }
}
