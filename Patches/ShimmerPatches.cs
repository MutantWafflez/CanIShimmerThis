using CanIShimmerThis.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CanIShimmerThis.Patches; 

public class ShimmerPatches : ILoadable {
    public void Load(Mod mod) {
        On_Item.GetShimmered += GetShimmered;
    }

    public void Unload() { }

    private void GetShimmered(On_Item.orig_GetShimmered orig, Item self) {
        if (Main.netMode != NetmodeID.Server) {
            Main.LocalPlayer.GetModPlayer<DiscoveryPlayer>().DiscoveredShimmers.Add(new Item(self.type));
        }

        orig(self);
    }
}