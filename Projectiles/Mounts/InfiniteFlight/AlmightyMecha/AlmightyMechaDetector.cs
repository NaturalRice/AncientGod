using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.ModLoader;
using AncientGod.Projectiles.Ammo;
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
using rail;

namespace AncientGod.Projectiles.Mounts.InfiniteFlight.AlmightyMecha
{
    public class AlmightyMechaDetector : ModProjectile
    {
        //存储宠物的轨迹路径点的数组
        public Vector2[] PositionsApollo1;
        public Vector2[] PositionsArtemis1;
        //存储相应宠物的旋转角度
        public float ApolloRotation1;
        public float ArtemisRotation1;
        //存储宠物拖尾的起始颜色
        public Color RibbonStartColor1 = new Color(34, 40, 48);

        //存储在投射物AI（人工智能）中的两个自定义参数
        public ref float Initialized1 => ref Projectile.ai[0];
        public ref float Behavior1 => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public static int TrailLenght = 30;
        public float SpinRadius => 125 + (Owner.GetModPlayer<AncientGodPlayer>().AlmightyMechaDetector ? 210 : 0);//Change the 200 for any distance you want to add whenever ares is equipped


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toy Twins");
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            AncientGodPlayer modPlayer = Owner.GetModPlayer<AncientGodPlayer>();
            if (Owner.dead || !Owner.mount.Active)//此处添加了一个条件用来保证坐骑消失时这些投射物也会消失
                Projectile.timeLeft = 0;
            if (!modPlayer.AlmightyMechaDetector)
            {
                Projectile.timeLeft = 0;
                Projectile.active = false;
            }
            if (modPlayer.AlmightyMechaDetector && Owner.mount.Active)//当然，还要再在这里强调一遍才能达到效果
            {
                Projectile.timeLeft = 2;
            }

            if (Initialized1 == 0)
            {
                //Initialize the arm positions
                PositionsArtemis1 = new Vector2[TrailLenght];
                PositionsApollo1 = new Vector2[TrailLenght];

                for (int i = 0; i < TrailLenght; i++)
                {
                    PositionsApollo1[i] = Projectile.Center;
                    PositionsArtemis1[i] = Projectile.Center;
                }
                Initialized1 = 1f;
            }

            Projectile.rotation += (MathHelper.Pi / (40f - (Owner.velocity.Length() / 20f) * 20f)) * (Owner.direction);
            Projectile.Center = Owner.Center;
            UpdateTwins();

        }

        public void UpdateTwins()//用于更新宠物的轨迹路径点，使它们形成一定的图案
        {
            //Shift down the previous positions
            for (int i = 0; i < TrailLenght - 1; i++)
            {
                PositionsApollo1[i] = PositionsApollo1[i + 1];
                PositionsArtemis1[i] = PositionsArtemis1[i + 1];
            }

            //Give them a new position

            //If the owner is going fast, place them at both sides of the player facing in the direction of the motion
            if (Owner.velocity.Length() > 10)
            {
                PositionsApollo1[TrailLenght - 1] = Vector2.Lerp(PositionsApollo1[TrailLenght - 1], Owner.Center + Vector2.Normalize(Owner.velocity.RotatedBy(MathHelper.PiOver2)) * SpinRadius + Owner.velocity, 0.2f);
                PositionsArtemis1[TrailLenght - 1] = Vector2.Lerp(PositionsArtemis1[TrailLenght - 1], Owner.Center - Vector2.Normalize(Owner.velocity.RotatedBy(MathHelper.PiOver2)) * SpinRadius + Owner.velocity, 0.2f);

                ApolloRotation1 = ApolloRotation1.AngleLerp(Owner.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);
                ArtemisRotation1 = ApolloRotation1.AngleLerp(Owner.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);
            }

            //If the owner is going slow make them rotate around the player
            else
            {
                PositionsApollo1[TrailLenght - 1] = Vector2.Lerp(PositionsApollo1[TrailLenght - 1], Projectile.Center + Projectile.rotation.ToRotationVector2() * SpinRadius, 0.2f);
                PositionsArtemis1[TrailLenght - 1] = Vector2.Lerp(PositionsArtemis1[TrailLenght - 1], Projectile.Center - Projectile.rotation.ToRotationVector2() * SpinRadius, 0.2f);

                float idealApolloRotation = Projectile.rotation - MathHelper.Pi * (Owner.direction < 0 ? 0 : 1);
                float idealArtemisRotation = Projectile.rotation - MathHelper.Pi * (Owner.direction < 0 ? 1 : 0);

                //Snap them in place if they're close enough to their ideal rotation or else some funky stuff starts to happen
                if (Math.Abs(ApolloRotation1 - idealApolloRotation) > MathHelper.PiOver4)
                    ApolloRotation1 = ApolloRotation1.AngleTowards(idealApolloRotation, 0.2f);
                else
                    ApolloRotation1 = idealApolloRotation;

                if (Math.Abs(ArtemisRotation1 - idealArtemisRotation) > MathHelper.PiOver4)
                    ArtemisRotation1 = ArtemisRotation1.AngleTowards(idealArtemisRotation, 0.2f);
                else
                    ArtemisRotation1 = idealArtemisRotation;
            }

            //Ribbons go blue if you go fast, go back to gray if you go slow
            Color IdealColor = Color.Lerp(new Color(34, 40, 48), Color.DeepSkyBlue, MathHelper.Clamp(Owner.velocity.Length() - 5, 0, 20) / 10f);
            RibbonStartColor1 = Color.Lerp(RibbonStartColor1, IdealColor, 0.2f);
        }

        //STOLED FROM THE REAL ONES???
        public float RibbonTrailWidthFunction(float completionRatio)
        {
            float tail = (float)Math.Pow(completionRatio, 2) * 7;
            float bump = Utils.GetLerpValue(0.7f, 0.8f, 1 - completionRatio, true) * Utils.GetLerpValue(1f, 0.8f, 1 - completionRatio, true) * 4;
            return tail + bump;
        }
        public Color OrangeRibbonTrailColorFunction(float completionRatio)
        {
            Color startingColor = RibbonStartColor1;
            Color endColor = new Color(219, 82, 28);
            return Color.Lerp(startingColor, endColor, (float)Math.Pow(1 - completionRatio, 1.5D)) * 0.7f;
        }
        public Color GreenRibbonTrailColorFunction(float completionRatio)
        {
            Color startingColor = RibbonStartColor1;
            Color endColor = new Color(40, 160, 32);
            return Color.Lerp(startingColor, endColor, (float)Math.Pow(1 - completionRatio, 1.5D)) * 0.7f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = (ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AlmightyMecha/AlmightyMechaDetector")).Value;

            bool secondPhase = false;
            if (Owner.velocity.Length() > 10)
                secondPhase = true;


            Rectangle apolloFrame = new Rectangle(0, secondPhase ? 146 : 0, 202, 146);
            Rectangle artemisFrame = new Rectangle(202, secondPhase ? 146 : 0, 198, 146);
            Vector2 origin = new Vector2(31, 36);

            if (Projectile.isAPreviewDummy)
            {
                Main.EntitySpriteDraw(tex, Projectile.position - Main.screenPosition + Vector2.UnitX * 20, new Rectangle(0, 0, 202, 146), lightColor, MathHelper.PiOver2, origin, Projectile.scale, 0, 0);
                Main.EntitySpriteDraw(tex, Projectile.position - Main.screenPosition + Vector2.UnitX * 20 - Vector2.UnitY * 40, new Rectangle(202, 0, 202, 146), lightColor, MathHelper.PiOver2, origin, Projectile.scale, 0, 0);
                return false;
            }

            float[] RotationsApollo = new float[PositionsApollo1.Length];
            float[] RotationsArtemis = new float[PositionsArtemis1.Length];
            for (int i = 1; i < PositionsApollo1.Length; i++)
            {
                RotationsApollo[i] = PositionsApollo1[i - 1].AngleTo(PositionsApollo1[i]);
                RotationsArtemis[i] = PositionsArtemis1[i - 1].AngleTo(PositionsArtemis1[i]);
            }
            RotationsApollo[0] = ApolloRotation1;
            RotationsArtemis[0] = ArtemisRotation1;

            Terraria.Graphics.VertexStrip apolloStrip = new Terraria.Graphics.VertexStrip();
            Terraria.Graphics.VertexStrip artemisStrip = new Terraria.Graphics.VertexStrip();
            apolloStrip.PrepareStripWithProceduralPadding(PositionsApollo1, RotationsApollo, GreenRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);
            artemisStrip.PrepareStripWithProceduralPadding(PositionsArtemis1, RotationsArtemis, OrangeRibbonTrailColorFunction, RibbonTrailWidthFunction, -Main.screenPosition, true);

            Effect vertexShader = ModContent.Request<Effect>($"{nameof(AncientGod)}/Effects/VertexShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            vertexShader.Parameters["uColor"].SetValue(Vector4.One);
            vertexShader.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            vertexShader.CurrentTechnique.Passes[0].Apply();

            apolloStrip.DrawTrail();
            artemisStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            Main.EntitySpriteDraw(tex, PositionsApollo1[TrailLenght - 1] - Main.screenPosition, apolloFrame, lightColor, ApolloRotation1, origin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(tex, PositionsArtemis1[TrailLenght - 1] - Main.screenPosition, artemisFrame, lightColor, ArtemisRotation1, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
