//using ExampleMod.Content.Biomes;
//using ExampleMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AncientGod.Items.Tiles
{
	public class WasteMetal : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
			DustType = DustID.Copper;
            //DustType = ModContent.DustType<Sparkle>();
            AddMapEntry(new Color(237, 193, 104));
			HitSound = SoundID.Tink;
		}
        public override bool CanDrop(int i, int j)
        {
            if (((Main.rand.Next(1919) * i % (j+114)) + 514 )% 100 > 83) {
				return true;
			}
			return false;
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
			int[] Loot = {ItemID.LeadOre, ItemID.CopperOre, ItemID.IronOre, ItemID.TinOre};// 大概率的普通金属矿掉落： 铅 铜 铁 锡
			int[] preciousLoot = { ItemID.PlatinumOre, ItemID.GoldOre, ItemID.SilverOre};//稀有金属掉落
			int dropItem = 0;
            int loot = (Main.rand.Next(100) + i + j) % 101; //掉落概率
			if (loot <= 97 && loot >= 23) {
				dropItem = Loot[loot%4];
			}
			else if (loot <= 17)
			{
				dropItem = preciousLoot[loot % 3];
			}
			else
			{
				dropItem = ModContent.ItemType<Placeables.WasteMetal>();
			}
			yield return new Item(dropItem);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) {
			//产生Dust的数量，没啥意义，纯粹就是想把传进来的参数全用了，不然不舒服
			if (fail)
			{
				num = 3;
			}
			else
			{
				num = Main.rand.Next(i + j) % 6;
			}
        }

        //public override void ChangeWaterfallStyle(ref int style) {
			//style = ModContent.GetInstance<ExampleWaterfallStyle>().Slot;
		//}
	}
}