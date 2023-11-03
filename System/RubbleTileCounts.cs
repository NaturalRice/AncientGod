using System;
using AncientGod.Items.Tiles;
using Terraria.ModLoader;

namespace AncientGod.System
{
    public class RubbleTileCounts : ModSystem
    {
        public int RubbleTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            RubbleTileCount = tileCounts[ModContent.TileType<Rubbish>()] + tileCounts[ModContent.TileType<WasteConcrete>()] + tileCounts[ModContent.TileType<WasteMetal>()];
        }
    }
}
