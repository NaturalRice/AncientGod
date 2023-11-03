﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


/*using AncientGod.Projectiles.Pets;
using AncientGod.Projectiles.Pets.LightPets;
using AncientGod.Items.Equips.Shirts;
using AncientGod.Projectiles.Pets.LightPets;
using AncientGod.Items.Equips.Shirts.ArousChestplate;*/

namespace AncientGod
{
    public class AncientGodPlayer : ModPlayer
    {
        //public static readonly ClassSpecificPlayerLayer DraedonSet = new ClassSpecificPlayerLayer("CalValEX/Items/Equips/Shirts/Draedon/", "CalValEX/Items/Equips/Hats/Draedon/", "DraedonChestplate", "DraedonHelmet");

        private const int saveVersion = 0;

        public bool aero;
        public bool andro;
        public bool Angrypup;
        public bool asPet;
        public bool AstPhage;
        public bool BabyCnidrion;
        public bool babywaterclone;
        public bool bDoge;
        public bool BoldLizard;
        public bool CalamityBABYBool;
        public bool CalamityBabyGotHit;
        public bool calMonolith = false;
        public bool catfish;
        public bool Chihuahua;
        public bool cloudmini;
        public bool cr;
        public bool Cryokid;
        public bool DraWGuard1;
        public bool DraWGuard2;
        public bool DraWGuard3;
        public bool DraWPet;
        public bool darksunSpirits;
        public bool dBall;
        public bool deusmain;
        public bool deussmall;
        public bool dogMonolith = false;
        public bool drone;
        public bool dsPet;
        public bool Dstone;
        public bool eb;
        public bool eidolist;
        public bool Enredpet;
        public bool euros;
        public bool EWyrm;
        public bool excal;
        public bool feel;
        public bool fog;
        public bool George;
        public bool GeorgeII;
        public bool goozmaPet;
        public bool hDoge;
        public bool HellLab;
        public bool hover;
        public bool jared;
        public bool junsi;
        public bool neroMonolith = false;
        public bool Lightshield;
        public bool mAero;
        public bool mAmb;
        public bool mAme;
        public bool mArmored;
        public bool mBirb;
        public bool mBirb2;
        public bool mChan;
        public bool mClam;
        public bool mCry;
        public bool mDebris;
        public bool mDia;
        public bool mDoge;
        public bool mDuke;
        public bool MechaGeorge;
        public bool mEme;
        public bool mFolly;
        public bool mHeat;
        public bool mHeat2;
        public bool mHive;
        public bool mImp;
        public bool MiniCryo;
        public bool mNaked;
        public bool moistPet;
        public bool conejo;

        public int morshuscal = 0;
        public int morshuTimer;
        public bool mPerf;
        public bool mPhan;
        public bool mRav;
        public bool mRub;
        public bool mSap;
        public bool mScourge;
        public bool mShark;
        public bool mSkater;
        public bool mSlime;
        public bool mTop;
        public bool Nugget;
        public bool oSquid;
        public bool PBGmini;
        public bool pbgMonolith = false;

        public bool provMonolith = false;
        public bool pylon;
        public bool rarebrimling;
        public bool raresandmini;
        public bool RepairBot;
        public bool rover;
        public bool rPanda;
        public bool rusty;
        public bool sandmini;
        public bool sBun;
        public int SCalHits;
        public bool scalMonolith = false;
        public bool sDuke;
        public bool seerL;
        public bool seerM;
        public bool seerS;
        public bool sepet;
        public bool SignusMini;
        public bool sirember;
        public bool Skeetyeet;
        public bool SmolCrab;
        public bool squid;
        public bool sSignus;
        public bool StarJelly;
        public bool strongWeeb;
        public bool sVoid;
        public bool sWeeb;
        public bool SWPet;
        public bool TerminalRock;
        public bool tub;
        public bool uSerpent;
        public bool voidling;
        public bool VoidOrb;
        public bool voreworm;
        public bool worb;
        public bool yharonMonolith = false;
        public bool cryoMonolith = false;
        public bool brMonolith = false;
        public bool exoMonolith = false;
        public bool ySquid;
        public bool amogus;
        public bool buppy;
        public bool BMonster;
        public bool hage;
        public bool Blok;
        public bool ZoneAstral;
        public bool ZoneLab;
        public bool ZoneMockDungeon;
        public bool aesthetic;
        public bool exorb;
        public bool rockhat;
        public bool prismshell;
        public bool rainbow;
        public bool avalon;
        public bool SupJ;
        public bool poltermask;
        public bool polterchest;
        public bool polterthigh;
        public bool bSignut;
        public bool bSlime;
        public bool buffboi;
        public bool smaul;
        public bool shart;
        public bool roverd;
        public bool morshu;
        public bool morshugun;
        public bool scaldown;
        public bool yharcar;
        public bool sepneo;
        //Signus Transformation bools
        public bool signutHide;
        public bool signutForce;
        public bool signutPrevious;
        public bool signutPower;
        public bool signutTrans;
        //Andromeda transformation bools
        public bool androHide;
        public bool androForce;
        public bool androPrevious;
        public bool androPower;
        public bool androTrans;
        //Classic Brimmy transformation bools
        public bool classicHide;
        public bool classicForce;
        public bool classicPrevious;
        public bool classicPower;
        public bool classicTrans;
        //Cloud transformation bools
        public bool cloudHide;
        public bool cloudForce;
        public bool cloudPrevious;
        public bool cloudPower;
        public bool cloudTrans;
        //Sand transformation bools
        public bool sandHide;
        public bool sandForce;
        public bool sandPrevious;
        public bool sandPower;
        public bool sandTrans;
        //Bloody mary bools
        public bool maryHide;
        public bool maryForce;
        public bool maryPrevious;
        public bool maryPower;
        public bool maryTrans;
        public double rotcounter = 0;
        public double rotdeg = 0;
        public double rotsin = 0;
        public int chopperframe = 0;
        public int choppercounter = 0;
        public int helicounter = 0;
        public int heliframe = 0;
        public int conecounter = 0;
        public int coneframe = 0;
        public int twincounter = 0;
        public int twinframe = 0;
        public int stwincounter = 0;
        public int stwinframe = 0;
        public bool wulfrumjam;
        public bool cassette;
        public bool specan;
        public bool carriage;
        public float bcarriagewheel = 0.0f;
        public bool twinballoon;
        public bool artballoon;
        public bool apballoon;
        public bool sapballoon;
        public bool sartballoon;
        //344
        //Pong stuff
        public bool pongactive;
        public int pongstage = 0;
        public int pongoutcome = 0;
        //Boi stuff
        public bool boiactive;
        public bool boiatlantis;
        public bool boienemy1;
        public bool boienemy2;
        public bool boienemy3;
        public int boistage;
        public int boihealth = 3;
        public int bossded;
        public bool arousarms;
        public bool lumpe;
        public bool geldonalive;
        public bool arous;
        public bool BigBangMecha;//这里为大爆炸机甲的布尔值
        public bool AncientMecha;//这里为远古机甲的布尔值
        public bool ModernMecha;//这里为现代机甲的布尔值
        public bool FutureMecha;//这里为未来机甲的布尔值
        public bool EntropySilenceMecha;//这里为熵寂机甲的布尔值
        public bool nurex;
        public bool nreyeball;
        public float pongballposx;
        public float pongballposy;
        public float sliderposx;
        public float sliderposy;
        //Enchants
        public bool soupench = false;

        // Bootleg Calamity bools
        public bool SirenHeart;
        public bool CirrusDress;

        //Æ: Drae's bools
        public bool digger;
        public bool BestInst;
        public bool DustChime;
        public bool NurseryBell;
        public bool AlarmClock;
        public bool MaladyBells;
        public bool Harbinger;
        public bool SpiritDiner;
        public bool AltarBell;
        public bool WormBell;
        public bool Vaseline;
        public bool ScratchedGong;
        public bool TubRune;
        public bool Pandora;
        public bool profanedCultist;
        public bool profanedCultistHide;
        public bool profanedCultistForce;
        public bool zygote;
        public bool brimberry;
        public bool bellaCloak;
        public bool bellaCloakHide;
        public bool bellaCloakForce;
        public bool helipack;
        public bool CalValPat;

        public override void Initialize()
        {
            ResetMyStuff();
            CalamityBabyGotHit = false;
            morshuTimer = 0;
        }

        public override void ResetEffects()
        {
            signutPrevious = signutTrans;
            signutTrans = signutHide = signutForce = signutPower = false;
            androPrevious = androTrans;
            androTrans = androHide = androForce = androPower = false;
            classicPrevious = classicTrans;
            classicTrans = classicHide = classicForce = classicPower = false;
            cloudPrevious = cloudTrans;
            cloudTrans = cloudHide = cloudForce = cloudPower = false;
            sandPrevious = sandTrans;
            sandTrans = sandHide = sandForce = sandPower = false;
            maryPrevious = maryTrans;
            maryTrans = maryHide = maryForce = maryPower = false;
            profanedCultist = profanedCultistHide = profanedCultistForce = false;
            bellaCloak = bellaCloakHide = bellaCloakForce = false;
            ResetMyStuff();
        }

        //Update vanity is fucking broken so they are detected here
        /*public override void UpdateEquips()
        {
            if (Player.armor[10].type == ItemType<Aestheticrown>())
            {
                aesthetic = true;
            }
            if (Player.armor[10].type == ItemType<StonePile>())
            {
                rockhat = true;
            }
            if (Player.armor[10].type == ItemType<TrueCosmicCone>())
            {
                conejo = true;
            }
            if (Player.armor[10].type == ItemType<SpectralstormHat>())
            {
                specan = true;
            }
            if (Player.armor[11].type == ItemType<Items.Equips.Shirts.ArousChestplate.ArousChestplate>())//原来必须加上这里才能真正显示出服装上的四个机械臂！！！
            {
                bool gausspawned = Player.ownedProjectileCounts[ModContent.ProjectileType<GaussArm>()] <= 0;
                bool laserpawned = Player.ownedProjectileCounts[ModContent.ProjectileType<LaserArm>()] <= 0;
                bool teslaspawned = Player.ownedProjectileCounts[ModContent.ProjectileType<TeslaArm>()] <= 0;
                bool plasmaspawned = Player.ownedProjectileCounts[ModContent.ProjectileType<PlasmaArm>()] <= 0;
                if (gausspawned && Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[11]), Player.position.X + Player.width / 2, Player.position.Y + Player.height / 2,
                        0, 0, ModContent.ProjectileType<GaussArm>(), 0, 0f, Player.whoAmI);
                }
                if (laserpawned && Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[11]), Player.position.X + Player.width / 2, Player.position.Y + Player.height / 2,
                       0, 0, ModContent.ProjectileType<LaserArm>(), 0, 0f, Player.whoAmI);
                }
                if (teslaspawned && Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[11]), Player.position.X + Player.width / 2, Player.position.Y + Player.height / 2,
                       0, 0, ModContent.ProjectileType<TeslaArm>(), 0, 0f, Player.whoAmI);
                }
                if (plasmaspawned && Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[11]), Player.position.X + Player.width / 2, Player.position.Y + Player.height / 2,
                       0, 0, ModContent.ProjectileType<PlasmaArm>(), 0, 0f, Player.whoAmI);
                }
                arousarms = true;
            }
        }*/

        /*public override void UpdateVisibleVanityAccessories()
        {
            Mod antisocial;
            ModLoader.TryGetMod("Antisocial", out antisocial);
            Item itemVan = Player.armor[11];
            if (itemVan.type == ItemType<BloodyMaryDress>())
            {
                maryHide = false;
                maryForce = true;
            }
            if (itemVan.type == ItemType<ProfanedCultistRobes>())
            {
                profanedCultistHide = false;
                profanedCultistForce = true;
            }
            if (itemVan.type == ItemType<BelladonnaCloak>())
            {
                bellaCloakHide = false;
                bellaCloakForce = true;
            }
            for (int n = 13; n < 18 + Player.extraAccessorySlots; n++)
            {
                Item item = Player.armor[n];
                if (item.type == ItemType<Signus>())
                {
                    signutHide = false;
                    signutForce = true;
                }
                else if (item.type == ItemType<ProtoRing>())
                {
                    androHide = false;
                    androForce = true;
                }
                else if (item.type == ItemType<BurningEye>())
                {
                    classicHide = false;
                    classicForce = true;
                }
                else if (item.type == ItemType<CloudWaistbelt>())
                {
                    cloudHide = false;
                    cloudForce = true;
                }
                else if (item.type == ItemType<Items.Equips.Transformations.SandyBangles>())
                {
                    sandHide = false;
                    sandForce = true;
                }
                //Update vanity is fucking broken so they are detected here
                else if (item.type == ItemType<Items.Equips.Backs.PrismShell>())
                {
                    prismshell = true;
                }
                else if (item.type == ItemType<Items.Equips.ExodiumMoon>())
                {
                    exorb = true;
                }
                else if (item.type == ItemType<ArtemisBalloonSmall>())
                {
                    sartballoon = true;
                }
                else if (item.type == ItemType<ApolloBalloonSmall>())
                {
                    sapballoon = true;
                }
                else if (item.type == ItemType<ApolloBalloon>())
                {
                    apballoon = true;
                }
                else if (item.type == ItemType<ArtemisBalloon>())
                {
                    artballoon = true;
                }
                else if (item.type == ItemType<ExoTwinsBalloon>())
                {
                    twinballoon = true;
                }
                else if (item.type == ItemType<WulfrumHelipack>())
                {
                    helipack = true;
                }
            }
        }*/
        /*public override void FrameEffects()
        {
            if ((maryTrans || maryForce) && !maryHide)
                Player.legs = EquipLoader.GetEquipSlot(Mod, "BloodyMaryDress", EquipType.Legs);
            if ((profanedCultist || profanedCultistForce) && !profanedCultistHide)
                Player.legs = EquipLoader.GetEquipSlot(Mod, "ProfanedCultistRobes", EquipType.Legs);
            if ((bellaCloak || bellaCloakForce) && !bellaCloakHide)
                Player.legs = EquipLoader.GetEquipSlot(Mod, "BelladonnaCloak", EquipType.Legs);

            if ((signutTrans || signutForce) && !signutHide)
            {
                var costume = GetInstance<Signus>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "Signus", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "Signus", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "Signus", EquipType.Legs);
            }
            else if ((androTrans || androForce) && !androHide)
            {
                var costume = GetInstance<ProtoRing>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "ProtoRing", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "ProtoRing", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "ProtoRing", EquipType.Legs);
            }
            else if ((classicTrans || classicForce) && !classicHide)
            {
                var costume = GetInstance<BurningEye>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "BurningEye", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "BurningEye", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "BurningEye", EquipType.Legs);
            }
            else if ((cloudTrans || cloudForce) && !cloudHide)
            {
                var costume = GetInstance<CloudWaistbelt>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "CloudWaistbelt", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "CloudWaistbelt", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "CloudWaistbelt", EquipType.Legs);
            }
            else if ((sandTrans || sandForce) && !sandHide)
            {
                var costume = GetInstance<SandyBangles>();
                Player.head = EquipLoader.GetEquipSlot(Mod, "SandyBangles", EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, "SandyBangles", EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, "SandyBangles", EquipType.Legs);
            }
            if (cassette)
            {
                Player.armorEffectDrawShadow = true;
                Player.armorEffectDrawOutlines = true;
            }
        }*/
        /*public override void UpdateVisibleAccessories()
        {
            if (signutTrans)
                Player.AddBuff(ModContent.BuffType<Buffs.Transformations.SignutTransformationBuff>(), 60, true);
            else if (androTrans)
                Player.AddBuff(ModContent.BuffType<Buffs.Transformations.ProtoRingBuff>(), 60, true);
            else if (classicTrans)
                Player.AddBuff(ModContent.BuffType<Buffs.Transformations.ClassicBrimmyBuff>(), 60, true);
            else if (cloudTrans)
                Player.AddBuff(ModContent.BuffType<Buffs.Transformations.CloudTransformationBuff>(), 60, true);
            else if (sandTrans)
                Player.AddBuff(ModContent.BuffType<Buffs.Transformations.SandTransformationBuff>(), 60, true);
        }*/

        public override void PreUpdateMovement()
        {
            if (pongactive)
            {
                Player.releaseHook = true;
                Player.releaseMount = true;
                Player.velocity.X = 0;
                if (Player.velocity.Y < 0)
                {
                    Player.velocity.Y = 0;
                }
            }
            if (!pongactive)
            {
                pongstage = 0;
                pongoutcome = 0;
            }
            if (boiactive)
            {
                Player.releaseHook = true;
                Player.releaseMount = true;
                Player.velocity.X = 0;
                if (Player.velocity.Y < 0)
                {
                    Player.velocity.Y = 0;
                }
            }
            if (!boiactive)
            {
                //boistage = 0;
                boihealth = 3;
                boienemy1 = false;
                boienemy2 = false;
                boienemy3 = false;
            }
        }

        /*public override void PostUpdateBuffs()
        {
            if (!Player.HasBuff(ModContent.BuffType<MorshuBuff>()))
            {
                morshuTimer = 0;
            }
        }*/

        public override void UpdateDead()
        {
            ResetMyStuff();
            CalamityBabyGotHit = false;
            SCalHits = 0;
            morshuTimer = 0;
        }

        [JITWhenModsEnabled("CalamityMod")]
        public override void PreUpdate()
        {
            //Custom player draw frame counters
            int coneflame = 6;
            if (conecounter >= 5)
            {
                conecounter = -1;
                coneframe = coneframe == coneflame - 1 ? 0 : coneframe + 1;
            }
            conecounter++;

            int twinflame = 5;
            if (twincounter >= 12)
            {
                twincounter = -1;
                twinframe = twinframe == twinflame - 1 ? 0 : twinframe + 1;
            }
            twincounter++;

            int stwinflame = 12;
            if (stwincounter >= 12)
            {
                stwincounter = -1;
                stwinframe = stwinframe == stwinflame - 1 ? 0 : stwinframe + 1;
            }
            stwincounter++;

            if (Main.LocalPlayer.velocity.X > 0)
            {
                bcarriagewheel += 1.0f;
            }
            else if (Main.LocalPlayer.velocity.X < 0)
            {
                bcarriagewheel -= 1.0f;
            }
            if (bossded > 0)
            {
                bossded--;
            }
            rotcounter += Math.PI / 80;
            rotdeg = Math.Cos(rotcounter);
            rotsin = -Math.Sin(rotcounter);

        }

        private void ResetMyStuff()
        {
            DraWGuard1 = false;
            DraWGuard2 = false;
            DraWGuard3 = false;
            DraWPet = false;
            mBirb = false;
            mBirb2 = false;
            mDoge = false;
            mAero = false;
            mSkater = false;
            mShark = false;
            mFolly = false;
            mPerf = false;
            mHive = false;
            mPhan = false;
            mChan = false;
            mNaked = false;
            mArmored = false;
            eidolist = false;
            excal = false;
            aero = false;
            mRav = false;
            tub = false;
            andro = false;
            seerS = false;
            seerM = false;
            seerL = false;
            mImp = false;
            George = false;
            rPanda = false;
            catfish = false;
            cr = false;
            eb = false;
            mSlime = false;
            fog = false;
            mDebris = false;
            mHeat = false;
            mHeat2 = false;
            dBall = false;
            mClam = false;
            mAme = false;
            mSap = false;
            mEme = false;
            mTop = false;
            mRub = false;
            mDia = false;
            mCry = false;
            mAmb = false;
            sBun = false;
            uSerpent = false;
            GeorgeII = false;
            junsi = false;
            SignusMini = false;
            MiniCryo = false;
            SmolCrab = false;
            VoidOrb = false;
            AstPhage = false;
            StarJelly = false;
            Dstone = false;
            EWyrm = false;
            PBGmini = false;
            BoldLizard = false;
            Nugget = false;
            Enredpet = false;
            sandmini = false;
            raresandmini = false;
            rarebrimling = false;
            babywaterclone = false;
            cloudmini = false;
            Skeetyeet = false;
            Angrypup = false;
            Cryokid = false;
            TerminalRock = false;
            BabyCnidrion = false;
            sVoid = false;
            sSignus = false;
            sWeeb = false;
            euros = false;
            hDoge = false;
            bDoge = false;
            SWPet = false;
            jared = false;
            asPet = false;
            dsPet = false;
            mDuke = false;
            sirember = false;
            deusmain = false;
            deussmall = false;
            roverd = false;
            rusty = false;
            sepet = false;
            pylon = false;
            worb = false;
            rover = false;
            drone = false;
            hover = false;
            RepairBot = false;
            MechaGeorge = false;
            CalamityBABYBool = false;
            Lightshield = false;
            sDuke = false;
            squid = false;
            voidling = false;
            mScourge = false;
            darksunSpirits = false;
            goozmaPet = false;
            voreworm = false;
            moistPet = false;
            Chihuahua = false;
            strongWeeb = false;
            ySquid = false;
            oSquid = false;
            feel = false;
            amogus = false;
            buppy = false;
            BMonster = false;
            hage = false;
            Blok = false;
            aesthetic = false;
            exorb = false;
            poltermask = false;
            polterchest = false;
            polterthigh = false;
            rockhat = false;
            prismshell = false;
            rainbow = false;
            SupJ = false;
            bSignut = false;
            bSlime = false;
            buffboi = false;
            smaul = false;
            shart = false;
            morshu = false;
            morshugun = false;
            scaldown = false;
            avalon = false;
            wulfrumjam = false;
            conejo = false;
            cassette = false;
            specan = false;
            carriage = false;
            yharcar = false;
            sepneo = false;
            twinballoon = false;
            artballoon = false;
            apballoon = false;
            sapballoon = false;
            sartballoon = false;
            pongactive = false;
            pongoutcome = 0;
            pongstage = 0;
            //Boi stuff
            boiactive = false;
            boiatlantis = false;
            boienemy1 = false;
            boienemy2 = false;
            boienemy3 = false;
            boistage = 0;
            boihealth = 3;
            soupench = false;
            arousarms = false;
            lumpe = false;
            arous = false;
            nurex = false;
            nreyeball = false;
            // Æ: Drae's bools but false !!
            digger = false;
            BestInst = false;
            DustChime = false;
            NurseryBell = false;
            AlarmClock = false;
            MaladyBells = false;
            Harbinger = false;
            SpiritDiner = false;
            AltarBell = false;
            WormBell = false;
            Vaseline = false;
            ScratchedGong = false;
            TubRune = false;
            Pandora = false;
            zygote = false;
            brimberry = false;
            helipack = false;
            CalValPat = false;
        }

        /*public override void OnHurt(Player.HurtInfo info)
        {
            DoCalamityBabyThings(info.Damage);
            if (signutTrans)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit49, Player.position);
                for (int x = 0; x < 15; x++)
                {
                    Dust dust;
                    Vector2 position = Main.LocalPlayer.Center;
                    dust = Main.dust[Terraria.Dust.NewDust(Player.Center, 26, 15, 191, 0f, 0f, 147, new Color(255, 255, 255), 0.9868422f)];
                }
            }

            if (classicTrans || cloudTrans || sandTrans)
                Terraria.Audio.SoundEngine.PlaySound(SoundID.FemaleHit, Player.position);
        }*/

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (signutTrans) modifiers.DisableSound();
            if (cloudTrans) modifiers.DisableSound();
            if (classicTrans) modifiers.DisableSound();
            if (sandTrans) modifiers.DisableSound();
            return;

        }
        /*public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            DoCalamityBabyThings(hurtInfo.Damage);

            if (npc.type == AncientGod.CalamityNPC("SupremeCalamitas") && Player.immuneTime <= 0)
            {
                SCalHits++;
            }
        }*/

        /*public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            DoCalamityBabyThings(hurtInfo.Damage);

            for (int i = 0; i < Main.maxNPCs; i++)
                if (Main.npc[i].active &&
                    Main.npc[i].type == NaturalRiceFirstMod.CalamityNPC("SupremeCalamitas"))
                {
                    if (Player.immuneTime <= 0)
                    {
                        SCalHits++;
                    }
                }
        }*/

        /*private void DoCalamityBabyThings(int damage)
        {
            if (CalamityBABYBool)
            {
                if (damage > 0)
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        if (Player.name == "Kawaggy")
                        {
                            string humiliationText = "";
                            switch (Main.rand.Next(0, 5))
                            {
                                case 0:
                                    humiliationText = "Why didn't you code me earlier?";
                                    break;

                                case 1:
                                    humiliationText = "Not worthy.";
                                    break;

                                case 2:
                                    humiliationText = "It's been months.";
                                    break;

                                case 3:
                                    humiliationText = "You will not be forgiven.";
                                    break;

                                case 4:
                                    humiliationText = "I hope you learn.";
                                    break;
                            }

                            CombatText.NewText(Player.getRect(), Color.White, humiliationText);
                            Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " is not worthy."), 99999999,
                                -1);
                        }

                        if (Main.rand.NextFloat() < 0.08f)
                        {
                            if (CalamityBABY.nameList.Contains(Player.name))
                            {
                                Player.AddBuff(BuffID.Lovestruck, Main.rand.Next(600, 3601));
                            }
                            else if (Player.HasBuff(BuffID.BabySlime) ||
                                       Main.LocalPlayer.HasItem(ItemID.LandMine))
                            {

                            }
                            else
                            {
                                CalamityBabyGotHit = true;
                            }
                        }
                    }
                }
            }
        }*/
        //[JITWhenModsEnabled("CalamityMod")]
        /*public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (Player.ZoneBeach && Main.rand.NextFloat() < 0.021f)
            {
                itemDrop = ItemType<Items.Pets.Elementals.StrangeMusicNote>();
            }
            if (ZoneAstral && Main.rand.NextFloat() < 0.0105f)
            {
                itemDrop = ItemType<Items.Tiles.Plants.AstralOldPurple>();
            }
            if (ZoneAstral && Main.rand.NextFloat() < 0.0105f)
            {
                itemDrop = ItemType<Items.Tiles.Plants.AstralOldYellow>();
            }
            if (NatrualRiceFirstMod.CalamityActive)
                if ((bool)ModLoader.GetMod("CalamityMod").Call("GetInZone", Player, "sunkensea") && Main.hardMode && Main.rand.NextFloat() < 0.021f)
                {
                    itemDrop = ItemType<Items.Tiles.SailfishTrophy>();
                }
        }
*/
        //[JITWhenModsEnabled("CalamityMod")]
        /*public override void PostUpdateMiscEffects()
        {
            bool bossIsAlive2 = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.boss)
                {
                    bossIsAlive2 = true;
                }
            }
            if (!bossIsAlive2 && NatrualRiceFirstMod.CalamityActive)
            {
                bool TerminalMonolith = NatrualRiceFirstMod.RockshrinEX;
                if (NatrualRiceFirstMod.RockshrinEX)
                {

                    CalamityMod.Skies.BossRushSky.ShouldDrawRegularly = true;
                    Player.ManageSpecialBiomeVisuals("CalamityMod:BossRush", TerminalMonolith, Player.Center);
                }
            }
        }*/

        public override void CopyClientState(ModPlayer clientClone) // tModPorter Suggestion: Replace Item.Clone usages with Item.CopyNetStateTo
        {
            AncientGodPlayer clone = clientClone as AncientGodPlayer;
            clone.SCalHits = SCalHits;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)AncientGod.MessageType.SyncNaturalRiceFirstModPlayer);
            packet.Write((byte)Player.whoAmI);
            packet.Write(SCalHits);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            AncientGodPlayer clone = clientPlayer as AncientGodPlayer;
            if (clone.SCalHits != SCalHits)
            {
                var packet = Mod.GetPacket();
                packet.Write((byte)AncientGod.MessageType.SyncSCalHits);
                packet.Write((byte)Player.whoAmI);
                packet.Write(SCalHits);
                packet.Send();
            }
        }

        /*public static readonly PlayerLayer Mimigun = new PlayerLayer("AncientGod", "Mimigun", PlayerLayer.Head, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("CalValEX");
            CalValEXPlayer modPlayer = drawPlayer.GetModPlayer<CalValEXPlayer>();
            if (modPlayer.morshugun)
            {
                int gnuflip = 66 * -drawPlayer.direction;
                Texture2D texture = mod.GetTexture("Items/Mounts/Morshu/Minigun");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X - gnuflip);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height - Main.screenPosition.Y + 56);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                Main.playerDrawData.Add(data);
            }
        });*/

        /*public static readonly PlayerLayer Mimigun2 = new PlayerLayer("CalValEX", "Mimigun2", PlayerLayer.Arms, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("CalValEX");
            CalValEXPlayer modPlayer = drawPlayer.GetModPlayer<CalValEXPlayer>();
            if (modPlayer.morshugun)
            {
                int gnuflip = 36 * -drawPlayer.direction;
                Texture2D texture = mod.GetTexture("Items/Mounts/Morshu/Minigun");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X - gnuflip);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height - Main.screenPosition.Y + 56);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                Main.playerDrawData.Add(data);
            }
        });*/

        /*public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            DraedonSet.Visible = true;
            int headLayer = layers.FindIndex(l => l == PlayerLayer.Head);
            int bodyLayer = layers.FindIndex(l => l == PlayerLayer.Body);

            if (headLayer > -1)
            {
                layers.Insert(headLayer + 1, DraedonSet.head);
            }

            if (bodyLayer > -1)
            {
                DraedonSet.Visible = true; layers.Insert(bodyLayer + 1, DraedonSet.body);
            }
        }*/

        /*public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            int headLayer = layers.FindIndex(l => l == PlayerHeadLayer.Armor);

            if (headLayer > -1)
            {
                layers.Insert(headLayer + 1, DraedonSet.map);
            }
        }*/

        /*public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) //i just really dont want to fix the sprite issues.
        {
            if (drawInfo.drawPlayer.legs == EquipLoader.GetEquipSlot(Mod, "DraedonLeggings", EquipType.Legs))
            {
                drawInfo.legColor = Color.Transparent;
                drawInfo.legGlowMaskColor = Color.Transparent;
                drawInfo.pantsColor = Color.Transparent;
            }

            if (drawInfo.drawPlayer.body == EquipLoader.GetEquipSlot(Mod, "DraedonChestplate", EquipType.Body))
            {
                drawInfo.armGlowMaskColor = Color.Transparent;
                drawInfo.bodyColor = Color.Transparent;
                drawInfo.bodyGlowMaskColor = Color.Transparent;
                drawInfo.shirtColor = Color.Transparent;
                drawInfo.underShirtColor = Color.Transparent;
            }

            if (drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, "DraedonHelmet", EquipType.Head))
            {
                drawInfo.eyeColor = Color.Transparent;
                drawInfo.eyeWhiteColor = Color.Transparent;
                drawInfo.faceColor = Color.Transparent;
                drawInfo.hairColor = Color.Transparent;
                drawInfo.headGlowMaskColor = Color.Transparent;
            }
        }*/
    }
}