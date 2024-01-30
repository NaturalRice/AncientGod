using AncientGod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace AncientGod.Projectiles.Ammo.AlmightyMechaBullet
{
    public class AlmightlyMechaMissile : ModProjectile
    {
        //以下到SetStaticDefaults之前是尾焰的
        public Vector2[] PositionsTailFlame;
        public float TailFlameRotation;
        public Color RibbonStartColor = new Color(4, 24, 25);
        public static int TrailLenght = 30;
        public ref float Initialized => ref Projectile.ai[0];
        public ref float Behavior => ref Projectile.ai[1];

        public Player Owner => Main.player[Projectile.owner];
        private bool shouldPlayLastFrames = false;
        private int target = -1; // 保存当前目标敌人的索引
        private float homingStrength = 0.02f; // 调整追踪的强度
        //当你希望使子弹像导弹一样具有追踪轨迹时，可以使用插值（interpolation）来平滑地调整子弹的速度和方向
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // 记录旧位置的长度
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // 记录模式            
        }

        public override void SetDefaults()
        {
            Projectile.width = 60; // 投射物碰撞盒的宽度
            Projectile.height = 60; // 投射物碰撞盒的高度
            Projectile.aiStyle = 1; // 投射物的AI风格，请参考Terraria源代码
            Projectile.friendly = true; // 投射物能对敌人造成伤害吗？
            Projectile.hostile = false; // 投射物能对玩家造成伤害吗？
            Projectile.DamageType = DamageClass.Ranged; // 投射物的伤害类型，是由远程武器发射的吗？
            Projectile.penetrate = 2; // 投射物能穿透多少个敌怪。（OnTileCollide方法也会减少穿透次数）
            Projectile.timeLeft = 600; // 投射物的生存时间（60 = 1秒，所以600是10秒）
            Projectile.alpha = 255; // 投射物的透明度，255为完全透明。（aiStyle 1会快速使投射物变透明。如果不使用逐渐变透明的aiStyle，请删除这一行，否则你的投射物会变得看不见）
            Projectile.light = 1f; // 投射物周围的光照强度
            Projectile.ignoreWater = false; // 投射物的速度是否受水影响？
            Projectile.tileCollide = true; // 投射物能否与地块碰撞？
            Projectile.extraUpdates = 1; // 如果希望投射物在一帧内更新多次，请设置大于0的值
            AIType = ProjectileID.Bullet;

            AIType = ProjectileID.Bullet; // 行为与默认子弹完全相同
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 如果与地块碰撞，减少穿透次数。
            // 因此，投射物最多可以反射0次
            Projectile.penetrate--;
            if (Projectile.penetrate < 2)
            {
                shouldPlayLastFrames = true;
                Explode();
                //Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            }

            return false;
        }

        private void Explode()//添加爆炸效果
        {
            // 产生爆炸粒子。
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireflyHit);
                Main.dust[dustIndex].noGravity = true;
            }

            // 播放爆炸声音。
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // 对附近的NPC造成范围伤害。
            int explosionRadius = 100; // 调整爆炸的范围。
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
                {
                    shouldPlayLastFrames = true;
                    npc.SimpleStrikeNPC(100, 0, true, 0f);
                    // 如果需要，可以在这里添加更多效果或逻辑。
                }
            }
        }

        public override void AI()
        {// 添加这个AI方法以检查与NPC的碰撞并造成伤害
            // 如果尚未锁定目标或者目标敌人已经消失，寻找新的目标
            if (target == -1 || !Main.npc[target].active)
            {
                float maxTargetDistance = 3000f; // 设置搜索范围
                int newTarget = -1;

                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc.Distance(Projectile.Center) < maxTargetDistance)
                    {
                        maxTargetDistance = npc.Distance(Projectile.Center);
                        newTarget = i;
                    }
                }

                target = newTarget;
            }

            // 如果找到了目标，追踪目标
            if (target != -1)
            {
                NPC targetNPC = Main.npc[target];

                // 使用插值调整子弹的速度和方向
                Vector2 desiredVelocity = targetNPC.Center - Projectile.Center;
                desiredVelocity.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity * 16f, homingStrength);

                // 使用插值调整子弹的旋转方向
                float targetRotation = MathHelper.WrapAngle((float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X));
                float currentRotation = MathHelper.WrapAngle(Projectile.rotation);
                float newRotation = MathHelper.Lerp(currentRotation, targetRotation, homingStrength);
                Projectile.rotation = newRotation;
                //当导弹离目标足够近时将彻底锁定目标
                if(Vector2.Distance(targetNPC.Center, Projectile.Center) < 50)
                {
                    Projectile.velocity = (targetNPC.Center - Projectile.Center) * 16f;
                }

                // 在这里可以添加其他追踪目标的逻辑
            }

            // 循环遍历所有NPC
            foreach (NPC npc in Main.npc)
            {
                // 检查NPC是否活跃、非友好，并且与投射物的碰撞框相交
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(Projectile.Hitbox))
                {
                    shouldPlayLastFrames = true;
                    // 对NPC造成伤害,伤害为100
                    Explode();
                    npc.SimpleStrikeNPC(40, 0, true, 0f);
                    // 如果需要，可以在此添加更多效果或逻辑。
                }
            }
            //尾焰
            if (Initialized == 0)
            {
                //Initialize the arm positions
                PositionsTailFlame = new Vector2[TrailLenght];
                for (int i = 0; i < TrailLenght; i++)
                {
                    PositionsTailFlame[i] = Projectile.Center;
                }
                Initialized = 1f;
            }
            
            UpdateTwins();
            // ...
            base.AI();
        }

        public void UpdateTwins()
        {
            //Projectile.Center = Owner.Center;
            //Shift down the previous positions
            for (int i = 0; i < TrailLenght - 1; i++)
            {
                PositionsTailFlame[i] = PositionsTailFlame[i + 1];
            }

            //此处调整尾焰的位置变化规律
            PositionsTailFlame[TrailLenght - 1] = Vector2.Lerp(PositionsTailFlame[TrailLenght - 1], Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) + Projectile.velocity, 0.5f);
            TailFlameRotation = TailFlameRotation.AngleLerp(Projectile.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);


            float thrusterOpacity1 = (1 - MathHelper.Clamp((float)Math.Sin(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果
            float thrusterOpacity2 = (1 - MathHelper.Clamp((float)Math.Cos(Main.time % MathHelper.Pi) * 2f, 0, 1)) * 1f + 0.1f;//闪耀光亮效果

            Color IdealColor = Color.Lerp(Color.LightSkyBlue * thrusterOpacity2, Color.Orange * thrusterOpacity1, MathHelper.Clamp(Owner.velocity.Length() - 5, 0, 20) / 20f);
            RibbonStartColor = Color.Lerp(RibbonStartColor, IdealColor, 0.2f);

            //尾焰
            float[] RotationsTailFlame = new float[PositionsTailFlame.Length];
            for (int i = 1; i < RotationsTailFlame.Length; i++)
            {
                RotationsTailFlame[i] = PositionsTailFlame[i - 1].AngleTo(PositionsTailFlame[i]);
            }
            RotationsTailFlame[0] = TailFlameRotation;

            Terraria.Graphics.VertexStrip tailFlameStrip = new Terraria.Graphics.VertexStrip();
            //此处用于显示尾焰效果(目前似乎并无显示效果）
            tailFlameStrip.PrepareStripWithProceduralPadding(PositionsTailFlame, RotationsTailFlame, OrangeRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);
            //使用 Effect 和自定义的着色方法实现了轨迹的颜色渐变效果。
            Effect vertexShader = ModContent.Request<Effect>($"{nameof(AncientGod)}/Effects/VertexShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            vertexShader.Parameters["uColor"].SetValue(Vector4.One);
            vertexShader.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            vertexShader.CurrentTechnique.Passes[0].Apply();

            tailFlameStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
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

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            SimpleAnimation(1);
            // 使用不受光照影响的颜色重新绘制投射物
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Main.EntitySpriteDraw(texture, Projectile.position, frame, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);            

            return true;
        }
        public void SimpleAnimation(int speed)//设置帧播放速度，数值越大，速度越慢
        {
            Projectile.frameCounter++;

            if (shouldPlayLastFrames == true)
            {
                Projectile.velocity = Vector2.Zero;
                if (Projectile.frame < 8)
                    Projectile.frame = 8;
                Projectile.frame++;
                if (Projectile.frame > 15)
                    Projectile.Kill();
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity *= 1.4f;
                }
                // 播放爆炸声音(是否需要什么声音？）
                //SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                if (Projectile.frameCounter > speed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 7)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
        }
    }
}
