namespace Umbrellas;

internal class GenericModConfigMenuHelper
{

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            mod: Globals.Manifest,
            reset: () => Globals.Config = new ModConfig(),
            save: () => Globals.Helper.WriteConfig(Globals.Config)
        );

        //Globals.GmcmApi.AddBoolOption(
        //    mod: Globals.Manifest,
        //    name: () => "Draw Simplified Gem Bubbles",
        //    tooltip: () => "If enabled, crystalariums will simply show the gem floating above them when ready.",
        //    getValue: () => Globals.Config.DrawSimplifiedGemBubbles,
        //    setValue: val => Globals.Config.DrawSimplifiedGemBubbles = val
        //);
    }
}
