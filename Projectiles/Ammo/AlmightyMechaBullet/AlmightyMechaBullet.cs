using Microsoft.Xna.Framework;
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
        private bool shouldPlayLastFrames = false;
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
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedStarfish);
                Main.dust[dustIndex].noGravity = true;
            }

            // 播放爆炸声音。
            //SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
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
                    shouldPlayLastFrames = true;
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
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            SimpleAnimation(1);
            // 使用不受光照影响的颜色重新绘制投射物
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            /*for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }*/
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
                if (Projectile.frameCounter > speed)
                {
                    Projectile.frameCounter = 0;
                    if (Projectile.frame < 4)
                    {
                        Projectile.frame++;
                    }
                    else if (Projectile.frame >= 4)
                    {
                        Projectile.frame++;
                        if (Projectile.frame > 7)
                        {
                            Projectile.frame = 4;
                        }
                    }

                }
            }
        }
        /*public override void OnKill(Projectile projectile, int timeLeft)
        {
            // 这段代码和上面在OnTileCollide方法中的代码一样，从与之碰撞的地块上产生粒子效果。SoundID.Item10 是弹跳声音。
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }*/
    }
}
