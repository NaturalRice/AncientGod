﻿using Terraria;
using Terraria.ModLoader;
using AncientGod.Items.Mounts.InfiniteFlight.AlmightyMecha;

namespace AncientGod.Buffs.Mounts
{
    public class AlmightyMechaDetectorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<AlmightyMechaDetector>(), player);
            player.buffTime[buffIndex] = 18000;

            player.GetModPlayer<AncientGodPlayer>().AlmightyMechaDetector = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<Projectiles.Mounts.InfiniteFlight.AlmightyMecha.AlmightyMechaDetector>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
