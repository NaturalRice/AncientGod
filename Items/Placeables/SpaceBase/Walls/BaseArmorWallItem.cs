﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod;
using AncientGod.Items.Tiles;


namespace AncientGod.Items.Placeables.SpaceBase.Walls
{
    public class BaseArmorWallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.rare = 0;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<Tiles.SpaceBase.Walls.BaseArmorWall>();
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BaseArmor>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 4);//目前不知道如何将结果设置为4个
            recipe.AddRecipe();
        }*/

        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ModContent.ItemType<BaseArmor>(), 1);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
