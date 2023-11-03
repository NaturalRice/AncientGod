using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.System;
namespace AncientGod.NPCs
{
    public class Guide : GlobalNPC
    {
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return npc.type == NPCID.Guide;
        }
        public override ITownNPCProfile ModifyTownNPCProfile(NPC npc)
        {
            if (ActiveNPC.Guide)
            {
                npc.townNPC = true;
                npc.active = true;
                return null;
            }
            npc.townNPC = false;
            npc.active = false;
            return base.ModifyTownNPCProfile(npc);
        }
        public override void AI(NPC npc)
        {
        }
    }
}
