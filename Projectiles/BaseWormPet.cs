using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace AncientGod.Projectiles
{
    // 虫子宠物的一个体节，包含位置、旧位置和是否是头部的信息
    public class WormPetSegment
    {
        public Vector2 position, oldPosition;
        public bool head;

        public WormPetVisualSegment visual;
    }
    // 虫子宠物的视觉体节，实现了 ICloneable 接口
    public class WormPetVisualSegment : ICloneable
    {
        // 贴图路径
        public string TexturePath;
        // 是否发光
        public bool Glows;
        // 变体数量
        public int Variants;
        // 帧数
        public int FrameCount;
        // 当前帧
        public int Frame;
        // 帧持续时间
        public int FrameDuration;
        // 帧计数器
        public int FrameCounter;
        // 是否方向性
        public bool Directional;
        // 横向位移
        public int LateralShift;

        /// <summary>
        /// 虫子宠物体节的视觉部分
        /// </summary>
        /// <param name="path">体节的贴图路径</param>
        /// <param name="glows">体节是否发光</param>
        /// <param name="variants">体节有多少变种（水平排列）</param>
        /// <param name="frames">体节是否有动画？</param>
        /// <param name="frameDuration">每帧持续的时间</param>
        /// <param name="directional">体节是否应该朝向“上”？如果设置为 true，则会尽量保持体节的左侧朝上，右侧朝下</param>
        /// <param name="lateralShift">体节的横向位移，从水平中心偏移多少像素。适用于有臂膀或者不对称的部分的体节</param>
        public WormPetVisualSegment(string path, bool glows = false, int variants = 1, int frames = 1, int frameDuration = 6, bool directional = false, int lateralShift = 0)
        {
            TexturePath = path;
            Glows = glows;
            Variants = variants;
            FrameCount = frames;
            Frame = 0;
            FrameCounter = 0;
            FrameDuration = frameDuration;
            Directional = directional;
            LateralShift = lateralShift;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public abstract class BaseWormPet : ModProjectile
    {
        public abstract WormPetVisualSegment HeadSegment();
        public abstract WormPetVisualSegment BodySegment();
        public abstract WormPetVisualSegment TailSegment();

        private Dictionary<string, WormPetVisualSegment> DefaultSegments => new Dictionary<string, WormPetVisualSegment>()
        {
            { "Head" , HeadSegment() },
            { "Body" , BodySegment() },
            { "Tail" , TailSegment() }
        };

        /// <summary>
        /// 如果需要使用自定义的体节类型，这个字典包含了这些自定义的体节类型
        /// 如果要添加自定义的体节，请记得不要使用“Head”、“Body”和“Tail”这些键，因为这些键已经被基础蠕虫使用了。
        /// </summary>
        public virtual Dictionary<string, WormPetVisualSegment> CustomSegments => new Dictionary<string, WormPetVisualSegment>() { };

        /// <summary>
        /// 一个体节的“高度”。仅计算身体的高度，头部和尾部可能更长，这将自动调整。
        /// </summary>
        public abstract int SegmentSize();
        /// <summary>
        /// 蠕虫中的体节数量
        /// </summary>
        public abstract int SegmentCount();
        /// <summary>
        /// 返回宠物是否应该存在。通常应该检查玩家的布尔值。
        /// </summary>
        /// <returns></returns>
        public abstract bool ExistenceCondition();
        /// <summary>
        /// 蠕虫头部想要去的位置，相对于玩家中心
        /// </summary>
        public Vector2 RelativeIdealPosition;
        /// <summary>
        /// 蠕虫头部想要去的世界坐标位置
        /// </summary>
        public Vector2 IdealPosition => Owner.Center;
        /// <summary>
        /// 投射物是否已初始化？这个变量会自动设置，所以请不要手动使用它
        /// </summary>
        public ref float Initialized => ref Projectile.ai[0];
        /// <summary>
        /// 如果蠕虫头部离理想位置很远，则这是蠕虫头部的转向角度
        /// </summary>
        public virtual float MinimumSteerAngle => MathHelper.PiOver4 / 4f;
        /// <summary>
        /// 如果蠕虫头部足够接近理想位置，则这是蠕虫头部的转向角度。最好将这个转向角度设置得高一些，这样蠕虫就不会一直错过目标
        /// </summary>
        public virtual float MaximumSteerAngle => MathHelper.E;
        /// <summary>
        /// 蠕虫身体的变种数量。变种会水平排列，因此您可以同时拥有多个变种和一个动画身体。当然，您也可以自己进行自定义绘制
        /// </summary>
        public virtual int BodyVariants => 1;
        /// <summary>
        /// 头部被压进身体的像素数。如果头部底部的精灵没有平坦的表面，则有用
        /// </summary>
        public virtual float BashHeadIn => 0;
        /// <summary>
        /// 每帧运行的绳链模拟迭代次数。更多的迭代次数=速度更高时链条间的间隙更少
        /// </summary>
        public virtual int NumSimulationIterations => 5;
        /// <summary>
        /// 蠕虫应该远离玩家中心的距离。这仅在没有覆盖 UpdateIdealPosition 方法时有用。
        /// </summary>
        public virtual float WanderDistance => 10; //加速时蠕虫距离玩家的量
        /// <summary>
        /// 蠕虫头部移动的速度
        /// </summary>
        public virtual float GetSpeed => MathHelper.Lerp(15, 40, MathHelper.Clamp(Projectile.Distance(IdealPosition) - 1f, 0, 1));
        /// <summary>
        /// 如果蠕虫将要发出发光，这是贴图文件末尾的文本字符串（例如，“_Glow”之类的）
        /// </summary>
        public virtual string GlowmaskSuffix => "Glow";
        /// <summary>
        /// 发光蒙版的不透明度
        /// </summary>
        public virtual float GlowmaskOpacity => 1;
        /// <summary>
        /// 贴图的进度。如果需要在进度中引入自定义贴图，请将其添加到 TexturePaths 字典中的新键下，并将该键用于列表中。
        /// 列表从第一个段开始，朝向尾部。您不需要将尾部放入列表中，因为它会自动考虑到。
        /// 如果列表比蠕虫本身还要短，则在列表停止后它将继续使用常规的身体段。如果列表更长，则超出尾部的所有内容都不会被考虑在内。
        /// 例如，如果我想在前两个体节中使用自定义的“arm”段，我会将 TextureProgression 设置为一个新的 String[] {"Arm", "Arm"}。
        /// </summary>
        public virtual string[] TextureProgression => new string[0];
        /// <summary>
        /// 发出的光的颜色。如果不想发光，请保持黑色。
        /// </summary>
        public virtual Color Lightcolor => Color.Black;
        /// <summary>
        /// 光的强度。
        /// </summary>
        public virtual float Intensity => 1f;
        /// <summary>
        /// 发光的级别，请检查维基以获取值。
        /// </summary>
        public virtual int AbyssLightLevel => 0;
        public Player Owner => Main.player[Projectile.owner];
        public AncientGodPlayer ModOwner => Owner.GetModPlayer<AncientGodPlayer>();    

        public List<WormPetSegment> Segments;

        public ref float TimeTillReset => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Base Worm Head");
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = SegmentSize();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        /// <summary>
        /// The full AI. If you don't need to entirely modify it, you can modify other behavior functions
        /// </summary>
        public virtual void WormAI()
        {
            bool shouldDoAI = CheckIfAlive();
            if (shouldDoAI)
            {
                UpdateIdealPosition();
                MoveTowardsIdealPosition();
                SimulateSegments();
                Animate();
                CustomAI();
            }
        }

        /// <summary>
        /// Initialize the worm. You shouldn't have to ever change this yourself
        /// </summary>
        public virtual void Initialize()
        {
            //Initialize the visual segments
            Dictionary<string, WormPetVisualSegment> defaultPool = DefaultSegments;
            Dictionary<string, WormPetVisualSegment> customPool = CustomSegments;
            //Create an ordered list of all the segments 
            WormPetVisualSegment[] visualSegments = new WormPetVisualSegment[SegmentCount()];

            //Start by filling the list with body segments
            for (int i = 0; i < visualSegments.Length; i++)
            {
                visualSegments[i] = (WormPetVisualSegment)defaultPool["Body"].Clone();
            }

            //If any custom segment progression exists, replace the body segments appropriately
            for (int i = 1; i < visualSegments.Length && i < TextureProgression.Length + 1; i++)
            {
                WormPetVisualSegment segment;
                string key = TextureProgression[i - 1];

                //Get the segment from the proper pool of segments
                if (defaultPool.ContainsKey(key))
                    segment = defaultPool[key];
                else
                    segment = customPool[key];

                //add it to the progression
                visualSegments[i] = (WormPetVisualSegment)segment.Clone();
            }
            //Make the first element be the head and the last element be the tail, obviously
            visualSegments[0] = (WormPetVisualSegment)defaultPool["Head"].Clone();
            visualSegments[visualSegments.Length - 1] = (WormPetVisualSegment)defaultPool["Tail"].Clone();


            //Initialize the segments
            Segments = new List<WormPetSegment>(SegmentCount());
            for (int i = 0; i < SegmentCount(); i++)
            {
                WormPetSegment segment = new WormPetSegment();
                segment.head = false;
                segment.position = Projectile.Center + Vector2.UnitY * SegmentSize() * i;
                segment.oldPosition = segment.position;
                segment.visual = visualSegments[i];
                Segments.Add(segment);
            }

            Segments[0].head = true;

            Initialized = 1f;
            return;
        }

        /// <summary>
        /// Checks if the worm should remain alive or if it should kill itself. Return false if the rest of the AI shouldn't even bother executing. This also kills the worm if its too far away from the player
        /// </summary>
        public virtual bool CheckIfAlive()
        {
            AncientGodPlayer modPlayer = Owner.GetModPlayer<AncientGodPlayer>();
            if (Owner.dead || !Owner.mount.Active)//此处添加了一个条件用来保证坐骑消失时这些投射物也会消失
                Projectile.timeLeft = 0;
            if (ExistenceCondition() && Owner.mount.Active && modPlayer.AlmightyMechaDigger)
            {
                Projectile.timeLeft = 2;
            }

            if (Owner.dead || !Owner.active || !modPlayer.AlmightyMechaDigger)
            {
                Projectile.timeLeft = 0;
                Projectile.active = false;
                return false;
            }

            return true;
        }
        /// <summary>
        /// Updates the ideal position the worm head wants to reach
        /// </summary>
        public virtual void UpdateIdealPosition()
        {
            TimeTillReset++;
            RelativeIdealPosition = Owner.velocity;
            TimeTillReset = 0;
        }
        /// <summary>
        /// Makes the head move towards its ideal position.
        /// </summary>
        public virtual void MoveTowardsIdealPosition()
        {
            Projectile.velocity = Owner.velocity;

            // 更新其段的位置
            Segments[0].oldPosition = Segments[0].position;
            Segments[0].position = Projectile.Center;
        }

        /// <summary>
        /// Makes the segments trail behind the worm
        /// </summary>
        public virtual void SimulateSegments()
        {
            //https://youtu.be/PGk0rnyTa1U?t=400 we use verlet integration chains here
            int i = 0;
            float movementLenght = Owner.velocity.Length();//可以通过调整 movementLenght 变量的值来改变体节之间的距离。
            //此处由Projectile.velocity.Length()改为Owner.velocity.Length()后解决了玩家静止时蠕虫体节乱转的问题，但头部仍然在乱转
            //解决了，是因为AlmightyMechaDigger里重写的MoveTowardsIdealPosition方法跟这里的原方法有冲突
            foreach (WormPetSegment segment in Segments)
            {
                if (!segment.head)
                {
                    Vector2 positionBeforeUpdate = segment.position;
                    //这行代码决定了体节之间的移动方向和距离。
                    segment.position += Utils.SafeNormalize(Segments[i - 1].oldPosition - segment.position, Vector2.Zero) * (movementLenght * 1f);
                    //Makes the segment move towards the forward segment 
                    segment.oldPosition = positionBeforeUpdate;
                }
                i++;
            }

            for (int k = 0; k < NumSimulationIterations; k++)
            {
                for (int j = 0; j < SegmentCount() - 1; j++)
                {
                    WormPetSegment pointA = Segments[j];
                    WormPetSegment pointB = Segments[j + 1];
                    Vector2 segmentCenter = (pointA.position + pointB.position) / 2f;
                    Vector2 segmentDirection = Utils.SafeNormalize(pointA.position - pointB.position, Vector2.UnitY);

                    if (!pointA.head)
                        pointA.position = segmentCenter + segmentDirection * SegmentSize() / 2f;

                    if (!pointB.head)
                        pointB.position = segmentCenter - segmentDirection * SegmentSize() / 2f;

                    Segments[j] = pointA;
                    Segments[j + 1] = pointB;

                    Player owner = Main.player[Projectile.owner];
                    Lighting.AddLight(segmentCenter, Lightcolor.ToVector3());
                }
            }
        }

        /// <summary>
        /// Mess with the frames here if you want a custom animation. By default, simply makes all frames cycle forward
        /// </summary>
        public virtual void Animate()
        {
            foreach (WormPetSegment segment in Segments)
            {
                segment.visual.FrameCounter++;
                if (segment.visual.FrameCounter > segment.visual.FrameDuration)
                {
                    segment.visual.Frame++;
                    if (segment.visual.Frame >= segment.visual.FrameCount)
                        segment.visual.Frame = 0;

                    segment.visual.FrameCounter = 0;
                }
            }
        }
        /// <summary>
        /// Gets ran after everything else. Do whatever you want
        /// </summary>
        public virtual void CustomAI() { }


        public override void AI()
        {
            if (Initialized == 0)
                Initialize();
            WormAI();

            // Consistently update the worm
            if ((int)Main.time % 120 == 0)
            {
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Initialized == 0f)
                return false;
            DrawWorm(lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (Initialized == 0f)
                return;

            DrawWorm(lightColor, true);
        }


        /// <summary>
        /// Draws the worm. Override this if you want to draw it yourself. 
        /// Glow indicates wether or not this is the glowmask. You can use it to just change the segmentlight to always be white, or use it to do custom drawing when in the glowmask
        /// </summary>
        public virtual void DrawWorm(Color lightColor, bool glow = false)
        {
            for (int i = SegmentCount() - 1; i >= 0; i--)
            {
                WormPetVisualSegment currentSegment = Segments[i].visual;
                //If the segment doesn't have a glowmask on the glow pass, simply don't draw it lol?
                if (glow & !currentSegment.Glows)
                    continue;

                bool bodySegment = i != 0 && i != SegmentCount() - 1;
                Texture2D sprite = ModContent.Request<Texture2D>(currentSegment.TexturePath + (glow ? GlowmaskSuffix : "")).Value;

                Vector2 angleVector = (i == 0 ? Projectile.rotation.ToRotationVector2() : (Segments[i - 1].position - Segments[i].position));
                bool flipped = Math.Sign(angleVector.X) < 0 && currentSegment.Directional;

                //Get the horizontal start of the frame (for segments with variants)
                int frameStartX = (i % currentSegment.Variants) * sprite.Width / currentSegment.Variants;

                //Get the vertical segment of the frame
                int frameStartY = sprite.Height / currentSegment.FrameCount * currentSegment.Frame;

                int frameWidth = sprite.Width / currentSegment.Variants;
                int frameHeight = (sprite.Height / currentSegment.FrameCount);

                //Remove 2 from the width and height of the frame if the segment has variants/is animated to account for the extra gap of 2 pixels
                frameWidth -= currentSegment.Variants > 1 ? 2 : 0;
                frameHeight -= (Main.projFrames[Projectile.type] > 1) ? 2 : 0;

                Rectangle frame = new Rectangle(frameStartX, frameStartY, frameWidth, frameHeight);
                Vector2 origin = bodySegment ? frame.Size() / 2f : i == 0 ? new Vector2(frame.Width / 2f, frame.Height - SegmentSize() / 2f) : new Vector2(frame.Width / 2f, SegmentSize() / 2f);

                if (i == 0)
                    origin -= Vector2.UnitY * BashHeadIn;

                origin -= Vector2.UnitX * currentSegment.LateralShift * (flipped ? -1 : 1);
                //此处将头部的角度由其与后面一个体节的相对角度得出，解决了玩家静止后头部自动角度为0的bug
                float rotation = i == 0 ? (Segments[i].position - Segments[i + 1].position).ToRotation() + MathHelper.PiOver2 : (Segments[i].position - Segments[i - 1].position).ToRotation() - MathHelper.PiOver2;

                Color segmentLight = glow ? Color.White * GlowmaskOpacity : Lighting.GetColor((int)Segments[i].position.X / 16, (int)Segments[i].position.Y / 16); //Lighting of the position of the segment. Pure white if its a glowmask

                Main.EntitySpriteDraw(sprite, Segments[i].position - Main.screenPosition, frame, segmentLight, rotation, origin, Projectile.scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }
    }
}
