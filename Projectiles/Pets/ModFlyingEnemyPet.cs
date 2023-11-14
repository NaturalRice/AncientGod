using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace AncientGod.Projectiles.Pets
{
    /// <summary>
    /// The base of a flying pet
    /// </summary>
    public abstract class ModFlyingEnemyPet : ModEnemyPet
    {
        public class States
        {
            public const int Flying = 0;
        }

        public sealed override void CustomBehaviour(Player player, ref int state)
        {
            base.CustomBehaviour(player, ref state);
        }

        /// <summary>
        /// <inheritdoc cref="ModPet.CustomBehaviour(Player, ref int)"/>
        /// </summary>
        /// <param name="player">The owner of this pet</param>
        /// <param name="state">The state this Flying Pet is in</param>
        /// <param name="flyingSpeed">The current flying speed</param>
        /// <param name="flyingInertia">The current flying inertia</param>
        public virtual void CustomBehaviour(Player player, ref int state, float flyingSpeed, float flyingInertia)
        {
        }

        /// <summary>
        /// Allows you to change the flying speed and inertia of this pet while it is active
        /// </summary>
        /// <param name="flyingSpeed">The flying speed</param>
        /// <param name="flyingInertia">The flying inertia</param>
        public virtual void ModifyFlyingValues(ref float flyingSpeed, ref float flyingInertia)
        {
        }

        /// <summary>
        /// Default is 60f
        /// </summary>
        public override float FlyingInertia => 60f;

        public sealed override void PetBehaviour()
        {
            if (!Main.player[Projectile.owner].active)
            {
                //Projectile.active = false;//因为算Boss，所以玩家死亡后并不消失?
                return;
            }

            Player player = Main.player[Projectile.owner];

            PetFunctionality(player);

            if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 0)//判断玩家朝向，决定宠物的飞行目标
            {
                if (FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
                else if (!FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }

            Vector2 ProjectileCenter = Projectile.Center;
            Vector2 vectorToPlayer = player.Center - ProjectileCenter;
            float lengthToPlayer = vectorToPlayer.Length();

            if (lengthToPlayer > TeleportThreshold && CanTeleport)
            {
                //Projectile.position = player.Center;//不要让Boss离太远时瞬移到玩家身边
                Projectile.velocity *= 0.5f;
                OnTeleport();
            }

            Animation(state);

            float flyingSpeed = FlyingSpeed;
            float flyingInertia = FlyingInertia;

            if ((lengthToPlayer > SpeedupThreshold) && ShouldSpeedup)
            {
                flyingSpeed *= 1.5f;
                flyingInertia *= 0.75f;
            }

            ModifyFlyingValues(ref flyingSpeed, ref flyingInertia);

            CustomBehaviour(player, ref state, flyingSpeed, flyingInertia);

            switch (state)
            {
                case States.Flying:

                    Projectile.tileCollide = false;

                    vectorToPlayer += FlyingOffset;
                    lengthToPlayer = vectorToPlayer.Length();

                    if (lengthToPlayer > FlyingArea)
                    {
                        vectorToPlayer.Normalize();
                        vectorToPlayer *= flyingSpeed;

                        if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity = new Vector2(-0.15f);
                        }

                        if (flyingInertia != 0f && flyingSpeed != 0f)
                            Projectile.velocity = (Projectile.velocity * (flyingInertia - 1) + vectorToPlayer) / flyingInertia;
                    }
                    else
                    {
                        Projectile.velocity *= FlyingDrag;
                    }

                    /*if (ShouldFlyRotate)//还是别旋转了吧
                    {
                        Projectile.rotation = Projectile.velocity.X * 0.1f;
                    }*/

                    int dustType = 16;
                    if (ModifyDustSpawn(ref dustType))
                    {
                        int theDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y) - Projectile.velocity, 8, 8, dustType, (0f - Projectile.velocity.X) * 0.5f, Projectile.velocity.Y * 0.5f, 50, default, 1.7f);
                        Main.dust[theDust].velocity.X = Main.dust[theDust].velocity.X * 0.2f;
                        Main.dust[theDust].velocity.Y = Main.dust[theDust].velocity.Y * 0.2f;
                        Main.dust[theDust].noGravity = true;
                    }

                    break;
            }

            auraRotation = MathHelper.WrapAngle(auraRotation);
        }
    }
}
