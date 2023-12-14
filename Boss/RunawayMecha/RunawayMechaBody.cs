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
using static Humanizer.In;
using Terraria.DataStructures;
using AncientGod.Content.BossBars;//圣遗物
using AncientGod.Items;
using AncientGod.Items.Armor.Vanity;//特色服装暂不考虑
using AncientGod.Items.Consumables;//可消耗召唤物
using AncientGod.Projectiles.Pets.MinionBossPet;
using AncientGod.Projectiles;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using AncientGod.Items.Boss;
using System.Security.Cryptography.X509Certificates;
using AncientGod.Projectiles.Ammo;
using Mono.Cecil;
using AncientGod.Projectiles.Pets.RunawayMecha;
//using Terraria.World.Generation;

namespace AncientGod.Boss.RunawayMecha
{
    [AutoloadBossHead]//显示Boss血条以及在大地图中显示Boss头像
    public class RunawayMechaBody : ModNPC
    {
        //public override float TeleportThreshold => 1200f;
        //public override Vector2 FlyingOffset => new Vector2(0f, 0f);//这里决定了飞行目标

        public int owner; // 自定义字段，用于存储投射物的源

        private const float TargetDistance = 1000f; // 设置寻找敌人的最大距离
        private int target = -1; // 记录当前目标敌人的索引
        private float fireCooldown = 240f; // 弹药冷却时间

        bool isInfernumActive;//用于标记Infernum模式是否激活
        Mod infern;//Mod类型的变量，用于引用Infernum模组
       
        //A list of the ideal positions for each arm. The first 2 variables of the Vector2 represent the relative position of the arm to the body and the last variable represents the rotation of the hand
        internal readonly List<Vector3> IdealPositions = new List<Vector3>()//一个列表，其中包含了四个Vector3元素，每个元素表示一个机械臂的理想位置。这些位置包括相对于机械臂中心的X和Y偏移，以及手的旋转角度
        {
            new Vector3(-170f, 50f, MathHelper.ToRadians(240)), //Top left arm  此处调整四个大臂的位置
            new Vector3(-260f, 260f, MathHelper.ToRadians(270)), //Bottom left arm
            
            new Vector3(260f, 260f, MathHelper.ToRadians(270)), //Bottom right arm
            new Vector3(170f, 50f, MathHelper.ToRadians(300)) //Top right arm
        };

        private Vector2 GetIdealPosition(int i) => NPC.Center + new Vector2(IdealPositions[i].X, IdealPositions[i].Y);

        private Vector2 BobVector => new Vector2(0, -2 + 4 * MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi), 0, 1) * 0.7f + 0.3f);
        //一个Vector2，用于模拟机械臂上下浮动的动画效果

        private Vector2 Forearmposition;//用于不同函数间传值
        private Vector2 Handposition;//用于不同函数间传值
        private Vector2 Armposition;//用于不同函数间传值

        private List<Vector3> ArmPositions;
        //一个列表，用于存储机械臂的位置信息

        public ref float Initialized => ref NPC.ai[0];       

        public Player player => Main.player[NPC.target];//将Owner改成player

        private const int FirstStageTimerMax = 90;
        // This is a reference property. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own name
        public ref float FirstStageTimer => ref NPC.localAI[1];
        public Vector2 FirstStageDestination
        {
            get => new Vector2(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }

        public int MinionMaxHealthTotal
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        public int MinionHealthTotal { get; set; }
        public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;




        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            /*NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "AncientGod/Assets/Bestiary/RunawayMecha",
                PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);*/
        }

        public override void SetDefaults()
        {
            NPC.damage = 20;
            NPC.width = 120;
            NPC.height = 134;
            NPC.defense = 10;
            NPC.lifeMax = 65000;//血量
            NPC.boss = true;
            NPC.aiStyle = -1;
            Main.npcFrameCount[NPC.type] = 4;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.alpha = 1;
            NPC.lavaImmune = true;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.GravityIgnoresLiquid = true;
            NPC.downedEmpressOfLight = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("敢不敢跟我比划比划？！")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<RunawayMechaBag>()));

            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeables.Furniture.RunawayMechaTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeables.Furniture.RunawayMechaRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RunawayMechaKey>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RunawayMechaMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined
            int itemType = ModContent.ItemType<ExampleItem>();
            var parameters = new DropOneByOne.Parameters()
            {
                ChanceNumerator = 1,
                ChanceDenominator = 1,
                MinimumStackPerChunkBase = 1,
                MaximumStackPerChunkBase = 1,
                MinimumItemDropsCount = 12,
                MaximumItemDropsCount = 15,
            };

            notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            // Here you'd want to change the potion type that drops when the boss is defeated. Because this boss is early pre-hardmode, we keep it unchanged
            // (Lesser Healing Potion). If you wanted to change it, simply write "potionType = ItemID.HealingPotion;" or any other potion type
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
            // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
            int startFrame = 0;
            int finalFrame = 3;

            int frameSpeed = 5;
            NPC.frameCounter += 0.5f;
            NPC.frameCounter += NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            // If the NPC dies, spawn gore and play a sound
            if (Main.netMode == NetmodeID.Server)
            {
                // We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
                return;
            }

            if (NPC.life <= 0)
            {
                // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
                int backGoreType = Mod.Find<ModGore>("RunawayMechaBody_Back").Type;
                int frontGoreType = Mod.Find<ModGore>("RunawayMechaBody_Front").Type;

                var entitySource = NPC.GetSource_Death();

                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), backGoreType);
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), frontGoreType);
                }

                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                // This adds a screen shake (screenshake) similar to Deerclops
                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(modifier);
            }
        }

        private void FindTarget()
        {
            target = -1; // 默认值，表示没有找到目标
            float maxDistance = TargetDistance; // 设置最大寻找距离

            for (int i = 0; i < Main.player.Length; i++)
            {
                // 检查敌人是否活着、活跃，并且在最大寻找范围内
                if (player.active && Vector2.Distance(player.Center, NPC.Center) < maxDistance)
                {
                    maxDistance = Vector2.Distance(player.Center, NPC.Center);
                    target = i; // 更新目标索引
                }
            }
        }

        public override void AI()
        {
            // This should almost always be the first code in AI() as it is responsible for finding the proper player target
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(10);
                return;
            }


            if (Initialized == 0)
            {
                //Initialize the arm positions
                ArmPositions = new List<Vector3>(4);
                for (int i = 0; i < 4; i++)
                {
                    ArmPositions.Add(new Vector3(GetIdealPosition(i), IdealPositions[i].Z));
                }
                Initialized = 1f;
            }

                  

            if (ArmPositions != null)
            {
                for (int i = 0; i < ArmPositions.Count; i++)
                {
                    //Make the arm lag behind a bit
                    Vector3 armPosition = ArmPositions[i];
                    Vector2 idealArmPosition = GetIdealPosition(i);
                    Vector2 vector2position = new Vector2(armPosition.X, armPosition.Y);
                    //vector2position 用于确定机械臂的位置，即机械臂的上部（臂部）的位置。这个位置是根据理想位置、浮动效果和其他变化因素计算出来的。
                    //vector2position 在接下来的计算中用于控制机械臂的位置，并使其在上下浮动以及跟踪鼠标光标时具有平滑的运动效果。这有助于模拟机械臂的运动和定位。
                    //vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.2f) + BobVector;//如果机械臂相对位置摆动得太逆天，可能要禁用这个动画效果
                    vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.6f);//手部位置鬼畜的问题出在这里的Lerp函数；经过多次测试，目前此数值0.6最适合

                    //Make the cannon look at the mouse cursor if its close enough 这里改成瞄准玩家
                    armPosition.Z = Utils.AngleLerp(armPosition.Z, IdealPositions[i].Z, MathHelper.Clamp((player.position - vector2position).Length() / 300f, 0, 1));
                    armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - player.position).ToRotation(), 1 - MathHelper.Clamp((player.position - vector2position).Length() / 300f, 0, 1));

                    ArmPositions[i] = new Vector3(vector2position, armPosition.Z);

                    // 寻找附近的敌人
                    FindTarget(); // 调用FindTarget方法来更新target值

                    if (target != -1)
                    {

                        //手部瞄准敌人                        
                        Vector2 direction = player.Center - NPC.position;


                        armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - direction).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));

                        direction.Normalize();

                        NPC.velocity = direction * 16f;



                        // 设置投射物的旋转角度
                        //armPosition.Z = direction.ToRotation();

                        if (fireCooldown <= 0f)
                        {
                            // 发射子弹
                            if (NPC.localAI[0] == 0f)
                            {
                                NPC.localAI[0] = 1f;//Projectile.localAI[0] 设置为1，表示已经发射过，以避免连续的发射。

                                // 计算发射角度，这里示例为向下发射
                                float shootAngle = direction.ToRotation();

                                // 发射弹药
                                for (int j = 0; j < 1; j++) // 你可以根据需要发射多个弹药（这里不是间隔的连射，而是一次性射多少弹药）
                                {
                                    direction = player.Center - (NPC.position + Vector2.UnitX * 170 + Vector2.UnitY * 150);
                                    shootAngle = direction.ToRotation();
                                    Vector2 shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f; // 这里示例为向右发射(底下是发射源的定义），瞄准方向
                                    int newProjectile11 = Projectile.NewProjectile(null, NPC.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, shotVelocity, ModContent.ProjectileType<RunawayMechaBullet>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//右下
                                    //int newProjectile12 = Projectile.NewProjectile(null, NPC.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//右下
                                    //int newProjectile13 = Projectile.NewProjectile(null, NPC.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//右下
                                    direction = player.Center - (NPC.position - Vector2.UnitX * 140 + Vector2.UnitY * 50);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    int newProjectile21 = Projectile.NewProjectile(null, NPC.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, shotVelocity, ModContent.ProjectileType<RunawayMechaBullet>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//左上
                                    //int newProjectile22 = Projectile.NewProjectile(null, NPC.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//左上
                                    //int newProjectile23 = Projectile.NewProjectile(null, NPC.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//左上
                                    direction = player.Center - (NPC.position + Vector2.UnitX * 195 + Vector2.UnitY * 50);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    int newProjectile31 = Projectile.NewProjectile(null, NPC.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, shotVelocity, ModContent.ProjectileType<RunawayMechaBullet>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//右上
                                    //int newProjectile32 = Projectile.NewProjectile(null, NPC.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//右上
                                    //int newProjectile33 = Projectile.NewProjectile(null, NPC.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//右上
                                    direction = player.Center - (NPC.position - Vector2.UnitX * 120 + Vector2.UnitY * 150);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    int newProjectile41 = Projectile.NewProjectile(null, NPC.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, shotVelocity, ModContent.ProjectileType<RunawayMechaBullet>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //int newProjectile42 = Projectile.NewProjectile(null, NPC.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //int newProjectile43 = Projectile.NewProjectile(null, NPC.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<BigBangMechaBulletSide>(), (int)(NPC.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //这里四个机关炮的准心有一定差别
                                    Main.projectile[newProjectile11].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile12].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile13].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile21].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile22].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile23].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile31].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile32].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile33].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile41].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile42].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile43].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile11].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile12].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile13].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile21].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile22].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile23].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile31].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile32].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile33].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile41].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile42].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile43].netUpdate = true;//是否将投射物的信息同步到其他客户端


                                    shootAngle += MathHelper.PiOver4; // 增加弹药之间的间隔角度
                                }

                                NPC.localAI[0] = 0f;//为了连射

                                // 重置冷却计时器
                                fireCooldown = 30f; // 设置为你想要的冷却时间（间隔时间0.5秒）
                            }
                        }
                        else
                        {
                            // 更新冷却计时器
                            fireCooldown--;
                        }
                    }
                    else
                    {
                        // 当没有目标时，保持静止
                        NPC.velocity = Vector2.Zero;
                        NPC.localAI[0] = 0f; // 重置发射标记
                    }
                }
            }



            DoFirstStage(player);

        }

        private void DoFirstStage(Player player)
        {
            // Each time the timer is 0, pick a random position a fixed distance away from the player but towards the opposite side
            // The NPC moves directly towards it with fixed speed, while displaying its trajectory as a telegraph

            FirstStageTimer++;
            if (FirstStageTimer > FirstStageTimerMax)
            {
                FirstStageTimer = 0;
            }

            float distance = 200; // Distance in pixels behind the player

            if (FirstStageTimer == 0)
            {
                Vector2 fromPlayer = NPC.Center - player.Center;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Important multiplayer consideration: drastic change in behavior (that is also decided by randomness) like this requires
                    // to be executed on the server (or singleplayer) to keep the boss in sync

                    float angle = fromPlayer.ToRotation();
                    float twelfth = MathHelper.Pi / 6;

                    angle += MathHelper.Pi + Main.rand.NextFloat(-twelfth, twelfth);
                    if (angle > MathHelper.TwoPi)
                    {
                        angle -= MathHelper.TwoPi;
                    }
                    else if (angle < 0)
                    {
                        angle += MathHelper.TwoPi;
                    }

                    Vector2 relativeDestination = angle.ToRotationVector2() * distance;

                    FirstStageDestination = player.Center + relativeDestination;
                    NPC.netUpdate = true;
                }
            }

            // Move along the vector
            Vector2 toDestination = FirstStageDestination - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
            float speed = Math.Min(distance, toDestination.Length());
            NPC.velocity = toDestinationNormalized * speed / 30;

            if (FirstStageDestination != LastFirstStageDestination)
            {
                // If destination changed
                NPC.TargetClosest(); // Pick the closest player target again

                // "Why is this not in the same code that sets FirstStageDestination?" Because in multiplayer it's ran by the server.
                // The client has to know when the destination changes a different way. Keeping track of the previous ticks' destination is one way
                if (Main.netMode != NetmodeID.Server)
                {
                    // For visuals regarding NPC position, netOffset has to be concidered to make visuals align properly
                    NPC.position += NPC.netOffset;

                    // Draw a line between the NPC and its destination, represented as dusts every 20 pixels
                    Dust.QuickDustLine(NPC.Center + toDestinationNormalized * NPC.width, FirstStageDestination, toDestination.Length() / 20f, Color.Yellow);

                    NPC.position -= NPC.netOffset;
                }
            }
            LastFirstStageDestination = FirstStageDestination;

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Darkness, 100, true);
                if (Main.zenithWorld)
                {
                    target.AddBuff(BuffID.Obstructed, 100, true);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)//用于在绘制前执行额外的逻辑。其中，DrawChain 方法用于绘制链条效果
        {
            //DrawChain();//链条暂时未使用


            if (ArmPositions == null)
                return true;

            Texture2D teslaTex = (ModContent.Request<Texture2D>("AncientGod/Boss/RunawayMecha/RunawayMechaTesla")).Value;
            Texture2D laserTex = (ModContent.Request<Texture2D>("AncientGod/Boss/RunawayMecha/RunawayMechaLaser")).Value;
            Texture2D nukeTex = (ModContent.Request<Texture2D>("AncientGod/Boss/RunawayMecha/RunawayMechaNuke")).Value;
            Texture2D plasmaTex = (ModContent.Request<Texture2D>("AncientGod/Boss/RunawayMecha/RunawayMechaPlasma")).Value;
            ModLoader.TryGetMod("InfernumMode", out infern);
            if (infern != null)
            {
                if ((bool)infern.Call("GetInfernumActive"))
                {
                    isInfernumActive = true;
                }
                else
                {
                    isInfernumActive = false;
                }
            }

            //Upper arms
            DrawSingleArm(laserTex, new Vector2(ArmPositions[0].X, ArmPositions[0].Y), ArmPositions[0].Z, new Vector2(0, -10), true);
            DrawSingleArm(nukeTex, new Vector2(ArmPositions[3].X, ArmPositions[3].Y), ArmPositions[3].Z, new Vector2(0, -10), true, isInfernumActive);


            //Lower arms
            DrawSingleArm(teslaTex, new Vector2(ArmPositions[1].X, ArmPositions[1].Y), ArmPositions[1].Z, Vector2.Zero, false);
            DrawSingleArm(plasmaTex, new Vector2(ArmPositions[2].X, ArmPositions[2].Y), ArmPositions[2].Z, Vector2.Zero, false);

            return true;
        }
        public void DrawSingleArm(Texture2D handTex, Vector2 handPosition, float rotation, Vector2 offset, bool top, bool infernum = false)
        {
            //用于绘制机械臂的各个部分，包括上臂、下臂和手部。
            //这里，handPosition 是通过 Projectile.Center（机械臂的中心）加上 position 乘以一个缩放系数来确定的。
            //handPosition 用于确定机械臂的手部位置，也就是机械臂的手部的坐标。
            //position 是一个矢量，代表手部相对于机械臂中心的偏移。通过将这个偏移乘以 0.1f，你可以调整手部相对于机械臂中心的位置。
            //这个值的改变可以影响手部的距离，使它更接近或更远离机械臂中心。
            Vector2 position = handPosition - NPC.Center;
            bool flipped = Math.Sign(position.X) != -1;

            Texture2D armTex = ModContent.Request<Texture2D>("AncientGod/Boss/RunawayMecha/RunawayMechaArm").Value;//大臂
            Texture2D forearmTex = ModContent.Request<Texture2D>("AncientGod/Boss/RunawayMecha/RunawayMechaForearm").Value;//小臂

            Rectangle armFrame = new Rectangle(0, top ? 0 : 59, armTex.Width, 60);
            Rectangle handFrame = new Rectangle(0, infernum ? 40 : 0, 60, 38);

            Vector2 armOrigin = new Vector2(flipped ? 0 : armFrame.Width, armFrame.Height / 3);//这里由除2改成除3
            Vector2 forearmOrigin = new Vector2(flipped ? 0 : forearmTex.Width, forearmTex.Height);//这里去掉除2
            Vector2 handOrigin = new Vector2(flipped ? 28 : 22, 22); //22

            Vector2 armPosition = NPC.Center + position * 0.1f;

            //Do some trigonometry to get the elbow position

            float armLenght = 175;
            float directLenght = MathHelper.Clamp((handPosition - armPosition).Length(), 0, armLenght); //Clamp the direct lenght to avoid getting an error from trying to calculate the square root of a negative number
            float elbowElevation = (float)Math.Sqrt(Math.Pow(armLenght / 2f, 2) - Math.Pow(directLenght / 2f, 2));
            Vector2 elbowPosition = Vector2.Lerp(handPosition, armPosition, 0.5f) + Utils.SafeNormalize(position, Vector2.Zero).RotatedBy(-MathHelper.PiOver2 * Math.Sign(position.X)) * elbowElevation;

            float armAngle = (elbowPosition - armPosition).ToRotation();
            float forearmAngle = (handPosition - elbowPosition).ToRotation();

            //这里，forearmPosition 是通过 elbowPosition 和 forearmAngle 计算得到的。首先，通过 elbowPosition 加上 forearmAngle 方向的矢量来确定下臂的位置。
            //forearmAngle 是下臂与手部之间的角度，它是通过(handPosition - elbowPosition).ToRotation() 计算的。
            Vector2 forearmPosition = elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40) / 2f);

            if (forearmPosition.Y > player.position.Y)//如果是下方的大臂则小臂用另一套相对位置
            {
                forearmPosition = elbowPosition - forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40)) * 0.6f;
            }


            armPosition += armAngle.ToRotationVector2() * (top ? 30 : 20);

            armAngle += flipped ? 0 : MathHelper.Pi;
            forearmAngle += flipped ? 0 : MathHelper.Pi;


            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects armFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //screenPosition 类似的变量用于将游戏中的虚拟世界坐标转换为屏幕上的像素坐标，offset 是一个用于微调绘制位置的向量，通常用于调整绘制对象在屏幕上的位置。
            Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition, armFrame, Color.White, armAngle, armOrigin, NPC.scale, armFlip, 0);
            Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition, null, Color.White, forearmAngle, forearmOrigin, NPC.scale, armFlip, 0);


            if (forearmPosition.Y > player.position.Y)//如果是下方的大臂则手部用另一套相对位置（不然手部离得太远）
            {
                //再判断左右边
                if (forearmPosition.X > player.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 110 - Vector2.UnitY * 140, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, NPC.scale, flip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 110 - Vector2.UnitY * 140, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, NPC.scale, flip, 0);
                }

            }
            else
            {
                Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, NPC.scale, flip, 0);
            }

            Forearmposition = NPC.Center;
            Handposition = handPosition;
            Armposition = armPosition;
        }

        

        public override float SpawnChance(NPCSpawnInfo spawnInfo)//添加的生成条件以及概率（有召唤物，先不加了）
        {
            if (!ActiveNPC.RunawayMecha)
            {
                return 0;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<RunawayMechaBody>()))
            {
                return 0;
            }
            //return SpawnCondition.OverworldNight.Chance * 0.9f;
            return SpawnCondition.Overworld.Chance * 0.01f;//全天候有概率生成
        }
    }
}

