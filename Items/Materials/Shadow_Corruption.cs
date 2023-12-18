﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.Items.Materials;
using AncientGod.System;
namespace AncientGod.Items.Materials
{
    internal class Shadow_Corruption : ModItem
    {
        //public override void OnConsumeItem(Player player)
        //{
        //    if (!GameProgress.FirstShadow)
        //    {
        //        GameProgress.FirstShadow = true;
        //    }
        //}
        public override void SetDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Item.maxStack = 9999;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Orange;
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Shadow_Corruption>());
        }
        public override void AddRecipes()
        {
            Recipe new_recipe = CreateRecipe();
            new_recipe.AddIngredient(ItemID.Ebonwood, 20);
            new_recipe.AddIngredient(ItemID.VileMushroom, 10);
            new_recipe.AddIngredient(ItemID.CorruptSeeds, 1);
            new_recipe.AddIngredient<MoonDevoration>(30);
            new_recipe.AddIngredient<Shadow>(2);
            new_recipe.AddTile(TileID.WorkBenches);
            new_recipe.Register();
        }
    }
}
