using System;
using AncientGod.Items.Placeables.SpaceBase;
using AncientGod.Items.Tiles;
using Terraria.ModLoader;

namespace AncientGod.System
{
    public class BaseTileCounts : ModSystem
    {
        public int BaseTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BaseTileCount = tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>()]
                          + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseDarkPanel>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>()]
                          + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BasePinkBlock>()]
                          + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseSkeleton>()] + tileCounts[ModContent.TileType<Items.Tiles.SpaceBase.BaseYellowBlock>()];
        }
    }
}
