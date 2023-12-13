using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;
using AncientGod.System;
using AncientGod.Biomes;

namespace AncientGod.Biomes
{
    public class SpaceBase : ModBiome
    {
        // Select all the scenery
        //public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // Sets a water style for when inside this biome
        //SurfaceBackgroundStyle 属性定义了生态群系表面的背景样式。在这里，使用了 SpaceBaseStyle，它是一个实现了 ModSurfaceBackgroundStyle 接口的类的实例。
        //这个类可能包含了定义生态群系表面背景的细节。
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<SpaceBaseStyle>();
        //TileColorStyle 属性定义了捕捉到的图像中瓦片的颜色风格。在这里，选择了 CaptureBiome.TileColorStyle.Crimson。这可能影响在捕捉屏幕截图时，生态群系中瓦片的颜色。
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

        // Select Music
        //public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

        //public override int BiomeTorchItemType => ModContent.ItemType<ExampleTorch>();
        //public override int BiomeCampfireItemType => ModContent.ItemType<ExampleCampfire>();

        // Populate the Bestiary Filter
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        // Calculate when the biome is active.
        /*IsBiomeActive 方法确定生态群系是否处于活跃状态。在这里，定义了三个条件：
		b1: 通过 RubbleTileCounts 实例的 RubbleTileCount 属性，检查是否有足够的瓦片（这可能是生态群系中的某种资源或元素）。
		b2: 检查玩家的位置是否在地图的水平中心的一半范围内，这可能是为了确保生态群系在地图的特定区域内活跃。
		b3: 检查玩家是否在天空高度或地表高度上。这可能是为了确保生态群系主要活跃在地表和天空。*/
        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<BaseTileCounts>().BaseTileCount >= 50;

            return b1;
        }

        // Declare biome priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
        //Priority 属性用于声明生态群系的优先级。在这里，选择了 SceneEffectPriority.BiomeHigh，表示生态群系的优先级较高。这可能影响在多个生态群系重叠时的渲染顺序。
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    }
}
