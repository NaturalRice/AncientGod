using AncientGod.Boss.MinionBoss;
using AncientGod.Boss.RunawayMecha;
using AncientGod.Items.Placeables.Furniture;
using AncientGod.Items.Tiles.Furniture;
using AncientGod.Projectiles.Pets.RunawayMecha;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Items.Consumables
{
    public class RunawayMechaSummonItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.

            // If this would be for a vanilla boss that has no summon item, you would have to include this line here:
            // NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

            // Otherwise the UseItem code to spawn it will not work in multiplayer
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            // If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
            // If you want more constraints for the summon item, combine them as boolean expressions:
            //    return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"
            return !NPC.AnyNPCs(ModContent.NPCType<global::AncientGod.Boss.RunawayMecha.RunawayMechaBody>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitly excluded serverside here)
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<global::AncientGod.Boss.RunawayMecha.RunawayMechaBody>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ModContent.ItemType<Materials.Shadow>(), 100);//加入材料
            recipe.AddIngredient(ModContent.ItemType<Materials.MoonDevoration>(), 100);//加入材料
            recipe.AddIngredient(ModContent.ItemType<Nanomaterial>(), 10);//加入材料
            recipe.AddTile(ModContent.TileType<Tiles.Furniture.BaseManufacturer>());//加入合成站：基地工作台
            recipe.Register();
        }

        /*public override void AddRecipes()//测试用,正常玩时注解掉
        {
            Recipe recipe = CreateRecipe();//创建一个配方
            recipe.AddIngredient(ItemID.Torch, 1);//加入材料（1火把）
            recipe.AddTile(TileID.Campfire);//加入合成站（这里为了有趣我改成了篝火）
            recipe.Register();
        }*/
    }
}
