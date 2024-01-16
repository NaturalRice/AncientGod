/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Projectiles.Ammo.AlmightyMechaBullet
{
    public class AlmightyMechaBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] =8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // 记录旧位置的长度
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // 记录模式
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // 投射物碰撞盒的宽度
            Projectile.height = 48; // 投射物碰撞盒的高度
            Projectile.aiStyle = 1; // 投射物的AI风格，请参考Terraria源代码
            Projectile.friendly = true; // 投射物能对敌人造成伤害吗？
            Projectile.hostile = false; // 投射物能对玩家造成伤害吗？
            Projectile.DamageType = DamageClass.Ranged; // 投射物的伤害类型，是由远程武器发射的吗？
            Projectile.penetrate = 5; // 投射物能穿透多少个敌怪。（OnTileCollide方法也会减少穿透次数）
            Projectile.timeLeft = 600; // 投射物的生存时间（60 = 1秒，所以600是10秒）
            Projectile.alpha = 255; // 投射物的透明度，255为完全透明。（aiStyle 1会快速使投射物变透明。如果不使用逐渐变透明的aiStyle，请删除这一行，否则你的投射物会变得看不见）
            Projectile.light = 1f; // 投射物周围的光照强度
            Projectile.ignoreWater = false; // 投射物的速度是否受水影响？
            Projectile.tileCollide = true; // 投射物能否与地块碰撞？
            Projectile.extraUpdates = 1; // 如果希望投射物在一帧内更新多次，请设置大于0的值


            AIType = ProjectileID.Bullet; // 行为与默认子弹完全相同

        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D glowMask = ModContent.Request<Texture2D>("NaturalRiceFirstMod/Projectiles/Pets/LightPets/DraWPet_Glow").Value;
            Rectangle frame = glowMask.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            frame.Height -= 1;
            float originOffsetX = (glowMask.Width - Projectile.width) * 0.5f + Projectile.width * 0.5f + DrawOriginOffsetX;
            Main.EntitySpriteDraw
            (
                glowMask,
                Projectile.position,
                frame,
                Color.White,
                Projectile.rotation = 0f,
                //这样宠物确实不旋转了，但是光亮跟本体会不重合；我目前的方法是将本体图片变成空白的，光亮图片即原先的本体图片，这样只显示光亮
                new Vector2(originOffsetX, Projectile.height / 2 - DrawOriginOffsetY),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 如果与地块碰撞，减少穿透次数。
            // 因此，投射物最多可以反射5次
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                // 如果投射物击中了地块的左侧或右侧，反转X速度
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                    Explode();
                }

                // 如果投射物击中了地块的顶部或底部，反转Y速度
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                    Explode();
                }
            }

            return false;
        }

        private void Explode()//添加爆炸效果
        {
            // 产生爆炸粒子。
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 1.4f;
            }

            // 播放爆炸声音。
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // 对附近的NPC造成范围伤害。
            int explosionRadius = 100; // 调整爆炸的范围。
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && Vector2.Distance(npc.Center, Projectile.Center) < explosionRadius)
                {
                    npc.SimpleStrikeNPC(40, 0, true, 0f);
                    // 如果需要，可以在这里添加更多效果或逻辑。
                }
            }
        }


        public override void AI()
        {
            // 添加这个AI方法以检查与NPC的碰撞并造成伤害

            // 循环遍历所有NPC
            foreach (NPC npc in Main.npc)
            {
                // 检查NPC是否活跃、非友好，并且与投射物的碰撞框相交
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(Projectile.Hitbox))
                {
                    // 对NPC造成伤害,伤害为100
                    Explode();
                    npc.SimpleStrikeNPC(40, 0, true, 0f);
                    // 如果需要，可以在此添加更多效果或逻辑。
                }
            }

            // 其余现有的AI代码
            // ...

            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // 使用不受光照影响的颜色重新绘制投射物
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        *//*public override void OnKill(Projectile projectile, int timeLeft)
        {
            // 这段代码和上面在OnTileCollide方法中的代码一样，从与之碰撞的地块上产生粒子效果。SoundID.Item10 是弹跳声音。
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }*//*
        public void AnimateProjectile() // Call this every frame, for example in the AI method.
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 9) // This will change the sprite every 8 frames (0.13 seconds).
            {
                Projectile.frame++;
                Projectile.frame %= 1; // Will reset to the first frame if you've gone through them all.
                Projectile.frameCounter = 4;
            }
        }
    }
}
*/