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
using Humanizer;
using System.Reflection;
using System.Reflection.Metadata;

namespace AncientGod.Projectiles.Mounts.InfiniteFlight.FourDimensionalInsect
{
    public class FourDimensionalInsectBody : ModProjectile//此类用于实现四维虫子的机械臂的行为。
    {
        public int owner; // 自定义字段，用于存储投射物的源

        private const float TargetDistance = 1200f; // 设置寻找敌人的最大距离
        private int target = -1; // 记录当前目标敌人的索引
        private float fireCooldown = 60f; // 弹药冷却时间

        //A list of the ideal positions for each arm. The first 2 variables of the Vector2 represent the relative position of the arm to the body and the last variable represents the rotation of the hand
        internal readonly List<Vector3> IdealPositions = new List<Vector3>()//一个列表，其中包含了四个Vector3元素，每个元素表示一个机械臂的理想位置。这些位置包括相对于机械臂中心的X和Y偏移，以及手的旋转角度
        {
            new Vector3(-170f, -250f, MathHelper.ToRadians(240)), //Top left arm  此处调整六个大臂的位置
            new Vector3(-370f, 0f, MathHelper.ToRadians(240)), //Middle left arm  
            new Vector3(-370f, 360f, MathHelper.ToRadians(270)), //Bottom left arm
            
            new Vector3(370f, 360f, MathHelper.ToRadians(270)), //Bottom right arm
            new Vector3(370f, 0f, MathHelper.ToRadians(270)), //Middle right arm
            new Vector3(170f, -250f, MathHelper.ToRadians(300)) //Top right arm
        };

        internal readonly List<Vector3> IdealWingPositions = new List<Vector3>()//一个列表，其中包含了四个Vector3元素，每个元素表示一个机械臂的理想位置。这些位置包括相对于机械臂中心的X和Y偏移，以及手的旋转角度
        {
            new Vector3(-170f, -250f, MathHelper.ToRadians(0)), //左外翼
            new Vector3(-370f, 0f, MathHelper.ToRadians(0)), //左中翼  
            new Vector3(-370f, 360f, MathHelper.ToRadians(0)), //左内翼
            new Vector3(-370f, 400f, MathHelper.ToRadians(0)), //左尾翼
            
            new Vector3(370f, 400f, MathHelper.ToRadians(0)), //右尾翼
            new Vector3(370f, 0f, MathHelper.ToRadians(0)), //右内翼
            new Vector3(170f, -250f, MathHelper.ToRadians(0)), //右中翼
            new Vector3(170f, -250f, MathHelper.ToRadians(0)) //右外翼
        };

        private Vector2 GetIdealPosition(int i)
        {
            return Projectile.Center + new Vector2(IdealPositions[i].X, IdealPositions[i].Y);//去关键帧
        }
        private Vector2 GetWingIdealPosition(int i)
        {
            return Projectile.Center + new Vector2(IdealWingPositions[i].X, IdealWingPositions[i].Y);//去关键帧
        }

        //private Vector2 BobVector => new Vector2(0, -2 + 4 * MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi), 0, 1) * 0.7f + 0.3f);
        //一个Vector2，用于模拟机械臂上下浮动的动画效果（暂时不使用）

        private List<Vector3> ArmPositions;
        //一个列表，用于存储机械臂的位置信息

        private List<Vector3> WingPositions;
        //一个列表，用于存储翅膀的位置信息

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
            if (!modPlayer.FourDimensionalInsect)
            {
                Projectile.timeLeft = 0;
                Projectile.active = false;
            }
            if (modPlayer.FourDimensionalInsect && Owner.mount.Active)//当然，还要再在这里强调一遍才能达到效果
            {
                Projectile.timeLeft = 2;


            }


            if (Initialized == 0)
            {
                //Initialize the arm positions
                ArmPositions = new List<Vector3>(6);
                WingPositions = new List<Vector3>(8);
                for (int i = 0; i < 6; i++)
                {
                    ArmPositions.Add(new Vector3(GetIdealPosition(i), IdealPositions[i].Z));
                }
                for(int i = 0; i < 8; i++)
                {
                    WingPositions.Add(new Vector3(GetWingIdealPosition(i), IdealWingPositions[i].Z));
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
                    vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.6f);//手部位置鬼畜的问题出在这里的Lerp函数；经过多次测试，目前此数值0.6最适合

                    //Make the cannon look at the mouse cursor if its close enough

                    armPosition.Z = Utils.AngleLerp(armPosition.Z, IdealPositions[i].Z, MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));

                    //armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - Main.MouseWorld).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));
                    //两个armPosition.Z都是必要的，后者用于在鼠标靠近时瞄准鼠标，前者用于鼠标远离时重新指向地面

                    ArmPositions[i] = new Vector3(vector2position, armPosition.Z);

                    // 寻找附近的敌人
                    FindTarget(); // 调用FindTarget方法来更新target值

                    if (target != -1)
                    {
                        NPC targetNPC = Main.npc[target];


                        //手部瞄准敌人                        
                        Vector2 direction = targetNPC.Center - Projectile.position;


                        armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - direction).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));

                        direction.Normalize();

                        Projectile.velocity = direction * 16f;



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
                                    //int newProjectile11 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, shotVelocity, ModContent.ProjectileType<MachineGunBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    //int newProjectile12 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    //int newProjectile13 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 170 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右下
                                    direction = targetNPC.Center - (Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    //int newProjectile21 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, shotVelocity, ModContent.ProjectileType<MachineGunBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左上
                                    //int newProjectile22 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左上
                                    //int newProjectile23 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 140 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左上
                                    direction = targetNPC.Center - (Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    //int newProjectile31 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, shotVelocity, ModContent.ProjectileType<MachineGunBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右上
                                    //int newProjectile32 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右上
                                    //int newProjectile33 = Projectile.NewProjectile(null, Projectile.position + Vector2.UnitX * 195 + Vector2.UnitY * 50, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//右上
                                    direction = targetNPC.Center - (Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150);
                                    shootAngle = direction.ToRotation();
                                    shotVelocity = Vector2.UnitX.RotatedBy(shootAngle) * 8f;
                                    //int newProjectile41 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, shotVelocity, ModContent.ProjectileType<MachineGunBullet>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //int newProjectile42 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle - 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //int newProjectile43 = Projectile.NewProjectile(null, Projectile.position - Vector2.UnitX * 120 + Vector2.UnitY * 150, Vector2.UnitX.RotatedBy(shootAngle + 50) * 8f, ModContent.ProjectileType<FourDimensionalInsectBulletSide>(), (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);//左下
                                    //这里四个机关炮的准心有一定差别
                                    //Main.projectile[newProjectile11].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile12].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile13].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile21].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile22].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile23].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile31].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile32].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile33].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile41].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile42].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile43].timeLeft = 1000;//弹药存活时间
                                    //Main.projectile[newProjectile11].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile12].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile13].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile21].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile22].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile23].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //.projectile[newProjectile31].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile32].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile33].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile41].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile42].netUpdate = true;//是否将投射物的信息同步到其他客户端
                                    //Main.projectile[newProjectile43].netUpdate = true;//是否将投射物的信息同步到其他客户端


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

            if (WingPositions != null)
            {
                for (int i = 0; i < WingPositions.Count; i++)
                {
                    //Make the arm lag behind a bit
                    Vector3 wingPosition = WingPositions[i];
                    Vector2 idealWingPosition = GetWingIdealPosition(i);
                    Vector2 vector2position = new Vector2(wingPosition.X, wingPosition.Y);
                    //vector2position 用于确定机械臂的位置，即机械臂的上部（臂部）的位置。这个位置是根据理想位置、浮动效果和其他变化因素计算出来的。
                    //vector2position 在接下来的计算中用于控制机械臂的位置，并使其在上下浮动以及跟踪鼠标光标时具有平滑的运动效果。这有助于模拟机械臂的运动和定位。


                    //vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.2f) + BobVector;//如果机械臂相对位置摆动得太逆天，可能要禁用这个动画效果
                    vector2position = Vector2.Lerp(vector2position, idealWingPosition, 0.6f);//手部位置鬼畜的问题出在这里的Lerp函数；经过多次测试，目前此数值0.6最适合

                    //Make the cannon look at the mouse cursor if its close enough

                    wingPosition.Z = Utils.AngleLerp(wingPosition.Z, IdealWingPositions[i].Z, MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));

                    //armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - Main.MouseWorld).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));
                    //两个armPosition.Z都是必要的，后者用于在鼠标靠近时瞄准鼠标，前者用于鼠标远离时重新指向地面

                    WingPositions[i] = new Vector3(vector2position, wingPosition.Z);
                }
            }
        }
        public override bool PreDrawExtras()//用于在绘制前执行额外的逻辑。其中，DrawChain 方法用于绘制链条效果
        {
            //DrawChain();            

            if (ArmPositions == null)
                return true;

            Texture2D tophandTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectHand/FourDimensionalInsectTopHand")).Value;
            Texture2D middlehandTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectHand/FourDimensionalInsectMiddleHand")).Value;
            Texture2D bottomhandTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectHand/FourDimensionalInsectBottomHand")).Value;


            //Top arms
            DrawSingleArm(tophandTex, new Vector2(ArmPositions[0].X, ArmPositions[0].Y), ArmPositions[0].Z, new Vector2(-5, -10), true, false);
            DrawSingleArm(tophandTex, new Vector2(ArmPositions[5].X, ArmPositions[5].Y), ArmPositions[5].Z, new Vector2(-5, -10), true, false);

            //Middle arms
            DrawSingleArm(middlehandTex, new Vector2(ArmPositions[1].X, ArmPositions[1].Y), ArmPositions[1].Z, new Vector2(0, -10), false, true);
            DrawSingleArm(middlehandTex, new Vector2(ArmPositions[4].X, ArmPositions[4].Y), ArmPositions[4].Z, new Vector2(0, -10), false, true);

            //Bottom arms
            DrawSingleArm(bottomhandTex, new Vector2(ArmPositions[2].X, ArmPositions[2].Y), ArmPositions[2].Z, Vector2.Zero, false, false);
            DrawSingleArm(bottomhandTex, new Vector2(ArmPositions[3].X, ArmPositions[3].Y), ArmPositions[3].Z, Vector2.Zero, false, false);

            return true;
        }

        public void DrawSingleArm(Texture2D handTex, Vector2 handPosition, float rotation, Vector2 offset, bool top, bool middle, bool infernum = false)
        {
            //用于绘制机械臂的各个部分，包括上臂、下臂和手部。
            //这里，handPosition 是通过 Projectile.Center（机械臂的中心）加上 position 乘以一个缩放系数来确定的。
            //handPosition 用于确定机械臂的手部位置，也就是机械臂的手部的坐标。
            /*position 是一个矢量，代表手部相对于机械臂中心的偏移。通过将这个偏移乘以 0.1f，你可以调整手部相对于机械臂中心的位置。
            这个值的改变可以影响手部的距离，使它更接近或更远离机械臂中心。*/
            Vector2 position = handPosition - Projectile.Center;
            bool flipped = Math.Sign(position.X) != -1;

            Texture2D armTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectArm").Value;//大臂
            Texture2D forearmTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectForearm").Value;//小臂

            Rectangle armFrame = new Rectangle(0, top ? 0 : (middle ? 140 : 280), armTex.Width, 140);//判断大臂高度位置
            Rectangle forearmFrame = new Rectangle(0, top ? 0 : (middle ? 140 : 280), forearmTex.Width, 140);//判断小臂高度位置
            Rectangle handFrame = new Rectangle(0, 0, 190, 160);//三个手部图片大小已一致

            //一下是判断是否要翻转贴图
            Vector2 armOrigin = new Vector2(flipped ? 0 : armFrame.Width, armFrame.Height / 2);//这里由除2改成除3
            Vector2 forearmOrigin = new Vector2(flipped ? 0 : forearmFrame.Width, forearmFrame.Height);//这里去掉除2
            Vector2 handOrigin = new Vector2(flipped ? 100 : 90, 90); //22

            Vector2 armPosition = Projectile.Center + position * 0.1f;

            //Do some trigonometry to get the elbow position

            float armLenght = 175;//175
            float directLenght = MathHelper.Clamp((handPosition - armPosition).Length(), 0, armLenght); //Clamp the direct lenght to avoid getting an error from trying to calculate the square root of a negative number
            float elbowElevation = (float)Math.Sqrt(Math.Pow(armLenght / 2f, 2) - Math.Pow(directLenght / 2f, 2));
            Vector2 elbowPosition = Vector2.Lerp(handPosition, armPosition, 0.5f) + Utils.SafeNormalize(position, Vector2.Zero).RotatedBy(-MathHelper.PiOver2 * Math.Sign(position.X)) * elbowElevation;//肘部位置

            //大小手臂角度
            //float armAngle = (elbowPosition - armPosition).ToRotation();
            float armAngle = 0;
            //float forearmAngle = (handPosition - elbowPosition).ToRotation();
            float forearmAngle = 0;

            /*这里，forearmPosition 是通过 elbowPosition 和 forearmAngle 计算得到的。首先，通过 elbowPosition 加上 forearmAngle 方向的矢量来确定下臂的位置。
            forearmAngle 是下臂与手部之间的角度，它是通过 (handPosition - elbowPosition).ToRotation() 计算的。(这里改了）*/
            Vector2 forearmPosition = elbowPosition;

            if (forearmPosition.Y < Owner.position.Y)
            {
                if (forearmPosition.X < Owner.position.X)
                {
                    forearmPosition = elbowPosition;
                }
                else
                {
                    forearmPosition = elbowPosition;
                }
            }

            if (forearmPosition.Y > Owner.position.Y)//如果是下方的大臂则小臂用另一套相对位置
            {
                forearmPosition = elbowPosition;
            }


            //armPosition += armAngle.ToRotationVector2() * (top ? 30 : 20);

            armAngle += flipped ? 0 : MathHelper.Pi;
            forearmAngle += flipped ? 0 : MathHelper.Pi;


            SpriteEffects flip = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects armFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipVertically;

            //screenPosition 类似的变量用于将游戏中的虚拟世界坐标转换为屏幕上的像素坐标，offset 是一个用于微调绘制位置的向量，通常用于调整绘制对象在屏幕上的位置。


            if (forearmPosition.Y > Owner.position.Y)//下方机械臂
            {
                if (forearmPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 50 + Vector2.UnitY * 220, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 105 + Vector2.UnitY * 185, forearmFrame, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);//小臂
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 205 + Vector2.UnitY * 10, handFrame, Color.White, 0, handOrigin, Projectile.scale, flip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 270 + Vector2.UnitY * 220, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 45 + Vector2.UnitY * 45, forearmFrame, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);//小臂
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 205 + Vector2.UnitY * 10, handFrame, Color.White, 0, handOrigin, Projectile.scale, flip, 0);
                }

            }
            else if(forearmPosition.Y < Owner.position.Y && forearmPosition.Y > (Owner.position.Y - 100))//中方机械臂
            {
                if (forearmPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition + Vector2.UnitX * 150 + Vector2.UnitY * 100, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition + Vector2.UnitX * 175 + Vector2.UnitY * 177, forearmFrame, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);//小臂
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 205 + Vector2.UnitY * 140, handFrame, Color.White, 0, handOrigin, Projectile.scale, flip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 380 + Vector2.UnitY * 100, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 338 + Vector2.UnitY * 35, forearmFrame, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);//小臂
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 205 + Vector2.UnitY * 140, handFrame, Color.White, 0, handOrigin, Projectile.scale, flip, 0);
                }
            }
            else//上方机械臂
            {
                if (forearmPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 10 - Vector2.UnitY * 190, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 65 - Vector2.UnitY * 118, forearmFrame, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);//小臂
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 70 - Vector2.UnitY * 55, handFrame, Color.White, 0, handOrigin, Projectile.scale, flip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {
                    Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition - Vector2.UnitX * 210 - Vector2.UnitY * 190, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition - Vector2.UnitX * 83 - Vector2.UnitY * 260, forearmFrame, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);//大臂
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 70 - Vector2.UnitY * 55, handFrame, Color.White, 0, handOrigin, Projectile.scale, flip, 0);
                }
            }
        }

        /*private void DrawChain()//用于绘制链条效果，它使用贝塞尔曲线来模拟链条的曲线，然后绘制多个链条段。
        {
            Texture2D chainTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/BigBangMecha/BigBangMechaChain").Value;

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
        }*/

        public void DrawWing(Texture2D wingTex, Vector2 wingPosition, float rotation, Vector2 offset, bool top, bool middle)
        {
            Vector2 position = wingPosition - Projectile.Center;
            bool flipped = Math.Sign(position.X) != -1;

            Rectangle wingFrame = new Rectangle(0, 0, wingTex.Width, wingTex.Height);//判断大臂高度位置
            Vector2 wingOrigin = new Vector2(flipped ? 0 : wingFrame.Width, wingFrame.Height);//这里去掉除2
            SpriteEffects flip = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (wingPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
            {
                Main.EntitySpriteDraw(wingTex, offset - Main.screenPosition + Vector2.UnitX * 0 + Vector2.UnitY * 0, wingFrame, Color.White, 0, wingOrigin, Projectile.scale, flip, 0);//
            }
            else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
            {
                Main.EntitySpriteDraw(wingTex, offset - Main.screenPosition + Vector2.UnitX * 0 + Vector2.UnitY * 0, wingFrame, Color.White, 0, wingOrigin, Projectile.scale, flip, 0);//
            }
        }

        public override void PostDraw(Color lightColor)//用于在绘制后执行额外的逻辑。在这里，它绘制了机械臂的眼睛
        {
            Texture2D eyesTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectEyes").Value;
            Texture2D topTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectTop").Value;
            Texture2D bottomTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectBottom").Value;

            Vector2 offset = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);
            float eyeOpacity = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 0.5f + 1f;

            Main.EntitySpriteDraw(eyesTex, Projectile.Center + offset - Main.screenPosition - Vector2.UnitX * 3 + Vector2.UnitY * 30, null, Color.White * eyeOpacity, Projectile.rotation, eyesTex.Size() / 2f, Projectile.scale, 0f, 0);

            Texture2D rightbottomwingTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectBottomWingRight").Value;//内翼
            Texture2D rightmiddlewingTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectMiddleWingRight").Value;//中翼
            Texture2D righttopwingTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectTopWingRight").Value;//外翼
            Texture2D rightbottomempennageTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectBottomEmpennageRight").Value;//尾翼
            Texture2D leftbottomwingTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectBottomWingLeft").Value;//内翼
            Texture2D leftmiddlewingTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectMiddleWingLeft").Value;//中翼
            Texture2D lefttopwingTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectTopWingLeft").Value;//外翼
            Texture2D leftbottomempennageTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/FourDimensionalInsect/FourDimensionalInsectWing/FourDimensionalInsectBottomEmpennageLeft").Value;//尾翼

            //右边翅膀
            Main.EntitySpriteDraw(rightbottomwingTex, Projectile.Center + offset - Main.screenPosition + Vector2.UnitX * 440 + Vector2.UnitY * 95, null, Color.White, Projectile.rotation, rightbottomwingTex.Size(), Projectile.scale, 0f, 0);//内翼
            Main.EntitySpriteDraw(rightmiddlewingTex, Projectile.Center + offset - Main.screenPosition + Vector2.UnitX * 640 + Vector2.UnitY * 20, null, Color.White, Projectile.rotation, rightmiddlewingTex.Size(), Projectile.scale, 0f, 0);//中翼
            Main.EntitySpriteDraw(righttopwingTex, Projectile.Center + offset - Main.screenPosition + Vector2.UnitX * 760 - Vector2.UnitY * 50, null, Color.White, Projectile.rotation, righttopwingTex.Size(), Projectile.scale, 0f, 0);//外翼
            Main.EntitySpriteDraw(rightbottomempennageTex, Projectile.Center + offset - Main.screenPosition + Vector2.UnitX * 340 + Vector2.UnitY * 195, null, Color.White, Projectile.rotation, rightbottomempennageTex.Size(), Projectile.scale, 0f, 0);//尾翼
            //左边翅膀
            Main.EntitySpriteDraw(leftbottomwingTex, Projectile.Center + offset - Main.screenPosition - Vector2.UnitX * 170 + Vector2.UnitY * 95, null, Color.White, Projectile.rotation, leftbottomwingTex.Size(), Projectile.scale, 0f, 0);//内翼
            Main.EntitySpriteDraw(leftmiddlewingTex, Projectile.Center + offset - Main.screenPosition - Vector2.UnitX * 230 + Vector2.UnitY * 20, null, Color.White, Projectile.rotation, leftmiddlewingTex.Size(), Projectile.scale, 0f, 0);//中翼
            Main.EntitySpriteDraw(lefttopwingTex, Projectile.Center + offset - Main.screenPosition - Vector2.UnitX * 270 - Vector2.UnitY * 50, null, Color.White, Projectile.rotation, lefttopwingTex.Size(), Projectile.scale, 0f, 0);//外翼
            Main.EntitySpriteDraw(leftbottomempennageTex, Projectile.Center + offset - Main.screenPosition - Vector2.UnitX * 220 + Vector2.UnitY * 195, null, Color.White, Projectile.rotation, leftbottomempennageTex.Size(), Projectile.scale, 0f, 0);//尾翼
            //头部和底部
            Main.EntitySpriteDraw(topTex, Projectile.Center + offset - Main.screenPosition + Vector2.UnitX * 97 - Vector2.UnitY * 140, null, Color.White, Projectile.rotation, topTex.Size(), Projectile.scale, 0f, 0);
            Main.EntitySpriteDraw(bottomTex, Projectile.Center + offset - Main.screenPosition + Vector2.UnitX * 54 + Vector2.UnitY * 400, null, Color.White, Projectile.rotation, bottomTex.Size(), Projectile.scale, 0f, 0);
        }

        public bool summonedProjectile = false;
    }
}
