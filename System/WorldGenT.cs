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
            !genpass.Name.Equals("Underworld") && !genpass.Name.Equals("Smooth World") && !genpass.Name.Equals("Dungeon") && !genpass.Name.Equals("Hellforge"));

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
                        if (t.WallType == WallID.Dirt)
                        {
                            t.WallType = (ushort)ModContent.WallType<RubbishWall>();
                        }
                        if (t.TileType == TileID.Stone)
                        {
                            t.TileType = (ushort)ModContent.TileType<WasteMetal>();
                        }
                        if (t.WallType == WallID.Stone)
                        {
                            t.WallType = (ushort)ModContent.WallType<WasteConcreteWall>();
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
                int houseY = Main.maxTilesY / 10;

                int wasteX = Main.maxTilesX / 2;
                int wasteY = Main.maxTilesY / 5;

                AncientGod.getlog().Info($"Generating house at: {houseX}, {houseY}");
                //创建基础建筑结构
                LoadBearingColumn(Main.maxTilesX / 2 + 40, Main.maxTilesY / 10  + 42, 30, 45);
                LoadBearingColumn(Main.maxTilesX / 2 - 40, Main.maxTilesY / 10 + 42, 30, 45);

                DetectionTower(Main.maxTilesX / 2 + 60, Main.maxTilesY / 10 - 20, 10, 40);
                DetectionTower(Main.maxTilesX / 2 - 60, Main.maxTilesY / 10 - 20, 10, 40);
                DetectionTower(Main.maxTilesX / 2 + 90, Main.maxTilesY / 10 - 16, 15, 30);
                DetectionTower(Main.maxTilesX / 2 - 90, Main.maxTilesY / 10 - 16, 15, 30);
                DetectionTower(Main.maxTilesX / 2 + 120, Main.maxTilesY / 10 - 3, 10, 30);
                DetectionTower(Main.maxTilesX / 2 - 120, Main.maxTilesY / 10 - 3, 10, 30);              

                SignalTower(Main.maxTilesX / 2 + 40, Main.maxTilesY / 10 - 30, 10, 40);
                SignalTower(Main.maxTilesX / 2 - 40, Main.maxTilesY / 10 - 30, 10, 40);
                SignalTower(Main.maxTilesX / 2 + 70, Main.maxTilesY / 10 - 18, 15, 45);
                SignalTower(Main.maxTilesX / 2 - 70, Main.maxTilesY / 10 - 18, 15, 45);
                SignalTower(Main.maxTilesX / 2 + 140, Main.maxTilesY / 10 - 1, 8, 30);
                SignalTower(Main.maxTilesX / 2 - 140, Main.maxTilesY / 10 - 1, 8, 30);

                SignalTower(Main.maxTilesX / 2 + 40, Main.maxTilesY / 10 + 80, 20, 50);
                SignalTower(Main.maxTilesX / 2 - 40, Main.maxTilesY / 10 + 80, 20, 50);
                SignalTower(Main.maxTilesX / 2 + 70, Main.maxTilesY / 10 + 74, 15, 40);
                SignalTower(Main.maxTilesX / 2 - 70, Main.maxTilesY / 10 + 74, 15, 40);
                SignalTower(Main.maxTilesX / 2 + 140, Main.maxTilesY / 10 + 65, 10, 30);
                SignalTower(Main.maxTilesX / 2 - 140, Main.maxTilesY / 10 + 65, 10, 30);

                ShelfTower(Main.maxTilesX / 2 + 100, Main.maxTilesY / 10 + 10, 50, 20);
                ShelfTower(Main.maxTilesX / 2 - 100, Main.maxTilesY / 10 + 10, 50, 20);
                ShelfTower(Main.maxTilesX / 2 + 110, Main.maxTilesY / 10 + 56, 40, 15);
                ShelfTower(Main.maxTilesX / 2 - 110, Main.maxTilesY / 10 + 56, 40, 15);
                ShelfTower(Main.maxTilesX / 2 + 130, Main.maxTilesY / 10 + 15, 45, 15);
                ShelfTower(Main.maxTilesX / 2 - 130, Main.maxTilesY / 10 + 15, 45, 15);
                ShelfTower(Main.maxTilesX / 2 + 150, Main.maxTilesY / 10 + 57, 40, 20);
                ShelfTower(Main.maxTilesX / 2 - 150, Main.maxTilesY / 10 + 57, 40, 20);
                ShelfTower(Main.maxTilesX / 2 + 30, Main.maxTilesY / 10 - 24, 40, 20);
                ShelfTower(Main.maxTilesX / 2 - 30, Main.maxTilesY / 10 - 24, 40, 20);



                GenerateHouse(houseX, houseY);

                GenerateWasteBuilding(wasteX, wasteY);

            }
        }

        private void LoadBearingColumn(int centerX, int centerY, int width, int height)//承重柱结构
        {
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                for (int y = centerY - height / 2; y <= centerY + height / 2; y++)
                {
                    if (y - centerY <= -width / 2 + 4 || y - centerY >= width / 2 - 4)
                    {
                        if ((y - centerY >= (x - centerX) * height / width && y - centerY >= -(x - centerX) * height / width) || (y - centerY <= (x - centerX) * height / width && y - centerY <= -(x - centerX) * height / width))
                        {
                            if(y - centerY >= -width / 2 + 2 && y- centerY <= -width / 2 + 3)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);
                            }
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseSkeleton>(), true, true);
                        }
                    }
                    else
                    {
                        if (x - centerX >= -width / 4 && x - centerX <= width / 4)
                        {
                            if (y - centerY == -width / 2 + 8 || y - centerY == width / 2 - 8)
                            {
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY == -width / 2 + 10 || y - centerY == width / 2 - 10)
                            {
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseYellowBlockWall>(), true);
                            }
                            if ((x - centerX >= -width / 4 && x - centerX <= -width / 8) || (x - centerX >= width / 8 && x - centerX <= width / 4))
                            {
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                            if ((y - centerY >= -width / 2 + 5 && y - centerY <= -width / 2 + 7) || (y - centerY >= width / 2 - 7 && y - centerY <= width / 2 - 5))
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>(), true, true);
                            }
                        }        
                        
                    }
                }
            }
        }     

        private void DetectionTower(int centerX, int centerY, int width, int height)//探测塔结构
        {
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                for (int y = centerY - height / 2; y <= centerY + height / 2; y++)
                {
                    if(y - centerY <= height / 2 - 15)
                    {
                        if((x - centerX > -width / 2 + 3 && x - centerX <= -width / 2 + 4) || (x - centerX >= width / 2 - 4 && x - centerX < width / 2 - 3) || (x - centerX > -width / 2 + 2 && x - centerX <= -width / 2 + 3 && y - centerY >= -height / 2 + 3) || (x - centerX >= width / 2 - 3 && x - centerX < width / 2 - 2 && y - centerY >= -height / 2 + 3))
                        {
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                        }
                        if ((x - centerX > -width / 2 + 1 && x - centerX < -width / 2 + 3 && y - centerY >= -height / 2 + 6) || (x - centerX > width / 2 - 3 && x - centerX < width / 2 - 1 && y - centerY >= -height / 2 + 6))
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                        }

                    }
                    else
                    {
                        if ((y - centerY >= height / 2 - 14 && y - centerY < height / 2 - 13) || (y - centerY >= height / 2 - 12 && y - centerY < height / 2 - 11) || (y - centerY >= height / 2 - 10 && y - centerY < height / 2 - 9))
                        {
                            if(x - centerX >= -width / 2 + 4 && x - centerX <= width / 2 - 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                        }
                         else if ((y - centerY >= height / 2 - 13 && y - centerY < height / 2 - 12) || (y - centerY >= height / 2 - 11 && y - centerY < height / 2 - 10) || (y - centerY >= height / 2 - 9 && y - centerY < height / 2 - 8))
                        {
                            if (x - centerX >= -width / 2 + 3 && x - centerX <= width / 2 - 3)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseArmorWall>(), true);
                            }
                        }
                        else if ((y - centerY >= height / 2 - 8 && y - centerY < height / 2 - 7) || (y - centerY >= height / 2 - 6 && y - centerY < height / 2 - 5))
                        {
                            if (x - centerX >= -width / 2 + 3 && x - centerX <= width / 2 - 3)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePinkBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseYellowBlockWall>(), true);
                            }
                        }
                        else if(y - centerY >= -(x - centerX) * height / width && y - centerY >= (x - centerX) * height / width)
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                        }
                    }
                }
            }
        }

        private void SignalTower(int centerX, int centerY, int width, int height)//信号塔结构
        {
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                for (int y = centerY - height / 2; y <= centerY + height / 2; y++)
                {
                    if (centerX - Main.maxTilesX / 2 > 0)//如果在右
                    {
                        if(centerY - Main.maxTilesY / 10 < 0)//如果在右上
                        {
                            if (x - centerX <= -width / 2 + 2 && y - centerY >= (x - centerX) * height / width)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if(x - centerX > -width / 2 + 2 && x - centerX <= -width / 2 + 5 && y - centerY >= -height / 2 + 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseArmorWall>(), true);
                            }
                            if (x - centerX > -width / 2 + 5 && x - centerX <= -width / 2 + 7 && y - centerY >= -height / 2 + 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseYellowBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY >= (x - centerX) * height / width + height / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                            }
                        }
                        else//如果在右下
                        {
                            if (x - centerX <= -width / 2 + 2 && y - centerY <= -(x - centerX) * height / width)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (x - centerX > -width / 2 + 2 && x - centerX <= -width / 2 + 5 && y - centerY <= height / 2 - 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseArmorWall>(), true);
                            }
                            if (x - centerX > -width / 2 + 5 && x - centerX <= -width / 2 + 7 && y - centerY <= height / 2 - 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseYellowBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY <= -(x - centerX) * height / width - height / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                            }
                        }
                    }
                    else//如果在左
                    {
                        if (centerY - Main.maxTilesY / 10 < 0)//如果在左上
                        {
                            if (x - centerX >= width / 2 - 2 && y - centerY >= -(x - centerX) * height / width)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (x - centerX < width / 2 - 2 && x - centerX >= width / 2 - 5 && y - centerY >= -height / 2 + 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseArmorWall>(), true);
                            }
                            if (x - centerX < width / 2 - 5 && x - centerX >= width / 2 - 7 && y - centerY >= -height / 2 + 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseYellowBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY >= -(x - centerX) * height / width + height / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                            }
                        }
                        else//如果在左下
                        {
                            if (x - centerX >= width / 2 - 2 && y - centerY <= (x - centerX) * height / width)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (x - centerX < width / 2 - 2 && x - centerX >= width / 2 - 5 && y - centerY <= height / 2 - 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseArmorWall>(), true);
                            }
                            if (x - centerX < width / 2 - 5 && x - centerX >= width / 2 - 7 && y - centerY <= height / 2 - 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseYellowBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY <= (x - centerX) * height / width - height / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBaffle>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);
                            }
                        }
                    }


                }
            }
        }

        private void ShelfTower(int centerX, int centerY, int width, int height)//承重柱结构
        {
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                for (int y = centerY - height / 2; y <= centerY + height / 2; y++)
                {
                    if (centerX - Main.maxTilesX / 2 > 0)//如果在右
                    {
                        if (centerY - Main.maxTilesY / 10 < 0)//如果在右上
                        {
                            if (y - centerY <= -height / 2 + 2 && x - centerX >= (y - centerY) * width / height)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 2 && y - centerY <= -height / 2 + 5 && x - centerX >= -width / 2 + 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 5 && y - centerY <= -height / 2 + 7 && x - centerX >= -width / 2 + 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseYellowBlockWall>(), true);
                            }
                            if (x - centerX >= (y - centerY) * width / height + width / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseSkeletonWall>(), true);
                            }
                        }
                        else//如果在右下
                        {
                            if (y - centerY <= -height / 2 + 2 && x - centerX <= -(y - centerY) * width / height)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 2 && y - centerY <= -height / 2 + 5 && x - centerX <= width / 2 - 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 5 && y - centerY <= -height / 2 + 7 && x - centerX <= width / 2 - 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseYellowBlockWall>(), true);
                            }
                            if (x - centerX <= -(y - centerY) * width / height - width / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseSkeletonWall>(), true);
                            }
                        }
                    }
                    else//如果在左
                    {
                        if (centerY - Main.maxTilesY / 10 < 0)//如果在左上
                        {
                            if (y - centerY <= -height / 2 + 2 && x - centerX >= -(y - centerY) * width / height)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 2 && y - centerY <= -height / 2 + 5 && x - centerX <= width / 2 - 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 5 && y - centerY <= -height / 2 + 7 && x - centerX <= width / 2 - 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseYellowBlockWall>(), true);
                            }
                            if (x - centerX <= -(y - centerY) * width / height - width / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseSkeletonWall>(), true);
                            }
                        }
                        else//如果在左下
                        {
                            if (y - centerY <= -height / 2 + 2 && x - centerX <= (y - centerY) * width / height)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseGlass>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseBaffleWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 2 && y - centerY <= -height / 2 + 5 && x - centerX >= -width / 2 + 10)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);
                            }
                            if (y - centerY > -height / 2 + 5 && y - centerY <= -height / 2 + 7 && x - centerX >= -width / 2 + 15)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseYellowBlockWall>(), true);
                            }
                            if (x - centerX >= (y - centerY) * width / height + width / 4)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseSkeletonWall>(), true);
                            }
                        }
                    }
                }
            }
        }








        private void GenerateHouse(int centerX, int centerY)//整个太空基地的生成逻辑 ~ \^o^/ ~
        {
            //这里是所有长度变量，要修改时改这里即可(记住！X轴坐标为:(x - centerX)，Y轴坐标为:(y - centerY) 🗯)
            int totalWidth = 180;//总宽度的一半
            int totalHeight = 120;//总高度的一半
            int majorAxis1 = 40;//主舱底舱长轴X
            int minorAxis1 = 30;//主舱/底舱短轴Y
            float slope1 = 0.25f;//主舱下方骨架顶部斜率
            float intercept1 = -25;//主舱下方骨架顶部截距

            float slope2 = 0.2f;//两侧旋翼顶部斜率
            float intercept2 = -10;//两侧旋翼顶部截距
            int bottom = 50;//旋翼底部相对中心高度

            float slope3 = 1;//腹舱两侧斜率
            float intercept3 = 120;//腹舱两侧截距
            int bottom2 = 70;//腹舱底部相对中心高度

            int top1 = 50;//顶部护甲最高高度（相对中心）
            float slope4 = 3f;//顶部护甲斜率
            float intercept4 = -90;//顶部护甲截距


            



            // 创建外层方块
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y ++)
                {
                    //椭圆公式，以主舱为中心
                    if (Math.Pow((x - centerX),2)/ Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY),2)/ Math.Pow(minorAxis1, 2) >= 0.9 && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 1)
                    {
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);//内层墙壳                        
                    }
                    else if(Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 1 && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 1.1)
                    {
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);//外层镶板
                    }
                    else if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 1.1 && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 2)
                    {
                        if(y - centerY >= -slope1 * (x - centerX) + intercept1 && y - centerY >= slope1 * (x - centerX) + intercept1)
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseSkeleton>(), true, true);//主舱下方骨架
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseSkeletonWall>(), true);
                        }
                    }
                    else if(Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 2)//旋翼外壳
                    {                      
                        if (y - centerY >= -slope2 * (x - centerX) + intercept2 - 2 && y - centerY >= slope2 * (x - centerX) + intercept2 - 2)
                        {
                            if (y - centerY < -slope2 * (x - centerX) + intercept2 || y - centerY < slope2 * (x - centerX) + intercept2)
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);//顶部外层镶板
                        }
                        if (y - centerY >= -slope2 * (x - centerX) + intercept2 && y - centerY >= slope2 * (x - centerX) + intercept2)
                        {
                            if ((y - centerY < -slope2 * (x - centerX) + intercept2 + 2 || y - centerY < slope2 * (x - centerX) + intercept2 + 2) && (x - centerX >= -totalWidth + 2 && x - centerX <= totalWidth - 2))
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);//顶部内层墙壳
                        }
                        if(((y - centerY >= -slope2 * (x - centerX) + intercept2 && y - centerY >= slope2 * (x - centerX) + intercept2) && (x - centerX < -totalWidth + 2 || x - centerX > totalWidth - 2) && (y - centerY <= bottom) || (y - centerY > bottom - 2 && y - centerY <= bottom)))
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);//两边及底部外层镶板
                        }
                        if (((y - centerY >= -slope2 * (x - centerX) + intercept2 + 2 && y - centerY >= slope2 * (x - centerX) + intercept2 + 2) && ((x - centerX >= -totalWidth + 2 && x - centerX < -totalWidth + 4) || (x - centerX > totalWidth - 4 && x - centerX <= totalWidth - 2))) && (y - centerY <= bottom - 2) || (y - centerY > bottom - 4 && y - centerY <= bottom - 2 && x - centerX >= -totalWidth + 2 && x - centerX <= totalWidth - 2))
                        {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);//两边及底部内层墙壳
                        }

                        if(y - centerY >= -slope3 * (x - centerX) + intercept3 || y - centerY >= slope3 * (x - centerX) + intercept3)
                        {
                            if((y - centerY < -slope3 * (x - centerX) + intercept3 + 3 && y - centerY < slope3 * (x - centerX) + intercept3 + 3) && y - centerY > bottom && y - centerY <= bottom2)
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);//腹部两边外层镶板              
                        }
                        if (y - centerY > bottom2 - 3 && y - centerY <= bottom2 && x - centerX <= (y - centerY - intercept3 - 3) / (-slope3) && x - centerX >= (y - centerY - intercept3 - 3) / slope3)
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);//腹部底部外层镶板
                        }
                        if ((y - centerY < -slope3 * (x - centerX) + intercept3 && y - centerY < slope3 * (x - centerX) + intercept3) && y - centerY > bottom - 2 && y - centerY <= bottom2 - 3)
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseEnclosure>(), true, true);//腹部两边及底部内层墙壳
                        }
                        if ((y - centerY < -slope3 * (x - centerX) + intercept3 - 3 && y - centerY < slope3 * (x - centerX) + intercept3 - 3) && y - centerY > bottom - 4 && y - centerY <= bottom2 - 6)
                        {
                            WorldGen.KillTile(x, y, false, false, true); // 移除现有瓷砖
                        }

                        if(y - centerY > bottom2)
                        {
                            if(Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY - bottom - 5), 2) / Math.Pow(minorAxis1, 2) >= 0.9 && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY - bottom - 5), 2) / Math.Pow(minorAxis1, 2) <= 1)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseDarkPanel>(), true, true);//底舱内层墙壳
                            }
                            if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY - bottom - 5), 2) / Math.Pow(minorAxis1, 2) > 1 && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY - bottom - 5), 2) / Math.Pow(minorAxis1, 2) <= 1.1)
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseSkeleton>(), true, true);//底舱外层镶板
                            }
                        }
                    }
                }
            }

            // 创建外部架构方块
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 1.1 && (y - centerY < -slope1 * (x - centerX) + intercept1 || y - centerY < slope1 * (x - centerX) + intercept1))
                    {
                        if((y - centerY > -slope4 * (x - centerX) + intercept4 && x - centerX < -5) || (x - centerX > 5 && y - centerY > slope4 * (x - centerX) + intercept4))
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);//顶部中央主舱护甲
                        if ((y - centerY > -slope4 * 0.4 * (x - centerX) + intercept4 + 30 && x - centerX < -10) || (x - centerX > 10 && y - centerY > slope4 * 0.4 * (x - centerX) + intercept4 + 30))
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);//顶部中央主舱支架                       
                    }
                    //这一段的效果太Low了，Low得不行，先注释掉
                    /*if ((y - centerY < -slope2 * (x - centerX) + intercept2 - 10 || y - centerY < slope2 * (x - centerX) + intercept2 - 10) && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 2)
                    {
                        if (y - centerY > -slope2 * 1.7 * (x - centerX) + intercept2 - 32 && y - centerY > slope2 * 1.7 * (x - centerX) + intercept2 - 32)
                        {
                            if (((x - centerX) > -80 && (x - centerX) < -75) || ((x - centerX) > -70 && (x - centerX) < -65) || ((x - centerX) > -100 && (x - centerX) < -95) || ((x - centerX) > -125 && (x - centerX) < -120) || ((x - centerX) > -165 && (x - centerX) < -160) || ((x - centerX) > -180 && (x - centerX) < -175))
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);//旋翼顶部外部护甲
                            if (((x - centerX) < 80 && (x - centerX) > 75) || ((x - centerX) < 70 && (x - centerX) > 65) || ((x - centerX) < 100 && (x - centerX) > 95) || ((x - centerX) < 125 && (x - centerX) > 120) || ((x - centerX) < 165 && (x - centerX) > 160) || ((x - centerX) < 180 && (x - centerX) > 175))
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseArmor>(), true, true);//旋翼顶部外部护甲
                        }
                    }
                    if ((y - centerY < -slope2 * (x - centerX) + intercept2 - 2 || y - centerY < slope2 * (x - centerX) + intercept2 - 2) && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 2)
                    {
                        if (y - centerY >= -slope2 * (x - centerX) + intercept2 - 10 && y - centerY >= slope2 * (x - centerX) + intercept2 - 10)
                        {
                            if (((x - centerX) > -80 && (x - centerX) < -75) || ((x - centerX) > -70 && (x - centerX) < -65) || ((x - centerX) > -100 && (x - centerX) < -95) || ((x - centerX) > -125 && (x - centerX) < -120) || ((x - centerX) > -165 && (x - centerX) < -160) || ((x - centerX) > -180 && (x - centerX) < -175))
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);//旋翼顶部外部支架
                            if (((x - centerX) < 80 && (x - centerX) > 75) || ((x - centerX) < 70 && (x - centerX) > 65) || ((x - centerX) < 100 && (x - centerX) > 95) || ((x - centerX) < 125 && (x - centerX) > 120) || ((x - centerX) < 165 && (x - centerX) > 160) || ((x - centerX) < 180 && (x - centerX) > 175))
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseBracket>(), true, true);//旋翼顶部外部支架
                        }
                    }*/
                }
            }

            // 创建内部架构方块
            //NPC住房隔间
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY + totalHeight - 1; y >= centerY - totalHeight; y--)
                {
                    if ((x - centerX >= - totalWidth + 26 && x - centerX < -75) || (x - centerX > 75 && x - centerX <= totalWidth - 26))
                    {
                        if(y - centerY > bottom - 28 && y - centerY <= bottom - 3)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);//内部隔间(横)
                    }
                    y -= 7;
                }
            }
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if (x - centerX >= -totalWidth + 26 && x - centerX < -75)
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 3)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);//左边内部隔间(竖)
                    }                  
                }
                x += 12;
            }
            for (int x = centerX + totalWidth; x >= centerX - totalWidth; x--)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if (x - centerX > 75 && x - centerX <= totalWidth - 26)
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 3)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BaseLightBlueBlock>(), true, true);//右边内部隔间(竖)
                    }
                }
                x -= 12;
            }




            

            





            // 创建墙
            //NPC住房
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY + totalHeight - 1; y >= centerY - totalHeight; y--)
                {
                    if ((x - centerX >= -totalWidth + 26 && x - centerX < -75) || (x - centerX > 75 && x - centerX <= totalWidth - 26))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 3)
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);//NPC住房粉色墙
                    }
                }
            }

            /*for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                //if (a == 12 || a == 11 || a == 13)
                //break;
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if (b == 7 || b == 6 || b == 8)
                    {
                        WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);//NPC住房淡蓝色墙
                        b = 0;
                    }
                    else
                    {
                        if ((x - centerX >= -totalWidth + 26 && x - centerX < -75) || (x - centerX > 75 && x - centerX <= totalWidth - 26))
                        {
                            if (y - centerY > bottom - 28 && y - centerY <= bottom - 3)
                                WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);//NPC住房粉色墙
                        }
                    }
                    b += 1;
                }
                //a += 1;
            }*/


            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {                   
                    if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 1)
                    {
                        if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 0.5 && y - centerY < 0)
                        {
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseGlassWall>(), true);//中央驾驶台周围玻璃舱盖
                        }
                        WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseEnclosureWall>(), true);//主舱外墙
                    }
                    if (y - centerY >= -slope2 * (x - centerX) + intercept2 && y - centerY >= slope2 * (x - centerX) + intercept2)
                    {
                        if (y - centerY <= bottom - 2 && (x - centerX >= -totalWidth + 2 && x - centerX <= totalWidth - 2))
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseEnclosureWall>(), true);//旋翼外墙
                    }
                    if ((y - centerY < -slope3 * (x - centerX) + intercept3 - 3 && y - centerY < slope3 * (x - centerX) + intercept3 - 3) && y - centerY > bottom - 4 && y - centerY <= bottom2 - 6)
                    {
                        WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseEnclosureWall>(), true);//腹舱外墙
                    }
                    if (y - centerY > bottom2 - 4)
                    {
                        if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY - bottom - 5), 2) / Math.Pow(minorAxis1, 2) <= 1.1)
                        {
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseEnclosureWall>(), true);//底舱外墙
                        }
                    }                   
                }
            }

            //创建平台和通道
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if(x - centerX >= -4 && x - centerX <= 4)
                    {
                        if (y - centerY >= -minorAxis1 - 1 && y - centerY <= bottom2 + 20 )
                        {
                            WorldGen.KillTile(x, y, false, false, true); // 移除现有瓷砖
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseGlassWall>(), true);
                            if ((y - centerY >= -minorAxis1 - 1 && y - centerY <= -minorAxis1) || (y - centerY >= minorAxis1 && y - centerY <= minorAxis1 + 1) || (y - centerY >= minorAxis1 + 11 && y - centerY <= minorAxis1 + 12) || (y - centerY >= bottom2 + 1 && y - centerY <= bottom2 + 2) || (y - centerY >= bottom2 + 15 && y - centerY <= bottom2 + 16) || (y - centerY >= bottom2 + 20 && y - centerY <= bottom2 + 21))
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BasePlatform>(), true, true);
                            }                           
                        }                        
                    }

                    if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 0.9)
                    {
                        if ((y - centerY >= 0 && y - centerY < 1) || (y - centerY >= 12 && y - centerY < 13) || (y - centerY >= -13 && y - centerY < -12))
                        {
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BasePlatform>(), true, true);
                        }
                    }

                    if ((Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) >= 2) && (y - centerY >= -slope2 * (x - centerX) + intercept2 && y - centerY >= slope2 * (x - centerX) + intercept2) && (y - centerY <= bottom - 2))
                    {
                        if(!((x - centerX >= -totalWidth + 26 && x - centerX < -75) || (x - centerX > 75 && x - centerX <= totalWidth - 26) && (y - centerY > bottom - 28 && y - centerY <= bottom - 3)))
                        {
                            if((y - centerY >= bottom - 27 && y - centerY < bottom - 26) || (y - centerY >= bottom - 19 && y - centerY < bottom - 18) || (y - centerY >= bottom - 2 && y - centerY < bottom - 1) || (y - centerY >= bottom - 11 && y - centerY < bottom - 10))
                            {
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BasePlatform>(), true, true);
                            }
                        }
                    }
                }
            }



            // 创建家具
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -40 && x - centerX <= -20) || (x - centerX >= 20 && x - centerX <= 40))
                    {
                        if (y - centerY >= -20 && y - centerY <= 20)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseBookshelf>(), true, true);//书架
                    }
                    y += 7;
                }
                x -= 12;
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -40 && x - centerX <= -10) || (x - centerX >= 10 && x - centerX <= 40))
                    {
                        if (y - centerY >= 0 && y - centerY <= 10)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.NanometerFurnace>(), true, true);//纳米熔炉
                    }
                }
                x -= 16;
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -20 && x - centerX <= 20))
                    {
                        if (y - centerY >= -10 && y - centerY <= 10)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseManufacturer>(), true, true);//工作台
                    }
                }
                x -= 10;
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -100 && x - centerX <= -10) || (x - centerX >= 10 && x - centerX <= 100))
                    {
                        if (y - centerY >= 0 && y - centerY <= 70)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseBed>(), true, true);//床
                    }
                }
                x -= 10;
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -100 && x - centerX <= -10) || (x - centerX >= 10 && x - centerX <= 100))
                    {
                        if (y - centerY >= 0 && y - centerY <= 70)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseBathtub>(), true, true);//浴盆
                    }
                }
                x -= 20;
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -120 && x - centerX <= -90) || (x - centerX >= 90 && x - centerX <= 120))
                    {
                        if (y - centerY >= 0 && y - centerY <= 70)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseChest>(), true, true);//箱子
                    }
                }
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if (x - centerX >= -20 && x - centerX <= 20)
                    {
                        if (y - centerY >= -10 && y - centerY <= 0)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseBlueprint>(), true, true);//箱子
                    }
                }
                x -= 10;
            }
            //主舱（其实是更大范围内,所有的灯光，除了NPC住房的）
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 30)
                    {
                        if (!((x - centerX >= -totalWidth + 26 && x - centerX < -75) || (x - centerX > 75 && x - centerX <= totalWidth - 26) && (y - centerY > bottom - 28 && y - centerY <= bottom - 3)))
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseLantern>(), true, true);//顶灯
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseLamp>(), true, true);//地灯
                    }
                }
                x += 5;
            }
            //NPC住房隔间
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -totalWidth + 26 && x - centerX < -75) || (x - centerX > 75 && x - centerX <= totalWidth - 26))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 3)
                        {
                            if ((y - centerY >= bottom - 22 && y - centerY <= bottom - 20) || (y - centerY >= bottom - 14 && y - centerY <= bottom - 12) || (y - centerY >= bottom - 6 && y - centerY <= bottom - 4))
                            {
                                WorldGen.KillTile(x, y, false, false, true); // 移除现有瓷砖
                                WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseDoorClosed>(), true, true);//NPC住房门                                            
                            }
                        }
                    }
                }
            }
            for (int x = centerX - totalWidth - 7; x <= centerX + totalWidth - 7; x++)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight + 1; y++)//灯要比方块往下一格
                {
                    if ((x - centerX >= -totalWidth + 29 && x - centerX < -75))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseChandelier>(), true, true);//左边灯
                    }
                    y += 7;
                }
                x += 12;
            }
            for (int x = centerX - totalWidth - 6; x <= centerX + totalWidth - 5; x++)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)//桌椅要比方块往下6格
                {
                    if ((x - centerX >= -totalWidth + 29 && x - centerX < -75))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseTable>(), true, true);//左边桌子
                    }
                    y += 7;
                }
                x += 12;
            }
            for (int x = centerX - totalWidth - 3; x <= centerX + totalWidth - 2; x++)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX >= -totalWidth + 29 && x - centerX < -75))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseChair>(), true, true);//左边椅子
                    }
                    y += 7;
                }
                x += 12;
            }

            for (int x = centerX + totalWidth + 5; x >= centerX - totalWidth + 5; x--)
            {
                for (int y = centerY - totalHeight; y <= centerY + totalHeight + 1; y++)
                {
                    if ((x - centerX > 75 && x - centerX <= totalWidth - 29))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseChandelier>(), true, true);//右边灯
                    }
                    y += 7;
                }
                x -= 12;
            }
            for (int x = centerX + totalWidth + 6; x >= 0; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX > 75 && x - centerX <= totalWidth - 29))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseTable>(), true, true);//右边桌子
                    }
                    y += 7;
                }
                x -= 12;
            }
            for (int x = centerX + totalWidth + 9; x >= 3; x--)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if ((x - centerX > 75 && x - centerX <= totalWidth - 29))
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseChair>(), true, true);//右边椅子
                    }
                    y += 7;
                }
                x -= 12;
            }


            




        }




        //下面的函数很奇怪，一开始可以运行，后面无论怎么简化，生成世界时始终会卡住
        private void GenerateWasteBuilding(int centerX, int centerY)//废墟建筑生成逻辑
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next(Main.maxTilesY / 4 - 50, Main.maxTilesY / 4);
            int w = WorldGen.genRand.Next(5, 20);
            int h = WorldGen.genRand.Next(60, 100);
            //创建断壁残垣和废铜烂铁
            for (int a = 0; a <= 300; a++)
            {
                WasteBuilding(x, y, w, h);
                x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                y = WorldGen.genRand.Next(Main.maxTilesY / 4 - 50, Main.maxTilesY / 4);
                w = WorldGen.genRand.Next(5, 20);
                h = WorldGen.genRand.Next(60, 100);
            }
            for (int a = 0; a <= 300; a++)
            {
                WasteMetal(x, y, w, h);
                x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                y = WorldGen.genRand.Next(Main.maxTilesY / 4 - 50, Main.maxTilesY / 4);
                w = WorldGen.genRand.Next(3, 5);
                h = WorldGen.genRand.Next(60, 90);
            }
        }

            private void WasteBuilding(int centerX, int centerY, int width, int height)
        {
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                for (int y = centerY - height / 2; y <= centerY + height / 2; y++)
                {
                    WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<WasteConcrete>(), true, true);
                    WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<WasteConcreteWall>(), true);
                }
            }
        }
        private void WasteMetal(int centerX, int centerY, int width, int height)
        {
            for (int x = centerX - width / 2; x <= centerX + width / 2; x++)
            {
                for (int y = centerY - height / 2; y <= centerY + height / 2; y++)
                {
                    WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<WasteMetal>(), true, true);
                    WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<WasteMetalWall>(), true);
                }
            }
        }

    }
}

/*
 泰拉瑞亚提供了强大的世界生成工具，通过使用 WorldGen 类，你可以生成各种形状和结构。
以下是一些常用的 WorldGen 方法，可以帮助你生成不同的结构：

WorldGen.PlaceTile(int x, int y, int type, bool style = 0, bool frame = true, int? forced = null)
这个方法可以用于在指定的坐标 (x, y) 处放置一个瓷砖。你可以指定瓷砖的类型 (type)，样式 (style)，是否显示边框 (frame) 等。

WorldGen.PlaceWall(int x, int y, int type, bool mute = false)
与 PlaceTile 类似，这个方法用于放置墙壁。

WorldGen.KillTile(int x, int y, bool fail = false, bool effectOnly = false, bool noItem = false)
这个方法用于移除指定坐标 (x, y) 处的瓷砖。你可以选择是否失败 (fail)，只有效果而不实际移除 (effectOnly)，以及是否不掉落物品 (noItem)。

WorldGen.SmoothSlope(int x1, int x2, int y)
这个方法用于在指定的 y 坐标上创建一个平滑的坡道。



WorldGen.TileRunner(int x, int y, double strength, int steps, int type)
这个方法用于创建从 (x, y) 开始的一系列相同类型的瓷砖，形成一个长方形结构，类似于挖掘或填充。
x, y： 这是你想要开始创建平坦结构的坐标。x 表示横坐标，y 表示纵坐标。

strength： 这是平坦结构的强度。它通常是一个小数，控制平坦结构的"光滑程度"。更高的值将导致更平坦的结构。典型的使用值范围在 0.0 到 1.0 之间。

steps： 这是平坦结构生成的步数。步数越多，平坦结构越大。

type： 这是你想要生成的平坦结构的瓦片类型（方块类型）。你应该传递一个瓦片类型的整数值，例如 TileID.Dirt。

这个函数通常用于创建各种地形特征，如平台、平坦区域等。例如，在世界生成或事件触发时，你可能会使用 WorldGen.TileRunner 来创建一个长长的平台。



WorldGen.TileRunner 的变体方法，如 WorldGen.TileRunnerRandom 和 WorldGen.TileRunnerWithYVar，
提供了更多的参数，以便生成不同形状的结构。

WorldGen.TileRunner 方法：
这个方法允许你在世界上生成一个方块的"通道"，用于模拟地下洞穴或其他结构。可以设置通道的宽度、高度和长度。

WorldGen.OreRunner 方法：
用于在地下生成矿物的方法，可以设置矿层的深度、大小和其他参数。

WorldGen.PlaceTile 和 WorldGen.KillTile 方法：
这两个方法允许你在指定位置放置或移除方块。

WorldGen.PlaceWall 和 WorldGen.KillWall 方法：
用于放置或移除墙壁的方法。

WorldGen.PlaceChest 方法：
允许你在世界上放置宝箱。

WorldGen.PlacePump 方法：
允许你放置液体泵。

WorldGen.PlaceStatue 方法：
允许你放置雕像。

WorldGen.TreeGrow 方法：
用于模拟树木生长的方法。

这只是 WorldGen 类的一小部分功能，你可以根据你的需求查看官方文档或探索该类的其他方法。通过这些方法，你可以创建各种各样的地形和结构。
 */
