using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using AncientGod.Items.Placeables.SpaceBase.Walls;

namespace AncientGod.Items.Tiles.SpaceBase.Walls
{
    public class BaseYellowBlockWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            //Main.wallHouse[Type] = true;//这里也会运行异常
            //ItemDrop = ModContent.ItemType<BaseEnclosureWall>();
            AddMapEntry(new Color(232, 39, 30));
        }
    }
}
