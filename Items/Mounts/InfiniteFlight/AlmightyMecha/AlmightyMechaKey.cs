using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using AncientGod.Utilities.UI;
using AncientGod.Utilities.UI.MechaChoose;

namespace AncientGod.Items.Mounts.InfiniteFlight.AlmightyMecha
{
    public class AlmightyMechaKey : ModItem
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

            base.SetDefaults();
            HandleSelection(MechaChooseUIState.Choose);
            // 在这里订阅事件
            MechaChooseUIState.ChooseChanged += OnChooseChanged;
        }

        // 增加一个方法来处理选择变化的逻辑
        private void OnChooseChanged(int newChoose)
        {
            HandleSelection(newChoose);
        }

        public void HandleSelection(int choose)//选择机甲类型
        {
            switch (choose)
            {
                case 0:
                    Item.mountType = ModContent.MountType<AlmightyMecha>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaBody>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaBuff>();
                    break;
                case 1:
                    Item.mountType = ModContent.MountType<AlmightyMechaDigger>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDigger>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaDiggerBuff>();
                    break;
                case 2:
                    Item.mountType = ModContent.MountType<AlmightyMechaDetector>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaDetectorBuff>();
                    break;
                case 3:
                    Item.mountType = ModContent.MountType<AlmightyMechaDetector>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaDetectorBuff>();
                    break;
                case 4:
                    Item.mountType = ModContent.MountType<AlmightyMechaDetector>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaDetectorBuff>();
                    break;
                case 5:
                    Item.mountType = ModContent.MountType<AlmightyMechaDetector>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaDetectorBuff>();
                    break;
                case 6:
                    Item.mountType = ModContent.MountType<AlmightyMechaDetector>();
                    Item.shoot = ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>();
                    Item.buffType = ModContent.BuffType<Buffs.Mounts.AlmightyMechaDetectorBuff>();
                    break;
            }
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

        public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddTile(TileID.Campfire);//加入合成站（这里为了有趣我改成了篝火）
            recipe.Register();
        }
    }
}

