using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace AncientGod.Boss.RunawayMecha
{
    public class RunawayMechaFist : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.hostile = true;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)MathHelper.Pi;
        }

        public void SpawnSmoke()
        {
            Vector2 placementrun = new Vector2(Projectile.Center.X + Main.rand.Next(-5, 5), Projectile.Center.Y + Main.rand.Next(-6, 6));
        }
    }
}
