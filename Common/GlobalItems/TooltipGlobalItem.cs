using System.Collections.Generic;
using CanIShimmerThis.Common.Configs;
using CanIShimmerThis.Common.Players;
using CanIShimmerThis.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CanIShimmerThis.Common.GlobalItems; 

/// <summary>
///     Main class of the mod. Adds a tooltip line to each item that
///     can be shimmered into something else.
/// </summary>
public class TooltipGlobalItem : GlobalItem {
    private static Item _shimmerItemDisplay;
    private static NPC _shimmerNPCDisplay;

    public override void SetStaticDefaults() {
        _shimmerItemDisplay = new Item();
        _shimmerNPCDisplay = new NPC();
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (!item.CanShimmer()) {
            return;
        }

        CISTModConfig config = ModContent.GetInstance<CISTModConfig>();
        DiscoveryPlayer discoveryPlayer = Main.LocalPlayer.GetModPlayer<DiscoveryPlayer>();

        int shimmerEquivalentType = ItemID.Sets.ShimmerCountsAsItem[item.type] is not -1 and var countsAsType ? countsAsType : item.type;
        int itemTransformType = ItemID.Sets.ShimmerTransformToItem[shimmerEquivalentType];
        // Vanilla hard-code I can't do a whole lot about; so here it is repeated
        int npcTransformType = -1;
        if (shimmerEquivalentType == ItemID.GelBalloon && !NPC.unlockedSlimeRainbowSpawn) {
            npcTransformType = NPCID.TownSlimeRainbow;
        }
        else if (item.makeNPC > NPCID.None) {
            npcTransformType = NPCID.Sets.ShimmerTransformToNPC[item.makeNPC] is var transformType and not -1 ? transformType : item.makeNPC;
        }

        // More if-else Vanilla hard-code
        if (ItemSets.RequiresMoonLordDefeatToShimmer[shimmerEquivalentType] is not -1 and var moonLordTransformType) {
            itemTransformType = moonLordTransformType;
        }
        else if (shimmerEquivalentType == ItemID.LunarBrick) {
            itemTransformType = Main.GetMoonPhase() switch {
                MoonPhase.QuarterAtRight => ItemID.StarRoyaleBrick,
                MoonPhase.HalfAtRight => ItemID.CryocoreBrick,
                MoonPhase.ThreeQuartersAtRight => ItemID.CosmicEmberBrick,
                MoonPhase.Full => ItemID.HeavenforgeBrick,
                MoonPhase.ThreeQuartersAtLeft => ItemID.LunarRustBrick,
                MoonPhase.HalfAtLeft => ItemID.AstraBrick,
                MoonPhase.QuarterAtLeft => ItemID.DarkCelestialBrick,
                _ => ItemID.MercuryBrick
            };
        }
        else if (item.createTile == TileID.MusicBoxes) {
            itemTransformType = ItemID.MusicBox;
        }

        bool showTransmutationResult = config.forceShowOutcome || (config.discoveryMode && discoveryPlayer.DiscoveredShimmers.Contains(item));
        string tooltipText = GetCISTTextValue("Shimmerable");
        if (itemTransformType != -1) {
            _shimmerItemDisplay.SetDefaults(itemTransformType);
            tooltipText = showTransmutationResult ? GetCISTTextValue("ShimmerableIntoItem", itemTransformType, _shimmerItemDisplay.Name) : tooltipText;
        }
        else if (npcTransformType != -1) {
            _shimmerNPCDisplay.SetDefaults(npcTransformType);
            tooltipText = showTransmutationResult ? GetCISTTextValue("ShimmerableIntoNPC", _shimmerNPCDisplay.GivenOrTypeName) : tooltipText;
        }
        else if (ItemID.Sets.CoinLuckValue[shimmerEquivalentType] is var coinLuck and > 0) {
            tooltipText = GetCISTTextValue("ShimmerCoinLuck", $"+{coinLuck:##,###}");
        }
        else {
            return;
        }

        tooltips.Add(
            new TooltipLine(Mod, "CanBeShimmered", tooltipText) { OverrideColor = new Color(218, 181, 229) }
        );
    }

    private string GetCISTTextValue(string suffix, params object[] args) => Language.GetTextValue($"Mods.CanIShimmerThis.{suffix}", args);
}