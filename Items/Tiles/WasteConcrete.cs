using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using System.Collections.Generic;

namespace AncientGod.Items.Tiles
{
    internal class WasteConcrete : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            int[] DustCandidate = { DustID.Stone, DustID.Copper, DustID.Glass, DustID.Sand };//尝试一下能否生成随机的敲击粒子效果
            DustType = DustCandidate[Main.rand.Next(3)];
            //DustType = ModContent.DustType<Sparkle>();
            HitSound = SoundID.Tink;//石头音效
            AddMapEntry(new Color(89, 102, 186));
            TileObjectData.addTile(Type);
        }
        public override bool CanDrop(int i, int j)
        {
            if (((Main.rand.Next(1919) * i % (j + 114)) + 514) % 100 > 75)
            {
                return true;
            }
            return false;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            //大概率掉落石头铁等材料，小概率掉落电线和开关和完整铁锭
            int[] LootCandidate = { ModContent.ItemType<Placeables.WasteConcrete>(), ModContent.ItemType<Materials.Refrigerant>(), ItemID.StoneBlock, ItemID.SandBlock, ItemID.TinOre, ItemID.IronOre, ItemID.Glass, ItemID.CopperOre};
            int[] rareloot = { ItemID.Wire, ItemID.Switch, ItemID.IronBar };
            int randSeed = Main.rand.Next(LootCandidate.Length);
            int randRare = Main.rand.Next(rareloot.Length);
            if (Main.rand.Next(10) == 1)
            {
                yield return new Item(rareloot[randSeed]);
            }
            else {
                yield return new Item(LootCandidate[randSeed]);
            }
            
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
