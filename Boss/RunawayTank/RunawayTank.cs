using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using AncientGod.Biomes;
using AncientGod.NPCs;
using AncientGod.System;
using Terraria.ModLoader.Utilities;
using AncientGod.Items.Boss;

namespace AncientGod.Boss.RunawayTank
{
    public class RunawayTank : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
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
				new FlavorTextBestiaryInfoElement("Runaway Tank"),
            });
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.InModBiome(ModContent.GetInstance<WasteLand>()))
            {
                return 0;
            }
            if (!ActiveNPC.RunawayTank)
            {
                return 0;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<RunawayTank>()))
            {
                return 0;
            }
            // 检查玩家是否使用了特定的召唤物(似乎不起作用）
            Player player = spawnInfo.Player;
            Item item = player.HeldItem;

            // 替换 YourSummonItem 为你实际使用的召唤物的 Item 类型
            if (item.type == ModContent.ItemType<RunawayTankKey>())
            {
                // 这里可以根据需要调整生成概率
                return 1;
            }

            return SpawnCondition.Overworld.Chance * 0.6f;
        }
        public override void OnKill()
        {
            if (ActiveNPC.RunawayTank)
            {
                ActiveNPC.RunawayTank = false;
            }
        }
    }
}
