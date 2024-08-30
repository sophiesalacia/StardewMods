using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calcifer.Features.ContentPatcherTokens;

internal class ShortDayNameToken
{
    internal static void RegisterTokens()
    {
        if (Globals.ContentPatcherApi is null)
            return;

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "ShortDayName", () =>
            {
                // world is initialized
                if (Context.IsWorldReady)
                    return new[] { Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth) };

                // no save loaded (e.g. on the title screen)
                return Enumerable.Empty<string>();
            });
    }
}
