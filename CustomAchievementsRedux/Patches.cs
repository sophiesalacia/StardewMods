using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace CustomAchievementsRedux;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0060 // Remove unused parameter

[HarmonyPatch]
public class Patches
{
    
    // patch CollectionsPage.createDescription
    [HarmonyPatch(nameof(CollectionsPage), nameof(CollectionsPage.createDescription))]
    [HarmonyPrefix]
    public static bool createDescription_Prefix(CollectionsPage __instance, string id, ref string __result)
    {
        string[] splitId = id.Split('/');
        
        if (splitId.Length != 2 || !Manager.AchievementsData.TryGetValue(splitId[0], out AchievementCollection collection))
            return true;
        
        Achievement achievement = collection.Achievements.Find(a => a.Id == splitId[1]);

        if (achievement is null)
            return true;

        achievement.ShouldReveal();

        if (Manager.DoesPlayerHaveAchievement(Game1.player, id))
        {
            __result = achievement.DisplayName;
            if (!string.IsNullOrEmpty(achievement.ObtainedDescription))
                __result += Environment.NewLine + Environment.NewLine + achievement.ObtainedDescription;
        }
        else if (achievement.Revealed)
        {
            __result = achievement.DisplayName;
            if (!string.IsNullOrEmpty(achievement.UnobtainedDescription))
                __result += Environment.NewLine + Environment.NewLine + achievement.UnobtainedDescription;
        }
        else
        {
            __result = "???";
            if (!string.IsNullOrEmpty(achievement.LockedDescription))
                __result += Environment.NewLine + Environment.NewLine + achievement.LockedDescription;
        }

        return false;
    }
    
    //// transpiler to make custom icon scale increase on hover match vanilla icon scale increase
    //[HarmonyPatch(nameof(CollectionsPage), nameof(CollectionsPage.performHoverAction))]
    //[HarmonyTranspiler]
    //public static IEnumerable<CodeInstruction> performHoverAction_Transpiler(IEnumerable<CodeInstruction> instructions)
    //{

    //    List<CodeInstruction> origInstructions = [..instructions]; // store unaltered instructions in case anything goes wrong
    //    CodeMatcher matcher = new(instructions);

    //    matcher.MatchEndForward(
    //        new CodeMatch(OpCodes.Ldarg_1),
    //        new CodeMatch(OpCodes.Ldarg_2),
    //        new CodeMatch(OpCodes.Ldc_I4_2),
    //        new CodeMatch(OpCodes.Callvirt),
    //        new CodeMatch(OpCodes.Brfalse)
    //    );

    // gave up halfway through writing this because it would be so fucking annoying and it's just to fix a tiny visual thing (the scale for custom icons doesn't increase as much as the vanilla icon when hovered over)
    // because the vanilla icon is drawn at 1x and the custom icons are drawn at 4x, and its an additive increase to the scale when you hover
    // thanks CA
    // im just going to use this hacky postfix and say fuck it. good enough

    [HarmonyPatch(nameof(CollectionsPage), nameof(CollectionsPage.performHoverAction))]
    [HarmonyPostfix]
    public static void performHoverAction_Postfix(CollectionsPage __instance, int x, int y)
    {
        if (__instance.currentTab != 5 || __instance.currentPage < 1)
            return;

        foreach (ClickableTextureComponent c in __instance.collections[__instance.currentTab][__instance.currentPage])
        {
            if (c.containsPoint(x, y, 2) && Math.Abs(c.baseScale - 4f) < 0.01f)
            {
                c.scale = Math.Min(c.scale + 0.4f, c.baseScale + 0.4f);
            }
            else if (Math.Abs(c.baseScale - 4f) < 0.01f)
            {
                c.scale = Math.Max(c.scale - 0.06f, c.baseScale);
            }
        }
    }

    // patch draw - custom draw logic if we're on collections[5][1+]
    [HarmonyPatch(nameof(CollectionsPage), nameof(CollectionsPage.draw))]
    [HarmonyPrefix]
    public static bool draw_Prefix(CollectionsPage __instance, SpriteBatch b, ref string ___hoverText)
    {
        if (__instance.currentTab != 5 || __instance.currentPage < 1)
            return true;

        // do custom draw stuff
        foreach (ClickableTextureComponent value in __instance.sideTabs.Values)
        {
            value.draw(b);
        }
        __instance.backButton.draw(b);
        if (__instance.currentPage < __instance.collections[__instance.currentTab].Count - 1)
        {
            __instance.forwardButton.draw(b);
        }

        string pageLabel = Manager.AchievementsData.Values.FirstOrDefault(col => col.Page == __instance.currentPage)?.DisplayName ?? "";

        int labelX = __instance.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
        int labelY = __instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;

        //Utility.drawTextWithShadow(
        //    b,
        //    pageLabel,
        //    Game1.dialogueFont,
        //    new Vector2(labelX, labelY),
        //Game1.textColor,
        //1f,
        //0.1f);

        SpriteText.drawString(b,
            pageLabel,
            labelX,
            labelY,
            999,
            -1,
            999,
            1f,
            0.1f);

        b.End();
        b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

        foreach (ClickableTextureComponent c in __instance.collections[__instance.currentTab][__instance.currentPage])
        {
            string[] splitId = c.name.Split('/');
        
            if (splitId.Length != 2 || !Manager.AchievementsData.TryGetValue(splitId[0], out AchievementCollection collection))
                continue;
        
            Achievement achievement = collection.Achievements.Find(a => a.Id == splitId[1]);

            if (achievement is null)
                continue;

            Color blendColor = Color.White;

            if (!Manager.DoesPlayerHaveAchievement(Game1.player, c.name))
            {
                if (achievement.Revealed)
                    blendColor = Color.DimGray * 0.4f;
                else
                    blendColor = Color.Black * 0.2f;

                c.draw(b, blendColor, 0.86f);
            }
            else
            {
                c.draw(b, blendColor, 0.86f);

                // draw face if provided
                if (!achievement.DrawOverlay || achievement.OverlaySpriteIndex is null)
                    continue;

                int index = achievement.OverlaySpriteIndex ?? -1;

                if (index < 0)
                    continue;

                int xPos = index * 16 % achievement.TextureSheet.Width;
                int yPos = index * 16 / achievement.TextureSheet.Width * 16;

                float scaleFactor = achievement.DrawCustomBase ? 1f : 4f;

                b.Draw(achievement.TextureSheet,
                    new Vector2(c.bounds.X + c.bounds.Width / 2, c.bounds.Y + c.bounds.Height / 2),
                    new Rectangle(xPos, yPos, 16, 16),
                    Color.White,
                    0f,
                    new Vector2(8f, 8f),
                    scaleFactor * c.scale,
                    SpriteEffects.None,
                    0.88f);
                //int startPos = Utility.CreateDaySaveRandom().Next(12);
                //b.Draw(Game1.mouseCursors, new Vector2(c.bounds.X + 16 + 16, c.bounds.Y + 20 + 16), new Rectangle(256 + startPos % 6 * 64 / 2, 128 + startPos / 6 * 64 / 2, 32, 32), Color.White, 0f, new Vector2(16f, 16f), c.scale, SpriteEffects.None, 0.88f);
            }
        }

        b.End();
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        if (!string.IsNullOrEmpty(___hoverText))
        {
            IClickableMenu.drawHoverText(b, ___hoverText, Game1.smallFont);
        }

        return false;
    }
}

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0060 // Remove unused parameter
