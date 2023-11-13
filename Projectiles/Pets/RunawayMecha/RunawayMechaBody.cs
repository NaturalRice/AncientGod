using AncientGod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using static Humanizer.In;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace AncientGod.Projectiles.Pets.RunawayMecha
{
    public class RunawayMechaBody : ModFlyingEnemyPet//此类用于实现失控机甲的所有部分的行为。
    {
        public override float TeleportThreshold => 1200f;
        public override Vector2 FlyingOffset => new Vector2(0f, 0f);//这里决定了飞行目标

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

        private Vector2 Forearmposition;//用于不同函数间传值
        private Vector2 Handposition;//用于不同函数间传值
        private Vector2 Armposition;//用于不同函数间传值

        private List<Vector3> ArmPositions;
        //一个列表，用于存储机械臂的位置信息

        public ref float Initialized => ref Projectile.ai[0];
        //一个浮点数引用，用于初始化机械臂

        public Player Owner => Main.player[Projectile.owner];
        //一个引用，表示拥有这个机械臂的玩家

        public override void SetStaticDefaults()
        {
            PetSetStaticDefaults(lightPet: false);
            // DisplayName.SetDefault("Cloudy");
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()//这里没有一块加，要加吗？
        {
            PetSetDefaults();
            Projectile.width = 120;
            Projectile.height = 134;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
            Projectile.light = 0.8f; // 设置光亮度（可以根据需要调整值）

            //Projectile.CloneDefaults(197);
            //base.AIType = 197;
        }

        public override bool PreAI()
        {                    
            //float exponent = 2.0f;//指数，二次方
            //if (((float)Math.Pow(Projectile.Center.X - Owner.position.X, exponent) + (float)Math.Pow(Projectile.Center.Y - Owner.position.Y, exponent)) <= 120)//如果离机甲距离小于120

            float distanceSquared = Vector2.DistanceSquared(Projectile.Center, Owner.Center);
            float distanceSquared1 = Vector2.DistanceSquared(Forearmposition, Owner.Center);
            float distanceSquared2 = Vector2.DistanceSquared(Handposition, Owner.Center);
            // 设置一个距离阈值的平方，避免使用开方操作提高性能
            float distanceThresholdSquared = 120 * 120;
            if (distanceSquared < distanceThresholdSquared || distanceSquared1 < distanceThresholdSquared || distanceSquared2 < distanceThresholdSquared)
            //Math.Pow 接受两个参数，第一个是底数，第二个是指数，返回底数的指数次幂
            {
                // 这里可以根据需要调整伤害值
                int damage = 50;
                // 在这里扣除玩家的生命值
                Owner.Hurt(PlayerDeathReason.ByCustomReason(Owner.name + " 被失控机甲整自闭了,十分甚至九分地自闭 "), damage, -10);
            }
            return true;
        }

        /*private void CheckDamage()//似乎没啥卵用
        {
            int life = 100;
            int damage = 0;
            if (Projectile.damage > 0 && Projectile.localAI[1] == 0f)
            {
                // 如果投射物受到伤害，处理逻辑
                damage = Projectile.damage;
                Projectile.localAI[1] = 1f;
            }

            if (damage > 0)
            { 
                // 处理伤害逻辑，例如减少生命值
                life -= damage;
                if (life <= 0)
                {
                    Projectile.Kill(); // 如果生命值小于等于零，销毁投射物
                }
            }
        }*/

        public override void AI()
        {
            /*这是机械臂的主要逻辑部分。在这个方法中，机械臂的行为受到不同条件的控制，包括拥有者是否已死亡、是否激活了坐骑等。
            机械臂的位置、浮动效果和旋转角度等都在这个方法中计算和更新。
            此外，机械臂的位置也会受到鼠标光标的位置影响，使其能够朝向鼠标光标。*/

            //CheckDamage(); // 检查是否受到伤害

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

            PetBehaviour();

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
            DrawChain();//链条暂时未使用


            if (ArmPositions == null)
                return true;

            Texture2D teslaTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaTesla")).Value;
            Texture2D laserTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaLaser")).Value;
            Texture2D nukeTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaNuke")).Value;
            Texture2D plasmaTex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaPlasma")).Value;
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
            Vector2 position = handPosition - Projectile.Center;
            bool flipped = Math.Sign(position.X) != -1;

            Texture2D armTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaArm").Value;//大臂
            Texture2D forearmTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaForearm").Value;//小臂

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

            //这里，forearmPosition 是通过 elbowPosition 和 forearmAngle 计算得到的。首先，通过 elbowPosition 加上 forearmAngle 方向的矢量来确定下臂的位置。
            //forearmAngle 是下臂与手部之间的角度，它是通过(handPosition - elbowPosition).ToRotation() 计算的。
            Vector2 forearmPosition = elbowPosition + forearmAngle.ToRotationVector2() * (((elbowPosition - handPosition).Length() - 40) / 2f);

            if (forearmPosition.Y > Owner.position.Y)//如果是下方的大臂则小臂用另一套相对位置
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

            Forearmposition = Projectile.Center;
            Handposition = handPosition;
            Armposition = armPosition;
        }

        private void DrawChain()//用于绘制链条效果，它使用贝塞尔曲线来模拟链条的曲线，然后绘制多个链条段，这段代码用于绘制一条连接玩家机甲的牵引线
        {//作为Boss,是否可以改写一下这里，使得链条用于牵引机械臂呢？
            Texture2D chainTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaChain").Value;

            int numPoints = 10; //"Should make dynamic based on curve length, but I'm not sure how to smoothly do that while using a bezier curve" -Graydee, from the code i referenced. I do agree.
            float curvature = MathHelper.Clamp(Math.Abs(Forearmposition.X - Handposition.X) / 50f * 80, 15, 80);

            Vector2 controlPoint1 = Forearmposition - Vector2.UnitY * curvature;//端点1
            Vector2 controlPoint2 = Handposition + Vector2.UnitY * curvature;//端点2
            Vector2 controlPoint3 = (Projectile.Center * 2 - Forearmposition) * Vector2.UnitX + Forearmposition * Vector2.UnitY - Vector2.UnitY * curvature;//端点3
            Vector2 controlPoint4 = (Projectile.Center * 2 - Handposition) * Vector2.UnitX + Handposition * Vector2.UnitY + Vector2.UnitY * curvature;//端点4
            Vector2 controlPoint5 = Forearmposition * Vector2.UnitX + (Projectile.Center * 2 - Forearmposition) * Vector2.UnitY - Vector2.UnitY * curvature;//端点5
            Vector2 controlPoint6 = Handposition * Vector2.UnitX + (Projectile.Center * 2 - Handposition) * Vector2.UnitY + Vector2.UnitY * curvature;//端点6
            Vector2 controlPoint7 = (Projectile.Center * 2 - Forearmposition) * Vector2.UnitX + (Projectile.Center * 2 - Forearmposition) * Vector2.UnitY - Vector2.UnitY * curvature;//端点7
            Vector2 controlPoint8 = (Projectile.Center * 2 - Handposition) * Vector2.UnitX + (Projectile.Center * 2 - Handposition) * Vector2.UnitY + Vector2.UnitY * curvature;//端点8

            BezierCurve curve12 = new BezierCurve(new Vector2[] { controlPoint1, controlPoint2, controlPoint2, controlPoint2 });
            BezierCurve curve34 = new BezierCurve(new Vector2[] { controlPoint3, controlPoint4, controlPoint4, controlPoint4 });
            BezierCurve curve56 = new BezierCurve(new Vector2[] { controlPoint5, controlPoint6, controlPoint6, controlPoint6 });
            BezierCurve curve78 = new BezierCurve(new Vector2[] { controlPoint7, controlPoint8, controlPoint8, controlPoint8 });

            Vector2[] chainPositions12 = curve12.GetPoints(numPoints).ToArray();
            Vector2[] chainPositions34 = curve34.GetPoints(numPoints).ToArray();
            Vector2[] chainPositions56 = curve56.GetPoints(numPoints).ToArray();
            Vector2[] chainPositions78 = curve78.GetPoints(numPoints).ToArray();


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

                Vector2 position78 = chainPositions78[i];
                float rotation78 = (chainPositions78[i] - chainPositions78[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale78 = Vector2.Distance(chainPositions78[i], chainPositions78[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale78 = new Vector2(1, yScale78);
                Color chainLightColor78 = Lighting.GetColor((int)position78.X / 16, (int)position78.Y / 16); //Lighting of the position of the chain segment
                Main.EntitySpriteDraw(chainTex, position78 - Main.screenPosition, null, chainLightColor78, rotation78, origin, scale78, SpriteEffects.None, 0);
            }
        }

        public override void PostDraw(Color lightColor)//用于在绘制后执行额外的逻辑。在这里，它绘制了机械臂的眼睛
        {
            Texture2D eyesTex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Pets/RunawayMecha/RunawayMechaEyes").Value;

            Vector2 offset = Utils.SafeNormalize(Main.MouseWorld - (Projectile.Center - Vector2.UnitY * 10), Vector2.Zero) * MathHelper.Clamp((Projectile.Center - Vector2.UnitY * 10 - Main.MouseWorld).Length(), 0, 1);
            float eyeOpacity = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 0.5f;

            Main.EntitySpriteDraw(eyesTex, Projectile.Center + offset - Main.screenPosition, null, Color.White * eyeOpacity, Projectile.rotation, eyesTex.Size() / 2f, Projectile.scale, 0f, 0);
        }

        public bool summonedProjectile = false;

        public override void PetFunctionality(Player player)
        {
            AncientGodPlayer modPlayer = player.GetModPlayer<AncientGodPlayer>();

            /*if (player.dead)
                modPlayer.RunawayMecha = false;*/

            if (modPlayer.RunawayMecha)
                Projectile.timeLeft = 2;
        }


        public override void Animation(int state)
        {
            SimpleAnimation(speed: 7);
        }
    }
}
