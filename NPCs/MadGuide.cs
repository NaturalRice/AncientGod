using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;
using AncientGod.Biomes;
using AncientGod.NPCs;
using AncientGod.System;

namespace AncientGod.NPCs
{
	//The ExampleZombieThief is essentially the same as a regular Zombie, but it steals ExampleItems and keep them until it is killed, being saved with the world if it has enough of them.
	public class MadGuide : ModNPC
	{
		public int StolenItems = 0;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
			{
				// Influences how the NPC looks in the Bestiary
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.width = 18;
			NPC.height = 40;
			NPC.damage = 30;
			NPC.defense = 15;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = 3; // Fighter AI, important to choose the aiStyle that matches the NPCID that we want to mimic

			AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
			AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
			//Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
			//BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.
			SpawnModBiomes = new int[] { ModContent.GetInstance<WasteLand>().Type }; // Associates this NPC with the ExampleSurfaceBiome in Bestiary
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("The mad Guide"),
			});
		}
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.InModBiome(ModContent.GetInstance<WasteLand>()))
            {
                return 0;
            }
            if (!ActiveNPC.MadGuide)
            {
                return 0;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<MadGuide>()))
            {
                return 0;
            }
            return SpawnCondition.OverworldNight.Chance * 0.1f;
        }

        public override void OnKill()
		{
			if (ActiveNPC.MadGuide)
			{
				ActiveNPC.MadGuide = false;
				ActiveNPC.Guide = true;
				NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.position.X, (int)NPC.position.Y, NPCID.Guide);
			}
		}
	}
}
		