using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CanIShimmerThis.Common.Configs; 

public class CISTModConfig : ModConfig {
    /// <summary>
    ///     Whether or not, if a given item can be shimmered, the tooltip will show what
    ///     the given item will shimmer/transmute into. Overrides <see cref="discoveryMode" />.
    /// </summary>
    [DefaultValue(false)]
    public bool forceShowOutcome;

    /// <summary>
    ///     Whether or not "discovery mode" is enabled, where tooltips only show the transmutation
    ///     outcome when the player has shimmered the given item previously.
    /// </summary>
    [DefaultValue(true)]
    public bool discoveryMode;

    public override ConfigScope Mode => ConfigScope.ClientSide;
}