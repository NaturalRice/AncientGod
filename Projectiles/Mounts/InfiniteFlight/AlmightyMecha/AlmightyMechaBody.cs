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

namespace AncientGod.Projectiles.Mounts.InfiniteFlight.AlmightyMecha
{
    public class AlmightyMechaBody : ModProjectile
    {
        public int owner; // 自定义字段，用于存储投射物的源

        private const float TargetDistance = 1200f; // 设置寻找敌人的最大距离
        private int target = -1; // 记录当前目标敌人的索引
        private float fireCooldown = 60f; // 弹药冷却时间

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


                    //vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.2f) + BobVector;//如果机械臂相对位置摆动得太逆天，可能要禁用这个动画效果
                    vector2position = Vector2.Lerp(vector2position, idealArmPosition, 1f);//手部位置鬼畜的问题出在这里的Lerp函数；经过多次测试，目前此数值0.6最适合

                    //Make the cannon look at the mouse cursor if its close enough

                    //armPosition.Z = Utils.AngleLerp(armPosition.Z, IdealPositions[i].Z, MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 1000f, 0, 1));

                    armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - Main.MouseWorld).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 1000f, 0, 1));
                    //两个armPosition.Z都是必要的，后者用于在鼠标靠近时瞄准鼠标，前者用于鼠标远离时重新指向地面

                    ArmPositions[i] = new Vector3(vector2position, armPosition.Z);

                    // 寻找附近的敌人
                    FindTarget(); // 调用FindTarget方法来更新target值

                    if (target != -1)
                    {
                        NPC targetNPC = Main.npc[target];


                        //手部瞄准敌人                        
                        Vector2 direction = targetNPC.Center - Projectile.position;

                        //这里是否加入手部摆动效果？或许要改良一下
                        armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - direction).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 1000f, 0, 1));

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
                                    direction = targetNPC.Center - (Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150);
                                    shootAngle = direction.ToRotation();
                                    Vector2 shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f; // 这里示例为向右发射(底下是发射源的定义），瞄准方向
                                    int newProjectile11 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, shotVelocity, ModContent.ProjectileType<ModernMechaBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    int newProjectile12 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    int newProjectile13 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    direction = targetNPC.Center - (Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    int newProjectile21 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, shotVelocity, ModContent.ProjectileType<ModernMechaBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左上
                                    int newProjectile22 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左上
                                    int newProjectile23 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左上
                                    direction = targetNPC.Center - (Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    int newProjectile31 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, shotVelocity, ModContent.ProjectileType<ModernMechaBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右上
                                    int newProjectile32 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右上
                                    int newProjectile33 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右上
                                    direction = targetNPC.Center - (Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    int newProjectile41 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, shotVelocity, ModContent.ProjectileType<ModernMechaBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左下
                                    int newProjectile42 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左下
                                    int newProjectile43 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<ModernMechaBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //这里四个机关炮的准心有一定差别
                                    Main.projectile[newProjectile11].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile12].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile13].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile21].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile22].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile23].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile31].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile32].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile33].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile41].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile42].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile43].timeLeft = 1000;//弹药存活时间
                                    Main.projectile[newProjectile11].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile12].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile13].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile21].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile22].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile23].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile31].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile32].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile33].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile41].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile42].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    Main.projectile[newProjectile43].netUpdate = true;//是否将投射物的信息同步到其他客户端


                                    shootAngle += MathHelper.PiOver4; // 增加弹药之间的间隔角度
                                }

                                Projectile.localAI[0] = 0f;//为了连射

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
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localAI[0] = 0f; // 重置发射标记
                    }



                }
            }
        }
        public override bool PreDrawExtras()//用于在绘制前执行额外的逻辑。其中，DrawChain 方法用于绘制链条效果
        {
            //DrawChain();//暂不使用


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

            Texture2D armTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaArm").Value;//大臂
            Texture2D forearmTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaForearm").Value;//小臂
            Texture2D elbowTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaElbow").Value;//肘部
            Texture2D shoulderTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaShoulder").Value;//肩膀
            Texture2D wristTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaWrist").Value;//肩膀
            Texture2D thrusterTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaThruster").Value;//推进器

            Rectangle armFrame = new Rectangle(0, top ? 0 : 80, armTex.Width, 80);
            Rectangle handFrame = new Rectangle(0, infernum ? 40 : 0, 90, 56);

            Vector2 armOrigin = new Vector2(flipped ? 0 : armFrame.Width, armFrame.Height / 3);//这里由除2改成除3
            Vector2 forearmOrigin = new Vector2(flipped ? 0 : forearmTex.Width, forearmTex.Height);//这里去掉除2
            Vector2 elbowOrigin = new Vector2(flipped ? 0 : elbowTex.Width, elbowTex.Height);
            Vector2 shoulderOrigin = new Vector2(flipped ? 0 : shoulderTex.Width, shoulderTex.Height);
            Vector2 wristOrigin = new Vector2(flipped ? 0 : wristTex.Width, wristTex.Height);
            Vector2 thrusterOrigin = new Vector2(thrusterTex.Width / 2, thrusterTex.Height / 2);
            Vector2 handOrigin = new Vector2(flipped ? 50 : 40, 28); //手部旋转中心

            Vector2 armPosition = Projectile.Center + position * 0.1f;

            //Do some trigonometry to get the elbow position

            float armLenght = 175;
            float directLenght = MathHelper.Clamp((handPosition - armPosition).Length(), 0, armLenght); //Clamp the direct lenght to avoid getting an error from trying to calculate the square root of a negative number
            float elbowElevation = (float)Math.Sqrt(Math.Pow(armLenght / 2f, 2) - Math.Pow(directLenght / 2f, 2));
            Vector2 elbowPosition = Vector2.Lerp(handPosition, armPosition, 0.5f) + Utils.SafeNormalize(position, Vector2.Zero).RotatedBy(-MathHelper.PiOver2 * Math.Sign(position.X)) * elbowElevation;

            float armAngle = top ? (elbowPosition - armPosition).ToRotation() : (elbowPosition - armPosition + Vector2.UnitY * 80).ToRotation();
            float forearmAngle = (handPosition - elbowPosition + Vector2.UnitY * 80).ToRotation();
            float elbowAngle = (elbowPosition + Vector2.UnitY * 0).ToRotation();
            float shoulderAngle = (elbowPosition + Vector2.UnitY * 0).ToRotation();
            float wristAngle = 0;
            float thrusterAngle = (elbowPosition + Vector2.UnitY * 0).ToRotation();

            /*这里，forearmPosition 是通过 elbowPosition 和 forearmAngle 计算得到的。首先，通过 elbowPosition 加上 forearmAngle 方向的矢量来确定下臂的位置。
            forearmAngle 是下臂与手部之间的角度，它是通过 (handPosition - elbowPosition).ToRotation() 计算的。*/
            Vector2 forearmPosition = elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40) / 2f);

            if (forearmPosition.Y < Owner.position.Y && forearmAngle > 200)
            {
                if (forearmPosition.X < Owner.position.X)
                {
                    forearmPosition = elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition - Vector2.UnitX * 120 + Vector2.UnitY * 120).Length() - 40));
                }
                else
                {
                    forearmPosition = elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition + Vector2.UnitX * 120 + Vector2.UnitY * 120).Length() - 40));
                }
            }

            if (forearmPosition.Y > Owner.position.Y)//如果是下方的大臂则小臂用另一套相对位置
            {
                forearmPosition = elbowPosition - forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40)) * 0.6f;
            }


            armPosition += armAngle.ToRotationVector2() * (top ? 30 : 20);

            armAngle += flipped ? 0 : MathHelper.Pi;
            forearmAngle += flipped ? 0 : MathHelper.Pi;
            elbowAngle += flipped ? 0 : MathHelper.Pi;
            shoulderAngle += flipped ? 0 : MathHelper.Pi;
            wristAngle += flipped ? 0 : MathHelper.Pi;
            thrusterAngle += flipped ? 0 : MathHelper.Pi;


            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects armFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects upElbowFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipVertically;
            SpriteEffects underElbowFlip = flipped ? SpriteEffects.FlipVertically : SpriteEffects.None;

            //screenPosition 类似的变量用于将游戏中的虚拟世界坐标转换为屏幕上的像素坐标，offset 是一个用于微调绘制位置的向量，通常用于调整绘制对象在屏幕上的位置。
            //Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);
            //Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition, null, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);


            if(armPosition.Y > Owner.position.Y - 10)//如果是下方的大臂
            {
                if (armPosition.X > Owner.position.X)//右下
                {
                    Main.EntitySpriteDraw(thrusterTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 60 + Vector2.UnitY * 100, null, Color.White * 1f, thrusterAngle + 1, thrusterOrigin, Projectile.scale, underElbowFlip, 0);//肘部
                    Main.EntitySpriteDraw(elbowTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 65 + Vector2.UnitY * 100, null, Color.White, elbowAngle, elbowOrigin, Projectile.scale, underElbowFlip, 0);//肘部
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 40 + Vector2.UnitY * 30, armFrame, Color.White, armAngle + 5, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(shoulderTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 0 + Vector2.UnitY * 30, null, Color.White, shoulderAngle + 1, shoulderOrigin, Projectile.scale, underElbowFlip, 0);//肩膀
                    
                }
                else//左下
                {
                    Main.EntitySpriteDraw(thrusterTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 70 + Vector2.UnitY * 85, null, Color.White * 1f, thrusterAngle - 1, thrusterOrigin, Projectile.scale, underElbowFlip, 0);
                    Main.EntitySpriteDraw(elbowTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 170 + Vector2.UnitY * 0, null, Color.White, elbowAngle, elbowOrigin, Projectile.scale, underElbowFlip, 0);
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 40 + Vector2.UnitY * 30, armFrame, Color.White, armAngle - 5, armOrigin, Projectile.scale, armFlip, 0);
                    Main.EntitySpriteDraw(shoulderTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 90 + Vector2.UnitY * 85, null, Color.White, shoulderAngle - 1, shoulderOrigin, Projectile.scale, underElbowFlip, 0);
                    
                }
            }
            else//上方
            {
                if (armPosition.X > Owner.position.X)//右上
                {
                    Main.EntitySpriteDraw(thrusterTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 70 - Vector2.UnitY * 80, null, Color.White * 1f, thrusterAngle - 1, thrusterOrigin, Projectile.scale, upElbowFlip, 0);
                    Main.EntitySpriteDraw(elbowTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 65 - Vector2.UnitY * 10, null, Color.White, elbowAngle, elbowOrigin, Projectile.scale, upElbowFlip, 0);
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 40 - Vector2.UnitY * 20, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);
                    Main.EntitySpriteDraw(shoulderTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 30 + Vector2.UnitY * 30, null, Color.White, shoulderAngle - 1, shoulderOrigin, Projectile.scale, upElbowFlip, 0);
                    
                }
                else//左上
                {
                    Main.EntitySpriteDraw(thrusterTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 50 - Vector2.UnitY * 75, null, Color.White * 1f, thrusterAngle + 1, thrusterOrigin, Projectile.scale, upElbowFlip, 0);
                    Main.EntitySpriteDraw(elbowTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 180 - Vector2.UnitY * 110, null, Color.White, elbowAngle, elbowOrigin, Projectile.scale, upElbowFlip, 0);
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 40 - Vector2.UnitY * 20, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);
                    Main.EntitySpriteDraw(shoulderTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 40 - Vector2.UnitY * 75, null, Color.White, shoulderAngle + 1, shoulderOrigin, Projectile.scale, upElbowFlip, 0);
                   
                }
            }


            if (forearmPosition.Y > Owner.position.Y)//如果是下方的小臂则手部用另一套相对位置（不然手部离得太远）
            {
                //再判断左右边
                if (forearmPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {//右下
                    Main.EntitySpriteDraw(wristTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 70 - Vector2.UnitY * 150, null, Color.White, wristAngle, wristOrigin, Projectile.scale, underElbowFlip, 0);                    
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 150 - Vector2.UnitY * 150, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);                    
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition + Vector2.UnitX * 80 + Vector2.UnitY * 30, null, Color.White, forearmAngle - 1, forearmOrigin, Projectile.scale, armFlip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {//左下
                    Main.EntitySpriteDraw(wristTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 170 - Vector2.UnitY * 190, null, Color.White, wristAngle, wristOrigin, Projectile.scale, underElbowFlip, 0);                   
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 150 - Vector2.UnitY * 150, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);                   
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 80 + Vector2.UnitY * 30, null, Color.White, forearmAngle + 1, forearmOrigin, Projectile.scale, armFlip, 0);
                }
                         
            }
            else
            {
                if (forearmPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {//右上
                    Main.EntitySpriteDraw(wristTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 160 - Vector2.UnitY * 30, null, Color.White, wristAngle, wristOrigin, Projectile.scale, upElbowFlip, 0);                   
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 250 - Vector2.UnitY * 50, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);                   
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition + Vector2.UnitX * 75 - Vector2.UnitY * 5, null, Color.White, forearmAngle - 1, forearmOrigin, Projectile.scale, armFlip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {//左上
                    Main.EntitySpriteDraw(wristTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 260 - Vector2.UnitY * 70, null, Color.White, wristAngle, wristOrigin, Projectile.scale, upElbowFlip, 0);                    
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 250 - Vector2.UnitY * 50, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);                    
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 75 - Vector2.UnitY * 5, null, Color.White, forearmAngle + 1, forearmOrigin, Projectile.scale, armFlip, 0);
                }
            }
        }

        //[JITWhenModsEnabled("CalamityMod")]
        private void DrawChain()//用于绘制链条效果，它使用贝塞尔曲线来模拟链条的曲线，然后绘制多个链条段。
        {
            Texture2D chainTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaChain").Value;

            float curvature = MathHelper.Clamp(Math.Abs(Owner.Center.X - Projectile.Center.X) / 50f * 80, 15, 80);

            Vector2 controlPoint1 = Owner.Center - Vector2.UnitY * curvature;
            Vector2 controlPoint2 = Projectile.Center + Vector2.UnitY * curvature;

            BezierCurve curve = new BezierCurve(new Vector2[] { Owner.Center, controlPoint1, controlPoint2, Projectile.Center });
            int numPoints = 20; //"Should make dynamic based on curve length, but I'm not sure how to smoothly do that while using a bezier curve" -Graydee, from the code i referenced. I do agree.
            Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment bar the very first one
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new Vector2(1, yScale);
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                Vector2 origin = new Vector2(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                Main.EntitySpriteDraw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void PostDraw(Color lightColor)//用于在绘制后执行额外的逻辑。在这里，它绘制了机械臂的眼睛和头部
        {
            Texture2D eyesTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaEyes").Value;

            Vector2 offset1 = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);
            float eyeOpacity = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 1f + 0.5f;//闪耀光亮效果

            

            Texture2D headTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaHead").Value;

            Vector2 offset2 = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);

            Main.EntitySpriteDraw(headTex, Projectile.Center + offset2 - Main.screenPosition - Vector2.UnitX * 0 - Vector2.UnitY * 50, null, Color.White, Projectile.rotation, headTex.Size() / 2f, Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(eyesTex, Projectile.Center + offset1 - Main.screenPosition - Vector2.UnitX * 0 - Vector2.UnitY * 50, null, Color.White * eyeOpacity, Projectile.rotation, eyesTex.Size() / 2f, Projectile.scale, 0f, 0);
        }

        public bool summonedProjectile = false;
    }
}
