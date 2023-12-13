using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using AncientGod.Items.Placeables.SpaceBase.Walls;

namespace AncientGod.Items.Tiles
{
    public class RubbishWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            //Main.wallHouse[Type] = true;这里也会运行异常
            //ItemDrop = ModContent.ItemType<RubbishWall>();
            AddMapEntry(new Color(232, 39, 30));
        }
    }
}
