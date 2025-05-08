using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace Calcifer.Features;

/// <summary>Allow any furniture to become a TV via context tag, and adjust position/scale of the screen</summary>
[HarmonyPatch]
class CustomTVFurniture
{
    /// <summary>Screen shape defines where the TV shows up, aspect ratio is still fixed at 42x28</summary>
    private record TVScreenShape(float PosX, float PosY, float Scale);
    private static readonly ConditionalWeakTable<TV, TVScreenShape?> TVScreens = [];

    /// <summary>TV context tag pattern, calcifer_tv_x(xpos)y(ypos)s(scale)</summary>
    private static readonly Regex TVContextTag = new Regex(@"^calcifer_tv_x(-?\d+(?:\.\d+)?)y(-?\d+(?:\.\d+)?)s(-?\d+(?:\.\d+)?)$");

    /// <summary>Make furniture a TV if they have matching context tag</summary>
    [HarmonyPatch(typeof(Furniture), nameof(Furniture.GetFurnitureInstance))]
    [HarmonyPostfix]
    public static void Furniture_GetFurnitureInstance_Postfix(string itemId, ref Furniture __result)
    {
        if (__result is TV)
            return;
        foreach (string tag in __result.GetContextTags())
        {
            if (TVContextTag.IsMatch(tag))
            {
                __result = new TV(itemId, __result.TileLocation);
                return;
            }
        }
    }

    /// <summary>Parse context tag into TV screen shape</summary>
    private static TVScreenShape? GetTVScreenShape(TV tv)
    {
        foreach (string tag in tv.GetContextTags())
        {
            if (TVContextTag.Match(tag) is Match tvTag && tvTag.Success &&
                float.TryParse(tvTag.Groups[1].Value, out float posX) &&
                float.TryParse(tvTag.Groups[2].Value, out float posY) &&
                float.TryParse(tvTag.Groups[3].Value, out float scale))
            {
                return new TVScreenShape(posX, posY, scale); ;
            }
        }
        return null;
    }

    /// <summary>Adjust screen position</summary>
    [HarmonyPatch(typeof(TV), nameof(TV.getScreenPosition))]
    [HarmonyPostfix]
    public static void TV_getScreenPosition_Postfix(TV __instance, ref Vector2 __result)
    {
        if (TVScreens.GetValue(__instance, GetTVScreenShape) is TVScreenShape shape)
        {
            __result = new(__instance.boundingBox.X + shape.PosX, __instance.boundingBox.Y + shape.PosY);
        }
    }

    /// <summary>Adjust screen scale</summary>
    [HarmonyPatch(typeof(TV), nameof(TV.getScreenSizeModifier))]
    [HarmonyPostfix]
    public static void TV_getScreenSizeModifier_Postfix(TV __instance, ref float __result)
    {
        if (TVScreens.GetValue(__instance, GetTVScreenShape) is TVScreenShape shape)
        {
            __result = shape.Scale;
        }
    }

}