using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CanIShimmerThis.Common.Configs;

public class CISTModConfig : ModConfig {
    /// <summary>
    ///     Whether or not "discovery mode" is enabled, where tooltips only show the transmutation
    ///     outcome when the player has shimmered the given item previously.
    /// </summary>
    [DefaultValue(true)]
    public bool discoveryMode;

    /// <summary>
    ///     Whether or not, if a given item can be shimmered, the tooltip will show what
    ///     the given item will shimmer/transmute into. Overrides <see cref="discoveryMode" />.
    /// </summary>
    [DefaultValue(false)]
    public bool forceShowOutcome;

    /// <summary>
    ///     Whether to display the additional tooltip that reveals what a given item will decraft into,
    ///     if it is shimmerable.
    /// </summary>
    [DefaultValue(false)]
    public bool showDecrafts;

    /// <summary>
    ///     Whether or not, if a given item can be decrafted, the tooltip will show what
    ///     the given item will decraft into. Overrides <see cref="discoveryMode" />.
    /// </summary>
    [DefaultValue(false)]
    public bool forceShowDecraftOutcome;

    /// <summary>
    ///     Whether the decraft tooltip, when revealing the list of decraft items, will have all the items on
    ///     one line (without their names).
    /// </summary>
    [DefaultValue(true)]
    public bool compactDecraftList;

    public override ConfigScope Mode => ConfigScope.ClientSide;
}