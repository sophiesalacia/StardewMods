using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace MoreTokens.Tokens;

internal class HardModeToken : SimpleToken
{
    public override string GetName() => "IsHardMode";

    public override IEnumerable<string> GetValue()
    {
        return !Context.IsWorldReady ? null : new[] {(Game1.netWorldState.Value.MinesDifficulty > 0).ToString()};
    }
}
