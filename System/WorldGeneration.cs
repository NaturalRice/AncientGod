using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Generation;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using AncientGod.Items.Tiles;

namespace AncientGod.System
{
    // 2. Our world generation code must start from a class extending ModSystem
    public class WorldGeneration : ModSystem
    {
        // We use PostWorldGen for this because we want to ensure that all chests have been placed before adding items.
        public override void PostWorldGen()
        {
        }
    }
}