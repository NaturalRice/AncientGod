using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AcientGod.Items.Materials;
using Terraria.GameContent.ItemDropRules;

namespace AcientGod.Rules
{
    internal class GlobalDropRules : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonDevoration>(),8,1,5));//1/8的概率掉落1-5个月噬
        }
    }
}
