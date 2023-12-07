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
                int houseY = Main.maxTilesY / 10;

                AncientGod.getlog().Info($"Generating house at: {houseX}, {houseY}");
                GenerateHouse(houseX, houseY);
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
                    if ((y - centerY < -slope2 * (x - centerX) + intercept2 - 10 || y - centerY < slope2 * (x - centerX) + intercept2 - 10) && Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) > 2)
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
                    }
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




            // 创建家具
            //主舱
            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y  = centerY - totalHeight; y <= centerY + totalHeight; y++)
                {
                    if (Math.Pow((x - centerX), 2) / Math.Pow(majorAxis1, 2) + Math.Pow((y - centerY), 2) / Math.Pow(minorAxis1, 2) <= 0.9)
                    {
                        WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseLantern>(), true, true);//灯
                    }
                }
            }
            //NPC住房隔间
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

            for (int x = centerX - totalWidth; x <= centerX + totalWidth; x++)
            {
                for (int y = centerY - totalHeight + 6; y <= centerY + totalHeight + 7; y++)
                {
                    if (x - centerX >= -totalWidth + 26 && x - centerX < -75)
                    {
                        if (y - centerY > bottom - 28 && y - centerY <= bottom - 4)
                            WorldGen.PlaceTile(x, y, (ushort)ModContent.TileType<Items.Tiles.Furniture.BaseDoorClosed>(), true, true);//NPC住房门
                    }
                    y += 7;
                }
                x += 12;
            }




            // 创建墙
            int a = 0;//用于分割墙壁图案X轴的变量
            int b = 0;//用于分割墙壁图案Y轴的变量

            for (int x = centerX - totalWidth + 26; x <= centerX + totalWidth - 26; x++)
            {
                
                for (int y = centerY + bottom - 27; y <= centerY + bottom - 3; y++)
                {
                    if (b == 7 || b == 6 || b == 8)
                    {
                        if (a == 12 || a == 11 || a == 13)
                        {
                            WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BaseLightBlueBlockWall>(), true);//NPC住房淡蓝色墙
                        }
                        if (a == 13)
                            a = 1;
                        if (b == 8)
                            b = 1;
                    }
                    else
                    {
                        WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<Items.Tiles.SpaceBase.Walls.BasePinkBlockWall>(), true);//NPC住房粉色墙
                    }                                                                            
                    b += 1;
                }
                a += 1;
            }
            a = 0;
            b = 0;
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

            // 创建屋顶
            /*for (int x = centerX; x < centerX + buildingWidth; x++)
            {
                int roofY = centerY;
                if (roofY >= 0 && roofY < Main.maxTilesY)
                {
                    WorldGen.PlaceTile(x, roofY, (ushort)ModContent.TileType<Items.Tiles.SpaceBase.BasePanel>(), true, true);
                }
            }*/

            // Create a door
            /*int doorY = centerY + 1;
            if (doorY >= 0 && doorY < 3)
            {
                WorldGen.KillTile(centerX, doorY, false, false, true); // 移除现有瓷砖
            }*/
        }
    }
}
