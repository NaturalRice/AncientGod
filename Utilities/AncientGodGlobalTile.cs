using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;

namespace AncientGod.Utilities
{
    internal class AncientGodGlobalTile : GlobalTile
    {
        public void TileGlowmask(int i, int j, Texture2D tex, SpriteBatch spr, int frameheight = 0, ushort type = 0)
        {
            int XOffset = Main.tile[i, j].TileFrameX;
            int YOffset = Main.tile[i, j].TileFrameY;

            
        }

    }
}
