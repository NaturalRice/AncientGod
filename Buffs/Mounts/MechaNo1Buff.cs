﻿using Terraria;
using Terraria.ModLoader;
using AncientGod.Items.Mounts.InfiniteFlight;

namespace AncientGod.Buffs.Mounts
{
    public class MechaNo1Buff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<MechaNo1>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
