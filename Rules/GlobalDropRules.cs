﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;
using Terraria.GameContent.ItemDropRules;
using AncientGod.Items.Weapons;

namespace AncientGod.Rules
{
    internal class GlobalDropRules : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {//加入影子和月噬的掉落规则
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonDevoration>(),8,1,5));
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Shadow>(), 8, 1, 5));
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Refrigerant>(), 8, 1, 5));
             npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordOfAG>(), 10000, 1, 1));//万分之一概率掉落
        }
    }
}
