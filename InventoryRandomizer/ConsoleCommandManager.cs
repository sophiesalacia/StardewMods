namespace InventoryRandomizer;

internal class ConsoleCommandManager
{
    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add("sophie.ir.randomize", "Randomizes inventory.",
            (_, _) => InventoryRandomizer.RandomizeInventory());
    }
}
