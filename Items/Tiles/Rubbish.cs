﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using System.Collections.Generic;

namespace AncientGod.Items.Tiles
{
    public class Rubbish : ModTile
    { public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            int[] DustCandidate = { DustID.Sand, DustID.Dirt, DustID.Mud};//尝试一下能否随机生成Dust粒子
            DustType = DustCandidate[Main.rand.Next(3)];
            //DustType = ModContent.DustType<Sparkle>();
            HitSound = SoundID.Dig;
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
            int[] LootCandidate = { ModContent.ItemType<Placeables.Rubbish>(), ItemID.Wood, ItemID.DirtBlock, ItemID.TinOre, ItemID.IronOre, ItemID.Glass, 
                ItemID.GrassSeeds, ItemID.JungleGrassSeeds, ItemID.DaybloomSeeds, ItemID.Acorn};
            int dropItem = 0;
            int randSeed = Main.rand.Next(100+i+j)% LootCandidate.Length;
            if (j <= Main.worldSurface)//地表层以上
            {
                dropItem = LootCandidate[randSeed];
            }
            else
            {
                dropItem = LootCandidate[randSeed%6];
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
    }
}
