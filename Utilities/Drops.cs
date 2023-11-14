﻿using AncientGod.Items;
/*using AncientGod.Items.Critters;
using AncientGod.Items.Equips.Backs;
using AncientGod.Items.Equips.Balloons;
using AncientGod.Items.Equips.Capes;
using AncientGod.Items.Equips.Hats;
using AncientGod.Items.Equips.Legs;
using AncientGod.Items.Equips.Scarves;
using AncientGod.Items.Equips.Shields;
using AncientGod.Items.Equips.Shirts;
using AncientGod.Items.Equips.Transformations;
using AncientGod.Items.Equips.Wings;
using AncientGod.Items.Hooks;
using AncientGod.Items.LightPets;*/
using AncientGod.Items.Mounts;
using AncientGod.Items.Mounts.InfiniteFlight;
/*using AncientGod.Items.Mounts.Ground;
using AncientGod.Items.Mounts.LimitedFlight;
using AncientGod.Items.Mounts.Morshu;
using AncientGod.Items.Pets;
using AncientGod.Items.Pets.Scuttlers;
using AncientGod.Items.Pets.Elementals;
using AncientGod.Items.Pets.ExoMechs;*/
using AncientGod.Items.Tiles;
/*using AncientGod.Items.Tiles.Balloons;
using AncientGod.Items.Tiles.Blocks;
using AncientGod.Items.Tiles.Plushies;
using AncientGod.NPCs.Critters;
using AncientGod.Items.Tiles.Paintings;
using AncientGod.Items.Tiles.Plants;
using AncientGod.NPCs.Oracle;*/
using System;
using Terraria.GameContent.ItemDropRules;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
//using AncientGod.AprilFools;
using Terraria.DataStructures;
using Terraria.Localization;
using System.Security.Policy;
/*using AncientGod.NPCs.JellyPriest;
using AncientGod.CalamityID;*/
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;
using AncientGod.Utilities;

namespace AncientGod
{
    public class AncientGodGlobalNPC : GlobalNPC
    {
        public bool bdogeMount;
        public bool geldonSummon;
        public bool junkoReference;
        public bool wolfram;

        public static int meldodon = -1;
        public static int RunawayTank = -1;

        public override bool InstancePerEntity => true;

        public override void ModifyShop(NPCShop shop)
        {
            Mod alchLite;
            ModLoader.TryGetMod("AlchemistNPCLite", out alchLite);
            Mod alchFull;
            ModLoader.TryGetMod("AlchemistNPC", out alchFull);
            int type = shop.NpcType;

            /*if (alchLite != null)
            {
                if (type == alchLite.Find<ModNPC>("Musician").Type)
                {
                    shop.Add(ModContent.ItemType<AstralMusicBox>(), AncientGodConditions.oreo);
                }
            }
            if (alchFull != null)
            {
                if (type == alchFull.Find<ModNPC>("Musician").Type)
                {
                    shop.Add(ModContent.ItemType<AstralMusicBox>(), AncientGodConditions.oreo);
                }
            }*/
            /*if (AncientGod.CalamityActive)
            {
                if (type == AncientGod.Calamity.Find<ModNPC>("SEAHOE").Type)
                {
                    shop.Add(ModContent.ItemType<BloodwormScarf>(), AncientGodConditions.boomer)
                        .Add(ModContent.ItemType<Yharlamitas>(), AncientGodConditions.scal);
                }
                if (type == AncientGod.Calamity.Find<ModNPC>("DILF").Type)
                {
                    shop.Add(ModContent.ItemType<FrostflakeBrick>())
                        .Add(ModContent.ItemType<Signut>(), AncientGodConditions.siggy);
                }
                if (type == AncientGod.Calamity.Find<ModNPC>("THIEF").Type)
                {
                    shop.Add(ModContent.ItemType<AureicFedora>(), AncientGodConditions.oreo)
                        .Add(ModContent.ItemType<AstrachnidCranium>(), AncientGodConditions.oreo)
                        .Add(ModContent.ItemType<AstrachnidTentacles>(), AncientGodConditions.oreo)
                        .Add(ModContent.ItemType<AstrachnidThorax>(), AncientGodConditions.oreo);
                }
                if (type == AncientGod.Calamity.Find<ModNPC>("FAP").Type)
                {
                    shop.Add(ModContent.ItemType<OddMushroomPot>(), AncientGodConditions.oreo);
                }
            }*/
            /*if (type == NPCID.Steampunker)
            {
                shop.Add(ModContent.ItemType<XenoSolution>(), Condition.Hardmode)
                    .Add(ModContent.ItemType<StarstruckSynthesizer>(), Condition.Hardmode);
            }
            if (type == NPCID.Dryad)
            {
                shop.Add(ModContent.ItemType<AstralGrass>(), Condition.Hardmode);
            }
            if (type == NPCID.Truffle)
            {
                shop.Add(ModContent.ItemType<SwearshroomItem>())
                    .Add(ModContent.ItemType<ShroomiteVisage>(), Condition.DownedPlantera);
            }
            if (type == NPCID.Clothier)
            {
                shop.Add(ModContent.ItemType<PolterMask>(), AncientGodConditions.dungeon)
                    .Add(ModContent.ItemType<Polterskirt>(), AncientGodConditions.dungeon)
                    .Add(ModContent.ItemType<PolterStockings>(), AncientGodConditions.dungeon)
                    .Add(ModContent.ItemType<BanditHat>(), AncientGodConditions.bandit)
                    .Add(ModContent.ItemType<Permascarf>(), AncientGodConditions.perma);
            }
            if (type == NPCID.PartyGirl)
            {
                shop.Add(ModContent.ItemType<TiedMirageBalloon>(), Condition.PlayerCarriesItem(ModContent.ItemType<Mirballoon>()))
                    .Add(ModContent.ItemType<TiedBoxBalloon>(), Condition.PlayerCarriesItem(ModContent.ItemType<BoxBalloon>()))
                    .Add(ModContent.ItemType<TiedChaosBalloon>(), Condition.PlayerCarriesItem(ModContent.ItemType<ChaosBalloon>()))
                    .Add(ModContent.ItemType<TiedBoB2>(), Condition.PlayerCarriesItem(ModContent.ItemType<BoB2>()))
                    .Add(ModContent.ItemType<ChaoticPuffball>(), AncientGodConditions.polt);
            }*/
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            /*if (npc.type == ModContent.NPCType<OracleNPC>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OracleBeanie>(), 1));
            }*/
            /*if (!AncientGodConfig.Instance.DisableVanityDrops)
            {
                if (AncientGod.CalamityActive)
                {
                    if (npc.type == AncientGod.CalamityNPC("DILF"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Permascarf>(), 1));
                    }
                    if (npc.type == AncientGod.CalamityNPC("THIEF"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BanditHat>(), 1));
                    }
                    if (npc.type == AncientGod.CalamityNPC("BoxJellyfish"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoxBalloon>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Rimehound"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TundraBall>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Rotdog"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RottenHotdog>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("PrismBack"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrismShell>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Toxicatfish"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DecayingFishtail>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("DespairStone"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DespairMask>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Scryllar"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScryllianWings>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ScryllarRage"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScryllianWings>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("RepairUnitCritter"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DisrepairUnit>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("WulfrumAmplifier"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumTransmitter>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumKeys>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumHelipack>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("WulfrumGyrator"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumController>(), 100));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumBalloon>(), 100));
                    }
                    if (npc.type == AncientGod.CalamityNPC("WulfrumRover"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumController>(), 100));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RoverSpindle>(), 1000));
                    }
                    if (npc.type == AncientGod.CalamityNPC("WulfrumHovercraft"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumController>(), 100));
                    }
                    if (npc.type == AncientGod.CalamityNPC("WulfrumDrone"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WulfrumController>(), 100));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Sunskater"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EssenceofYeet>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerAmethyst"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AmethystGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerSapphire"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SapphireGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerTopaz"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TopazGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerEmerald"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EmeraldGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerRuby"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RubyGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerDiamond"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DiamondGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerAmber"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AmberGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CrawlerCrystal"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalGeode>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ShockstormShuttle"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShuttleBalloon>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("SulphurousSkater"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AcidLamp>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("AeroSlime"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AeroWings>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("SeaFloaty"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FloatyCarpetItem>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Orthocera"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Help>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Trilobite"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TrilobiteShield>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Bohldohr"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Eggstone>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("FlakCrab"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlakHeadCrab>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("GammaSlime"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GammaHelmet>(), 30));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NuclearFumes>(), 5));
                    }
                    if (npc.type == AncientGod.CalamityNPC("PerennialSlime"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PerennialFlower>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PerennialDress>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientPerennialFlower>(), 30));
                    }
                    if (npc.type == AncientGod.CalamityNPC("SightseerCollider"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AstralBinoculars>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("HeatSpirit"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EssenceofDisorder>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("BelchingCoral"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CoralMask>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("AnthozoanCrab"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrackedFossil>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Cryon"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cryocap>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cryocoat>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("RenegadeWarlock"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CultistHood>(), 30));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CultistRobe>(), 30));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CultistLegs>(), 30));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ImpiousImmolator"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedChewToy>(), 40));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HolyTorch>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Cnidrion"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SunDriedShrimp>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("EidolonWyrmHead"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CanofWyrms>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("OverloadedSoldier"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnloadedHelm>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HauntedPebble>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("DevilFish"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DevilfishMask2>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DevilfishMask3>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("DevilFishAlt"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DevilfishMask1>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DevilfishMask3>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("MirageJelly"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Mirballoon>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OldMirage>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Hadarian"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HadarianTail>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("AstralSlime"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AstraEGGeldon>(), 87000));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Eidolist"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EidoMask>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Eidcape>(), 10));
                    }
                    if (npc.type == NPCID.SandElemental)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmallSandPail>(), 5));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmallSandPlushie>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SandyBangles>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ProfanedEnergyBody"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedChewToy>(), 40));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedEnergyHook>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedBalloon>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ScornEater"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedChewToy>(), 40));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScornEaterMask>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ChaoticPuffer"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ChaosBalloon>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ReaperShark"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ReaperSharkArms>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OmegaBlue>(), 40));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ReaperoidPills>(), 10));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ColossalSquid"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SquidHat>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OmegaBlue>(), 40));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Horse"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EarthShield>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EarthenHelmet>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EarthenBreastplate>(), 20));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EarthenLeggings>(), 20));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ArmoredDiggerHead"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 10000));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConstructionRemote>(), 4));
                    }
                    if (npc.type == AncientGod.CalamityNPC("PlaguebringerMiniboss"))
                    {
                        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlaguebringerPowerCell>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlaugeWings>(), 15));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 10000));
                    }
                    if (npc.type == AncientGod.CalamityNPC("Mauler"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NuclearFumes>(), 1, 10, 25));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BubbledFin>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), ModContent.ItemType<MaulerPlush>(), 4));
                    }
                    if (npc.type == AncientGod.CalamityNPC("NuclearTerror"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NuclearFumes>(), 1, 10, 25));
                        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), ModContent.ItemType<NuclearTerrorPlush>(), 4));
                    }
                    if (npc.type == CalNPCID.GiantClam)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ClamHermitMedallion>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ClamMask>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), ModContent.ItemType<GiantClamPlush>(), 4));
                    }
                    if (npc.type == AncientGod.CalamityNPC("ThiccWaifu"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CloudCandy>(), 10));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CloudWaistbelt>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new RunawayMechaCondition(), ModContent.ItemType<PurifiedFog>(), 20));
                        npcLoot.Add(ItemDropRule.ByCondition(new RunawayMechaCondition2(), ModContent.ItemType<PurifiedFog>(), 999999));
                    }
                    if (npc.type == AncientGod.CalamityNPC("CragmawMire"))
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MawHook>(), 1));
                        npcLoot.Add(ItemDropRule.ByCondition(new PolterDowned(), ModContent.ItemType<NuclearFumes>(), 1, 5, 8));
                        npcLoot.Add(ItemDropRule.ByCondition(new Polteralive(), ModContent.ItemType<MirePlushP1>(), 4));
                        npcLoot.Add(ItemDropRule.ByCondition(new Polterdead(), ModContent.ItemType<MirePlushP1>(), 8));
                        npcLoot.Add(ItemDropRule.ByCondition(new Polterdead(), ModContent.ItemType<MirePlushP2>(), 8));
                    }
                    if (npc.type == AncientGod.CalamityNPC("GreatSandShark"))
                    {
                        AddPlushDrop(npcLoot, ModContent.ItemType<SandSharkPlush>());
                    }
                    if (npc.type == ModContent.NPCType<Xerocodile>())
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new YharonDowned(), ModContent.ItemType<Termipebbles>()));
                    }
                    if (npc.type == ModContent.NPCType<XerocodileSwim>())
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new YharonDowned(), ModContent.ItemType<Termipebbles>()));
                    }
                    //Scourge
                    if (npc.type == CalNPCID.DesertScourge)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DesertMedallion>(), 5));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SlightlyMoistbutalsoSlightlyDryLocket>(), 7));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DriedLocket>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<DesertScourgePlush>());
                    }
                    //Crabulon
                    if (npc.type == CalNPCID.Crabulon)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ClawShroom>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<CrabulonPlush>());
                    }
                    //Perfs
                    if (npc.type == CalNPCID.Perforators)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SmallWorm>(), 7));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MidWorm>(), 7));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BigWorm>(), 7));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MeatyWormTumor>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<PerforatorPlush>());
                    }
                    //Hive Mind
                    if (npc.type == CalNPCID.HiveMind)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<RottenKey>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<HiveMindPlush>());
                    }
                    //Slime Gods
                    if (npc.type == CalNPCID.SlimeGod)
                    {
                        AddBlockDrop(npcLoot, AncientGod.CalamityItem("StatigelBlock"));
                        AddPlushDrop(npcLoot, ModContent.ItemType<SlimeGodPlush>());
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<IonizedJellyCrystal>(), 50));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SlimeGodMask>(), 7));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SlimeDeitysSoul>(), 3));
                    }
                    //Cryogen
                    if (npc.type == CalNPCID.Cryogen)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<CoolShades>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<CryogenPlush>());
                    }
                    //Aqua
                    if (npc.type == CalNPCID.AquaticScourge)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MoistLocket>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BleachBallItem>(), 4));
                        AddPlushDrop(npcLoot, ModContent.ItemType<AquaticScourgePlush>());
                    }
                    //Brimmy
                    if (npc.type == CalNPCID.BrimstoneElemental)
                    {
                        AddBlockDrop(npcLoot, AncientGod.CalamityItem("BrimstoneSlag"));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BrimmySpirit>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BrimmyBody>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<FoilSpoon>(), 20));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<RareBrimtulip>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<BrimstoneElementalPlush>());
                    }
                    //Clone
                    if (npc.type == CalNPCID.CalamitasClone)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Calacirclet>(), 5));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 10000));
                        AddPlushDrop(npcLoot, ModContent.ItemType<ClonePlush>());
                    }
                    //Leviathan
                    if (npc.type == CalNPCID.Leviathan)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Levihita(), ModContent.ItemType<FoilAtlantis>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new Levihita(), ModContent.ItemType<StrangeMusicNote>(), 40));
                        AddPlushDrop(npcLoot, ModContent.ItemType<LeviathanPlush>());
                        npcLoot.Add(ItemDropRule.ByCondition(new LevihitaPlushies(), ModContent.ItemType<AnahitaPlush>(), 20));
                    }
                    if (npc.type == CalNPCID.Anahita)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Levihita(), ModContent.ItemType<FoilAtlantis>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new Levihita(), ModContent.ItemType<StrangeMusicNote>(), 40));
                        AddPlushDrop(npcLoot, ModContent.ItemType<AnahitaPlush>());
                        npcLoot.Add(ItemDropRule.ByCondition(new LevihitaPlushies(), ModContent.ItemType<LeviathanPlush>(), 20));
                    }
                    //Astrum Aureus
                    if (npc.type == CalNPCID.AstrumAureus)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AureusShield>(), 5));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AstralInfectedIcosahedron>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), ModContent.ItemType<SpaceJunk>()));
                        AddPlushDrop(npcLoot, ModContent.ItemType<AstrumAureusPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 10000));
                        npcLoot.Add(ItemDropRule.ByCondition(new GeldonDrop(), ModContent.ItemType<SpaceJunk>()));
                    }
                    //PBG
                    if (npc.type == CalNPCID.PlaguebringerGoliath)
                    {
                        AddBlockDrop(npcLoot, AncientGod.CalamityItem("PlaguedContainmentBrick"));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<InfectedController>(), 5));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<PlaguePack>(), 5));
                        AddPlushDrop(npcLoot, ModContent.ItemType<PlaguebringerGoliathPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 1000));
                    }
                    //Ravager
                    if (npc.type == CalNPCID.Ravager)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, new int[]{
                        ModContent.ItemType<SkullBalloon>(),
                        ModContent.ItemType<StonePile>(),
                        ModContent.ItemType<RavaHook>(),
                        ModContent.ItemType<SkullCluster>() }));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ScavaHook>(), 15));
                        AddBlockDrop(npcLoot, ModContent.ItemType<Necrostone>());
                        AddPlushDrop(npcLoot, ModContent.ItemType<RavagerPlush>());
                    }
                    //Deus
                    //PS: FUCK deus lootcode, I pray to anyone who wants to make weakref support for this abomination _ YuH 2022
                    if (npc.type == CalNPCID.AstrumDeus)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                        npcLoot.Add(ItemDropRule.ByCondition(new DeusFUCKBlight(), ModContent.ItemType<AstrumDeusMask>()));
                        npcLoot.Add(ItemDropRule.ByCondition(new DeusFUCKMasorev(), ModContent.ItemType<AstrumDeusPlush>(), 4));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new DeusFUCK(), ModContent.ItemType<AstBandana>(), 4));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new DeusFUCK(), ModContent.ItemType<Geminga>(), 3));
                    }
                    //Bumblebirb
                    if (npc.type == CalNPCID.Bumblebirb)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, new int[]{
                        ModContent.ItemType<Birbhat>(),
                        ModContent.ItemType<FollyWings>(),
                        ModContent.ItemType<DocilePheromones>()}));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new SilvaCrystal(), AncientGod.CalamityItem("SilvaCrystal"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), ModContent.ItemType<ExtraFluffyFeather>()));
                        AddPlushDrop(npcLoot, ModContent.ItemType<BumblefuckPlush>()); ;
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 500));
                        npcLoot.Add(ItemDropRule.ByCondition(new DogeDrop(), ModContent.ItemType<ExtraFluffyFeather>()));
                    }
                    //Providence
                    if (npc.type == CalNPCID.Providence)
                    {
                        AddBlockDrop(npcLoot, AncientGod.CalamityItem("ProfanedRock"));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ProviCrystal>(), 4));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ProfanedHeart>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<ProvidencePlush>());
                    }
                    //Storm Weaver
                    if (npc.type == CalNPCID.StormWeaver)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new OtherworldlyStoneDrop(), AncientGod.CalamityItem("OtherworldlyStone"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<StormBandana>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ArmoredScrap>(), 6));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<StormMedal>(), 6));
                        AddPlushDrop(npcLoot, ModContent.ItemType<StormWeaverPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 250));
                    }
                    //Signus
                    if (npc.type == CalNPCID.Signus)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, new int[]{
                        ModContent.ItemType<SignusBalloon>(),
                        ModContent.ItemType<SigCape>(),
                        ModContent.ItemType<SignusEmblem>(),
                        ModContent.ItemType<ShadowCloth>(),
                        ModContent.ItemType<SignusNether>()}));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new OtherworldlyStoneDrop(), AncientGod.CalamityItem("OtherworldlyStone"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new JunkoDrop(), ModContent.ItemType<SuspiciousLookingChineseCrown>()));
                        AddPlushDrop(npcLoot, ModContent.ItemType<SignusPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 250));
                    }
                    //CV
                    if (npc.type == CalNPCID.CeaselessVoid)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new OtherworldlyStoneDrop(), AncientGod.CalamityItem("OtherworldlyStone"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<VoidWings>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<OldVoidWings>(), 15));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MirrorMatter>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<CeaselessVoidPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 250));
                    }
                    //Polterghast
                    if (npc.type == CalNPCID.Polterghast)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new BlockDrops(), AncientGod.CalamityItem("StratusBricks"), 2, 155, 265));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new BlockDrops(), ModContent.ItemType<PhantowaxBlock>(), 2, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Polterhook>(), 20));
                        npcLoot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), ModContent.ItemType<ToyScythe>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<PolterghastPlush>());
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ZygoteinaBucket>(), 3));
                    }
                    //Old Duke
                    if (npc.type == CalNPCID.OldDuke)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<OldWings>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<CorrodedCleaver>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<CharredChopper>(), 6));
                        AddPlushDrop(npcLoot, ModContent.ItemType<OldDukePlush>());
                    }
                    //DoG
                    if (npc.type == CalNPCID.DevourerofGods)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<CosmicWormScarf>(), 5));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<RapturedWormScarf>(), 20));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<CosmicRapture>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<DevourerofGodsPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 100));
                    }
                    //Yharon
                    if (npc.type == CalNPCID.Yharon)
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<YharonShackle>(), 3));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<JunglePhoenixWings>(), 5));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<YharonsAnklet>(), 10));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<NuggetinaBiscuit>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<YharonPlush>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 20));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Termipebbles>(), 1, 3, 8));
                        npcLoot.Add(ItemDropRule.ByCondition(new RoverDrop(), ModContent.ItemType<RoverSpindle>()));
                    }
                    //Supreme Cal
                    if (npc.type == CalNPCID.SupremeCalamitas)
                    {
                        AddBlockDrop(npcLoot, AncientGod.CalamityItem("OccultBrickItem"));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GruelingMask>(), 3));
                        AddPlushDrop(npcLoot, ModContent.ItemType<CalamitasFumo>());
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AncientAuricTeslaHelm>(), 10));
                    }
                    //Ares
                    if (npc.type == CalNPCID.Ares)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<Items.Equips.Shirts.AresChestplate.AresChestplate>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<DraedonBody>(), 5));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<DraedonLegs>(), 5));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<OminousCore>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<AncientAuricTeslaHelm>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new ExoPlating(), AncientGod.CalamityItem("ExoPlating"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<AresPlush>()));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<DraedonPlush>(), 10));
                    }
                    //Thanatos
                    if (npc.type == CalNPCID.Thanatos)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<XMLightningHook>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<DraedonBody>(), 5));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<DraedonLegs>(), 5));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<GunmetalRemote>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<AncientAuricTeslaHelm>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new ExoPlating(), AncientGod.CalamityItem("ExoPlating"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<ThanatosPlush>()));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<DraedonPlush>(), 10));
                    }
                    //Apollo
                    if (npc.type == CalNPCID.Apollo)
                    {
                        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<ArtemisBalloonSmall>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<ApolloBalloonSmall>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<DraedonBody>(), 5));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<DraedonLegs>(), 5));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<GeminiMarkImplants>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new Exodrop(), ModContent.ItemType<AncientAuricTeslaHelm>(), 3));
                        notExpertRule.OnSuccess(ItemDropRule.ByCondition(new ExoPlating(), AncientGod.CalamityItem("ExoPlating"), 1, 155, 265));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<ArtemisPlush>()));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<ApolloPlush>()));
                        npcLoot.Add(ItemDropRule.ByCondition(new ExoPlush(), ModContent.ItemType<DraedonPlush>(), 10));
                    }
                    //Wyrm
                    if (npc.type == CalNPCID.PrimordialWyrm)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Tiles.RespirationShrine>()));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulShard>()));
                        AddPlushDrop(npcLoot, ModContent.ItemType<JaredPlush>());
                    }
                    //Donuts
                    if (npc.type == CalNPCID.GuardianCommander)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedWheels>(), 3));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedCultistMask>(), 5));
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedCultistRobes>(), 5));
                        AddPlushDrop(npcLoot, ModContent.ItemType<ProfanedGuardianPlush>());
                    }
                    if (npc.type == CalNPCID.GuardianDefender)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedFrame>(), 3));
                    }
                    if (npc.type == CalNPCID.GuardianHealer)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProfanedBattery>(), 3));
                    }
                }
            }*/
            //Meldosaurus

            /*if (npc.type == ModContent.NPCType<AprilFools.Meldosaurus.Meldosaurus>())
            {
                LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, new int[]{
                        ModContent.ItemType<ShadesBane>(),
                        ModContent.ItemType<Nyanthrop>()}));


                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AprilFools.Meldosaurus.MeldosaurusTrophy>(), 10));
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AprilFools.Meldosaurus.MeldosaurusMask>(), 7));
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<AprilFools.Meldosaurus.MeldosaurusBag>()));
                
                npcLoot.Add(ItemDropRule.ByCondition(new MeldosaurusDowned(), ModContent.ItemType<AprilFools.Meldosaurus.KnowledgeMeldosaurus>()));
            }
            //RunawayMecha
            if (npc.type == ModContent.NPCType<AprilFools.RunawayMecha>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PurifiedFog>(), 1));                
            }
            Mod Goozmod;
            Mod CatalystMod;
            Mod Hypnos;
            ModLoader.TryGetMod("CalamityHunt", out Goozmod);
            ModLoader.TryGetMod("CatalystMod", out CatalystMod);
            ModLoader.TryGetMod("Hypnos", out Hypnos);
            if (Hypnos != null)
            {
                if (npc.type == Hypnos.Find<ModNPC>("HypnosBoss").Type)
                {
                    AddPlushDrop(npcLoot, ModContent.ItemType<HypnosPlush>());
                }
            }
            if (CatalystMod != null)
            {
                if (npc.type == CatalystMod.Find<ModNPC>("Astrageldon").Type)
                {
                    AddPlushDrop(npcLoot, ModContent.ItemType<AstrageldonPlush>());
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SpaceJunk>(), 3));
                }
            }
            if (Goozmod != null)
            {
                if (npc.type == Goozmod.Find<ModNPC>("Goozma").Type)
                {
                    AddPlushDrop(npcLoot, ModContent.ItemType<GoozmaPlush>());
                }
            }*/

            //Yharexs' Dev Pet (Calamity BABY)
            /*if (AncientGod.CalamityActive && (bool)AncientGod.Calamity.Call("GetDifficultyActive", "death"))
            {
                if (npc.type == AncientGod.CalamityNPC("SupremeCalamitas"))
                {
                    bool didIGetHit = false;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player player = Main.player[i];
                        if (player.active && !player.dead)
                        {
                            if (player.GetModPlayer<AncientGodPlayer>().SCalHits > 0)
                            {
                                didIGetHit = true;
                            }
                        }
                    }

                    if (!didIGetHit)
                    {
                        Item.NewItem(npc.GetSource_FromAI(), npc.getRect(), ModContent.ItemType<AstraEGGeldon>());
                    }
                    else
                    {
                        if (Main.rand.Next(1000) == 0)
                        {
                            Item.NewItem(npc.GetSource_FromAI(), npc.getRect(), ModContent.ItemType<AstraEGGeldon>());
                        }
                    }
                }
            }*/
        }
        public static void AddPlushDrop(NPCLoot loot, int item)
        {
            //loot.Add(ItemDropRule.ByCondition(new MasterRevCondition(), item, 4));
        }
        public static void AddBlockDrop(NPCLoot loot, int item)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.ByCondition(new BlockDrops(), item, 1, 55, 265));
        }

        int signuskill;
        private bool signusbackup = false;
        int signusshaker = 0;
        Vector2 RunawayTankpos;
        int calashoot = 0;

        public override void AI(NPC npc)
        {
            /*if (AncientGod.CalamityActive)
            {
                Mod.TryFind<ModProjectile>("RunawayTankKiller", out ModProjectile brimbuck);
                if (npc.type == AncientGod.CalamityNPC("WITCH") && (!AncientGodWorld.jharinter || !NPC.downedMoonlord))
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<AprilFools.RunawayTank.RunawayTank>()))
                    {
                        for (int x = 0; x < Main.maxNPCs; x++)
                        {
                            NPC npc3 = Main.npc[x];
                            if (npc3.type == ModContent.NPCType<AprilFools.RunawayTank.RunawayTank>() && npc3.active)
                            {
                                RunawayTankpos.X = npc3.Center.X;
                                RunawayTankpos.Y = npc3.Center.Y;
                                calashoot++;
                                if (npc3.position.X - npc.position.X >= 0)
                                {
                                    npc.direction = 1;
                                }
                                else
                                {
                                    npc.direction = -1;
                                }
                            }
                        }
                    }
                    if (calashoot >= 5)
                    {
                        Vector2 position = npc.Center;
                        position.X = npc.Center.X + (20f * npc.direction);
                        Vector2 direction = RunawayTankpos - position;
                        direction.Normalize();
                        float speed = 10f;
                        int type = brimbuck.Type;
                        int damage = 6666;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), position, direction * speed, type, damage, 0f, Main.myPlayer);
                        calashoot = 0;
                    }
                }
                if (npc.type == AncientGod.CalamityNPC("Signus"))
                {
                    if ((npc.ai[0] == -33f || signusbackup) && Main.LocalPlayer.GetModPlayer<AncientGodPlayer>().junsi)
                    {
                        Dust dust;
                        Vector2 position = npc.position;
                        for (int a = 0; a < 3; a++)
                        {
                            dust = Main.dust[Terraria.Dust.NewDust(position, npc.width, npc.height, 16, 0f, 0f, 0, new Color(255, 255, 255), 1.578947f)];
                            dust.shader = GameShaders.Armor.GetSecondaryShader(131, Main.LocalPlayer);
                        }
                        npc.rotation = 0;
                        npc.direction = -1;
                        npc.spriteDirection = -1;
                        signusshaker++;
                        npc.alpha = 0;
                        npc.velocity.X = 0;
                        npc.velocity.Y = 0;
                        signuskill++;
                        npc.dontTakeDamage = true;
                        for (int k = 0; k < npc.buffImmune.Length; k++)
                        {
                            npc.buffImmune[k] = true;
                        }
                        if (signuskill == 64)
                        {
                            signuskill = 0;
                            npc.knockBackResist = 20f;
                            npc.SimpleStrikeNPC(499999, npc.direction, false, npc.direction * 50, noPlayerInteraction: true);
                            signusbackup = false;
                        }
                        if (signusshaker == 1)
                        {
                            npc.velocity.X = -5;
                        }
                        else if (signusshaker == 2)
                        {
                            npc.velocity.X = 5;
                            signusshaker = 0;
                        }
                    }
                    else
                    {
                        signuskill = 0;
                        signusshaker = 0;
                    }
                    if (!Main.LocalPlayer.GetModPlayer<AncientGodPlayer>().junsi)
                    {
                        signusbackup = false;
                        if (npc.ai[0] == -33f)
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                        }
                    }
                }
            }*/
        }

        public override bool CheckDead(NPC npc)
        {
            return true;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            /*if (npc.aiStyle == NPCAIStyleID.Slime && !NPC.AnyNPCs(ModContent.NPCType<NPCs.TownPets.Slimes.NinjaSlime>()) && Main.rand.NextBool(22))
            {
                bool titShuriken = AncientGod.CalamityActive ? projectile.type == AncientGod.CalamityProjectile("TitaniumShurikenProjectile") : false;
                bool boomShuriken = false;
                if (ModLoader.HasMod("Fargowiltas"))
                {
                    if (projectile.type == ModLoader.GetMod("Fargowiltas").Find<ModProjectile>("ShurikenProj").Type)
                    {
                        boomShuriken = true;
                    }
                }
                if (projectile.type == ProjectileID.Shuriken || titShuriken || boomShuriken)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NPCs.TownPets.Slimes.NinjaSlime>());
                    if (!AncientGodWorld.ninja)
                    {
                        AncientGodWorld.ninja = true;
                        AncientGodWorld.UpdateWorldBool();
                    }

                    npc.active = false;
                    projectile.Kill();
                }
            }
            if (AncientGod.CalamityActive)
            {
                if (npc.type == AncientGod.CalamityNPC("AstrumAureus"))
                {
                    if (projectile.type == AncientGod.CalamityProjectile("AstrageldonLaser") || projectile.type == AncientGod.CalamityProjectile("AstrageldonSummon"))
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().geldonSummon = true;
                        if (!NPC.AnyNPCs(ModContent.NPCType<NPCs.TownPets.Slimes.AstroSlime>()))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.NewNPC(npc.GetSource_FromThis(), (int)projectile.Center.X, (int)projectile.Center.Y, ModContent.NPCType<NPCs.TownPets.Slimes.AstroSlime>());
                            if (!AncientGodWorld.astro)
                            {
                                AncientGodWorld.astro = true;
                                AncientGodWorld.UpdateWorldBool();
                            }
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item117, projectile.Center);
                            int dustAmt = 16;
                            for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                            {
                                int assdust = AncientGod.Calamity.Find<ModDust>("AstralOrange").Type;
                                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                                vector6 = vector6.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                                Vector2 vector7 = vector6 - projectile.Center;
                                int dusty = Dust.NewDust(vector6 + vector7, 0, 0, assdust, vector7.X * 1f, vector7.Y * 1f, 100, default, 1.1f);
                                Main.dust[dusty].noGravity = true;
                                Main.dust[dusty].noLight = true;
                                Main.dust[dusty].velocity = vector7;
                            }
                            projectile.Kill();
                        }
                    }
                    else
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().geldonSummon = false;
                    }
                }
                else if (npc.type == AncientGod.CalamityNPC("Bumblefuck"))
                {
                    if (projectile.type == AncientGod.CalamityProjectile("MiniatureFolly") && projectile.DamageType != DamageClass.Ranged)
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().bdogeMount = true;
                    }
                    else
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().bdogeMount = false;
                    }
                }
                else if (npc.type == AncientGod.CalamityNPC("Yharon"))
                {
                    if (projectile.type == AncientGod.CalamityProjectile("WulfrumDroid"))
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().wolfram = true;
                    }
                    else
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().wolfram = false;
                    }
                }
                else if (npc.type == AncientGod.CalamityNPC("Signus"))
                {
                    if (projectile.type == AncientGod.CalamityProjectile("PristineFire") ||
                        projectile.type == AncientGod.CalamityProjectile("PristineSecondary"))
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().junkoReference = true;
                    }
                    else
                    {
                        npc.GetGlobalNPC<AncientGodGlobalNPC>().junkoReference = false;
                    }
                }
            }*/
        }

        public void BossExclam(NPC npc, int[] types, bool downed, int boss, bool additionalCondition = true)
        {
            /*if (npc.type == boss && !downed && additionalCondition)
            {
                CalamityMod.NPCs.CalamityGlobalNPC.SetNewShopVariable(types, downed);
            }*/
        }

        public override bool PreKill(NPC npc)
        {
            /*if (ModLoader.HasMod("CalamityHunt"))
            {
                if (npc.type == ModLoader.GetMod("CalamityHunt").Find<ModNPC>("Goozma").Type)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<NPCs.TownPets.Slimes.Tarr>()))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NPCs.TownPets.Slimes.Tarr>());
                        if (!AncientGodWorld.tar)
                        {
                            AncientGodWorld.tar = true;
                            AncientGodWorld.UpdateWorldBool();
                        }
                    }
                }
            }
            if (AncientGod.CalamityActive)
            {
                int jellyID = ModContent.NPCType<JellyPriestNPC>();
                int oracleID = ModContent.NPCType<OracleNPC>();

                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "giantclam"), AncientGod.CalamityNPC("GiantClam"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "crabulon"), AncientGod.CalamityNPC("Crabulon"));
                BossExclam(npc, new int[] { jellyID }, NPC.downedBoss3, NPCID.SkeletronHead);
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "slimegod"), AncientGod.CalamityNPC("SlimeGodCore"));
                BossExclam(npc, new int[] { jellyID }, Main.hardMode, NPCID.WallofFlesh);
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "cryogen"), AncientGod.CalamityNPC("Cryogen"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "aquaticscourge"), AncientGod.CalamityNPC("AquaticScourgeHead"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "brimstoneelemental"), AncientGod.CalamityNPC("BrimstoneElemental"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "calamitasclone"), AncientGod.CalamityNPC("CalamitasClone"));
                BossExclam(npc, new int[] { jellyID }, NPC.downedPlantBoss, NPCID.Plantera);
                BossExclam(npc, new int[] { jellyID }, NPC.downedGolemBoss, NPCID.Golem);
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "leviathan"), AncientGod.CalamityNPC("Anahita"), !NPC.AnyNPCs(AncientGod.CalamityNPC("Leviathan")));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "leviathan"), AncientGod.CalamityNPC("Leviathan"), !NPC.AnyNPCs(AncientGod.CalamityNPC("Anahita")));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "plaguebringergoliath"), AncientGod.CalamityNPC("PlaguebringerGoliath"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "ravager"), AncientGod.CalamityNPC("RavagerBody"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "providence"), AncientGod.CalamityNPC("Providence"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "stormweaver"), AncientGod.CalamityNPC("StormWeaverHead"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "ceaselessvoid"), AncientGod.CalamityNPC("CeaselessVoid"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "signus"), AncientGod.CalamityNPC("Signus"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "polterghast"), AncientGod.CalamityNPC("Polterghast"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "oldduke"), AncientGod.CalamityNPC("OldDuke"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "devourerofgods"), AncientGod.CalamityNPC("DevourerofGodsHead"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "yharon"), AncientGod.CalamityNPC("Yharon"));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "exomechs"), AncientGod.CalamityNPC("AresBody"), !NPC.AnyNPCs(AncientGod.CalamityNPC("Apollo")) && !NPC.AnyNPCs(AncientGod.CalamityNPC("ThanatosHead")));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "exomechs"), AncientGod.CalamityNPC("Apollo"), !NPC.AnyNPCs(AncientGod.CalamityNPC("AresBody")) && !NPC.AnyNPCs(AncientGod.CalamityNPC("ThanatosHead")));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "exomechs"), AncientGod.CalamityNPC("ThanatosHead"), !NPC.AnyNPCs(AncientGod.CalamityNPC("Apollo")) && !NPC.AnyNPCs(AncientGod.CalamityNPC("AresBody")));
                BossExclam(npc, new int[] { jellyID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "supremecalamitas"), AncientGod.CalamityNPC("SupremeCalamitas"));

                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "hivemind"), AncientGod.CalamityNPC("HiveMind"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "perforator"), AncientGod.CalamityNPC("PerforatorHive"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "cryogen"), AncientGod.CalamityNPC("Cryogen"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "brimstoneelemental"), AncientGod.CalamityNPC("BrimstoneElemental"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "calamitasclone"), AncientGod.CalamityNPC("CalamitasClone"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "dragonfolly"), AncientGod.CalamityNPC("Bumblefuck"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "signus"), AncientGod.CalamityNPC("Signus"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "devourerofgods"), AncientGod.CalamityNPC("DevourerofGodsHead"));
                BossExclam(npc, new int[] { oracleID }, (bool)AncientGod.Calamity.Call("GetBossDowned", "yharon"), AncientGod.CalamityNPC("Yharon"));

                BossExclam(npc, new int[] { AncientGod.CalamityNPC("SEAHOE") }, (bool)AncientGod.Calamity.Call("GetBossDowned", "oldduke"), AncientGod.CalamityNPC("OldDuke"));
                BossExclam(npc, new int[] { AncientGod.CalamityNPC("SEAHOE") }, (bool)AncientGod.Calamity.Call("GetBossDowned", "scal"), AncientGod.CalamityNPC("SupremeCalamitas"));

                BossExclam(npc, new int[] { AncientGod.CalamityNPC("DILF") }, (bool)AncientGod.Calamity.Call("GetBossDowned", "signus"), AncientGod.CalamityNPC("Signus"));

                BossExclam(npc, new int[] { AncientGod.CalamityNPC("THIEF") }, (bool)AncientGod.Calamity.Call("GetBossDowned", "astrumaureus"), AncientGod.CalamityNPC("AstrumAureus"));

                BossExclam(npc, new int[] { NPCID.PartyGirl }, (bool)AncientGod.Calamity.Call("GetBossDowned", "polterghast"), AncientGod.CalamityNPC("Polterghast"));

                if (npc.type == AncientGod.CalamityNPC("CrabShroom") && Main.LocalPlayer.HasItem(ModContent.ItemType<PutridShroom>()) && NPC.AnyNPCs(AncientGod.CalamityNPC("Crabulon")))
                {
                    NPC.NewNPC(npc.GetSource_Death(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Swearshroom>());
                }

                if (npc.type == AncientGod.CalamityNPC("Signus") && junkoReference)
                {
                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ModContent.ItemType<SuspiciousLookingChineseCrown>());
                }

                if (npc.type == AncientGod.CalamityNPC("Yharon") && wolfram)
                {
                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ModContent.ItemType<RoverSpindle>());
                }

                if (npc.type == AncientGod.CalamityNPC("AstrumAureus") && geldonSummon)
                {
                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ModContent.ItemType<SpaceJunk>());
                }

                if (npc.type == AncientGod.CalamityNPC("Bumblefuck") && bdogeMount)
                {
                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ModContent.ItemType<ExtraFluffyFeatherClump>());
                }
            }*/

            int droppedMoney = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    /*if (player.HasBuff(ModContent.BuffType<MorshuBuff>()))
                    {
                        float value = npc.value;
                        value /= 5;
                        if (value < 0)
                        {
                            value = 1;
                        }

                        if (droppedMoney == 0)
                        {
                            while (value > 0)
                            {
                                if (value > 1000000f)
                                {
                                    int platCoins = (int)(value / 1000000f);
                                    if (platCoins > 50 && Main.rand.Next(5) == 0)
                                    {
                                        platCoins /= Main.rand.Next(3) + 1;
                                    }

                                    if (Main.rand.Next(5) == 0)
                                    {
                                        platCoins /= Main.rand.Next(3) + 1;
                                    }

                                    value -= 1000000f * platCoins;
                                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ItemID.PlatinumCoin, platCoins);
                                    continue;
                                }

                                if (value > 10000f)
                                {
                                    int goldCoins = (int)(value / 10000f);
                                    if (goldCoins > 50 && Main.rand.Next(5) == 0)
                                    {
                                        goldCoins /= Main.rand.Next(3) + 1;
                                    }

                                    if (Main.rand.Next(5) == 0)
                                    {
                                        goldCoins /= Main.rand.Next(3) + 1;
                                    }

                                    value -= 10000f * goldCoins;
                                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ItemID.GoldCoin, goldCoins);
                                    continue;
                                }

                                if (value > 100f)
                                {
                                    int silverCoins = (int)(value / 100f);
                                    if (silverCoins > 50 && Main.rand.Next(5) == 0)
                                    {
                                        silverCoins /= Main.rand.Next(3) + 1;
                                    }

                                    if (Main.rand.Next(5) == 0)
                                    {
                                        silverCoins /= Main.rand.Next(3) + 1;
                                    }

                                    value -= 100f * silverCoins;
                                    Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ItemID.SilverCoin, silverCoins);
                                    continue;
                                }

                                int copperCoins = (int)value;
                                if (copperCoins > 50 && Main.rand.Next(5) == 0)
                                {
                                    copperCoins /= Main.rand.Next(3) + 1;
                                }

                                if (Main.rand.Next(5) == 0)
                                {
                                    copperCoins /= Main.rand.Next(4) + 1;
                                }

                                if (copperCoins < 1)
                                {
                                    copperCoins = 1;
                                }

                                value -= copperCoins;
                                Item.NewItem(npc.GetSource_FromAI(), npc.Hitbox, ItemID.CopperCoin, copperCoins);
                            }

                            droppedMoney++;
                        }
                    }*/
                }
            }
            return true;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenpos, Color drawColor)
        {
            /*if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Biomes.AstralBlight>()) || Main.LocalPlayer.GetModPlayer<AncientGodPlayer>().Blok)
            {
                //DEUS HEAD
                if (npc.type == AncientGod.CalamityNPC("AstrumDeusHead"))
                {
                    Texture2D deusheadsprite = (ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusHeadOld").Value);

                    float deusheadframe = 1f / (float)Main.npcFrameCount[npc.type];
                    int deusheadheight = (int)((float)(npc.frame.Y / npc.frame.Height) * deusheadframe) * (deusheadsprite.Height / 1);

                    Rectangle deusheadsquare = new Rectangle(0, deusheadheight, deusheadsprite.Width, deusheadsprite.Height / 1);
                    Color deusheadalpha = npc.GetAlpha(drawColor);
                    Main.EntitySpriteDraw(deusheadsprite, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), deusheadsquare, deusheadalpha, npc.rotation, Utils.Size(deusheadsquare) / 2f, npc.scale, SpriteEffects.None, 0);
                    return false;
                }

                //DEUS BODY
                else if (npc.type == AncientGod.CalamityNPC("AstrumDeusBody"))
                {
                    Texture2D deusbodsprite = npc.localAI[3] == 1f ? ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusBodyAltOld").Value : ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusBodyOld").Value;

                    float deusbodframe = 1f / (float)Main.npcFrameCount[npc.type];
                    int deusbodheight = (int)((float)(npc.frame.Y / npc.frame.Height) * deusbodframe) * (deusbodsprite.Height / 1);

                    Rectangle deusbodsquare = new Rectangle(0, deusbodheight, deusbodsprite.Width, deusbodsprite.Height / 1);
                    Color deusbodalpha = npc.GetAlpha(drawColor);
                    Main.EntitySpriteDraw(deusbodsprite, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), deusbodsquare, deusbodalpha, npc.rotation, Utils.Size(deusbodsquare) / 2f, npc.scale, SpriteEffects.None, 0);
                    return false;
                }

                //DEUS TAIL
                else if (npc.type == AncientGod.CalamityNPC("AstrumDeusTail"))
                {
                    Texture2D deustailsprite = ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusTailOld").Value;

                    float deustailframe = 1f / (float)Main.npcFrameCount[npc.type];
                    int deustailheight = (int)((float)(npc.frame.Y / npc.frame.Height) * deustailframe) * (deustailsprite.Height / 1);

                    Rectangle deustailsquare = new Rectangle(0, deustailheight, deustailsprite.Width, deustailsprite.Height / 1);
                    Color deustailalpha = npc.GetAlpha(drawColor);
                    Main.EntitySpriteDraw(deustailsprite, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), deustailsquare, deustailalpha, npc.rotation, Utils.Size(deustailsquare) / 2f, npc.scale, SpriteEffects.None, 0);
                    return false;
                }
            }*/
            return true;

        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenpos, Color drawColor)
        {
            /*if (AncientGod.CalamityActive)
            {
                if (npc.type == AncientGod.CalamityNPC("SlimeGodCore") && AncientGod.AprilFoolDay)
                {
                    Texture2D tidepodgsprite = (ModContent.Request<Texture2D>("AncientGod/ExtraTextures/SlimeGod").Value);

                    float tidepodgframe = 1f / (float)Main.npcFrameCount[npc.type];
                    int tidepodgheight = (int)((float)(npc.frame.Y / npc.frame.Height) * tidepodgframe) * (tidepodgsprite.Height / 1);

                    Rectangle tidepodgsquare = new Rectangle(0, tidepodgheight, tidepodgsprite.Width, tidepodgsprite.Height / 1);
                    Color tidepodgalpha = npc.GetAlpha(drawColor);
                    spriteBatch.Draw(tidepodgsprite, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), tidepodgsquare, tidepodgalpha, npc.rotation, Utils.Size(tidepodgsquare) / 2f, npc.scale, SpriteEffects.None, 0f);
                }

                else if (Main.LocalPlayer.GetModPlayer<AncientGodPlayer>().ZoneAstral || Main.LocalPlayer.GetModPlayer<AncientGodPlayer>().Blok)
                {
                    //DEUS HEAD
                    if (npc.type == AncientGod.CalamityNPC("AstrumDeusHead"))
                    {
                        Texture2D deusheadsprite2 = (ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusHeadOld_Glow").Value);

                        float deusheadframe2 = 1f / (float)Main.npcFrameCount[npc.type];
                        int deusheadheight2 = (int)((float)(npc.frame.Y / npc.frame.Height) * deusheadframe2) * (deusheadsprite2.Height / 1);

                        Rectangle deusheadsquare2 = new Rectangle(0, deusheadheight2, deusheadsprite2.Width, deusheadsprite2.Height / 1);
                        spriteBatch.Draw(deusheadsprite2, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), deusheadsquare2, Color.White, npc.rotation, Utils.Size(deusheadsquare2) / 2f, npc.scale, SpriteEffects.None, 0f);
                    }

                    //DEUS BODY
                    else if (npc.type == AncientGod.CalamityNPC("AstrumDeusBody"))
                    {
                        Texture2D deusbodsprite2 = npc.localAI[3] == 1f ? ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusBodyAltOld_Glow").Value : ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusBodyOld_Glow").Value;

                        float deusbodframe2 = 1f / (float)Main.npcFrameCount[npc.type];
                        int deusbodheight2 = (int)((float)(npc.frame.Y / npc.frame.Height) * deusbodframe2) * (deusbodsprite2.Height / 1);

                        Rectangle deusbodsquare2 = new Rectangle(0, deusbodheight2, deusbodsprite2.Width, deusbodsprite2.Height / 1);
                        spriteBatch.Draw(deusbodsprite2, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), deusbodsquare2, Color.White, npc.rotation, Utils.Size(deusbodsquare2) / 2f, npc.scale, SpriteEffects.None, 0f);
                    }

                    //DEUS TAIL
                    else if (npc.type == AncientGod.CalamityNPC("AstrumDeusTail"))
                    {
                        Texture2D deustailsprite2 = (ModContent.Request<Texture2D>("AncientGod/NPCs/AstrumDeus/DeusTailOld_Glow").Value);

                        float deustailframe2 = 1f / (float)Main.npcFrameCount[npc.type];
                        int deustailheight2 = (int)((float)(npc.frame.Y / npc.frame.Height) * deustailframe2) * (deustailsprite2.Height / 1);

                        Rectangle deustailsquare2 = new Rectangle(0, deustailheight2, deustailsprite2.Width, deustailsprite2.Height / 1);
                        spriteBatch.Draw(deustailsprite2, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), deustailsquare2, Color.White, npc.rotation, Utils.Size(deustailsquare2) / 2f, npc.scale, SpriteEffects.None, 0f);
                    }
                }
            }*/
        }

        //Disable Astral Blight overworld spawns
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo LocalPlayer)
        {
            Player player = LocalPlayer.Player;
            AncientGodPlayer modPlayer = player.GetModPlayer<AncientGodPlayer>();
            //bool acid = AncientGod.CalamityActive ? !(bool)AncientGod.Calamity.Call("AcidRainActive") : true;
            //bool noevents = acid && !Main.eclipse && !Main.snowMoon && !Main.pumpkinMoon && Main.invasionType == 0 && !player.ZoneTowerSolar && !player.ZoneTowerStardust && !player.ZoneTowerVortex & !player.ZoneTowerNebula && !player.ZoneOldOneArmy;
            Mod cata;
            ModLoader.TryGetMod("CatalystMod", out cata);
            if (!AncientGodConfig.Instance.CritterSpawns)
            {
                if (modPlayer.sBun)
                {
                    pool.Add(NPCID.Bunny, 0.001f);
                }
            }
            /*if (player.InModBiome(ModContent.GetInstance<Biomes.AstralBlight>()) && noevents)
            {
                pool.Clear();
                if (!AncientGodConfig.Instance.CritterSpawns)
                {
                    pool.Add(ModContent.NPCType<NPCs.Critters.Blightolemur>(), 0.1f);
                    pool.Add(ModContent.NPCType<NPCs.Critters.Blinker>(), 0.1f);
                    pool.Add(ModContent.NPCType<NPCs.Critters.AstJR>(), 0.1f);
                    pool.Add(ModContent.NPCType<NPCs.Critters.GAstJR>(), 0.1f);
                    if (modPlayer.sBun)
                    {
                        pool.Add(NPCID.Bunny, 0.001f);
                    }
                }
            }*/
        }

        /*public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            if (!AncientGod.CalamityActive)
                return;

            if (AncientGod.Bumble && !AncientGodConfig.Instance.DragonballName)
            {
                if (npc.type == AncientGod.CalamityNPC("Bumblefuck") && npc.TypeName == "The Dragonfolly")
                {
                    if (Main.rand.NextFloat() < 0.01f)
                    {
                        typeName = "Bumblebirb";
                    }
                    else
                    {
                        typeName = "Blunderbird";
                    }
                }

                if (npc.type == AncientGod.CalamityNPC("Bumblefuck2") && npc.TypeName == "Draconic Swarmer")
                {
                    if (Main.rand.NextFloat() < 0.01f)
                    {
                        typeName = "Bumblebirb";
                    }
                    else
                    {
                        typeName = "Blunderling";
                    }
                }
            }
        }*/

        public override void OnKill(NPC npc)
        {
            /*if (AncientGod.CalamityActive && !ModLoader.HasMod("CalamityHunt"))
            {
                if (NPCID.Sets.IsTownSlime[npc.type] && NPC.AnyNPCs(AncientGod.CalamityNPC("SlimeGodCore")))
                {
                    Main.NewText("Woe is me");
                    if (!NPC.AnyNPCs(ModContent.NPCType<NPCs.TownPets.Slimes.Tarr>()))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(npc.GetSource_Death(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<NPCs.TownPets.Slimes.Tarr>());
                        if (!AncientGodWorld.tar)
                        {
                            AncientGodWorld.tar = true;
                            AncientGodWorld.UpdateWorldBool();
                        }
                    }
                }
            }*/
        }
    }
}