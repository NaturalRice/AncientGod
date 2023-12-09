using AncientGod.Boss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Utilities;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Terraria.Localization;
using AncientGod.Utilities;
using AncientGod.Biomes;
using AncientGod.NPCs;
using AncientGod.System;
using Terraria.ModLoader.Utilities;

namespace AncientGod.Boss.RunawayTank
{
    [AutoloadHead]
    public class RunawayTank : ModNPC
    {
        private bool MELDOSAURUSED;
        private int textcounter;
        public int framebuffer = 0;
        public int framecounter = 0;
        public bool firinglaser = false;
        public bool lasercheck = false;
        public int shopnum = 1;
        public int maxshops = 18;
        public List<int> cvitems = new List<int> { };
        Vector2 RunawayTankpos;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public Player Owner => Main.player[NPC.ladyBugGoodLuckTime];//用来表示玩家的

        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, "AncientGod/Boss/RunawayTank/RunawayTank_Shimmer_Head");
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jungle Tyrant");
            Main.npcFrameCount[NPC.type] = 23;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;

            NPCID.Sets.ShimmerTownTransform[Type] = true; // Allows for this NPC to have a different texture after touching the Shimmer liquid.


            // This creates a "profile" for ExamplePerson, which allows for different textures during a party and/or while the NPC is shimmered.
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture),
                new Profiles.DefaultNPCProfile("AncientGod/Boss/RunawayTank/RunawayTank_Shimmer", ShimmerHeadIndex)
            );
            NPC.Happiness
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Like) // Example Person prefers the jungle.
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Hate) // Example Person dislikes the ocean.
                .SetBiomeAffection<MushroomBiome>(AffectionLevel.Love) // Example Person likes the shroom biome.
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Like) // Likes living near the pirate.
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike) // Dislikes living near the tax collector.
            ;
            
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = false;//非友好NPC
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = -1;//第7类NPC，具体类型详见此代码末尾(试试-1)
            NPC.damage = 1;
            NPC.defense = 150;
            NPC.lifeMax = 250000;

            NPC.boss = true;//它tm是个Boss
            Main.npcFrameCount[NPC.type] = 1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.alpha = 1;
            NPC.lavaImmune = true;
            NPC.behindTiles = true;
            NPC.noGravity = false;//因为是坦克，受重力影响
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath3;
            Music = MusicID.LunarBoss;
            NPC.netAlways = true;
            NPC.chaseable = true;

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
            //Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
            //BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.
            SpawnModBiomes = new int[] { ModContent.GetInstance<WasteLand>().Type }; // Associates this NPC with the ExampleSurfaceBiome in Bestiary
        }

        private void EdgyTalk(string text, Color color, bool combatText = false)
        {
            if (combatText)
            {
                CombatText.NewText(NPC.getRect(), color, text, true);
            }
            else
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, color);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    //NetMessage.BroadcastChatMessage(NetworkText.FromKey(text), color);
                }
            }
        }

        public override void AI()
        {
            // 确保 NPC 是敌对的
            NPC.friendly = false;

            Player target = Main.player[NPC.target];//确定目标？
            float lifeRatio = NPC.life / NPC.lifeMax;


            if (!AncientGodWorld.RunawayTank)
            {
                AncientGodWorld.RunawayTank = true;
            }
            if (MELDOSAURUSED || ActiveNPC.RunawayTank)
            {
                NPC.dontTakeDamage = true;
                NPC.dontTakeDamageFromHostiles = true;
                textcounter++;
                NPC.velocity.X *= 0.04f;
                if (textcounter == 1)
                {
                    EdgyTalk(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.EdgyTalk1"), Color.White, true);
                }
                else if (textcounter == 120)
                {
                    EdgyTalk(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.EdgyTalk2"), Color.White, true);
                }
                else if (textcounter == 240 || textcounter == 280 || textcounter == 300 || textcounter == 310 || textcounter == 305 || textcounter == 310 || textcounter == 315 || textcounter == 320)
                {
                    EdgyTalk(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.EdgyTalk3"), Color.DarkGreen, true);
                }
                /*if (textcounter == 360)//这里似乎可以召唤出一些小怪，目前先暂时不加
                {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit1, NPC.position);
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<AprilFools.Meldosaurus.Meldosaurus>());
                    NPC.active = false;
                }*/

            }
        }

        public override List<string> SetNPCNameList() => new List<string>() { "RunawayTank" };
        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override string GetChat()//设置NPC的台词
        {
            AncientGodGlobalNPC.RunawayTank = NPC.whoAmI;
            Player player = Main.player[Main.myPlayer];
            AncientGodPlayer AncientGodPlayer = player.GetModPlayer<AncientGodPlayer>();

            if (!MELDOSAURUSED)
            {
                if (NPC.homeless)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            return Language.GetTextValue("指定没你好果汁吃嗷你等着我~~┗|｀O′|┛ ");

                        case 1:
                            return Language.GetTextValue("敢不敢跟我比划比划？！(╯▔皿▔)╯");

                        default:
                            return Language.GetTextValue("别让我看到你，看到你头给你薅下来！/(ㄒoㄒ)/~~");
                    }
                }

                WeightedRandom<string> dialogue = new WeightedRandom<string>();

                if ((NPC.AnyNPCs(NPCID.LunarTowerNebula) || NPC.AnyNPCs(NPCID.LunarTowerVortex) || NPC.AnyNPCs(NPCID.LunarTowerStardust) || NPC.AnyNPCs(NPCID.LunarTowerSolar)) && Main.rand.NextFloat() < 0.25f)
                {
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.LunarTower"));
                }

                /*if (AncientGod.CalamityActive)
                {
                    if (AncientGodPlayer.CirrusDress)
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.CirrusDress"));
                    }

                    int FAP = NPC.FindFirstNPC(AncientGod.CalamityNPC("FAP"));
                    if (FAP >= 0)
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.FAP"));
                    }

                    int Cal = NPC.FindFirstNPC(AncientGod.CalamityNPC("WITCH"));
                    if (Cal >= 0)
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.WITCH"));
                    }

                    int SEAHOE = NPC.FindFirstNPC(AncientGod.CalamityNPC("SEAHOE"));
                    if (SEAHOE >= 0)
                    {
                        if (Cal >= 0)
                            dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.SEAHOE1"));
                        else
                            dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.SEAHOE2"));
                    }

                    if (NPC.AnyNPCs(AncientGod.CalamityNPC("Draedon")))
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Draedon"));
                    }

                    if (NPC.AnyNPCs(AncientGod.CalamityNPC("THELORDE")))
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.THELORDE"));
                    }

                    if (NPC.AnyNPCs(AncientGod.CalamityNPC("Yharon")))
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Yharon"));
                    }

                    Mod clamMod = AncientGod.Calamity;
                    if (((bool)clamMod.Call("GetBossDowned", "supremecalamitas")) && ((bool)clamMod.Call("GetBossDowned", "exomechs")))
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.GetBossDowned1"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.GetBossDowned2"));
                    }

                    if (player.ownedProjectileCounts[AncientGod.CalamityProjectile("WaterElementalMinion")] > 0)
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.WaterElementalMinion"));
                    }
                }*/

                if (Main.eclipse)
                {
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.eclipse1"));
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.eclipse2"));

                }

                /*if (AncientGod.CalamityActive)
                {
                    if ((bool)AncientGod.Calamity.Call("GetDifficultyActive", "bossrush"))
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.bossrush1"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.bossrush2"));
                    }

                    if ((bool)AncientGod.Calamity.Call("AcidRainActive"))
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.AcidRainActive1"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.AcidRainActive2"));
                    }
                }*/

                if (BirthdayParty.PartyIsUp)
                {
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.PartyIsUp1"));
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.PartyIsUp2"));
                }

                if (!Main.bloodMoon)
                {
                    if (Main.dayTime)
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Day1"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Day2"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Day3"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Day4"));

                    }
                    else
                    {
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Night1"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Night2"));
                        dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.Night3"));
                    }
                }
                else
                {
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.bloodMoon1"));
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.bloodMoon2"));
                    dialogue.Add(Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.bloodMoon3"));
                }
                return dialogue;
            }
            else
            {
                return Language.GetTextValue("Mods.AncientGod.NPCs.RunawayTank.Chat.0");
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (!MELDOSAURUSED)
            {
                if (firstButton)
                {
                    shopName = "RunawayTank" + shopnum;
                }
                else if (!firstButton)
                {
                    if (shopnum < maxshops)
                        shopnum++;
                    else
                        shopnum = 1;
                }
            }
        }
        public override void AddShops()
        {
            for (int i = 0; i < ContentSamples.ItemsByType.Count; i++)
            {
                Item item = ContentSamples.ItemsByType[i];
                // if the item is from CalVal, add it to the shop
                if ((item.ModItem?.Mod ?? null) == AncientGod.instance && item.Name != "Compensation")
                {
                    cvitems.Add(i);
                }
            }

            int progress = 0;

            for (int s = 1; s < maxshops + 1; s++)
            {
                var shope = new NPCShop(Type, "RunawayTank" + s);
                for (int i = 0 + progress; i < 39 + progress; i++)
                {
                    if (i < cvitems.Count)
                    {
                        Item item = ContentSamples.ItemsByType[cvitems[i]];
                        item.value = DecidePrice(item);
                        shope.Add(item);
                    }
                }
                progress += 39;
                shope.Register();
            }
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
        }

        public static int DecidePrice(Item item)
        {
            int price = 0;
            // Ore-HM
            if (item.rare < 4)
            {
                price = Item.buyPrice(gold: 20);
                // Blocks
                if ((item.createTile > 0 || item.createWall > 0) && item.rare == 0)
                {
                    price /= 1000;
                }
            }
            // Post-ML
            else if (item.rare > 10)
            {
                price = Item.buyPrice(gold: 60);
            }
            // Hardmode
            else
            {
                price = Item.buyPrice(gold: 40);
            }
            return price;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)/* tModPorter Suggestion: Copy the implementation of NPC.SpawnAllowed_Merchant in vanilla if you to count money, and be sure to set a flag when unlocked, so you don't count every tick. */
        {
            //确定其生成方式，可能需要添加一条：召唤物生成
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
            return SpawnCondition.Overworld.Chance * 1f;//全天候有概率生成
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return true;
        }

        public override void OnGoToStatue(bool toKingStatue)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)NPC.whoAmI);
                packet.Send();
            }
            else
            {
                StatueTeleport();
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("RunawayTankGore").Type, 1f);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("RunawayTankGore2").Type, 1f);
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("RunawayTankGore3").Type, 1f);
            }
        }

        public void StatueTeleport()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = Main.rand.NextVector2Square(-20, 21);
                if (Math.Abs(position.X) > Math.Abs(position.Y))
                {
                    position.X = Math.Sign(position.X) * 20;
                }
                else
                {
                    position.Y = Math.Sign(position.Y) * 20;
                }
                Dust.NewDustPerfect(NPC.Center + position, 50, Vector2.Zero).noGravity = true;
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 1;
            knockback = 0f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 100;
            randExtraCooldown = 20;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.None;
            attackDelay = 1;
            return;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            randomOffset = 2f;
            multiplier = 24f;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!NPC.IsShimmerVariant)
            {
                if (projectile.type == ProjectileID.VortexBeaterRocket)
                {
                    MELDOSAURUSED = true;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            /*if (MELDOSAURUSED)//这里也和召唤小怪有关，先注释掉
            {
                framebuffer++;
                if (framebuffer >= 6)
                {
                    framecounter++;
                    framebuffer = 0;
                }
                if (framecounter > 5)
                {
                    framecounter = 0;
                }

                Texture2D deusheadsprite = (ModContent.Request<Texture2D>("AncientGod/AprilFools/Meldosaurus/RunawayTanksBane").Value);

                int deusheadheight = framecounter * (deusheadsprite.Height / 6);

                Rectangle deusheadsquare = new Rectangle(0, deusheadheight, deusheadsprite.Width, deusheadsprite.Height / 6);
                Color deusheadalpha = NPC.GetAlpha(drawColor);
                spriteBatch.Draw(deusheadsprite, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), deusheadsquare, deusheadalpha, NPC.rotation, Utils.Size(deusheadsquare) / 2f, NPC.scale, SpriteEffects.None, 0);
                return false;
            }
            else
            {
                return true;
            }*/

            return true;
        }       
    }
}
/*
        所有aiStyle值（一部分）：
        0 - None:
        NPC 不执行任何 AI，通常用于静态的背景 NPC。
        1 - Slime:
        NPC 将表现得像史莱姆，会在玩家附近随机跳跃。
        2 - Fighter:
        NPC 会追踪和攻击玩家。
        3 - Eye:
        NPC 会飞行，并追踪玩家，通常用于眼球类敌人。
        4 - Passive:
        NPC 不主动攻击玩家，通常用于友好的生物。
        5 - Worm:
        NPC 表现得像蠕虫，通常用于蠕虫类敌人。
        6 - Zomber:
        NPC 会追踪和爆炸，通常用于炸弹怪。
        7 - Fighter2:
        NPC 会追踪和攻击玩家，类似于 aiStyle 为 2 的 Fighter。
        8 - Inactive:
        NPC 是不活动的，通常用于静态的生物或物体。
        9 - Caster:
        NPC 会使用魔法攻击，通常用于魔法类敌人。
        10 - Passive:
        NPC 不主动攻击玩家，类似于 aiStyle 为 4 的 Passive。
        11 - Hovering:
        NPC 悬浮在空中，通常用于飞行敌人。
        12 - Thrown:
        NPC 会投掷物体，通常用于投掷类敌人。
        13 - Crab:
        NPC 会追踪和攻击玩家，类似于 aiStyle 为 2 的 Fighter。
        14 - Fighter3:
        NPC 会追踪和攻击玩家，类似于 aiStyle 为 2 的 Fighter。
        15 - Inactive:
        NPC 是不活动的，类似于 aiStyle 为 8 的 Inactive。
        16 - Inactive:
        NPC 是不活动的，类似于 aiStyle 为 8 的 Inactive。
        17 - Worm:
        NPC 表现得像蠕虫，类似于 aiStyle 为 5 的 Worm。
        18 - Passive:
        NPC 不主动攻击玩家，类似于 aiStyle 为 4 的 Passive。
        19 - Passive:
        NPC 不主动攻击玩家，类似于 aiStyle 为 4 的 Passive。       
         */
