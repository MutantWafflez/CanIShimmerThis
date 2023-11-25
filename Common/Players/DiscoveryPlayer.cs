using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CanIShimmerThis.Common.Players {
    public class DiscoveryPlayer : ModPlayer {
        public readonly struct HashableItem {
            public readonly Item item;

            public static implicit operator Item(HashableItem item) => item.item;

            public static implicit operator HashableItem(Item item) => new(item);

            public HashableItem(Item item) {
                this.item = item;
            }

            public override int GetHashCode() => item.type.GetHashCode();

            public override bool Equals(object obj) => obj is HashableItem hashItem && item.type == hashItem.item.type || obj is Item otherItem && item.type == otherItem.type;

            public override string ToString() => item.ToString();
        }

        /// <summary>
        /// All items that this player has shimmered.
        /// </summary>
        public HashSet<HashableItem> DiscoveredShimmers {
            get;
            private set;
        }

        public override void Initialize() {
            DiscoveredShimmers = new HashSet<HashableItem>();
        }

        public override void SaveData(TagCompound tag) {
            tag[nameof(DiscoveredShimmers)] = DiscoveredShimmers.Select(item => item.item).ToList();
        }

        public override void LoadData(TagCompound tag) {
            if (tag.TryGet(nameof(DiscoveredShimmers), out List<Item> discoveredShimmers)) {
                DiscoveredShimmers = discoveredShimmers.Select(item => new HashableItem(item)).ToHashSet();
            }
        }
    }
}