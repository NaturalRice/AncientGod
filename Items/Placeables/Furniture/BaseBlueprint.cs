using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using AncientGod.Items.Tiles.Furniture;
using Terraria.ID;
using Terraria;
using AncientGod.Items.Placeables.SpaceBase;

namespace AncientGod.Items.Placeables.Furniture
{
    public class BaseBlueprint : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Aged Blueprint");
            /* Tooltip
                .SetDefault("Do Not Distribute?"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Furniture.BaseBlueprint>();
            Item.width = 56;
            Item.height = 40;
            Item.rare = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //rarity 12 (Turquoise) = new Color(0, 255, 200)
            //rarity 13 (Pure Green) = new Color(0, 255, 0)
            //rarity 14 (Dark Blue) = new Color(43, 96, 222)
            //rarity 15 (Violet) = new Color(108, 45, 199)
            //rarity 16 (Hot Pink/Developer) = new Color(255, 0, 255)
            //rarity rainbow (no expert tag on item) = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB)
            //rarity rare variant = new Color(255, 140, 0)
            //rarity dedicated(patron items) = new Color(139, 0, 0)
            //look at https://calamitymod.gamepedia.com/Rarity to know where to use the colors
            foreach (TooltipLine tooltipLine in tooltips)
            {
                if (tooltipLine.Mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.OverrideColor = new Color(204, 71, 35); //change the color accordingly to above
                }
            }
        }

        /*public override void AddRecipes()
        {
            Mod CalValEX = ModLoader.GetMod("CalamityMod");
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(ModLoader.GetMod("CalamityMod").ItemType("PowerCell"), 10);
                recipe.AddTile(ModLoader.GetMod("CalamityMod").TileType("LaboratoryConsole"));
                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }*/

        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ModContent.ItemType<BaseEnclosure>(), 10);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
