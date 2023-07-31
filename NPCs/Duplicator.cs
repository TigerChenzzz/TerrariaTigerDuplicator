using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Utilities;
using Terraria.Localization;
using Terraria.GameContent.Personalities;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Default;

namespace TigerDuplicator.NPCs;
[AutoloadHead]
public class Duplicator : ModNPC {
    private static TigerDuplicatorConfig Config => TigerDuplicator.config;
    private static bool checkSpawn = false;

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
        if(checkSpawn) {
            return true;
        }
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
        if(NPC.SpawnAllowed_Merchant()) {
            checkSpawn = true;
            return true;
        }
        return false;
    }

    public override List<string> SetNPCNameList() {
        return new List<string>() {
            this.GetLocalizedValue("NameList.0"),
            this.GetLocalizedValue("NameList.1"),
            this.GetLocalizedValue("NameList.2"),
            this.GetLocalizedValue("NameList.3"),
        };
    }

    #region 攻击相关属性设置
    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        if(!Main.hardMode) {
            damage = 20;
        }
        if(!NPC.downedMoonlord && Main.hardMode) {
            damage = 50;
        }
        if(NPC.downedMoonlord) {
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
        if(firstButton) {
            shopName = "Shop";
        }
    }

    public override void AddShops() {
        new NPCShop(Type).Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items) {
        Player player = Main.player[Main.myPlayer];
        int nextSlot = 0;
        for(int i = 0; i < player.inventory.Length; ++i) {
        CheckInventoryStart:
            if(items.Length <= nextSlot) {
                break;
            }
            if(items[nextSlot] != null && !items[nextSlot].IsAir) {
                nextSlot += 1;
                goto CheckInventoryStart;
            }
            Item item = player.inventory[i];
            #region 查重
            bool con = false;
            for(int j = 0; j < i; ++j) {
                if(item.type == player.inventory[j].type) {
                    con = true;
                }
            }
            if(con) {
                continue;
            }
            #endregion
            if(item != null && item.type > ItemID.None) {
                ItemDefinition itemd = new(item.type);
                if(Config.DuplicateBlacklist.Contains(itemd)) {
                    continue;
                }
                if(!Config.CanDuplicateUnloadedItem && item.type == ItemType<UnloadedItem>()) {
                    continue;
                }
                if(!Config.CanDuplicateBag && TigerDuplicator.RightClickable(item)) {
                    continue;
                }
                if(!Config.CanDuplicateCoin && item.IsACoin) {
                    continue;
                }
                items[nextSlot] = item.Clone();
                items[nextSlot].stack = 1;
                items[nextSlot].shopCustomPrice = item.type switch {
                    ItemID.DefenderMedal => (int)(Config.DefenderMedalValue * Config.MoneyCostMultiple * Config.ExtraMoneyCostMultiple * 10),
                    _ => (int?)(int)(item.value * Config.MoneyCostMultiple * Config.ExtraMoneyCostMultiple * 2),
                };
                if(items[nextSlot].shopCustomPrice < Config.MoneyCostAtLeast) {
                    items[nextSlot].shopCustomPrice = Config.MoneyCostAtLeast;
                }
            }
        }
    }
}
