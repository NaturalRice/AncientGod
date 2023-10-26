using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using System;

namespace AncientGod
{
    public class AncientGodGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public bool isCalValPet;
        public override bool PreDraw(Projectile projectile, ref Color drawColor)
        {
            return true;
        }

        public override void AI(Projectile proj)
        {
            if (isCalValPet)
            {
                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    Projectile otherProj = Main.projectile[k];
                    if (!otherProj.active || otherProj.owner != proj.owner || k == proj.whoAmI)
                        continue;

                    bool bothPets = otherProj.GetGlobalProjectile<AncientGodGlobalProjectile>().isCalValPet;
                    float dist = Math.Abs(proj.position.X - otherProj.position.X) + Math.Abs(proj.position.Y - otherProj.position.Y);
                    if (bothPets && dist < proj.width)
                    {
                        if (proj.position.X < otherProj.position.X)
                            proj.velocity.X -= 0.4f;
                        else
                            proj.velocity.X += 0.4f;

                        if (proj.position.Y < otherProj.position.Y)
                            proj.velocity.Y -= 0.4f;
                        else
                            proj.velocity.Y += 0.4f;
                    }
                }
            }

            if (proj.owner == Main.myPlayer && proj.type == Terraria.ID.ProjectileID.PureSpray)
                PureConvert((int)(proj.position.X + proj.width / 2) / 16, (int)(proj.position.Y + proj.height / 2) / 16, 2);
            if (proj.owner == Main.myPlayer && (proj.type == ProjectileID.CorruptSpray || proj.type == ProjectileID.CrimsonSpray || proj.type == ProjectileID.HallowSpray))
                VoidConvert((int)(proj.position.X + proj.width / 2) / 16, (int)(proj.position.Y + proj.height / 2) / 16, 2);

        }

        public void PureConvert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
                    {
                        int type = Main.tile[k, l].TileType;
                        int typemed = Main.tile[k - 1, l].TileType;
                        int wall = Main.tile[k, l].WallType;

                    }
                }
            }
        }



        /// <summary>
        /// Allows you to determine whether the vanilla code for Kill and the Kill hook will be called. Return false to stop them from being called. Returns true by default. Note that this does not stop the projectile from dying.
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="timeLeft"></param>
        /// <returns></returns>
        public virtual bool PreKill(Projectile projectile, int timeLeft)
        {
            return true;
        }

        /// <summary>
        /// Allows you to control what happens when a projectile is killed (for example, creating dust or making sounds).
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="timeLeft"></param>
        public virtual void OnKill(Projectile projectile, int timeLeft)
        {
        }

        [Obsolete("Renamed to OnKill", error: true)] // Remove in 2023_10
        public virtual void Kill(Projectile projectile, int timeLeft)
        {
        }

        public void VoidConvert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
                    {
                        int type = Main.tile[k, l].TileType;
                        int typemed = Main.tile[k - 1, l].TileType;
                        int wall = Main.tile[k, l].WallType;

                    }
                }
            }
        }

        public virtual bool NeroBullet(Projectile projectile, int timeLeft)
        {
            return true;
        }

    }
}