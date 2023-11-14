using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;

namespace AncientGod.Projectiles.Mounts.InfiniteFlight.AncientMecha
{
    public class AncientMechaBody : ModProjectile//此类用于实现AncientMecha的机械臂的行为。
    {
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


        public override void AI()
        {
            /*这是机械臂的主要逻辑部分。在这个方法中，机械臂的行为受到不同条件的控制，包括拥有者是否已死亡、是否激活了坐骑等。
            机械臂的位置、浮动效果和旋转角度等都在这个方法中计算和更新。
            此外，机械臂的位置也会受到鼠标光标的位置影响，使其能够朝向鼠标光标。*/
            AncientGodPlayer modPlayer = Owner.GetModPlayer<AncientGodPlayer>();
            if (Owner.dead || !Owner.mount.Active)//此处添加了一个条件用来保证坐骑消失时这些投射物也会消失
                Projectile.timeLeft = 0;
            if (!modPlayer.AncientMecha)
                Projectile.timeLeft = 0;
            if (modPlayer.AncientMecha && Owner.mount.Active)//当然，还要再在这里强调一遍才能达到效果
                Projectile.timeLeft = 2;

            


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
            Projectile.Center = Vector2.Lerp(Projectile.Center, idealPosition, 0.2f);
            Projectile.Center.MoveTowards(idealPosition, 2);

            //Teleport ares to the ideal position if the owner is too far
            if ((idealPosition - Projectile.Center).Length() > 0)//这里由2000改为0，使得机械臂在快速移动时不会脱离得太离谱
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
                    vector2position = Vector2.Lerp(vector2position, idealArmPosition, 0.2f);

                    //Make the cannon look at the mouse cursor if its close enough

                    armPosition.Z = Utils.AngleLerp(armPosition.Z, IdealPositions[i].Z, MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));

                    armPosition.Z = Utils.AngleLerp(armPosition.Z, (vector2position - Main.MouseWorld).ToRotation(), 1 - MathHelper.Clamp((Main.MouseWorld - vector2position).Length() / 300f, 0, 1));


                    ArmPositions[i] = new Vector3(vector2position, armPosition.Z);
                }
            }
        }
        public override bool PreDrawExtras()//用于在绘制前执行额外的逻辑。其中，DrawChain 方法用于绘制链条效果
        {
            DrawChain();


            if (ArmPositions == null)
                return true;

            Texture2D teslaTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaTesla")).Value;
            Texture2D laserTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaLaser")).Value;
            Texture2D nukeTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaNuke")).Value;
            Texture2D plasmaTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaPlasma")).Value;
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

            Texture2D armTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaArm").Value;//大臂
            Texture2D forearmTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaForearm").Value;//小臂

            Rectangle armFrame = new Rectangle(0, top ? 0 : 59, armTex.Width, 60);
            Rectangle handFrame = new Rectangle(0, infernum ? 40 : 0, 60, 38);

            Vector2 armOrigin = new Vector2(flipped ? 0 : armFrame.Width, armFrame.Height / 3);//这里由除2改成除3
            Vector2 forearmOrigin = new Vector2(flipped ? 0 : forearmTex.Width, forearmTex.Height);//这里去掉除2
            Vector2 handOrigin = new Vector2(flipped ? 28 : 22, 22); //22

            Vector2 armPosition = Projectile.Center + position * 0.1f;

            //Do some trigonometry to get the elbow position

            float armLenght = 175;
            float directLenght = MathHelper.Clamp((handPosition - armPosition).Length(), 0, armLenght); //Clamp the direct lenght to avoid getting an error from trying to calculate the square root of a negative number
            float elbowElevation = (float)Math.Sqrt(Math.Pow(armLenght / 2f, 2) - Math.Pow(directLenght / 2f, 2));
            Vector2 elbowPosition = Vector2.Lerp(handPosition, armPosition, 0.5f) + Utils.SafeNormalize(position, Vector2.Zero).RotatedBy(-MathHelper.PiOver2 * Math.Sign(position.X)) * elbowElevation;

            float armAngle = (elbowPosition - armPosition).ToRotation();
            float forearmAngle = (handPosition - elbowPosition).ToRotation();

            /*这里，forearmPosition 是通过 elbowPosition 和 forearmAngle 计算得到的。首先，通过 elbowPosition 加上 forearmAngle 方向的矢量来确定下臂的位置。
            forearmAngle 是下臂与手部之间的角度，它是通过 (handPosition - elbowPosition).ToRotation() 计算的。*/
            Vector2 forearmPosition = elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40) / 2f);

            if(forearmPosition.Y > Owner.position.Y)//如果是下方的大臂则小臂用另一套相对位置
            {
                forearmPosition = elbowPosition - forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40)) * 0.6f;
            }


            armPosition += armAngle.ToRotationVector2() * (top ? 30 : 20);

            armAngle += flipped ? 0 : MathHelper.Pi;
            forearmAngle += flipped ? 0 : MathHelper.Pi;


            SpriteEffects flip = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects armFlip = flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //screenPosition 类似的变量用于将游戏中的虚拟世界坐标转换为屏幕上的像素坐标，offset 是一个用于微调绘制位置的向量，通常用于调整绘制对象在屏幕上的位置。
            Main.EntitySpriteDraw(armTex, armPosition + offset - Main.screenPosition, armFrame, Color.White, armAngle, armOrigin, Projectile.scale, armFlip, 0);
            Main.EntitySpriteDraw(forearmTex, forearmPosition + offset - Main.screenPosition, null, Color.White, forearmAngle, forearmOrigin, Projectile.scale, armFlip, 0);
           
            

            if (forearmPosition.Y > Owner.position.Y)//如果是下方的大臂则手部用另一套相对位置（不然手部离得太远）
            {
                //再判断左右边
                if (forearmPosition.X > Owner.position.X)//在右边则减去几个X轴和Y轴单位向量（Unit.X，Unit.Y)
                {
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition - Vector2.UnitX * 110 - Vector2.UnitY * 140, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);
                }
                else//在左边则加上几个X轴和Y轴单位向量（Unit.X，Unit.Y)，经过多次调整，目前这个数值应该最合适
                {
                    Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition + Vector2.UnitX * 110 - Vector2.UnitY * 140, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);
                }
                
            }
            else
            {
                Main.EntitySpriteDraw(handTex, handPosition + offset - Main.screenPosition, handFrame, Color.White, rotation + (flipped ? 0 : MathHelper.Pi), handOrigin, Projectile.scale, flip, 0);
            }
        }

        //[JITWhenModsEnabled("CalamityMod")]
        private void DrawChain()//用于绘制链条效果，它使用贝塞尔曲线来模拟链条的曲线，然后绘制多个链条段，这段代码用于绘制一条连接玩家机甲的牵引线
        {
            Texture2D chainTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaChain").Value;

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

        public override void PostDraw(Color lightColor)//用于在绘制后执行额外的逻辑。在这里，它绘制了机械臂的眼睛
        {
            Texture2D eyesTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaEyes").Value;

            Vector2 offset = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);
            float eyeOpacity = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 0.5f;

            Main.EntitySpriteDraw(eyesTex, Projectile.Center + offset - Main.screenPosition, null, Color.White * eyeOpacity, Projectile.rotation, eyesTex.Size() / 2f, Projectile.scale, 0f, 0);
        }

        public bool summonedProjectile = false;

    }
}