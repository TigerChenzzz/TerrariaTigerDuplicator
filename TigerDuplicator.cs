using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TigerDuplicator
{
	public class TigerDuplicator : Mod {
        internal static TigerDuplicatorConfig config;
        static bool[] ItemRightClickable;

        public static bool RightClickable(Item item) {
            //if(ItemID.Sets.OpenableBag[item.type])
            //    return true;
            //bool oldRight = Main.mouseRight;
            //Main.mouseRight = true;
            //bool ret = ItemRightClickable[item.type] || ItemLoader.CanRightClick(item);
            bool ret = ItemLoader.CanRightClick(item);
            //Main.mouseRight = oldRight;
            return ret;
            var rules = Main.ItemDropsDB.GetRulesForItemID(item.type);
            if (rules != null && rules.Count > 0) {
                return true;
            }
            return false;
        }

        public override void PostSetupContent() {
            ItemRightClickable = ItemID.Sets.Factory.CreateBoolSet(false,
                ItemID.HerbBag,
                ItemID.CanOfWorms,
                ItemID.Oyster,
                ItemID.CapricornLegs,
                ItemID.CapricornTail,
                ItemID.GoodieBag,
                ItemID.LockBox,
                ItemID.ObsidianLockbox,
                ItemID.Present,
                ItemID.BluePresent,
                ItemID.GreenPresent,
                ItemID.YellowPresent
            );
            for(int i = 0; i < ItemLoader.ItemCount; i++) {
                if(ItemID.Sets.IsFishingCrate[i] || ItemID.Sets.BossBag[i]) {
                    ItemRightClickable[i] = true;
                }
            }
        }

        public override void Unload() {
            config = null;
        }
    }
}