using AncientGod.Projectiles;
using Mono.Cecil;
using AncientGod.Projectiles.Pets.RunawayMecha;
using Terraria.ID;
using Terraria.Audio;
using static Terraria.Utils;
using System.Collections;
using Terraria.ModLoader.Core;
using AncientGod.Dusts;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using AncientGod.Items.Mounts.InfiniteFlight.ModernMecha;
using AncientGod.Items.Dyes;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;
using Terraria.GameContent;
using AncientGod.Projectiles.Ammo.AlmightyMechaBullet;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Animations;
using static Humanizer.In;

namespace AncientGod.Projectiles.Mounts.InfiniteFlight.AlmightyMecha
{
    public class AlmightyMechaDigger : BaseWormPet
    {
        public override string Texture => "AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightlyMechaDiggerHead";
        public override WormPetVisualSegment HeadSegment() => new WormPetVisualSegment("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightlyMechaDiggerHead", true, 1, 5);
        public override WormPetVisualSegment BodySegment() => new WormPetVisualSegment("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightlyMechaDiggerBody", true, 2, 5);
        public override WormPetVisualSegment TailSegment() => new WormPetVisualSegment("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightlyMechaDiggerTail", true, 1, 5);

        public override int SegmentSize() => 40;

        public override int SegmentCount() => 20;//有一个点子，或许可以搞个通过刷怪数上升来增加体节数这种机制？(☆▽☆)

        public override bool ExistenceCondition() => ModOwner.AlmightyMechaDigger;

        public override float GetSpeed => MathHelper.Lerp(17, 40, MathHelper.Clamp(Projectile.Distance(IdealPosition) / (WanderDistance * 2.2f) - 1f, 0, 1));

        public override int BodyVariants => 2;
        public override float BashHeadIn => 5;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toy Nurex");
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
        }

        public override void MoveTowardsIdealPosition()
        {
            //THIS CODE NEEDS CALAMITY 1.5.1.001 STUFF TO WORK PROPERLY!

            //If the owner is holding right click, shift its goal from the worms ideal position tothe mouse cursor
            /*if (NaturalRiceFirstMod.CalamityActive)
            {
                if ((bool)NaturalRiceFirstMod.Calamity.Call("GetRightClick", Owner) && Owner.HeldItem.type == ModContent.ItemType<GunmetalRemote>())
                    RelativeIdealPosition = (Vector2)NaturalRiceFirstMod.Calamity.Call("GetMouseWorld", Owner) - Owner.Center;
            }*/

            Projectile.velocity = Owner.velocity;

            // 更新其段的位置
            Segments[0].oldPosition = Segments[0].position;
            Segments[0].position = Projectile.Center;
        }

        public override void Animate()
        {
            foreach (WormPetSegment segment in Segments)
            {
                if (Owner.statLife / (float)Owner.statLifeMax > 0.5f && segment.visual.Frame != 0)
                {
                    segment.visual.FrameCounter++;
                    if (segment.visual.FrameCounter > segment.visual.FrameDuration)
                    {
                        segment.visual.Frame--;
                        segment.visual.FrameCounter = 0;
                    }
                }
                else if (Owner.statLife / (float)Owner.statLifeMax <= 0.75f && Owner.statLife / (float)Owner.statLifeMax > 0.5f && segment.visual.Frame != 1)
                {
                    // 当玩家生命值百分比在0.5到0.75时，虫子帧设置为2（第三帧）
                    segment.visual.FrameCounter++;
                    if (segment.visual.FrameCounter < segment.visual.FrameDuration)
                    {
                        segment.visual.Frame = 2;
                        segment.visual.FrameCounter = 0;
                    }
                }
                else if (Owner.statLife / (float)Owner.statLifeMax <= 0.5f && segment.visual.Frame != segment.visual.FrameCount - 1)
                {
                    segment.visual.FrameCounter++;
                    if (segment.visual.FrameCounter > segment.visual.FrameDuration)
                    {
                        segment.visual.Frame++;
                        segment.visual.FrameCounter = 0;
                    }
                }
            }
        }
    }
}
