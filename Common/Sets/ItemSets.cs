using Terraria.ID;
using static Terraria.ID.ItemID;

namespace CanIShimmerThis.Common.Sets {
    public static class ItemSets {
        public static int[] RequiresMoonLordDefeatToShimmer = factory.CreateIntSet(-1,
            Clentaminator, Clentaminator2,
            RodofDiscord, RodOfHarmony,
            BottomlessBucket, BottomlessShimmerBucket,
            BottomlessShimmerBucket, BottomlessBucket
        );

        private static SetFactory factory => ItemID.Sets.Factory;
    }
}