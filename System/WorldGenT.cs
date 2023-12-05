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
            // 在模组加载时进行一些操作
        }
    }

    public class WorldGenT : ModSystem
    {
        public static LocalizedText WorldGenTMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            WorldGenTMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(WorldGenTMessage)}"));
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // 移除所有不需要的生成任务
            tasks.RemoveAll(genpass => !genpass.Name.Equals("Reset") && !genpass.Name.Equals("Terrain") &&
            !genpass.Name.Equals("Underworld") && !genpass.Name.Equals("Smooth World"));

            // 找到 Smooth World 任务的索引
            int resetIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (resetIndex != -1)
            {
                // 在 Smooth World 之后插入自定义的生成任务
                tasks.Insert(resetIndex + 1, new RubbishGenPass("World Gen Tutorial Ores", 100f));
                tasks.Insert(resetIndex + 1, new SatelitteGenPass("World Gen Satelitte", 100f));
            }
        }
    }

    public class RubbishGenPass : GenPass
    {
        public RubbishGenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            // 设置世界生成的起始位置
            Main.spawnTileX = Main.maxTilesX / 2;
            int j = 0;
            for (j = 0; j < Main.maxTilesY; j++)
            {
                if (Main.tile[Main.spawnTileX, j].HasTile)
                {
                    break;
                }
            }
            Main.spawnTileY = j;

            // 设置进度消息
            progress.Message = WorldGenT.WorldGenTMessage.Value;

            // 开始修改世界
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (j = (int)((Main.worldSurface * 0.35)); j < Main.rockLayer; j++)
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

    public class SatelitteGenPass : GenPass
    {
        public static int StructureWidth = 2;
        public static int StructureHeight = 2;
        public static ushort[] StructureData =
        {
            TileID.Stone, TileID.Stone,
            TileID.Stone, TileID.Stone,
        };

        public SatelitteGenPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int generateTries = WorldGen.genRand.Next(3, 10);
            for (int i = 0; i < generateTries; i++)
            {
                int houseX = Main.maxTilesX / 2;
                int houseY = Main.maxTilesY / 12;

                AncientGod.getlog().Info($"Generating house at: {houseX}, {houseY}");
                GenerateHouse(houseX, houseY);
            }
        }

        private void GenerateHouse(int centerX, int centerY)
        {
            int buildingWidth = 30;
            int buildingHeight = 50;

            // 创建墙
            for (int x = centerX + 1; x < centerX + buildingWidth; x++)
            {
                for (int y = centerY; y < centerY + buildingHeight; y += 7)
                {
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);
                    }
                }
            }

            // 创建家具
            for (int x = centerX + 1; x < centerX + buildingWidth; x += 12)
            {
                for (int y = centerY; y < centerY + buildingHeight; y++)
                {
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseBed>(), true, true);
                    }
                }
            }

            // 创建墙
            for (int x = centerX + 4; x < centerX + buildingWidth; x++)
            {
                for (int y = centerY; y < centerY + buildingHeight; y++)
                {
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {
                        WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseEnclosureWall>(), true);
                    }
                }
            }

            // 创建屋顶
            for (int x = centerX; x < centerX + buildingWidth; x++)
            {
                int roofY = centerY;
                if (roofY >= 0 && roofY < Main.maxTilesY)
                {
                    WorldGen.PlaceTile(x, roofY, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);
                }
            }
        }
    }
}
