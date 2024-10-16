﻿using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Default;

namespace TigerDuplicator.NPCs;
[AutoloadHead]
public class Duplicator : ModNPC {
    private static TigerDuplicatorConfig Config => TigerDuplicator.config;

    public override void SetStaticDefaults() {
        // DisplayName.SetDefault(Language.GetTextValue($"{_path}.Name"));
        Main.npcFrameCount[Type] = 23;
        NPCID.Sets.AttackFrameCount[Type] = 2;
        NPCID.Sets.DangerDetectRange[Type] = 500;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 22;
        NPCID.Sets.AttackAverageChance[Type] = 10;
        NPCID.Sets.HatOffsetY[Type] = -4;

        NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
            .SetNPCAffection(NPCID.Guide, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like);
    }

    public override void SetDefaults() {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = 7;
        NPC.damage = 10;
        NPC.defense = 100;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        AnimationType = NPCID.Clothier;
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)/* tModPorter Suggestion: Copy the implementation of NPC.SpawnAllowed_Merchant in vanilla if you to count money, and be sure to set a flag when unlocked, so you don't count every tick. */
    {
        //for(int k = 0; k <= 255; ++k) {
        //    Player player = Main.player[k];
        //    if(!player.active) {
        //        continue;
        //    }
        //    if(player.CanAfford(10000)) {
        //        checkSpawn = true;
        //        return true;
        //    }
        //}
        return NPC.SpawnAllowed_Merchant();
    }

    public override List<string> SetNPCNameList() {
        return [
            this.GetLocalizedValue("NameList.0"),
            this.GetLocalizedValue("NameList.1"),
            this.GetLocalizedValue("NameList.2"),
            this.GetLocalizedValue("NameList.3"),
        ];
    }

    #region 攻击相关属性设置
    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        if (!Main.hardMode) {
            damage = 20;
        }
        if (!NPC.downedMoonlord && Main.hardMode) {
            damage = 50;
        }
        if (NPC.downedMoonlord) {
            damage = 100;
        }
        knockback = 8f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
        cooldown = 120;
        randExtraCooldown = 30;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        projType = ProjectileID.TopazBolt;
        attackDelay = 20;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
        multiplier = 15f;
        gravityCorrection = 0f;
        randomOffset = 2f;
    }
    #endregion

    public override string GetChat() {
        return Main.rand.NextBool(2) ?
            this.GetLocalizedValue("Dialog.Default0") :
            this.GetLocalizedValue("Dialog.Default1");
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = this.GetLocalizedValue("ShopButtonName");
        //button = Language.GetTextValue("LegacyInterface.28");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
        }
    }

    public override void AddShops() {
        new NPCShop(Type).Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items) {
        Player player = Main.player[Main.myPlayer];
        int nextSlot = 0;
        for (int i = 0; i < player.inventory.Length; ++i) {
        CheckInventoryStart:
            if (items.Length <= nextSlot) {
                break;
            }
            if (items[nextSlot] != null && !items[nextSlot].IsAir) {
                nextSlot += 1;
                goto CheckInventoryStart;
            }
            Item item = player.inventory[i];
            #region 查重
            if (Config.RepeatDuplicate) {
                goto AfterRepeatCheck;
            }
            bool con = false;
            for (int j = 0; j < i; ++j) {
                if (item.type == player.inventory[j].type) {
                    con = true;
                    break;
                }
            }
            if (con) {
                continue;
            }
        AfterRepeatCheck:
            #endregion
            if (item != null && item.type > ItemID.None) {
                ItemDefinition itemd = new(item.type);
                if (Config.DuplicateWhitelist.Contains(itemd)) {
                    goto AfterConditionCheck;
                }
                if (Config.DuplicateBlacklist.Contains(itemd)) {
                    continue;
                }
                if (!Config.CanDuplicateUnloadedItem && item.type == ModContent.ItemType<UnloadedItem>()) {
                    continue;
                }
                if (!Config.CanDuplicateBag && IsBag(item)) {
                    continue;
                }
                if (!Config.CanDuplicateRightClickable && (!IsBag(item) && RightClickable(item))) {
                    continue;
                }
                if (!Config.CanDuplicateCoin && item.IsACoin) {
                    continue;
                }
            AfterConditionCheck:
                items[nextSlot] = item.Clone();
                items[nextSlot].stack = 1;
                items[nextSlot].favorited = false;
                items[nextSlot].newAndShiny = false;
                long value = Config.GetCustomValue().ContainsKey(itemd) ?
                    Config.GetCustomValue()[itemd] : item.value;
                value = (long)(value * Config.MoneyCostMultiple * Config.ExtraMoneyCostMultiple * 2);
                if (Config.MoneyCostAtMost >= 0 && value > Config.MoneyCostAtMost) {
                    value = Config.MoneyCostAtMost;
                }
                if (value < Config.MoneyCostAtLeast) {
                    value = Config.MoneyCostAtLeast;
                }
                if (value > int.MaxValue) {
                    value = int.MaxValue;
                }
                items[nextSlot].shopCustomPrice = (int)value;
            }
        }
    }

    private static bool IsBag(Item item) => ItemID.Sets.OpenableBag[item.type];

    private static bool RightClickable(Item item) {
        //if(ItemID.Sets.OpenableBag[item.type])
        //    return true;
        //bool oldRight = Main.mouseRight;
        //Main.mouseRight = true;
        //bool ret = ItemRightClickable[item.type] || ItemLoader.CanRightClick(item);
        bool ret = ItemLoader.CanRightClick(item);
        //Main.mouseRight = oldRight;
        return ret;
        // var rules = Main.ItemDropsDB.GetRulesForItemID(item.type);
        // if (rules != null && rules.Count > 0) {
        //     return true;
        // }
        // return false;
    }

    public override void Load() {
        On_Player.GetItemExpectedPrice += ItemPriceCapTo999;
    }

    private void ItemPriceCapTo999(On_Player.orig_GetItemExpectedPrice orig, Player self, Item item, out long calcForSelling, out long calcForBuying) {
        orig(self, item, out calcForSelling, out calcForBuying);
        // ItemID.CopperCoin = 71
        // ItemID.SilverCoin = 72
        // ItemID.GoldCoin = 73
        // ItemID.PlatinumCoin = 74
        int cap = 999_99_99_99;

        if (Config.Cap999 == TigerDuplicatorConfig.CapRange.AllNPC ||
            Config.Cap999 == TigerDuplicatorConfig.CapRange.OnlyDuplicator &&
            Main.LocalPlayer.TalkNPC?.type == ModContent.NPCType<Duplicator>()) {
            if (calcForBuying > cap) {
                calcForBuying = cap;
            }
        }
    }
}
