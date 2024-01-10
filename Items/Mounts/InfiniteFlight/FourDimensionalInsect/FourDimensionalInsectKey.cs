using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.DataStructures;
using AncientGod.Items.Mounts.InfiniteFlight.BigBangMecha;
using AncientGod.Items.Mounts.InfiniteFlight.AncientMecha;
using AncientGod.Items.Mounts.InfiniteFlight.ModernMecha;
using AncientGod.Items.Mounts.InfiniteFlight.FutureMecha;
using AncientGod.Items.Mounts.InfiniteFlight.EntropySilenceMecha;
using AncientGod.Items.Weapons;

namespace AncientGod.Items.Mounts.InfiniteFlight.FourDimensionalInsect
{
    public class FourDimensionalInsectKey : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = 6;
            Item.UseSound = SoundID.NPCHit56;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.FourDimensionalInsect.FourDimensionalInsectBody>();
            Item.mountType = ModContent.MountType<FourDimensionalInsect>();

            
            Item.buffType = ModContent.BuffType<Buffs.Mounts.FourDimensionalInsectBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
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
                    tooltipLine.OverrideColor = new Color(108, 45, 199); //change the color accordingly to above
                }
            }
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ModContent.ItemType<BigBangMechaKey>(), 1);//加入材料
            recipe.AddIngredient(ModContent.ItemType<AncientMechaKey>(), 1);//加入材料
            recipe.AddIngredient(ModContent.ItemType<ModernMechaKey>(), 1);//加入材料
            recipe.AddIngredient(ModContent.ItemType<FutureMechaKey>(), 1);//加入材料
            recipe.AddIngredient(ModContent.ItemType<EntropySilenceMechaKey>(), 1);//加入材料
            recipe.AddIngredient(ModContent.ItemType<Nanomaterial>(), 9999);//加入材料
            recipe.AddIngredient(ModContent.ItemType<SwordOfAG>(), 1);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }
    }
}
