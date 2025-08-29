using System.Collections.Generic;
using System.Text;
using CanIShimmerThis.Common.Configs;
using CanIShimmerThis.Common.Players;
using CanIShimmerThis.Common.Sets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
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
    private static readonly Color TooltipColor = new (218, 181, 229);

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

        if (TryToApplyTransmutationTooltip(item, tooltips, config, discoveryPlayer)) {
            return;
        }

        TryToApplyDecraftTooltip(item, tooltips, config, discoveryPlayer);
    }

    private bool TryToApplyTransmutationTooltip(Item item, List<TooltipLine> tooltips, CISTModConfig config, DiscoveryPlayer discoveryPlayer) {
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

        string defaultShimmerText = GetCISTTextValue("Shimmerable");
        string transmuteText = null;
        if (itemTransformType != -1) {
            _shimmerItemDisplay.SetDefaults(itemTransformType);
            transmuteText = showTransmutationResult ? GetCISTTextValue("ShimmerableIntoItem", itemTransformType, _shimmerItemDisplay.Name) : defaultShimmerText;
        }
        else if (npcTransformType != -1) {
            _shimmerNPCDisplay.SetDefaults(npcTransformType);
            transmuteText = showTransmutationResult ? GetCISTTextValue("ShimmerableIntoNPC", _shimmerNPCDisplay.GivenOrTypeName) : defaultShimmerText;
        }
        else if (ItemID.Sets.CoinLuckValue[shimmerEquivalentType] is var coinLuck and > 0) {
            transmuteText = GetCISTTextValue("ShimmerCoinLuck", $"+{coinLuck:##,###}");
        }

        if (transmuteText is null) {
            return false;
        }

        tooltips.Add(
            new TooltipLine(Mod, "CanBeShimmered", transmuteText) { OverrideColor = TooltipColor }
        );

        return true;
    }

    private void TryToApplyDecraftTooltip(Item item, List<TooltipLine> tooltips, CISTModConfig config, DiscoveryPlayer discoveryPlayer) {
        if (!config.showDecrafts) {
            return;
        }

        int decraftIndex = ShimmerTransforms.GetDecraftingRecipeIndex(item.type);
        if (decraftIndex < 0) {
            return;
        }

        string decraftText = GetDecraftText(config.forceShowDecraftOutcome || (config.discoveryMode && discoveryPlayer.DiscoveredShimmers.Contains(item)), decraftIndex, config);
        if (decraftText is null) {
            return;
        }

        tooltips.Add(
            new TooltipLine(Mod, "CanBeDecrafted", decraftText) { OverrideColor = TooltipColor }
        );
    }

    private string GetDecraftText(bool showDecraftResult, int decraftIndex, CISTModConfig config) {
        string decraftText = GetCISTTextValue("Decraftable");
        if (!showDecraftResult) {
            return decraftText;
        }

        Recipe decraftRecipe = Main.recipe[decraftIndex];
        List<Item> decraftOutcome = decraftRecipe.customShimmerResults ?? decraftRecipe.requiredItem;
        if (decraftOutcome is null || decraftOutcome.Count == 0) {
            return null;
        }

        StringBuilder builder = new();
        if (!config.compactDecraftList) {
            for (int i = 0; i < decraftOutcome.Count - 1; i++) {
                Item decraftItem = decraftOutcome[i];

                _shimmerItemDisplay.SetDefaults(decraftItem.type);
                builder.AppendLine($"[i:{decraftItem.type}] ({_shimmerItemDisplay.Name})");
            }

            Item lastItem = decraftOutcome[^1];

            _shimmerItemDisplay.SetDefaults(lastItem.type);
            builder.Append($"[i:{lastItem.type}] ({_shimmerItemDisplay.Name})");

            decraftText = GetCISTTextValue("DecraftsInto", builder.ToString());
        }
        else {
            foreach (Item decraftItem in decraftOutcome) {
                _shimmerItemDisplay.SetDefaults(decraftItem.type);
                builder.Append($"[i:{decraftItem.type}]");
            }

            decraftText = GetCISTTextValue("DecraftsIntoCompact", builder.ToString());
        }

        return decraftText;
    }

    private string GetCISTTextValue(string suffix, params object[] args) => Language.GetTextValue($"Mods.CanIShimmerThis.{suffix}", args);
}