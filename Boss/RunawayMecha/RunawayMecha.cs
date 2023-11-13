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
//using Terraria.World.Generation;

namespace AncientGod.Boss.RunawayMecha
{
    [AutoloadBossHead]
    public class RunawayMecha : ModNPC
    {
        public override void SetStaticDefaults()
        {
            if (!AncientGod.AprilFoolMonth)
            {
                NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
                {
                    Hide = true
                };
                NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            }
        }
        public override void SetDefaults()
        {
            NPC.damage = 6;
            NPC.width = 120;
            NPC.height = 134;
            NPC.defense = 10;
            NPC.lifeMax = 650;//血量
            NPC.boss = true;
            NPC.aiStyle = -1;
            Main.npcFrameCount[NPC.type] = 1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.alpha = 1;
            NPC.lavaImmune = true;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath3;
            Music = MusicID.LunarBoss;
            NPC.netAlways = true;
            NPC.chaseable = false;
        }
        public override void SetBestiary(Terraria.GameContent.Bestiary.BestiaryDatabase database, Terraria.GameContent.Bestiary.BestiaryEntry bestiaryEntry)
        {
            if (AncientGod.AprilFoolMonth)
            {
                bestiaryEntry.Info.AddRange(new Terraria.GameContent.Bestiary.IBestiaryInfoElement[] {
                Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new Terraria.GameContent.Bestiary.FlavorTextBestiaryInfoElement("The soul the Death God, we call it the RunawayMecha. Although one may logically assume that a god possesses two souls when they consume an Auric Soul, this is not the case. Rather, they fuse into something greater than the sum of its parts."),
            });
            }
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            float lifeRatio = NPC.life / NPC.lifeMax;
            if (!Main.LocalPlayer.GetModPlayer<AncientGodPlayer>().Blok)
            {
                NPC.active = false;
            }
            if (Main.dayTime && NPC.life <= NPC.lifeMax * 0.5f)
            {
                for (int k = 0; k < 50; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 16, 0, -1f, 0, default, 1f);
                NPC.active = false;

                Main.NewText("You foolish mortal, you may only witness me at my fullest at the witching hour...", Color.Gray);

            }
            NPC.localAI[0]++;
            if (NPC.localAI[0] >= 600 && NPC.ai[0] == 1)
            {
                Vector2 dist = target.Center - NPC.Center;
                float yRand = 0;
                float xRand = 0;
                while (xRand == 0)
                {
                    xRand = Main.rand.Next(-1, 2);
                }
                while (yRand == 0)
                {
                    yRand = Main.rand.Next(-1, 2);
                }
                for (int k = 0; k < 50; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 16, 0, -1f, 0, default, 1f);
                SoundEngine.PlaySound(SoundID.Item20);
                NPC.Center = new Vector2((int)(target.Center.X + dist.X * xRand), (int)(target.Center.Y - dist.Y * yRand));
                NPC.localAI[0] = 0;
            }
            switch (NPC.ai[0])
            {
                case 0:
                    {
                        NPC.damage = 0;
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = -10;
                        NPC.ai[1]++;
                        if (NPC.ai[1] > 60)
                        {
                            ChangeAI(1);
                        }
                    }
                    break;
                case 1:
                    {
                        NPC.damage = 6;
                        float fistGate = NERBValues(20, 15, 10, 7, 5);
                        Vector2 distanceFromDestination = target.Center - NPC.Center;

                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.position) * 7, 0.1f);
                        NPC.ai[1]++;
                        NPC.ai[2]++;
                        NPC.rotation = NPC.rotation.AngleLerp(0, 0.2f);
                        if (NPC.ai[1] >= fistGate)
                        {
                            FistMeDaddy();
                            NPC.ai[1] = 0;
                        }
                        if (NPC.ai[2] >= 300)
                        {
                            ChangeAI(2);
                        }
                    }
                    break;
                case 2:
                    {
                        float phaseTime = Math.Clamp(100 - (lifeRatio * 100), 10, 10000);
                        float chargeSpeed = 20 + (10 - (lifeRatio * 10));
                        if (NPC.ai[1] == 0)
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar);
                            NPC.velocity = NPC.DirectionTo(target.position) * chargeSpeed;
                        }
                        NPC.ai[1]++;
                        NPC.rotation += NPC.velocity.X > 0 ? 5 : -5;
                        if (NPC.ai[1] >= phaseTime)
                        {
                            ChangeAI(1);
                        }
                    }
                    break;
                case 3:
                    {
                        NPC.velocity *= 0f;
                        if (NPC.ai[1] == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, AncientGod.instance.Find<ModProjectile>("FogLaser").Type, NPC.damage * 2, 0f, Main.myPlayer, NPC.whoAmI);
                                if (p < Main.maxProjectiles + 1)
                                {
                                    // Shoot laser either to the left or right of the player
                                    float offset = Main.rand.NextBool(2) ? MathHelper.PiOver2 : -MathHelper.PiOver2;
                                    // Rotate left and right respectively
                                    Main.projectile[p].ai[1] = offset == MathHelper.PiOver2 ? 0 : 1;
                                    Main.projectile[p].rotation = NPC.Center.AngleTo(target.Center) + offset;
                                    NPC.localAI[2] = p;
                                }
                            }
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                Projectile proj = Main.projectile[i];
                                if (proj.type == ModContent.ProjectileType<RunawayMechaFist>())
                                {
                                    for (int j = 0; j < 20; j++)
                                    {
                                        Dust.NewDust(proj.position, proj.width, proj.height, 16, 0, -1f, 0, default, 1f);
                                    }
                                    proj.active = false;
                                }
                            }
                        }
                        else if (NPC.ai[1] < 120)
                        {
                            NPC.rotation = NPC.rotation.AngleLerp(0, 0.4f);
                            int num5 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 16, 0f, 0f, 200, default, 1.5f);
                            Main.dust[num5].noGravity = true;
                            Main.dust[num5].velocity *= 0.75f;
                            Main.dust[num5].fadeIn = 1.3f;
                            Vector2 vector = new Vector2((float)Main.rand.Next(-200, 201), (float)Main.rand.Next(-200, 201));
                            vector.Normalize();
                            vector *= (float)Main.rand.Next(100, 200) * 0.04f;
                            Main.dust[num5].velocity = vector;
                            vector.Normalize();
                            vector *= 34f;
                            Main.dust[num5].position = NPC.Center - vector;
                        }
                        else if (NPC.ai[1] == 120)
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                        }
                        NPC.ai[1]++;
                        if (NPC.ai[1] >= 480)
                        {
                            ChangeAI(2);
                        }
                    }
                    break;
            }
        }

        public void ShakeScreen()
        {
            float screenShakePower = 20 * Utils.GetLerpValue(1300f, 0f, NPC.Distance(Main.LocalPlayer.Center), true);
        }

        public static int NERBValues(int? normal = null, int? expert = null, int? rev = null, int? death = null, int? malice = null)
        {
            if (Main.expertMode)
            {
                if (expert != null)
                    return (int)expert;
                else
                    return (int)normal;
            }
            else
            {
                return (int)normal;
            }
        }

        public void ChangeAI(int phase)//改变AI模式
        {
            if (NPC.ai[0] == 1 && !Main.dayTime && NPC.life <= NPC.lifeMax * 0.5f)
            {
                NPC.ai[0] = 3;
            }
            else
            {
                NPC.ai[0] = phase;
            }
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            NPC.TargetClosest();
        }

        public void FistMeDaddy()
        {
            float startDist = Main.rand.NextFloat(1260f, 1270f);
            Vector2 startDir = Main.rand.NextVector2Unit();
            Vector2 startPoint = Main.player[NPC.target].Center + (startDir * startDist);

            float speed = 10;
            Vector2 velocity = startDir * (-speed);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), startPoint, velocity, ModContent.ProjectileType<RunawayMechaFist>(), NPC.damage / 2, 0, Main.myPlayer, NPC.whoAmI);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.55f * balance);
            NPC.damage = (int)(NPC.damage * 0.65f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Darkness, 120, true);
                if (Main.zenithWorld)
                {
                    target.AddBuff(BuffID.Obstructed, 120, true);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D fisttexture = ModContent.Request<Texture2D>("AncientGod/AprilFools/RunawayMechaFist").Value;
            Texture2D glowtexture = NPC.ai[0] == 3 ? ModContent.Request<Texture2D>("AncientGod/AprilFools/RunawayMechaGlowEnrage").Value : ModContent.Request<Texture2D>("AncientGod/AprilFools/RunawayMechaGlow").Value;
            Player target = Main.player[NPC.target];
            Vector2 dist = NPC.Center - target.Center;
            Rectangle frameRectangle = texture.Frame(1, 1, 0, 0);
            List<Vector2> points = new List<Vector2>
            {
                NPC.Center - Main.screenPosition,
                NPC.Center - Main.screenPosition + -2 * dist.Y * Vector2.UnitY,
                NPC.Center - Main.screenPosition + -2 * dist.Y * Vector2.UnitY + -2 * dist.X * Vector2.UnitX,
                NPC.Center - Main.screenPosition + -2 * dist.X * Vector2.UnitX
            };
            for (int i = 0; i < points.Count; i++)
            {
                float rot = i > 1 ? -NPC.rotation : NPC.rotation;
                SpriteEffects dir = dist.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically;
                if (i > 1)
                {
                    dir = dist.X >= 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically;
                }
                spriteBatch.Draw(texture, points[i], null, drawColor, rot, frameRectangle.Size() / 2, 1f, SpriteEffects.None, 0);
                if (NPC.life <= NPC.lifeMax * 0.5f)
                {
                    spriteBatch.Draw(fisttexture, points[i] + Vector2.UnitX * 180 + Vector2.UnitY * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5) * 16, null, drawColor, rot, frameRectangle.Size() / 2, 1f, dir, 0);
                    spriteBatch.Draw(fisttexture, points[i] - Vector2.UnitX * 120 + Vector2.UnitY * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5) * 16, null, drawColor, rot, frameRectangle.Size() / 2, 1f, dir, 0);
                }
                spriteBatch.Draw(glowtexture, points[i], null, Color.White, rot, frameRectangle.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return Main.expertMode ? false : true;
        }

        public override void OnKill()
        {
            AncientGodWorld.downedRunawayMecha = true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)//添加的生成条件以及概率（有召唤物，先不加了）
        {
            if (!spawnInfo.Player.InModBiome(ModContent.GetInstance<WasteLand>()))
            {
                return 0;
            }
            if (!ActiveNPC.RunawayMecha)
            {
                return 0;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<RunawayMecha>()))
            {
                return 0;
            }
            return SpawnCondition.OverworldNight.Chance * 0.9f;
        }
    }
}

