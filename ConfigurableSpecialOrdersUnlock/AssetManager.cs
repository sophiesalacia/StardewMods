using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConfigurableSpecialOrdersUnlock;

internal class AssetManager
{
    [EventPriority(EventPriority.Low)]
    internal static void LoadOrEditAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Town"))
            e.Edit(EditSpecialOrdersInstallationEvent);
    }

    private static void EditSpecialOrdersInstallationEvent(IAssetData asset)
    {
        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

        int unlockDays = Globals.Config.GetUnlockDaysPlayed();

        if (unlockDays - 1 == 57)
            return;

        data[$"15389722/j {unlockDays - 1}"] = data["15389722/j 57"];
        data.Remove("15389722/j 57");
    }
}
