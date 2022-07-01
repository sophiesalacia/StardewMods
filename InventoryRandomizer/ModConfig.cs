using System;

namespace InventoryRandomizer;

class ModConfig
{
    // General Options
    public int SecondsUntilInventoryRandomization = 300;
    public bool PlaySoundOnRandomization = true;
    public bool ChatMessageAlerts = true;

    // Weights
    public float RecipeChance = 0.25f;

    public int BigCraftablesWeight = 3;
    public int BootsWeight = 1;
    public int ClothingWeight = 1;
    public int FurnitureWeight = 2;
    public int HatsWeight = 1;
    public int ObjectsWeight = 5;
    public int WeaponsWeight = 2;
    public int ToolsWeight = 3;

    internal int GetTotalWeight()
    {
        return BigCraftablesWeight + BootsWeight + ClothingWeight + FurnitureWeight + HatsWeight + ObjectsWeight +
               WeaponsWeight + ToolsWeight;
    }

}
