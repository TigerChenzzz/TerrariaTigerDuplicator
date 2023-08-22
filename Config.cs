using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace TigerDuplicator {
    public class TigerDuplicatorConfig : ModConfig {
        const string _path = "Mods.TigerDuplicator.Configs.TigerDuplicatorConfig";
        public override ConfigScope Mode => ConfigScope.ServerSide;
		
		#region 预设
        [Header($"${_path}.Headers.Presets")]
		
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

		[DefaultValue(true)]
		public bool Normal {
			get =>
				MoneyCostMultiple is 0.8f &&
				ExtraMoneyCostMultiple is 5 &&
				MoneyCostAtLeast is 200;
			set {
				if(value) {
					MoneyCostMultiple = 0.8f;
					ExtraMoneyCostMultiple = 5;
					MoneyCostAtLeast = 200;
				}
			}
		}
		
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

		[Increment(.01f)]
        [DefaultValue(0.8)]
        public float MoneyCostMultiple { get; set; }

        [Range(1, 100)]
        [DefaultValue(5)]
        public int ExtraMoneyCostMultiple { get; set; }

        [Range(0, 100000)]
        [DefaultValue(200)]
        public int MoneyCostAtLeast { get; set; }

		public List<ItemValuePair> CustomValue { get; set; } = new() {
			new(new(ItemID.DefenderMedal), 50000),
		};
		private Dictionary<ItemDefinition, int> customValue;
		public Dictionary<ItemDefinition, int> GetCustomValue() => customValue;

		public class ItemValuePair {
			public ItemDefinition Item;
			[Range(0, int.MaxValue)]
			public int Value;
            public ItemValuePair(ItemDefinition item, int value) {
                Item = item;
                Value = value;
            }
        }
        #endregion

        #region 复制名单设置
        [Header($"${_path}.Headers.DuplicateListSettings")]

        public bool CanDuplicateCoin { get; set; }

        public bool CanDuplicateBag { get; set; }

        public bool CanDuplicateRightClickable { get; set; }

        public bool CanDuplicateUnloadedItem { get; set; }

		public List<ItemDefinition> DuplicateBlacklist { get; set; } = new() {
			new(ItemID.WaterBucket),
			new(ItemID.LavaBucket),
			new(ItemID.HoneyBucket)
		};
		//{
        //    get => duplicateBlacklist;
        //    set => duplicateBlacklist = value;
        //}
        //private List<ItemDefinition> duplicateBlacklist = new() {
        //    new(ItemID.WaterBucket),
        //    new(ItemID.LavaBucket),
        //    new(ItemID.HoneyBucket)
        //};

		public List<ItemDefinition> DuplicateWhitelist { get; set; } = new() { };
		#endregion

		#region 其他设置
        [Header($"${_path}.Headers.OtherSettings")]
		
        public bool RepeatDuplicate { get; set; }

		public class SeeChangeLogClass {
			[CustomModConfigItem(typeof(ChangeLogDisplay))]
			public int ChangeLogDisplay;
		}

		[SeparatePage]
		public SeeChangeLogClass SeeChangeLog { get; set; } = new();

		public class ChangeLogDisplay : FloatElement {
			Asset<DynamicSpriteFont> _font;
			Asset<DynamicSpriteFont> Font => _font ??= FontAssets.MouseText;
			LocalizedText _changeLog;
			string ChangeLog {
				get {
					_changeLog ??= Language.GetText("Mods.TigerDuplicator.ChangeLog.Value");
					if(_changeLog.Value.Length > 0 && Font.IsLoaded && Parent != null) {
						Vector2 stringSize = Font.Value.MeasureString(_changeLog.Value);
						if(stringSize.Y != Height.Pixels || stringSize.X != Width.Pixels) {
							Height.Set(stringSize.Y, 0);
							Width.Set(stringSize.X, 0);
							Parent.Height.Set(stringSize.Y, 0);
							Parent.Width.Set(stringSize.Y, 0);
							//UIElement root = this;
							//while(root.Parent != null) {
							//	root = root.Parent;
							//}
							//root.Recalculate();
						}
					}
                    return _changeLog.Value;
                }
            }
            public override void OnBind() {
                base.OnBind();
				string _ = ChangeLog;
            }

            public override void Draw(SpriteBatch spriteBatch) {
				//base.Draw(spriteBatch);
				var dimensions = GetDimensions();
				spriteBatch.DrawString(Font.Value, ChangeLog, dimensions.Position(), Color.White);
            }
        }
        #endregion

        public override void OnChanged() {
			customValue ??= new();
			customValue.Clear();
			foreach (var pair in CustomValue) {
				customValue.Add(pair.Item, pair.Value);
			}
        }

        public override void OnLoaded()
        {
            TigerDuplicator.config = this;
        }
    }
}
