using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Projectiles.Ammo.FourDimensionalInsectBullet
{
    public class TrackingCentralBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 1000;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;

            AIType = ProjectileID.Bullet;
            Main.projFrames[Projectile.type] = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 绘制你的自定义纹理或激光束
            DrawLaser(lightColor);
            return false; // 返回false以避免绘制默认的弹药纹理
        }

        private void DrawLaser(Color lightColor)
        {
            // 将 "YourTexture" 替换为你自定义激光纹理的路径
            Texture2D laserTexture = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Ammo/FourDimensionalInsectBullet/TrackingCentralBeam")).Value;

            // 计算绘制位置和旋转
            Vector2 drawOrigin = new Vector2(laserTexture.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // 绘制激光纹理
            Main.EntitySpriteDraw(laserTexture, drawPos, null, lightColor, rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }


        public override void AI()
        {
            foreach (NPC npc in Main.npc)
            {
                Vector2 projectilePosition = Projectile.position;
                Vector2 projectileDimensions = new Vector2(Projectile.width, Projectile.height);

                Vector2 npcPosition = npc.position;
                Vector2 npcDimensions = new Vector2(npc.width, npc.height);

                if (npc.active && !npc.friendly && Collision.CheckAABBvAABBCollision(projectilePosition, projectileDimensions, npcPosition, npcDimensions) && (projectilePosition.X - npcPosition.X) <= 15 && (projectilePosition.X - npcPosition.X) >= -5)
                //if (npc.active && !npc.friendly && npc.Hitbox.Intersects(Projectile.Hitbox))
                {
                    npc.SimpleStrikeNPC(100, 0, true, 0f);
                }
            }

            // 移动代码移到碰撞检测之后
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]) * 1.1f;

            if (Projectile.timeLeft <= 300)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

    }
}
