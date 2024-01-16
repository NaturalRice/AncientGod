using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;
using AncientGod.Projectiles.Ammo;
using AncientGod.Projectiles;
using Mono.Cecil;
using AncientGod.Projectiles.Pets.RunawayMecha;
using Terraria.ID;
using Terraria.Audio;
using static Terraria.Utils;
using System.Collections;
using Terraria.ModLoader.Core;
using AncientGod.Dusts;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using AncientGod.Items.Mounts.InfiniteFlight.ModernMecha;
using AncientGod.Items.Dyes;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;

namespace AncientGod.Projectiles.Mounts.InfiniteFlight.AlmightyMecha
{
    public class AlmightyMechaBody : ModProjectile
    {
        /*public override void Load()
        {
            // 在加载阶段注册你的着色器
            RegisterShader();
        }

        private void RegisterShader()
        {
            // 替换 "YourShaderName" 为你在效果文件中定义的着色器名称
            string shaderName = "DraedonHologramDye";

            // 在这里加载你的着色器
            Effect shader = ModContent.Request<Effect>($"{nameof(AncientGod)}/Effects/DraedonHologramDye", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value; // 替换为你的效果文件路径
            Filters.Scene["DraedonHologramDye"] = new Filter(new ScreenShaderData(shaderName), EffectPriority.VeryHigh);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public override void Unload()
        {
            // 在卸载 mod 时清理资源
            Filters.Scene["DraedonHologramDye"].Deactivate();
        }*/


        public int owner; // 自定义字段，用于存储投射物的源

        private const float TargetDistance = 1200f; // 设置寻找敌人的最大距离
        private int target = -1; // 记录当前目标敌人的索引
        private float fireCooldown = 60f; // 弹药冷却时间

        bool isInfernumActive;//用于标记Infernum模式是否激活
        Mod infern;//Mod类型的变量，用于引用Infernum模组

        //以下到SetStaticDefaults之前是尾焰的
        public Vector2[] PositionsThruster1;
        public Vector2[] PositionsThruster2;
        public Vector2[] PositionsThruster3;
        public Vector2[] PositionsThruster4;
        public float Thruster1Rotation;
        public float Thruster2Rotation;
        public float Thruster3Rotation;
        public float Thruster4Rotation;
        public Color RibbonStartColor = new Color(34, 17, 10);
        public ref float Behavior => ref Projectile.ai[1];
        public static int TrailLenght = 30;

        //A list of the ideal positions for each arm. The first 2 variables of the Vector2 represent the relative position of the arm to the body and the last variable represents the rotation of the hand
        internal readonly List<Vector3> IdealPositions = new List<Vector3>()//一个列表，其中包含了四个Vector3元素，每个元素表示一个机械臂的理想位置。这些位置包括相对于机械臂中心的X和Y偏移，以及手的旋转角度
        {
            new Vector3(-460f, -100f, MathHelper.ToRadians(240)), //Top left arm  此处调整四个大臂的位置,所有机械臂零件的位置若需变动，从此处开始调整
            new Vector3(-400f, 90f, MathHelper.ToRadians(270)), //Bottom left arm
            
            new Vector3(400f, 90f, MathHelper.ToRadians(270)), //Bottom right arm
            new Vector3(460f, -100f, MathHelper.ToRadians(300)) //Top right arm
        };

        private Vector2 GetIdealPosition(int i) => Projectile.Center + new Vector2(IdealPositions[i].X, IdealPositions[i].Y);

        private Vector2 BobVector => new Vector2(0, -2 + 4 * MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi), 0, 1) * 0.7f + 0.3f);
        //一个Vector2，用于模拟机械臂上下浮动的动画效果

        private List<Vector3> ArmPositions;
        //一个列表，用于存储机械臂的位置信息

        public ref float Initialized => ref Projectile.ai[0];
        //一个浮点数引用，用于初始化机械臂

        public Player Owner => Main.player[Projectile.owner];
        //一个引用，表示拥有这个机械臂的玩家

        public override void SetStaticDefaults()//用于设置静态默认值，包括指定该投射物作为玩家的宠物
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()//用于设置投射物的默认属性，包括宽度、高度、穿透、是否与地块碰撞等
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;           
        }

        private void FindTarget()
        {
            target = -1; // 默认值，表示没有找到目标
            float maxDistance = TargetDistance; // 设置最大寻找距离

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                // 检查敌人是否活着、活跃，并且在最大寻找范围内
                if (npc.active && !npc.friendly && Vector2.Distance(npc.Center, Projectile.Center) < maxDistance)
                {
                    maxDistance = Vector2.Distance(npc.Center, Projectile.Center);
                    target = i; // 更新目标索引
                }
            }
        }


        public override void AI()
        {
            /*这是机械臂的主要逻辑部分。在这个方法中，机械臂的行为受到不同条件的控制，包括拥有者是否已死亡、是否激活了坐骑等。
            机械臂的位置、浮动效果和旋转角度等都在这个方法中计算和更新。
            此外，机械臂的位置也会受到鼠标光标的位置影响，使其能够朝向鼠标光标。*/
            AncientGodPlayer modPlayer = Owner.GetModPlayer<AncientGodPlayer>();
            if (Owner.dead || !Owner.mount.Active)//此处添加了一个条件用来保证坐骑消失时这些投射物也会消失
                Projectile.timeLeft = 0;
            if (!modPlayer.AlmightyMecha)
            {
                Projectile.timeLeft = 0;
                Projectile.active = false;
            }
            if (modPlayer.AlmightyMecha && Owner.mount.Active)//当然，还要再在这里强调一遍才能达到效果
            {
                Projectile.timeLeft = 2;


            }


            if (Initialized == 0)
            {
                //Initialize the arm positions(SmallBug)
                PositionsThruster1 = new Vector2[TrailLenght];
                PositionsThruster2 = new Vector2[TrailLenght];
                PositionsThruster3 = new Vector2[TrailLenght];
                PositionsThruster4 = new Vector2[TrailLenght];

                for (int i = 0; i < TrailLenght; i++)
                {
                    PositionsThruster1[i] = Projectile.Center;
                    PositionsThruster2[i] = Projectile.Center;
                    PositionsThruster3[i] = Projectile.Center;
                    PositionsThruster4[i] = Projectile.Center;
                }
                //Initialize the arm positions
                ArmPositions = new List<Vector3>(4);
                for (int i = 0; i < 4; i++)
                {
                    ArmPositions.Add(new Vector3(GetIdealPosition(i), IdealPositions[i].Z));
                }
                Initialized = 1f;
            }

            Vector2 idealPosition = Owner.Center + Vector2.UnitY * -60;
            Projectile.Center = idealPosition;

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

                    //这里Lerp方法的第一个变量表示机械臂朝向目标坐标,并且加入了鼠标位置作为影响机械臂位置的又一因素
                    if (Main.mouseLeft)
                    {
                        //这个效果要不要加，会不会过于夸张？
                        /*if((Main.MouseWorld - Owner.position).Length() <= 600f)
                        {
                            vector2position = Main.MouseWorld + BobVector;
                        }*/
                        vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.2f) + (Main.MouseWorld - Projectile.Center) * 0.05f + BobVector;
                    }
                    else
                    {
                        vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.2f) + (Main.MouseWorld - Projectile.Center) * 0.01f + BobVector;
                    }                   

                    armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - Main.MouseWorld).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 2000f, 0, 1));

                    ArmPositions[i] = new Vector3(vector2position, armPosition.Z);

                    // 寻找附近的敌人
                    FindTarget(); // 调用FindTarget方法来更新target值

                    if (target != -1)
                    {
                        NPC targetNPC = Main.npc[target];


                        //手部瞄准敌人                        
                        Vector2 direction = targetNPC.Center - Projectile.position;

                        //这里是否加入手部摆动效果？或许要改良一下
                        armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - direction).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 2000f, 0, 1));

                        direction.Normalize();

                        //这里会产生大臂小臂朝敌人方向摆动的效果，太夸张了，先去掉
                        //Projectile.velocity = direction * 16f;



                        // 设置投射物的旋转角度
                        //armPosition.Z = direction.ToRotation();

                        if (fireCooldown <= 0f)
                        {
                            // 发射子弹
                            if (Projectile.localAI[0] == 0f)
                            {
                                Projectile.localAI[0] = 1f;//Projectile.localAI[0] 设置为1，表示已经发射过，以避免连续的发射。

                                // 计算发射角度，这里示例为向下发射
                                float shootAngle = direction.ToRotation();

                                // 发射弹药
                                for (int j = 0; j < 1; j++) // 你可以根据需要发射多个弹药（这里不是间隔的连射，而是一次性射多少弹药）
                                {
                                    direction = Main.MouseWorld - new Vector2(ArmPositions[i].X, ArmPositions[i].Y);
                                    shootAngle = direction.ToRotation();
                                    Vector2 shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f; // 这里示例为向右发射(底下是发射源的定义），瞄准方向
                                    int newProjectile11 = Projectile.NewProjectile(null, new Vector2(ArmPositions[i].X, ArmPositions[i].Y), shotVelocity, ModContent.ProjectileType<ModernMechaBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    int newProjectile12 = Projectile.NewProjectile(null, new Vector2(ArmPositions[i].X, ArmPositions[i].Y), Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    int newProjectile13 = Projectile.NewProjectile(null, new Vector2(ArmPositions[i].X, ArmPositions[i].Y), Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下

                                    //这里四个机关炮的准心有一定差别
                                    Main.projectile[newProjectile11].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile12].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile13].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile11].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile12].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile13].netUpdate = true;//是否将投射物的信息同步到其他客户端

                                    shootAngle += MathHelper.PiOver4; // 增加弹药之间的间隔角度
                                }

                                Projectile.localAI[0] = 0f;//为了连射

                                // 重置冷却计时器
                                fireCooldown = 10f; // 设置为你想要的冷却时间（间隔时间0.5秒）
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
                        // 当没有目标时，保持静止且瞄准鼠标
                        armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - Main.MouseWorld).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 2000f, 0, 1));

                        Projectile.velocity = Vector2.Zero;
                        Projectile.localAI[0] = 0f; // 重置发射标记
                    }



                }
            }
            //推进器尾焰
            UpdateTwins();

            // 获取Shader
            /*MiscShaderData shader1 = GameShaders.Misc["DraedonHologramDye"];
            // 将Shader应用于Projectile
            shader1.Apply();            
            // 绘制前还原Shader
            GameShaders.Misc["Default"].Apply(null);*/

            // 替换 "YourShaderName" 为你在效果文件中定义的着色器名称
            string shaderName = "DraedonHologramDye";

            // 在这里加载你的着色器
            Effect shader = ModContent.Request<Effect>($"{nameof(AncientGod)}/Effects/DraedonHologramDye", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value; // 替换为你的效果文件路径
            Filters.Scene["DraedonHologramDye"] = new Filter(new ScreenShaderData(shaderName), EffectPriority.VeryHigh);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
        public void UpdateTwins()
        {
            //Projectile.Center = Owner.Center;
            //Shift down the previous positions
            for (int i = 0; i < TrailLenght - 1; i++)
            {
                PositionsThruster1[i] = PositionsThruster1[i + 1];
                PositionsThruster2[i] = PositionsThruster2[i + 1];
                PositionsThruster3[i] = PositionsThruster3[i + 1];
                PositionsThruster4[i] = PositionsThruster4[i + 1];
            }
            if (Owner.velocity.Length() > 10)
            {
                //此处调整尾焰的位置变化规律
                PositionsThruster1[TrailLenght - 1] = Vector2.Lerp(PositionsThruster1[TrailLenght - 1], Owner.Center + Vector2.UnitX * 50 + Vector2.UnitY * 40 + Vector2.Normalize(Owner.velocity.RotatedBy(MathHelper.PiOver2)) + Owner.velocity, 0.5f);
                PositionsThruster2[TrailLenght - 1] = Vector2.Lerp(PositionsThruster2[TrailLenght - 1], Owner.Center - Vector2.UnitX * 50 + Vector2.UnitY * 40 + Vector2.Normalize(Owner.velocity.RotatedBy(MathHelper.PiOver2)) + Owner.velocity, 0.5f);
                PositionsThruster3[TrailLenght - 1] = Vector2.Lerp(PositionsThruster3[TrailLenght - 1], Owner.Center + Vector2.UnitX * 80 - Vector2.UnitY * 70 + Vector2.Normalize(Owner.velocity.RotatedBy(MathHelper.PiOver2)) + Owner.velocity, 0.5f);
                PositionsThruster4[TrailLenght - 1] = Vector2.Lerp(PositionsThruster4[TrailLenght - 1], Owner.Center - Vector2.UnitX * 80 - Vector2.UnitY * 70 + Vector2.Normalize(Owner.velocity.RotatedBy(MathHelper.PiOver2)) + Owner.velocity, 0.5f);

                Thruster1Rotation = Thruster1Rotation.AngleLerp(Owner.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);
                Thruster2Rotation = Thruster2Rotation.AngleLerp(Owner.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);
                Thruster3Rotation = Thruster3Rotation.AngleLerp(Owner.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);
                Thruster4Rotation = Thruster4Rotation.AngleLerp(Owner.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);
            }

            float thrusterOpacity1 = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果
            float thrusterOpacity2 = (1 - MathHelper.Clamp((float)Math.Cos(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果
            //RibbonStartColor 用于表示轨迹的颜色，它会在快速移动时变为由橙色到，慢速移动时恢复为灰色。
            Color IdealColor = Color.Lerp(Color.LightSkyBlue * thrusterOpacity2, Color.Orange * thrusterOpacity1, MathHelper.Clamp(Owner.velocity.Length() - 5, 0, 20) / 10f);
            RibbonStartColor = Color.Lerp(RibbonStartColor, IdealColor, 0.2f);
        }
        public float RibbonTrailWidthFunction(float completionRatio)
        {
            float thrusterOpacity1 = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 0.5f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果
            float thrusterOpacity2 = (1 - MathHelper.Clamp((float)Math.Cos(Main.time % MathHelper.Pi) * 0.5f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果

            float tail = (float)Math.Pow(completionRatio, 2) * 12 * thrusterOpacity1;//此处调整尾焰宽度和尖锐度
            float bump = Utils.GetLerpValue(0.7f, 0.8f, 1 - completionRatio, true) * Utils.GetLerpValue(1f, 1f * thrusterOpacity2, 1 - completionRatio, true) * 4;
            return tail + bump;
        }
        public Color OrangeRibbonTrailColorFunction(float completionRatio)
        {
            Color startingColor = RibbonStartColor;
            Color endColor = new Color(78, 0, 78);
            return Color.Lerp(startingColor, endColor, (float)Math.Pow(1 - completionRatio, 1.5D)) * 0.7f;
        }
        public Color GreenRibbonTrailColorFunction(float completionRatio)
        {
            Color startingColor = RibbonStartColor;
            Color endColor = new Color(78, 78, 0);
            return Color.Lerp(startingColor, endColor, (float)Math.Pow(1 - completionRatio, 1.5D)) * 0.7f;
        }
        public override bool PreDrawExtras()//用于在绘制前执行额外的逻辑。其中，DrawChain 方法用于绘制链条效果
        {
            if (ArmPositions == null)
                return true;

            Texture2D teslaTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaTesla")).Value;
            Texture2D laserTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaLaser")).Value;
            Texture2D nukeTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaNuke")).Value;
            Texture2D plasmaTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaPlasma")).Value;
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
            /*position 是一个矢量，代表手部相对于机械臂中心的偏移。通过将这个偏移乘以 0.1f，你可以调整手部相对于机械臂中心的位置。
            这个值的改变可以影响手部的距离，使它更接近或更远离机械臂中心。*/
            Vector2 position = handPosition - Projectile.Center;
            bool flipped = Math.Sign(position.X) != -1;
            //绘制贴图
            Texture2D chainTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaChain").Value;
            Texture2D armTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaArm").Value;//大臂
            Texture2D forearmTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaForearm").Value;//小臂
            Texture2D elbowTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaElbow").Value;//肘部
            Texture2D shoulderTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaShoulder").Value;//肩膀
            Texture2D wristTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaWrist").Value;//肩膀
            Texture2D thrusterTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaThruster").Value;//推进器

            //Frame后缀表示贴图起始的绘制坐标与宽高，不同部位共用同一贴图的才需要此变量
            Rectangle armFrame = new Rectangle(0, top ? 0 : 80, armTex.Width, 80);
            Rectangle handFrame = new Rectangle(0, infernum ? 40 : 0, 90, 56);

            //Origin后缀表示旋转中心
            Vector2 armOrigin = new Vector2(flipped ? 0 : armFrame.Width, armFrame.Height / 2);
            Vector2 forearmOrigin = new Vector2(flipped ? 0 : forearmTex.Width, forearmTex.Height / 2);
            Vector2 elbowOrigin = new Vector2(elbowTex.Width / 2, elbowTex.Height / 2);
            Vector2 shoulderOrigin = new Vector2(shoulderTex.Width / 2, shoulderTex.Height / 2);
            Vector2 wristOrigin = new Vector2(flipped ? 0 : wristTex.Width, wristTex.Height);
            Vector2 thrusterOrigin = new Vector2(thrusterTex.Width / 2, thrusterTex.Height / 2);
            Vector2 handOrigin = new Vector2(flipped ? 50 : 40, 28); //手部旋转中心

            //Position后缀表示贴图位置
            Vector2 armPosition = top ? Projectile.Center + position * 0.2f + (flipped ? Vector2.UnitX * 10 : -Vector2.UnitX * 10) + Vector2.UnitY * 10 : Projectile.Center + position * 0.2f
                    + (flipped ? Vector2.UnitX * 10 : -Vector2.UnitX * 10) + Vector2.UnitY * 50;//上下方大臂是不同的摆放逻辑
            Vector2 shoulderPosition = top ? Projectile.Center + position * 0.2f + (flipped ? Vector2.UnitX * 30 : -Vector2.UnitX * 30) - Vector2.UnitY * 30 : Projectile.Center + position * 0.2f 
                    + (flipped ? Vector2.UnitX * 20 : -Vector2.UnitX * 20) + Vector2.UnitY * 50;//上下方肩膀也是不同的摆放逻辑;            
            //Do some trigonometry to get the elbow position
            float armLenght = 275;
            float directLenght = MathHelper.Clamp((handPosition - armPosition).Length(), 0, armLenght); //Clamp the direct lenght to avoid getting an error from trying to calculate the square root of a negative number
            float elbowElevation = (float)Math.Sqrt(Math.Pow(armLenght / 2f, 2) - Math.Pow(directLenght / 2f, 2));
            //Pow函数：返回指定数字的倒数的估计值。
            Vector2 elbowPosition = Vector2.Lerp(handPosition, armPosition, 0.7f) + Utils.SafeNormalize(position, Vector2.Zero).RotatedBy(-MathHelper.PiOver2 * Math.Sign(position.X)) * elbowElevation;
            //上面Lerp函数中第三个浮点数越大，小臂越靠近大臂

            //Angle后缀表示贴图旋转角度
            float armAngle = top ? (elbowPosition - armPosition - Vector2.UnitY * 20).ToRotation() : (elbowPosition - armPosition - Vector2.UnitY * 100).ToRotation();
            float forearmAngle = top ? (handPosition - elbowPosition + Vector2.UnitY * 50).ToRotation() : (handPosition - elbowPosition + Vector2.UnitY * 50).ToRotation();
            float elbowAngle = top ? forearmAngle * 2f + (flipped ? 0 : -0) : forearmAngle * -2f + (flipped ? -1 : 1);
            float shoulderAngle = top ? armAngle * 2f + (flipped ? -0 : 0) : armAngle * -2f + (flipped ? -1 : 1);
            float wristAngle = forearmAngle;
            float thrusterAngle = top ? armAngle * 2f + (flipped ? -0 : 0) : armAngle * -2f + (flipped ? -0 : 0);


            /*这里，forearmPosition 是通过 elbowPosition 和 forearmAngle 计算得到的。首先，通过 elbowPosition 加上 forearmAngle 方向的矢量来确定下臂的位置。
            forearmAngle 是下臂与手部之间的角度，它是通过 (handPosition - elbowPosition).ToRotation() 计算的。*/
            Vector2 forearmPosition = top ? elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40) / 16f) - Vector2.UnitX * 0 - Vector2.UnitY * 50
                    : elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40) / 16f) - Vector2.UnitX * 0 - Vector2.UnitY * 70;//上下方小臂也是不同的摆放逻辑
            armPosition += armAngle.ToRotationVector2() * (top ? 30 : 20);

            armAngle += flipped ? 0 : MathHelper.Pi;
            forearmAngle += flipped ? 0 : MathHelper.Pi;
            elbowAngle += flipped ? 0 : MathHelper.Pi;
            shoulderAngle += flipped ? 0 : MathHelper.Pi;
            wristAngle += flipped ? 0 : MathHelper.Pi;
            thrusterAngle += flipped ? 0 : MathHelper.Pi;

            //贴图的翻转逻辑
            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects armFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects upElbowFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipVertically;
            SpriteEffects underElbowFlip = flipped ? SpriteEffects.FlipVertically : SpriteEffects.None;



            //此处用于定义链条的绘制方法
            //链条使用贝塞尔曲线逻辑绘制
            int numPoints = 10; //"Should make dynamic based on curve length, but I'm not sure how to smoothly do that while using a bezier curve" -Graydee, from the code i referenced. I do agree.
            float curvature = MathHelper.Clamp(Math.Abs(shoulderPosition.X - Projectile.Center.X) / 50f * 80, 15, 80);

            //关于链条的端点定义
            Vector2 controlPoint1 = Projectile.Center - Vector2.UnitY * 50 + Vector2.UnitY * curvature; //端点1:机甲中心
            Vector2 controlPoint2 = shoulderPosition + (flipped ? -Vector2.UnitX * 0 : Vector2.UnitX * 0) + (top ? Vector2.UnitY * 70 : Vector2.UnitY * 70) - Vector2.UnitY * curvature;//端点2:肩膀中心
            Vector2 controlPoint3 = armPosition + (flipped ? (top ? Vector2.UnitX * 55 : Vector2.UnitX * 30) : (top ? -Vector2.UnitX * 55 : -Vector2.UnitX * 30)) + (top ? Vector2.UnitY * 60 : Vector2.UnitY * 60) - Vector2.UnitY * curvature;//端点3:大臂中心
            Vector2 controlPoint4 = elbowPosition +(flipped ? Vector2.UnitX * 10 : -Vector2.UnitX * 10) + (top ? Vector2.UnitY * 0 : Vector2.UnitY * 0) - Vector2.UnitY * curvature;//端点4:手肘中心
            Vector2 controlPoint5 = handPosition + (flipped ? -Vector2.UnitX * 0 : Vector2.UnitX * 0) + (top ? Vector2.UnitY * 60 : Vector2.UnitY * 65) - Vector2.UnitY * curvature;//端点5:手腕中心
            Vector2 controlPoint6 = forearmPosition + (flipped ? -Vector2.UnitX * 0 : Vector2.UnitX * 0) + (top ? Vector2.UnitY * 90 : Vector2.UnitY * 90) - Vector2.UnitY * curvature;//端点6:小臂中心
            //Vector2 controlPoint7 = (Projectile.Center * 2 - Forearmposition) * Vector2.UnitX + (Projectile.Center * 2 - Forearmposition) * Vector2.UnitY - Vector2.UnitY * curvature;//端点7
            //Vector2 controlPoint8 = (Projectile.Center * 2 - Handposition) * Vector2.UnitX + (Projectile.Center * 2 - Handposition) * Vector2.UnitY + Vector2.UnitY * curvature;//端点8

            BezierCurve curve12 = new BezierCurve(new Vector2[] { controlPoint1, controlPoint2, controlPoint2, controlPoint2 });
            BezierCurve curve13 = new BezierCurve(new Vector2[] { controlPoint1, controlPoint3, controlPoint3, controlPoint3 });
            BezierCurve curve34 = new BezierCurve(new Vector2[] { controlPoint3, controlPoint4, controlPoint4, controlPoint4 });
            BezierCurve curve56 = new BezierCurve(new Vector2[] { controlPoint5, controlPoint6, controlPoint6, controlPoint6 });

            Vector2[] chainPositions12 = curve12.GetPoints(numPoints).ToArray();
            Vector2[] chainPositions13 = curve13.GetPoints(numPoints).ToArray();
            Vector2[] chainPositions34 = curve34.GetPoints(numPoints).ToArray();
            Vector2[] chainPositions56 = curve56.GetPoints(numPoints).ToArray();

            //Draw each chain segment bar the very first one
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture

                Vector2 position12 = chainPositions12[i];
                float rotation12 = (chainPositions12[i] - chainPositions12[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale12 = Vector2.Distance(chainPositions12[i], chainPositions12[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale12 = new Vector2(1, yScale12);
                Color chainLightColor12 = Lighting.GetColor((int)position12.X / 16, (int)position12.Y / 16); //Lighting of the position of the chain segment                
                Main.EntitySpriteDraw(chainTex, position12 - Main.screenPosition, null, chainLightColor12, rotation12, origin, scale12, SpriteEffects.None, 0);

                Vector2 position13 = chainPositions13[i];
                float rotation13 = (chainPositions13[i] - chainPositions13[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale13 = Vector2.Distance(chainPositions13[i], chainPositions13[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale13 = new Vector2(1, yScale13);
                Color chainLightColor13 = Lighting.GetColor((int)position13.X / 16, (int)position13.Y / 16); //Lighting of the position of the chain segment
                Main.EntitySpriteDraw(chainTex, position13 - Main.screenPosition, null, chainLightColor13, rotation13, origin, scale13, SpriteEffects.None, 0);

                Vector2 position34 = chainPositions34[i];
                float rotation34 = (chainPositions34[i] - chainPositions34[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale34 = Vector2.Distance(chainPositions34[i], chainPositions34[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale34 = new Vector2(1, yScale34);
                Color chainLightColor34 = Lighting.GetColor((int)position34.X / 16, (int)position34.Y / 16); //Lighting of the position of the chain segment
                Main.EntitySpriteDraw(chainTex, position34 - Main.screenPosition, null, chainLightColor34, rotation34, origin, scale34, SpriteEffects.None, 0);

                Vector2 position56 = chainPositions56[i];
                float rotation56 = (chainPositions56[i] - chainPositions56[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale56 = Vector2.Distance(chainPositions56[i], chainPositions56[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale56 = new Vector2(1, yScale56);
                Color chainLightColor56 = Lighting.GetColor((int)position56.X / 16, (int)position56.Y / 16); //Lighting of the position of the chain segment
                Main.EntitySpriteDraw(chainTex, position56 - Main.screenPosition, null, chainLightColor56, rotation56, origin, scale56, SpriteEffects.None, 0);

                DrawLightningEffect(Main.spriteBatch, chainTex, controlPoint1, controlPoint2, Color.LightSkyBlue, new Rectangle(0, 0, chainTex.Width, chainTex.Height), origin);
                DrawLightningEffect(Main.spriteBatch, chainTex, controlPoint1, controlPoint3, Color.LightSkyBlue, new Rectangle(0, 0, chainTex.Width, chainTex.Height), origin);
                DrawLightningEffect(Main.spriteBatch, chainTex, controlPoint3, controlPoint4, Color.LightSkyBlue, new Rectangle(0, 0, chainTex.Width, chainTex.Height), origin);
                DrawLightningEffect(Main.spriteBatch, chainTex, controlPoint5, controlPoint6, Color.LightSkyBlue, new Rectangle(0, 0, chainTex.Width, chainTex.Height), origin);
            }

            //////////////////////////////////////////////////////////////////////////
            //此处设置尾焰                     
            float[] RotationsThruster1 = new float[PositionsThruster1.Length];
            float[] RotationsThruster2 = new float[PositionsThruster2.Length];
            float[] RotationsThruster3 = new float[PositionsThruster3.Length];
            float[] RotationsThruster4 = new float[PositionsThruster4.Length];
            for (int i = 1; i < RotationsThruster1.Length; i++)
            {
                RotationsThruster1[i] = PositionsThruster1[i - 1].AngleTo(PositionsThruster1[i]);
                RotationsThruster2[i] = PositionsThruster2[i - 1].AngleTo(PositionsThruster2[i]);
                RotationsThruster3[i] = PositionsThruster3[i - 1].AngleTo(PositionsThruster3[i]);
                RotationsThruster4[i] = PositionsThruster4[i - 1].AngleTo(PositionsThruster4[i]);
            }
            RotationsThruster1[0] = Thruster1Rotation;
            RotationsThruster2[0] = Thruster2Rotation;
            RotationsThruster3[0] = Thruster3Rotation;
            RotationsThruster4[0] = Thruster4Rotation;

            Terraria.Graphics.VertexStrip thruster1Strip = new Terraria.Graphics.VertexStrip();
            Terraria.Graphics.VertexStrip thruster2Strip = new Terraria.Graphics.VertexStrip();
            Terraria.Graphics.VertexStrip thruster3Strip = new Terraria.Graphics.VertexStrip();
            Terraria.Graphics.VertexStrip thruster4Strip = new Terraria.Graphics.VertexStrip();
            //此处用于显示尾焰效果
            thruster1Strip.PrepareStripWithProceduralPadding(PositionsThruster1, RotationsThruster1, OrangeRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);
            thruster2Strip.PrepareStripWithProceduralPadding(PositionsThruster2, RotationsThruster2, OrangeRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);
            thruster3Strip.PrepareStripWithProceduralPadding(PositionsThruster3, RotationsThruster3, OrangeRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);
            thruster4Strip.PrepareStripWithProceduralPadding(PositionsThruster4, RotationsThruster4, OrangeRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);
            //使用 Effect 和自定义的着色方法实现了轨迹的颜色渐变效果。
            Effect vertexShader = ModContent.Request<Effect>($"{nameof(AncientGod)}/Effects/VertexShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            vertexShader.Parameters["uColor"].SetValue(Vector4.One);
            vertexShader.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            vertexShader.CurrentTechnique.Passes[0].Apply();

            thruster1Strip.DrawTrail();
            thruster2Strip.DrawTrail();
            thruster3Strip.DrawTrail();
            thruster4Strip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();


            //screenPosition 类似的变量用于将游戏中的虚拟世界坐标转换为屏幕上的像素坐标，offset 是一个用于微调绘制位置的向量，通常用于调整绘制对象在屏幕上的位置。
            //显示贴图
            Main.EntitySpriteDraw(thrusterTex, shoulderPosition + offset - Main.screenPosition + (flipped ? -Vector2.UnitX * 50 : Vector2.UnitX * 50) + (top ? Vector2.UnitY * 50 : -Vector2.UnitY * 0),
                                    null, Color.White * 1f, thrusterAngle, thrusterOrigin, Projectile.scale, top ? upElbowFlip : underElbowFlip, 0);//推进器
            Main.EntitySpriteDraw(elbowTex, elbowPosition + offset - Main.screenPosition + (flipped ? Vector2.UnitX * 10 : -Vector2.UnitX * 10) + (top ? -Vector2.UnitY * 70 : -Vector2.UnitY * 80), 
                                    null, Color.White, elbowAngle, elbowOrigin, Projectile.scale, top ? upElbowFlip : underElbowFlip, 0);//肘部
            Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
            Main.EntitySpriteDraw(shoulderTex, shoulderPosition + offset - Main.screenPosition, null, Color.White, shoulderAngle, shoulderOrigin, Projectile.scale, top ?  upElbowFlip : underElbowFlip, 0);;//肩膀
                    
            Main.EntitySpriteDraw(wristTex, handPosition + offset - Main.screenPosition + (flipped ? -Vector2.UnitX * 70 : Vector2.UnitX * 70) + (top ? Vector2.UnitY * 20 : -Vector2.UnitY * 10),
                                    null, Color.White, wristAngle, wristOrigin, Projectile.scale, armFlip, 0);//手腕                   
            Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);//手部                    
            Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition, null, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);


            float thrusterOpacity1 = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 0.5f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果
            float thrusterOpacity2 = (1 - MathHelper.Clamp((float)Math.Cos(Main.time % MathHelper.Pi) * 0.5f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果
            if (Owner.velocity.Length() > 10)
            {
                Explode(shoulderPosition + offset + (flipped ? (top ? -Vector2.UnitX * 45 : -Vector2.UnitX * 60) : (top ? Vector2.UnitX * 15 : Vector2.UnitX * 35))
                + (top ? Vector2.UnitY * 30 : Vector2.UnitY * 32), 25, 25);
            }

            //MechaLight(forearmPosition + offset, forearmTex.Width / 2, forearmTex.Height / 2, 308, 1, 1, 1, Color.LightGoldenrodYellow, 1f);           

        }


        public override void PostDraw(Color lightColor)//用于在绘制后执行额外的逻辑。在这里，它绘制了机械臂的眼睛和头部
        {
            Texture2D eyesTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaEyes").Value;

            Vector2 offset1 = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);
            float eyeOpacity = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 1f + 0.5f;//闪耀光亮效果

            

            Texture2D headTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaHead").Value;

            Vector2 offset2 = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);

            Main.EntitySpriteDraw(headTex, Projectile.Center + offset2 - Main.screenPosition - Vector2.UnitX * 0 - Vector2.UnitY * 50, null, Color.White, 0, headTex.Size() / 2f, Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(eyesTex, Projectile.Center + offset1 - Main.screenPosition - Vector2.UnitX * 0 - Vector2.UnitY * 50, null, Color.White * eyeOpacity, 0, eyesTex.Size() / 2f, Projectile.scale, 0f, 0);
        }


        //以下为各种特效 ////////////////////////////////////////////////////////////////////////////////////////////////
        private void Explode(Vector2 position,int width, int height)
        {//给推进器尾焰添加粒子效果
            // 产生爆炸粒子。
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(position, width, height, DustID.OrangeStainedGlass);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            // 播放爆炸声音(是否需要什么声音？）
            //SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }

        private void MechaLight(Vector2 Position, int Width, int Height, int Type, float SpeedX, float SpeedY, int Alpha, Color newColor = default(Color), float Scale = 1f)
        {//给机甲添加光效
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = MechaDust.NewDust(Position, Width, Height, Type, SpeedX, SpeedY, Alpha, newColor = default(Color), Scale);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            // 播放爆炸声音(是否需要什么声音？）
            //SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }
             
        private void DrawLightningEffect(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color, Rectangle rectangle, Vector2 origin)
        {
            Vector2 direction = end - start;
            direction.Normalize();

            float rotation = direction.ToRotation();
            Vector2 scale = new Vector2(Vector2.Distance(start, end) / texture.Width, 1f);

            spriteBatch.Draw(texture, start, rectangle, color, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public bool summonedProjectile = false;
    }
}
