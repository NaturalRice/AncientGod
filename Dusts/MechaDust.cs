using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.ID;
using System.Collections.Generic;

namespace AncientGod.Dusts//此处将反编译中的NewDust方法修改了一下
{
    public class MechaDust : ModDust
    {
        public static float dCount;
        internal static readonly IList<ModDust> dusts = new List<ModDust>();
        internal static int DustCount { get; private set; } = DustID.Count;
        public static int NewDust(Vector2 Position, int Width, int Height, int Type, float SpeedX = 0f, float SpeedY = 0f, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
        {
            /*
            参数：
            Position: 粒子的起始位置。
            Width 和 Height: 粒子的宽度和高度。
            Type: 粒子的类型。
            SpeedX 和 SpeedY: 粒子的初始速度。
            Alpha: 粒子的透明度。
            newColor: 粒子的颜色。
            Scale: 粒子的缩放比例。

            返回值： 
            返回一个整数，代表新创建的粒子的索引，用于后续对该粒子的操作。

            逻辑：
            检查一系列条件，如果不符合则直接返回 6000，表示不创建新的粒子。
            在屏幕可见区域内，使用循环查找空闲的粒子槽位。
            设置粒子的各种属性，包括位置、速度、透明度、颜色、缩放等。
            针对特定粒子类型进行额外的处理，例如调整速度、透明度等。
            返回新创建粒子的索引。
            */
            if (Main.gameMenu)
            {
                return 6000;
            }

            if (Main.rand == null)
            {
                Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
            }

            if (Main.gamePaused)
            {
                return 6000;
            }

            if (WorldGen.gen)
            {
                return 6000;
            }

            if (Main.netMode == 2)
            {
                return 6000;
            }

            int num = (int)(400f * (1f - dCount));
            Rectangle rectangle = new Rectangle((int)(Main.screenPosition.X - (float)num), (int)(Main.screenPosition.Y - (float)num), Main.screenWidth + num * 2, Main.screenHeight + num * 2);
            Rectangle value = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
            if (!rectangle.Intersects(value))
            {
                return 6000;
            }

            int result = 6000;
            for (int i = 0; i < 6000; i++)
            {
                Dust dust = Main.dust[i];
                if (dust.active)
                {
                    continue;
                }

                if ((double)i > (double)Main.maxDustToDraw * 0.9)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        return 6000;
                    }
                }
                else if ((double)i > (double)Main.maxDustToDraw * 0.8)
                {
                    if (Main.rand.Next(3) != 0)
                    {
                        return 6000;
                    }
                }
                else if ((double)i > (double)Main.maxDustToDraw * 0.7)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        return 6000;
                    }
                }
                else if ((double)i > (double)Main.maxDustToDraw * 0.6)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        return 6000;
                    }
                }
                else if ((double)i > (double)Main.maxDustToDraw * 0.5)
                {
                    if (Main.rand.Next(5) == 0)
                    {
                        return 6000;
                    }
                }
                else
                {
                    dCount = 0f;
                }

                int num2 = Width;
                int num3 = Height;
                if (num2 < 5)
                {
                    num2 = 5;
                }

                if (num3 < 5)
                {
                    num3 = 5;
                }

                result = i;
                dust.fadeIn = 0f;
                dust.active = true;
                dust.type = Type;
                dust.noGravity = false;
                dust.color = newColor;
                dust.alpha = Alpha;
                dust.position.X = Position.X + (float)Main.rand.Next(num2 - 4) + 4f;
                dust.position.Y = Position.Y + (float)Main.rand.Next(num3 - 4) + 4f;
                dust.velocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedX;
                dust.velocity.Y = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedY;
                dust.frame.X = 10 * Type;
                dust.frame.Y = 10 * Main.rand.Next(3);
                dust.shader = null;
                dust.customData = null;
                dust.noLightEmittence = false;
                int num4 = Type;
                while (num4 >= 100)
                {
                    num4 -= 100;
                    dust.frame.X -= 1000;
                    dust.frame.Y += 30;
                }

                dust.frame.Width = 8;
                dust.frame.Height = 8;
                dust.rotation = 0f;
                dust.scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
                dust.scale *= Scale;
                dust.noLight = false;
                dust.firstFrame = true;
                if (dust.type == 228 || dust.type == 279 || dust.type == 269 || dust.type == 135 || dust.type == 6 || dust.type == 242 || dust.type == 75 || dust.type == 169 || dust.type == 29 || (dust.type >= 59 && dust.type <= 65) || dust.type == 158 || dust.type == 293 || dust.type == 294 || dust.type == 295 || dust.type == 296 || dust.type == 297 || dust.type == 298 || dust.type == 302 || dust.type == 307 || dust.type == 310)
                {
                    dust.velocity.Y = (float)Main.rand.Next(-10, 6) * 0.1f;
                    dust.velocity.X *= 0.3f;
                    dust.scale *= 0.7f;
                }

                if (dust.type == 127 || dust.type == 187)
                {
                    dust.velocity *= 0.3f;
                    dust.scale *= 0.7f;
                }

                if (dust.type == 308)
                {//这里改了一下数值
                    dust.velocity *= 0.1f;//dust.velocity *= 0.5f;
                    dust.velocity.Y = 0f;//dust.velocity.Y += 1f;
                }

                if (dust.type == 33 || dust.type == 52 || dust.type == 266 || dust.type == 98 || dust.type == 99 || dust.type == 100 || dust.type == 101 || dust.type == 102 || dust.type == 103 || dust.type == 104 || dust.type == 105)
                {
                    dust.alpha = 170;
                    dust.velocity *= 0.5f;
                    dust.velocity.Y += 1f;
                }

                if (dust.type == 41)
                {
                    dust.velocity *= 0f;
                }

                if (dust.type == 80)
                {
                    dust.alpha = 50;
                }

                SetupDust(dust);
                if (dust.type == 34 || dust.type == 35 || dust.type == 152)
                {
                    dust.velocity *= 0.1f;
                    dust.velocity.Y = -0.5f;
                    if (dust.type == 34 && !Collision.WetCollision(new Vector2(dust.position.X, dust.position.Y - 8f), 4, 4))
                    {
                        dust.active = false;
                    }
                }

                break;
            }

            return result;
        }

        internal static void SetupDust(Dust dust)
        {
            /*
            参数：
            dust: ModDust 对象，代表要设置的粒子。

            逻辑：
            获取与指定粒子类型相对应的 ModDust 对象。
            调用 ModDust 对象的 OnSpawn 方法，对粒子进行额外的设置。这可以用于执行特殊类型粒子的初始化逻辑。
            */
            ModDust dust2 = GetDust(dust.type);
            if (dust2 != null)
            {
                dust.frame.X = 0;
                dust.frame.Y %= 30;
                dust2.OnSpawn(dust);
            }
        }

        public static ModDust GetDust(int type)
        {
            /*
            参数：
            type: 粒子类型。

            返回值： 
            返回对应粒子类型的 ModDust 对象。

            逻辑：
            检查粒子类型是否在有效范围内，如果不在范围内返回 null。
            返回与指定粒子类型相对应的 ModDust 对象。
            */
            if (type < DustID.Count || type >= DustCount)
            {
                return null;
            }

            return dusts[type - DustID.Count];
        }
    }
}
