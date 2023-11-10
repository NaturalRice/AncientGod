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

namespace AncientGod.Boss.RunawayTank
{
    public class RunawayTankLaser : ModProjectile
    {
        float laserscale = 1f;
        bool playedsound = false;
        public override string Texture => "AncientGod/Items/Equips/Shields/Invishield_Shield";

        public float MaxScale => laserscale;//这里以下几个定义去掉了对灾厄模组的重写
        public float LaserLength => 1400f;//这里添加了命名
        public float MaxLaserLength => 1400f;
        public float Lifetime => 240f;
        public Color LaserOverlayColor => Color.White;
        public Color LightCastColor => LaserOverlayColor;
        public Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("AncientGod/Boss/RunawayTank/RunawayTankLaserStart", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("AncientGod/Boss/RunawayTank/RunawayTankLaserMiddle", AssetRequestMode.ImmediateLoad).Value;
        public Texture2D LaserEndTexture => ModContent.Request<Texture2D>("AncientGod/Boss/RunawayTank/RunawayTankLaserEnd", AssetRequestMode.ImmediateLoad).Value;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("RunawayTank Buster");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.penetrate = -1;
            //Projectile.alpha = 100;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.npcProj = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool PreAI()
        {
            //If RunawayTank dies during the laser, kill it
            if (!NPC.AnyNPCs(ModContent.NPCType<RunawayTank>()))
            {
                Projectile.active = false;
            }
            //Theres definitely a better way to do this but im lazy
            if (!playedsound)
            {
                Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Zombie104, Projectile.Center);
                playedsound = true;
            }
            return true;
        }

        //Animeme
        public override void PostAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 0f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
        }
        public override bool ShouldUpdatePosition() => false;

        //When do we get an animated laser field tbh
        public override bool PreDraw(ref Color lightColor)
        {
            //Textures and variables
            Rectangle beginsquare = LaserBeginTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle middlesquare = LaserMiddleTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Rectangle endsquare = LaserEndTexture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            float lenghtofmidlaser = LaserLength + middlesquare.Height;
            Vector2 centerr = Projectile.Center;

            //The beginning...
            Main.EntitySpriteDraw(LaserBeginTexture, Projectile.Center - Main.screenPosition, beginsquare, LaserOverlayColor, Projectile.rotation, LaserBeginTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            //The middle!
            if (lenghtofmidlaser > 0f)
            {
                //Size variables
                float laserOffset = middlesquare.Height * Projectile.scale;
                float incrementalBodyLength = 0f;
                //Draw textures until the end of the laser
                while (incrementalBodyLength + 1f < lenghtofmidlaser - laserOffset)
                {
                    //The middle (for real)
                    Main.EntitySpriteDraw(LaserMiddleTexture, centerr - Main.screenPosition, middlesquare, LaserOverlayColor, Projectile.rotation, LaserMiddleTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                    //Prepare for the next laser segment (woo)
                    incrementalBodyLength += laserOffset;
                    centerr += Projectile.velocity * laserOffset;
                    middlesquare.Y += LaserMiddleTexture.Height / Main.projFrames[Projectile.type];
                    if (middlesquare.Y + middlesquare.Height > LaserMiddleTexture.Height)
                    {
                        middlesquare.Y = 0;
                    }
                }
            }
            //The end.
            Main.EntitySpriteDraw(LaserEndTexture, centerr - Main.screenPosition, endsquare, LaserOverlayColor, Projectile.rotation, LaserEndTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        //Shitcode
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(playedsound);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            playedsound = reader.ReadBoolean();
        }

        public override bool? CanHitNPC(NPC target)
        {
            //Specifically only hurts SCal and no other town NPC
            return null;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Interaction never happens again after performed once           
        }
    }
}
