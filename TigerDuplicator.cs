using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;

namespace TigerDuplicator
{
	public class TigerDuplicator : Mod {
        internal static TigerDuplicatorConfig config;

        public override void Unload() {
            config = null;
        }
    }
}