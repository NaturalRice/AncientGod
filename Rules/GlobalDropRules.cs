using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;
using Terraria.GameContent.ItemDropRules;

namespace AncientGod.Rules
{
    internal class GlobalDropRules : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {//加入影子和月噬的掉落规则
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonDevoration>(),8,1,5));//月噬，普通材料，掉落概率1/8
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Shadow>(), 100, 1, 2));//影子，稀有材料，掉落概率1/100
        }
    }
}
