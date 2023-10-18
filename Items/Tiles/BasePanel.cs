using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace AncientGod.Items.Tiles
{
    internal class BasePanel : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            int[] DustCandidate = { DustID.Stone, DustID.Copper, DustID.Glass, DustID.Sand };//尝试一下能否随机生成石头，铜矿，玻璃
            DustType = DustCandidate[Main.rand.Next(3)];
            //DustType = ModContent.DustType<Sparkle>();

            AddMapEntry(new Color(89, 102, 186));
        }
        public override bool CanDrop(int i, int j)
        {
            if (((Main.rand.Next(1919) * i % (j + 114)) + 514) % 100 > 83)
            {
                return true;
            }
            return false;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            //地表附近掉种子，其余地方不掉
            int[] LootCandidate = { ModContent.ItemType<Placeables.BasePanel>(), ItemID.Wood, ItemID.DirtBlock, ItemID.TinOre, ItemID.IronOre, ItemID.Glass,
                ItemID.GrassSeeds, ItemID.JungleGrassSeeds, ItemID.DaybloomSeeds, ItemID.Acorn};
            int Height = Main.tile[i, j].TileFrameY;
            int dropItem = 0;
            int randSeed = Main.rand.Next(LootCandidate.Length);
            if (Height <= Main.rockLayer && Height >= Main.worldSurface * 0.35)//泥土层以上天空层以下
            {
                dropItem = LootCandidate[randSeed];
            }
            else
            {
                dropItem = LootCandidate[randSeed % 6];
            }
            yield return new Item(dropItem);
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            int RandBound = fail ? 4 : 20;
            num = Main.rand.Next(RandBound) % 5;
        }

        //public override void ChangeWaterfallStyle(ref int style)
        //{
        //    style = ModContent.GetInstance<ExampleWaterfallStyle>().Slot;
        //}

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture = ModContent.Request<Texture2D>("AncientGod/Items/Tiles/BasePanel").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AncientGod/Items/Tiles/BasePanel_Glow").Value;

            // If you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
            // The reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting mode
            // Press Shift+F9 to change lighting modes quickly to verify your code works for all lighting modes
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Because height of third tile is different we change it
            int height = tile.TileFrameY % AnimationFrameHeight == 36 ? 18 : 16;

            // Offset along the Y axis depending on the current frame
            int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;

            // Firstly we draw the original texture and then glow mask texture
            spriteBatch.Draw(
                texture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
            // Make sure to draw with Color.White or at least a color that is fully opaque
            // Achieve opaqueness by increasing the alpha channel closer to 255. (lowering closer to 0 will achieve transparency)
            spriteBatch.Draw(
                glowTexture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Return false to stop vanilla draw
            return false;
        }

    }
}
