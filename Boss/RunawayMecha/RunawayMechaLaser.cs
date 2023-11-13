using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using static Terraria.Projectile;
using System.IO;
using System.Linq;
using ReLogic.Content;

namespace AncientGod.Boss.RunawayMecha
{
    public class RunawayMechaLaser : ModProjectile
    {
        public override string Texture => "AncientGod/Items/Equips/Shields/Invishield_Shield";

        public float scale;

        public int lasertimer;
        public float MaxScale => scale;
        public float MaxLaserLength => 4000f;
        public float Lifetime => 420f;
        public Color LaserOverlayColor => Color.White;
        public Color LightCastColor => LaserOverlayColor;
        //public Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        //public Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        //public Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fogck you");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }
        public override bool PreAI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<RunawayMecha>()))
            {
                Projectile.active = false;
            }
            lasertimer++;
            if (lasertimer > 120)
            {
                Projectile.damage = Main.npc[(int)Projectile.ai[0]].damage;
                scale = 4;
            }
            else
            {
                Projectile.damage = 0;
                scale = 1;
            }
            return true;
        }

        public override bool ShouldUpdatePosition() => false;
        public void UpdateLaserMotion()
        {
            if (lasertimer > 120)
            {
                float laserSpeed = 0.01f;
                Projectile.rotation += Projectile.ai[1] == 1 ? -laserSpeed : laserSpeed;
            }
            Projectile.velocity = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
        }
    }
}
