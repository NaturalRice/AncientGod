﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Projectiles.Ammo
{
    internal class EntropySilenceMechaBullet : ModProjectile
    {
        private float fireCooldown = 60f; // 光子转向间隔时间

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // 记录旧位置的长度
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // 记录模式
        }

        public override void SetDefaults()
        {
            Projectile.width = 3; // 投射物碰撞盒的宽度
            Projectile.height = 108; // 投射物碰撞盒的高度
            Projectile.aiStyle = 1; // 投射物的AI风格，请参考Terraria源代码
            Projectile.friendly = true; // 投射物能对敌人造成伤害吗？
            Projectile.hostile = false; // 投射物能对玩家造成伤害吗？
            Projectile.DamageType = DamageClass.Ranged; // 投射物的伤害类型，是由远程武器发射的吗？

            Projectile.timeLeft = 600; // 投射物的生存时间（60 = 1秒，所以600是10秒）
            Projectile.alpha = 255; // 投射物的透明度，255为完全透明。（aiStyle 1会快速使投射物变透明。如果不使用逐渐变透明的aiStyle，请删除这一行，否则你的投射物会变得看不见）
            Projectile.light = 1f; // 投射物周围的光照强度
            Projectile.ignoreWater = false; // 投射物的速度是否受水影响？
            Projectile.tileCollide = false; // 投射物能否与地块碰撞？
            Projectile.extraUpdates = 1; // 如果希望投射物在一帧内更新多次，请设置大于0的值

            AIType = ProjectileID.Bullet; // 行为与默认子弹完全相同

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
                    npc.SimpleStrikeNPC(20, 0, true, 0f);
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
                    npc.SimpleStrikeNPC(20, 0, true, 0f);
                    // 如果需要，可以在此添加更多效果或逻辑。
                }
            }

            // 更新冷却间隔计时器，准备下一次光子转向
            fireCooldown--;

            if (fireCooldown <= 0f)
            {
                // 生成一个随机方向
                Vector2 randomDirection = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                Projectile.velocity = randomDirection * 10f; // 设置速度，可以根据需要调整

                // 重置冷却计时器，设置为你想要的冷却时间（间隔时间0.5秒）
                fireCooldown = Main.rand.Next(30, 90); // 设置一个随机的冷却时间，可以根据需要调整
            }

            base.AI();
        }

        private float Random(Vector2 vector21, Vector2 vector22)//用于随机转向
        {
            throw new NotImplementedException();
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

        /*public override void OnKill(Projectile projectile, int timeLeft)
        {
            // 这段代码和上面在OnTileCollide方法中的代码一样，从与之碰撞的地块上产生粒子效果。SoundID.Item10 是弹跳声音。
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }*/
    }
}
