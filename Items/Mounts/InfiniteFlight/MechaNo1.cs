using Terraria;
using Terraria.ModLoader;
using AncientGod.Buffs.Mounts;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace AncientGod.Items.Mounts.InfiniteFlight
{
    public class MechaNo1 : ModMount
    {
        private int horizontalDirection = 1; // Ĭ��Ϊ����


        public override void SetStaticDefaults()
        {
            MountData.acceleration = 0.7f;//���ٶ�
            //MountData.jumpHeight = 0;//��Ծ�߶�
            //MountData.jumpSpeed = 0f;//��Ծ�ٶ�            

            //MountData.heightBoost = 15;//����ĸ߶�����
            //MountData.fallDamage = 0f;//�������ʱ���˺�
            MountData.runSpeed = 17f;//�����ٶ�
            //MountData.dashSpeed = 14f;//����ĳ���ٶ�

            MountData.fatigueMax = int.MaxValue - 1;//�����ƣ�Ͷ�����(ȥ���˴��������ٶȻ᲻�ɿأ����½��ٶ���Ϊ41mph)

            MountData.blockExtraJumps = false;//�Ƿ���ֹ������Ծ
            MountData.usesHover = true;//�Ƿ�ʹ�����������У�����
            MountData.flightTimeMax = int.MaxValue - 1;//�����������ʱ��

            MountData.buff = ModContent.BuffType<MechaNo1Buff>();//�����ṩ��Buff����
            MountData.spawnDust = 33;//ʹ������Ч������33
            MountData.spawnDustNoGravity = true;//����Ч������������Ӱ�죬��Ư���ڿ���

            MountData.totalFrames = 12;//֡��
            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray();
            //���������������ʱ������Y���ϵ�ƫ������ͨ��Enumerable.Repeat�����ǽ�ֵ20�ظ���12�Σ�Ȼ����ת��Ϊ���飬��ȷ��ÿһ֡����ĸ߶ȶ�һ��
            MountData.xOffset = 0;//������������X���ϵ�ƫ���������ֵ��ʾ�����������ҵ�ˮƽλ��
            MountData.yOffset = -16;//������������Y���ϵ�ƫ���������ֵ��ʾ�����������ҵĴ�ֱλ��
            MountData.playerHeadOffset = 0; //���������������ͷ����ƫ������ͨ�����ڵ��������λ�ã���ȷ�����￴������ȷ�����ͷ������
            MountData.bodyFrame = 6;//�����������"bodyFrame"���ԣ�����ʾ��������嶯��֡����ʼ����������������У���ʼ֡������Ϊ6

            MountData.flyingFrameCount = MountData.inAirFrameCount = MountData.idleFrameCount = MountData.swimFrameCount = 6;
            //�����������ڲ�ͬ״̬�µ�֡��������ͨ�����в�ͬ�Ķ���֡�����Ա��ڲ�ͬ����²��Ų�ͬ�Ķ���
            MountData.flyingFrameDelay = MountData.inAirFrameDelay = MountData.idleFrameDelay = MountData.swimFrameDelay = 8;
            //��һϵ�������������ڲ�ͬ״̬�µ�֡�ӳ٣����ƶ������ٶ�
            MountData.flyingFrameStart = MountData.inAirFrameStart = MountData.swimFrameStart = 6;
            //��һϵ�������������ڲ�ͬ״̬�µ�֡�ӳ٣����ƶ������ٶ�
            MountData.idleFrameStart = 0;//�����������ڿ���״̬�¶���֡����ʼ������ͨ��������״̬�Ķ�����ӵ�0֡��ʼ����
            MountData.idleFrameLoop = true;//�����˿���״̬�µĶ����Ƿ�ѭ�����š��������Ϊ true���򶯻���ѭ������



            if (!Main.dedServ) // ����Ƿ��ڷ�������������Ϸ����δ����ֻ���ڿͻ�������ʱִ��
            {
                Asset<Texture2D> texture = ModContent.Request<Texture2D>("AncientGod/Items/Mounts/InfiniteFlight/MechaNo1");
                MountData.backTexture = texture;
                MountData.textureWidth = texture.Width(); // ����������������Ⱥ͸߶ȡ���Щֵͨ������ȷ���������ײ�������Ⱦ
                MountData.textureHeight = texture.Height();
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            SpriteEffects effects = horizontalDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // ����ˮƽ����ת�����ͼ��
            if (effects == SpriteEffects.FlipHorizontally)
            {
                // ��������λ�ú�֡����ʼλ�ã�����������ת������Ҫ����X����
                drawPosition.X -= frame.Width * drawScale * 0.01f;
            }

            texture = (Texture2D)MountData.backTexture;

            float alpha = 1f; // ����͸����Ϊ��͸����1.0��

            // �����������ݲ���ӵ��б���
            DrawData data = new DrawData(texture, drawPosition, frame, drawColor * alpha, rotation, drawOrigin, drawScale, effects, 0f);
            playerDrawData.Add(data);

            Texture2D mountTex = ModContent.Request<Texture2D>("AncientGod/Items/Mounts/InfiniteFlight/MechaNo1_Glow").Value;

            // ����һ���µ� Rectangle ����ʾ��������� frame;��ʹ�������ﱳ��������ͬ�� frame ��������ᵼ��ֻ��ʾ��벿�ֵ�6֡��Ϊ�˽��������⣬
            // ����ҪΪ��������� frame ��������һ���ʵ���ֵ���Ա���ʾ�������ֵ�����֡(������Ŀǰ��δ�����
            //Rectangle glowFrame = frame;

            // ��ԭ�еĻ�������֮ǰ����ӹ�������Ļ������ݣ���ʹ���µ� glowFrame
            DrawData glowData = new DrawData(mountTex, drawPosition, frame, Color.White * alpha, rotation, drawOrigin, drawScale, effects, 0f);
            playerDrawData.Add(glowData);



            // ���� false ��ʾ�Զ�������߼���Ч������ִ��Ĭ�ϻ����߼�
            return false;
        }


        public override void UpdateEffects(Player player)//ͨ�����´����Ѿ������ﵽ���������ε�Ч����Ψһ�ź����������ƶ��ƺ����ֻ�ܵ�51mph��
        {
            // ����������ҵ����벢������Ҫ�޸Ĵ�ֱ������ٶ�
            if (player.controlUp || Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space)) // �����Ұ��£��ϼ���ո��
            {
                player.velocity.Y = -17f; // ��ߴ�ֱ�����ٶ�
                player.runAcceleration = 20f;

            }
            else if (player.controlDown && !player.mount._abilityCharging) // �����Ұ��£��¼�
            {
                player.velocity.Y += 17f; // ���Ӵ�ֱ�½��ٶ�
                player.runAcceleration = 20f;
            }
            else
            {
                player.velocity.Y = 0;
            }

            if (player.controlLeft)//ˮƽ����
            {
                player.velocity.X = -17f;
                player.runAcceleration = 20f;
                horizontalDirection = -1; // ���������ˮƽ����Ϊ��
                player.direction = horizontalDirection;//��ҳ���������ͬ��
            }
            else if (player.controlRight)//ˮƽ����
            {
                player.velocity.X = 17f;
                player.runAcceleration = 20f;
                horizontalDirection = 1; // ���������ˮƽ����Ϊ��
                player.direction = horizontalDirection;
            }
            else
            {
                player.velocity.X = 0;
            }

        }
    }
}