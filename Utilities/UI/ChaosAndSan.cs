using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.UI;

namespace AncientGod.Utilities.UI
{
    public class ChaosAndSan : UIElement//这个类继承自 UIElement 类，表示一个 Terraria 用户界面元素。ChaosAndSan 类是用于显示一个条形图的特殊 UI 元素。
    {
        public static ChaosAndSan INSTANCE;

        private static Texture2D tex;

        public static void init()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new ChaosAndSan();
                INSTANCE.Activate();
                tex = ModContent.Request<Texture2D>("AncientGod/Projectiles/Mounts/InfiniteFlight/AncientMecha/AncientMechaArm").Value;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            float quotient = (float)AncientGodWorld.sanity / AncientGodWorld.MAX_SANITY;

            string text = "" + AncientGodWorld.sanity + '/' + AncientGodWorld.MAX_SANITY;

            // Define bar size and position
            Vector2 position = new Vector2(Main.screenWidth + 20, Main.screenHeight - 20) / 2f; // Example position
            Vector2 size = new Vector2(200, 20); // Example size

            // Draw the background of the bar (empty part)
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.Gray);

            // Draw the filled part of the bar
            spriteBatch.Draw(tex, new Rectangle((int)position.X, (int)position.Y, (int)(size.X * quotient), (int)size.Y), Color.Green);
        }
    }
}
