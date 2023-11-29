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
using AncientGod.Items.Placeables.SpaceBase;

namespace AncientGod.System
{
    public class DetourShimmer : ModSystem
    {
        public override void Load()
        {
            // 这里可以加载一些在模组加载时需要进行的操作
        }
    }
    // 2. Our world generation code must start from a class extending ModSystem
    public class WorldGenT : ModSystem
    {
        // 用于本地化的文本
        // 3. These lines setup the localization for the message shown during world generation. Update your localization files after building and reloading the mod to provide values for this.
        public static LocalizedText WorldGenTMessage { get; private set; }

        // 设置默认值
        public override void SetStaticDefaults()
        {
            WorldGenTMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(WorldGenTMessage)}"));
        }

        // 修改世界生成任务的顺序
        // 4. We use the ModifyWorldGenTasks method to tell the game the order that our world generation code should run
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            
            // 5. We use FindIndex to locate the index of the vanilla world generation task called "Shinies". This ensures our code runs at the correct step.
            int ResetIndex = -1;
            // 移除所有不需要的生成任务
            tasks.RemoveAll(genpass => !genpass.Name.Equals("Reset") && !genpass.Name.Equals("Terrain") && 
            !genpass.Name.Equals("Underworld") && !genpass.Name.Equals("Smooth World"));
            // 找到 Smooth World 任务的索引
            ResetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (ResetIndex != -1)
            {
                // 在 Smooth World 之后插入自定义的生成任务
                // 6. We register our world generation pass by passing in an instance of our custom GenPass class below. The GenPass class will execute our world generation code.
                tasks.Insert(ResetIndex + 1, new RubbishGenPass("World Gen Tutorial Ores", 100f));
            }
        }
    }

    // 7. Make sure to inherit from the GenPass class.
    public class RubbishGenPass : GenPass
    {
        public RubbishGenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        // 生成任务的实际执行代码
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
                for (j = (int)((Main.worldSurface * 0.35) ); j < Main.rockLayer ; j++)
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

            progress.Message = WorldGenT.WorldGenTMessage.Value;
          
        }
    }

    // 7. Make sure to inherit from the GenPass class.
    public class SatelitteGenPass : GenPass
    {
        public static int STRUCTURE_WIDTH = 2;
        public static int STRUCTURE_HEIGHT = 2;
        public static ushort[] STRUCTURE_DATA =
        {
            TileID.Stone, TileID.Stone,
            TileID.Stone, TileID.Stone,
        };


        public SatelitteGenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        // 8. The ApplyPass method is where the actual world generation code is placed.
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int generateTries = WorldGen.genRand.Next(3, 10);
            for (int i = 0; i < generateTries; i++)
            {
                // 获取一个随机位置，作为建筑物的左上角
                //int houseX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                //int houseY = WorldGen.genRand.Next(Main.maxTilesY - 300, Main.maxTilesY - 100);
                int houseX = Main.maxTilesX / 2; // 设置为世界中心
                int houseY = Main.maxTilesY / 6; // 设置为更高的高度


                GenerateHouse(houseX, houseY);
            }
        }

        private void GenerateHouse(int startX, int startY)
        {
            // 建筑物的尺寸
            int buildingWidth = 300;
            int buildingHeight = 500;

            // Create walls
            for (int x = startX; x < startX + buildingWidth; x++)
            {
                for (int y = startY; y < startY + buildingHeight; y++)
                {
                    // 确保在边界内
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);
                    }
                }
            }

            // Create a door
            int doorY = startY + 1;
            if (doorY >= 0 && doorY < Main.maxTilesY)
            {
                WorldGen.KillTile(startX, doorY, false, false, true); // 移除现有瓷砖
            }

            // Create a roof
            for (int x = startX; x < startX + buildingWidth; x++)
            {
                int roofY = startY;
                if (roofY >= 0 && roofY < Main.maxTilesY)
                {
                    WorldGen.PlaceTile(x, roofY, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);
                }
            }
            // More customizations like windows, furniture, etc., can be added here
        }


    }
}
