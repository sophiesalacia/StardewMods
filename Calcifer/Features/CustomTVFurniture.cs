using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace Calcifer.Features;

[HasEventHooks]
[HarmonyPatch]
class CustomTVFurniture
{
    internal const string TVScreenX = "sophie.Calcifer/TV.X";
    public const string TVScreenY = "sophie.Calcifer/TV.Y";
    public const string TVScale = "sophie.Calcifer/TV.S";

    private record TVScreenShape(float PosX, float PosY, float Scale);
    private static readonly ConditionalWeakTable<TV, TVScreenShape?> TVScreens = [];

    private static readonly Regex TVContextTag = new Regex(@"^calcifer_tv_x(-?\d+(?:\.\d+)?)y(-?\d+(?:\.\d+)?)s(-?\d+(?:\.\d+)?)$");

    [HarmonyPatch(typeof(Furniture), nameof(Furniture.GetFurnitureInstance))]
    [HarmonyPostfix]
    public static void Furniture_GetFurnitureInstance_Postfix(string itemId, ref Furniture __result)
    {
        foreach (string tag in __result.GetContextTags())
        {
            if (TVContextTag.Match(tag) is Match tvTag && tvTag.Success)
            {
                TV newTV = new TV(itemId, __result.TileLocation);
                newTV.modData[TVScreenX] = tvTag.Groups[1].Value;
                newTV.modData[TVScreenY] = tvTag.Groups[2].Value;
                newTV.modData[TVScale] = tvTag.Groups[3].Value;
                __result = newTV;
                return;
            }
        }
    }

    private static TVScreenShape? GetTVScreenShape(TV tv)
    {
        if (tv.modData.TryGetValue(TVScreenX, out string strX) && float.TryParse(strX, out float posX) &&
            tv.modData.TryGetValue(TVScreenY, out string strY) && float.TryParse(strY, out float posY) &&
            tv.modData.TryGetValue(TVScale, out string strScale) && float.TryParse(strScale, out float scale))
        {
            var res = new TVScreenShape(posX, posY, scale);
            return res;
        }
        return null;
    }

    [HarmonyPatch(typeof(TV), nameof(TV.getScreenPosition))]
    [HarmonyPostfix]
    public static void TV_getScreenPosition_Postfix(TV __instance, ref Vector2 __result)
    {
        if (TVScreens.GetValue(__instance, GetTVScreenShape) is TVScreenShape shape)
        {
            __result = new(__instance.boundingBox.X + shape.PosX, __instance.boundingBox.Y + shape.PosY);
        }
    }

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