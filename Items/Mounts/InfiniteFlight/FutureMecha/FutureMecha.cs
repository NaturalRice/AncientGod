using Terraria;
using Terraria.ModLoader;
using AncientGod.Buffs.Mounts;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using System;

namespace AncientGod.Items.Mounts.InfiniteFlight.FutureMecha
{
    internal class FutureMecha : ModMount
    {
        private int horizontalDirection = 1; // 默认为向右



        public override void SetStaticDefaults()
        {
            MountData.acceleration = 0.7f;//加速度
            MountData.runSpeed = 17f;//飞行速度
            //MountData.dashSpeed = 14f;//坐骑的冲刺速度

            MountData.fatigueMax = int.MaxValue - 1;//坐骑的疲劳度上限(去除此处，上升速度会不可控，但下降速度仍为41mph)

            MountData.blockExtraJumps = false;//是否阻止额外跳跃
            MountData.usesHover = true;//是否使用悬浮（飞行）能力
            MountData.flightTimeMax = int.MaxValue - 1;//坐骑的最大飞行时间

            MountData.buff = ModContent.BuffType<FutureMechaBuff>();//坐骑提供的Buff类型
            MountData.spawnDust = 33;//使用粒子效果类型33
            MountData.spawnDustNoGravity = true;//粒子效果将不受重力影响，会漂浮在空中

            MountData.totalFrames = 12;//帧数
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray();
            //设置了玩家骑坐骑时坐骑在Y轴上的偏移量。通过Enumerable.Repeat，我们将值20重复了12次，然后将其转换为数组，以确保每一帧坐骑的高度都一致
            MountData.xOffset = 0;//设置了坐骑在X轴上的偏移量。这个值表示坐骑相对于玩家的水平位置
            MountData.yOffset = -16;//设置了坐骑在Y轴上的偏移量。这个值表示坐骑相对于玩家的垂直位置
            MountData.playerHeadOffset = 0; //设置了坐骑与玩家头部的偏移量。通常用于调整坐骑的位置，以确保坐骑看起来正确与玩家头部对齐
            MountData.bodyFrame = 6;//设置了坐骑的"bodyFrame"属性，它表示坐骑的身体动画帧的起始索引。在这个例子中，起始帧被设置为6

            MountData.flyingFrameCount = MountData.inAirFrameCount = MountData.idleFrameCount = MountData.swimFrameCount = 12;
            //设置了坐骑在不同状态下的帧数。坐骑通常具有不同的动画帧数，以便在不同情况下播放不同的动画
            MountData.flyingFrameDelay = MountData.inAirFrameDelay = MountData.idleFrameDelay = MountData.swimFrameDelay = 8;
            //这一系列设置了坐骑在不同状态下的帧延迟，控制动画的速度
            MountData.flyingFrameStart = MountData.inAirFrameStart = MountData.idleFrameStart = MountData.swimFrameStart = 0;           
            //设置了坐骑在空闲状态下动画帧的起始索引。通常，空闲状态的动画会从第0帧开始播放
            MountData.idleFrameLoop = true;//设置了空闲状态下的动画是否循环播放。如果设置为 true，则动画将循环播放


            if (!Main.dedServ) // 检查是否在服务器端运行游戏。这段代码块只会在客户端运行时执行
            {
                Asset<Texture2D> texture = ModContent.Request<Texture2D>("AncientGod/Items/Mounts/InfiniteFlight/FutureMecha/FutureMecha_Back");
                MountData.backTexture = texture;
                MountData.textureWidth = texture.Width(); // 设置了坐骑的纹理宽度和高度。这些值通常用于确定坐骑的碰撞区域和渲染
                MountData.textureHeight = texture.Height();
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            SpriteEffects effects = horizontalDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // 根据水平方向翻转坐骑的图像
            if (effects == SpriteEffects.FlipHorizontally)
            {
                // 调整绘制位置和帧的起始位置，这里是向左翻转，所以要调整X坐标
                drawPosition.X -= frame.Width * drawScale * 0.01f;
            }

            texture = (Texture2D)MountData.backTexture;

            float alpha = 1f; // 设置透明度为不透明（1.0）

            // 创建绘制数据并添加到列表中
            DrawData data = new DrawData(texture, drawPosition, frame, drawColor * alpha, rotation, drawOrigin, drawScale, effects, 0f);
            playerDrawData.Add(data);

            Texture2D mountTex = ModContent.Request<Texture2D>("AncientGod/Items/Mounts/InfiniteFlight/FutureMecha/FutureMecha_Back_Glow").Value;

            // 创建一个新的 Rectangle 来表示光亮纹理的 frame;若使用与坐骑背部纹理相同的 frame 变量，这会导致只显示后半部分的6帧。为了解决这个问题，
            // 你需要为光亮纹理的 frame 变量设置一个适当的值，以便显示光亮部分的所有帧(此问题目前尚未解决）
            //Rectangle glowFrame = frame;

            // 在原有的绘制数据之前，添加光亮纹理的绘制数据，并使用新的 glowFrame
            DrawData glowData = new DrawData(mountTex, drawPosition, frame, Color.White * alpha, rotation, drawOrigin, drawScale, effects, 0f);
            playerDrawData.Add(glowData);



            // 返回 false 表示自定义绘制逻辑生效，不会执行默认绘制逻辑
            return false;
        }


        public override void UpdateEffects(Player player)//通过以下代码已经几乎达到了星流飞椅的效果，唯一遗憾的是向下移动似乎最高只能到51mph？
        {
            // 在这里检查玩家的输入并根据需要修改垂直方向的速度
            if (player.controlUp || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space)) // 如果玩家按下：上键或空格键
            {
                player.velocity.Y = -17f; // 提高垂直上升速度
                player.runAcceleration = 20f;

            }
            else if (player.controlDown) // 如果玩家按下：下键
            {
                player.velocity.Y += 17f; // 增加垂直下降速度
                player.runAcceleration = 20f;
            }
            else
            {
                player.velocity.Y = 0;
            }

            if (player.controlLeft)//水平向左
            {
                player.velocity.X = -17f;
                player.runAcceleration = 20f;
                horizontalDirection = -1; // 设置坐骑的水平朝向为左
                player.direction = horizontalDirection;//玩家朝向与坐骑同步
            }
            else if (player.controlRight)//水平向右
            {
                player.velocity.X = 17f;
                player.runAcceleration = 20f;
                horizontalDirection = 1; // 设置坐骑的水平朝向为右
                player.direction = horizontalDirection;
            }
            else
            {
                player.velocity.X = 0;
            }

        }
    }
}
