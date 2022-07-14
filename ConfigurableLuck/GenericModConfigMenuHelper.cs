using StardewModdingAPI;
using StardewValley;

namespace ConfigurableLuck;

internal class GenericModConfigMenuHelper
{

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            Globals.Manifest,
            () => Globals.Config = new ModConfig(),
            () =>
                {
                    Log.Trace($"Luck val: {Globals.Config.LuckValue}");
                    Globals.Helper.WriteConfig(Globals.Config);

                    // if in-game and mod is enabled, adjust luck value immediately
                    if (!Context.IsWorldReady || !Globals.Config.Enabled)
                        return;

                    Globals.Config.ApplyConfigChangesToGame();
                }
            );

        Globals.GmcmApi.AddBoolOption(
            Globals.Manifest,
            name: () => "Enabled",
            tooltip: () => "If this option is selected, your daily luck will be overridden with the selected value.",
            getValue: () => Globals.Config.Enabled,
            setValue: val => Globals.Config.Enabled = val
        );

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Luck Value",
            tooltip: () => "The value to override your luck with.",
            getValue: () => Globals.Config.LuckValue,
            setValue: val => Globals.Config.LuckValue = val,
            min: -0.12f,
            max: 0.12f,
            interval: 0.01f,
            formatValue: val =>
            {
                string formattedValue = val switch
                {
                    -0.12f => "Worst",
                    > -0.12f and < -0.07f => "Very Bad",
                    >= -0.07f and < -0.02f => "Bad",
                    >= -0.02f and <= 0.02f => "Neutral",
                    > 0.02f and <= 0.07f => "Good",
                    > 0.07f and < 0.12f => "Very Good",
                    0.12f => "Best",
                    _ => "Unknown"
                };
                formattedValue += $"\n({val})";
                return formattedValue;
            }
        );
    }
}
