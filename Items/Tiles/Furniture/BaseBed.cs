﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using AncientGod.Items.Tiles.Furniture;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;

namespace AncientGod.Items.Tiles.Furniture
{
    public class BaseBed : ModTile//床的设计似乎改不了比例，在设计个新图，现有的图换另一个家具
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.Width = 4;
            Terraria.ID.TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileID.Sets.CanBeSleptIn[Type] = true; // Facilitates calling ModifySleepingTargetInfo
            TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
            TileObjectData.addTile(Type);//这里每次运行时都出错，不知为什么
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Phantowax Bed");
            AddMapEntry(new Color(94, 39, 93), name);

            AdjTiles = new int[] { TileID.Beds };

        }

        public override bool HasSmartInteract(int i, int j, Terraria.GameContent.ObjectInteractions.SmartInteractScanSettings settings) => true;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }
        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            info.VisualOffset.Y -= 2f;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = (i - (tile.TileFrameX / 18)) + (tile.TileFrameX >= 72 ? 5 : 2);
            int spawnY = j + 2;

            if (tile.TileFrameY % 38 != 0)
            {
                spawnY--;
            }

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, i, j);
                }
            }
            else
            {
                player.FindSpawn();

                if (player.SpawnX == spawnX && player.SpawnY == spawnY)
                {
                    player.RemoveSpawn();
                    Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
                }
                else if (Player.CheckSpawn(spawnX, spawnY))
                {
                    player.ChangeSpawn(spawnX, spawnY);
                    Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
                }
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ItemID.SleepingIcon;
                }
            }
            else
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<Placeables.Furniture.BaseBed>();
            }
        }
    }
}
