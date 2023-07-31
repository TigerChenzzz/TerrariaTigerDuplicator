using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace TigerDuplicator {
    [Label($"${_path}.Config.Label")]
    public class TigerDuplicatorConfig : ModConfig {
        const string _path = "Mods.TigerDuplicator.Configs.TigerDuplicatorConfig";
        public override ConfigScope Mode => ConfigScope.ServerSide;
		
		#region 预设
        [Header($"${_path}.Headers.Presets")]
		
        [DefaultValue(false)]
		public bool Cheat {
			get =>
				MoneyCostMultiple is 0f &&
				MoneyCostAtLeast is 0;
			set {
				if (value){
					MoneyCostMultiple = 0f;
					MoneyCostAtLeast = 0;
				}
			}
		}
		
        [DefaultValue(false)]
		public bool Easy{
			get =>
				MoneyCostMultiple is 0.8f &&
				ExtraMoneyCostMultiple is 1 &&
				MoneyCostAtLeast is 10;
			set {
				if (value){
					MoneyCostMultiple = 0.8f;
					ExtraMoneyCostMultiple = 1;
					MoneyCostAtLeast = 10;
				}
			}
		}
		
        [DefaultValue(false)]
		public bool Normal{
			get =>
				MoneyCostMultiple is 0.8f &&
				ExtraMoneyCostMultiple is 5 &&
				MoneyCostAtLeast is 200;
			set {
				if (value){
					MoneyCostMultiple = 0.8f;
					ExtraMoneyCostMultiple = 5;
					MoneyCostAtLeast = 200;
				}
			}
		}
		
        [DefaultValue(false)]
		public bool Hard{
			get =>
				MoneyCostMultiple is 0.8f &&
				ExtraMoneyCostMultiple is 20 &&
				MoneyCostAtLeast is 10000;
			set {
				if (value){
					MoneyCostMultiple = 0.8f;
					ExtraMoneyCostMultiple = 20;
					MoneyCostAtLeast = 10000;
				}
			}
		}
		#endregion
        
		#region 花费设置
        [Header($"${_path}.Headers.CostSettings")]
		
        [DefaultValue(0.8)]
        public float MoneyCostMultiple { get; set; }

        [Range(1, 100)]
        [DefaultValue(5)]
        public int ExtraMoneyCostMultiple { get; set; }

        [Range(0, 100000)]
        [DefaultValue(200)]
        public int MoneyCostAtLeast { get; set; }
        #endregion

        #region 额外设置
        [Header($"${_path}.Headers.ExtraSettings")]

        [Range(0, 1000000)]
        [DefaultValue(10000)]
        public int DefenderMedalValue { get; set; }

        [DefaultValue(false)]
        public bool CanDuplicateCoin { get; set; }

        [DefaultValue(false)]
        public bool CanDuplicateBag { get; set; }

        [DefaultValue(false)]
        public bool CanDuplicateUnloadedItem { get; set; }

        public List<ItemDefinition> DuplicateBlacklist {
            get => duplicateBlacklist;
            set => duplicateBlacklist = value;
        }
        private List<ItemDefinition> duplicateBlacklist = new() {
            new(ItemID.WaterBucket),
            new(ItemID.LavaBucket),
            new(ItemID.HoneyBucket)
        };
        #endregion

        public override void OnLoaded()
        {
            TigerDuplicator.config = this;
        }
    }
}
