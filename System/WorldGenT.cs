using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Generation;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.Localization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using AncientGod.Items.Tiles;

namespace AncientGod.System
{
    // 2. Our world generation code must start from a class extending ModSystem
    public class WorldGenT : ModSystem
    {
        // 3. These lines setup the localization for the message shown during world generation. Update your localization files after building and reloading the mod to provide values for this.
        public static LocalizedText WorldGenTMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            WorldGenTMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(WorldGenTMessage)}"));
        }

        // 4. We use the ModifyWorldGenTasks method to tell the game the order that our world generation code should run
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // 5. We use FindIndex to locate the index of the vanilla world generation task called "Shinies". This ensures our code runs at the correct step.
            int ResetIndex = -1;
            tasks.RemoveAll(genpass => !genpass.Name.Equals("Reset") && !genpass.Name.Equals("Terrain") && 
            !genpass.Name.Equals("Underworld") && !genpass.Name.Equals("Smooth World"));
            ResetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Underworld"));
            if (ResetIndex != -1)
            {
                // 6. We register our world generation pass by passing in an instance of our custom GenPass class below. The GenPass class will execute our world generation code.
                tasks.Insert(ResetIndex + 1, new RubbishGenPass("World Gen Rubbish", 100f));
            }
        }
    }
// 7. Make sure to inherit from the GenPass class.
public class RubbishGenPass : GenPass
    {
        public RubbishGenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        // 8. The ApplyPass method is where the actual world generation code is placed.
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            Main.spawnTileX = Main.maxTilesX / 2;
            int j = 0;
            for (j = 0; j < Main.maxTilesY; j++)
            {
                if (Main.tile[Main.spawnTileX,j].HasTile)
                {
                    break;
                }
            }
            Main.spawnTileY = j;
            // 9. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful to help users and modders identify passes that are stuck.      
            progress.Message = WorldGenT.WorldGenTMessage.Value;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (j = 0; j < Main.maxTilesY - 200  ; j++)
                {
                    Tile t = Main.tile[i, j];
                    if (t.HasTile)
                    {
                        if (t.TileType == TileID.Dirt)
                        {
                            t.TileType = (ushort)ModContent.TileType<Rubbish>();
                        }
                        if (t.TileType == TileID.Stone)
                        {
                            t.TileType = (ushort)ModContent.TileType<WasteConcrete>();
                        }
                    }
                }
            }
        }
    }
}
