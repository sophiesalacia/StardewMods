using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Delegates;
using StardewValley.Menus;
using StardewValley.Triggers;

namespace CustomAchievementsRedux;

[HasEventHooks]
public class Hooks
{
    internal static void InitHooks()
    {
        // when collections menu is opened, inject custom achievements
        Globals.EventHelper.Display.MenuChanged += AlterCollectionsMenu;

        // register trigger action, query, token
        Globals.EventHelper.GameLoop.GameLaunched += RegisterAction;
        Globals.EventHelper.GameLoop.GameLaunched += RegisterQuery;
        Globals.EventHelper.GameLoop.GameLaunched += RegisterToken;

        // content pipeline
        Globals.EventHelper.Content.AssetReady += OnAssetReady;
        Globals.EventHelper.Content.AssetRequested += OnAssetRequested;
    }

    private static void RegisterToken(object? sender, GameLaunchedEventArgs e)
    {
        Globals.ContentPatcherApi = Globals.ModRegistry.GetApi<IContentPatcherApi>("Pathoschild.ContentPatcher");

        if (Globals.ContentPatcherApi is null)
        {
            Log.Info("Unable to load Content Patcher API; skipping token registration.");
            return;
        }

        Globals.ContentPatcherApi.RegisterToken(Globals.Manifest, "PlayerHasAchievement", new HasAchievementToken());
    }

    private static void RegisterQuery(object? sender, GameLaunchedEventArgs e)
    {
        GameStateQuery.Register("sophie.CustomAchievementsRedux_PlayerHasAchievement", PlayerHasAchievementQuery);
    }

    private static bool PlayerHasAchievementQuery(string[] query, GameStateQueryContext context)
    {
        if (!ArgUtility.TryGet(query, 1, out string playerKey, out string error, allowBlank: false))
        {
            return GameStateQuery.Helpers.ErrorResult(query, error);
        }

        return GameStateQuery.Helpers.WithPlayer(context.Player, playerKey, farmer =>
            {
                if (!ArgUtility.TryGetRemainder(query, 2, out string? achievementArg, out error))
                {
                    return GameStateQuery.Helpers.ErrorResult(query, error);
                }

                if (achievementArg is null)
                {
                    error = "The query must be provided at least one achievement to check for.";
                    return GameStateQuery.Helpers.ErrorResult(query, error);
                }

                string[] achievementsToCheck = ArgUtility.SplitBySpaceQuoteAware(achievementArg);

                return achievementsToCheck.All(achievementToCheck => Manager.DoesPlayerHaveAchievement(farmer, achievementToCheck));
            }
        );
    }

    private static void RegisterAction(object? sender, GameLaunchedEventArgs e)
    {
        TriggerActionManager.RegisterAction("sophie.CustomAchievementsRedux/GiveAchievement", GiveAchievementAction);
    }

    private static bool GiveAchievementAction(string[] args, TriggerActionContext context, out string error)
    {
        string achievementId = ArgUtility.Get(args, 1);

        if (string.IsNullOrEmpty(achievementId))
        {
            error = $"Action \"{args[0]}\" failed with args {string.Join(' ', args)}: Achievement ID cannot be null or empty.";
            return false;
        }

        if (!Manager.AchievementsData.Any(col => col.Value.Achievements.Any(ach => ach.FullId == achievementId)))
        {
            error = $"Action \"{args[0]}\" failed with args {string.Join(' ', args)}: Achievement with full ID \"{achievementId}\" not found.";
            return false;
        }

        Manager.SetAchievement(Game1.player, achievementId);
        error = null;
        return true;
    }

    private static void AlterCollectionsMenu(object? sender, MenuChangedEventArgs e)
    {
        if (e.NewMenu is not GameMenu gameMenu)
            return;

        CollectionsPage menu = (CollectionsPage)gameMenu.pages[7];

        Manager.AchievementsData = Globals.GameContent.Load<Dictionary<string, AchievementCollection>>(Manager.AchievementsAssetName.Name);

        int baseX = menu.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
        //int baseY = menu.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;
        int baseY = menu.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 48;
        int widthUsed = 0;
        const int collectionWidth = 10;

        int page = 1;
        foreach (AchievementCollection collection in Manager.AchievementsData.OrderBy(id => id.Key).Select(kvp => kvp.Value))
        {
            menu.collections[5].Add([]);

            foreach (Achievement achievement in collection.Achievements)
            {
                achievement.ShouldReveal();

                int xPos3 = baseX + widthUsed % collectionWidth * 68;
                int yPos3 = baseY + widthUsed / collectionWidth * 68;
                widthUsed++;

                if (achievement is {DrawCustomBase: true, BaseSpriteIndex: not null})
                {
                    menu.collections[5][page].Add(
                        new ClickableTextureComponent(achievement.FullId,
                            new Rectangle(xPos3, yPos3, 64, 64),
                            null,
                            "",
                            achievement.TextureSheet,
                            Game1.getSourceRectForStandardTileSheet(achievement.TextureSheet, (int)achievement.BaseSpriteIndex, 16, 16),
                            4f)
                    );
                }
                else
                {
                    menu.collections[5][page].Add(
                        new ClickableTextureComponent(achievement.FullId,
                            new Rectangle(xPos3, yPos3, 64, 64),
                            null,
                            "",
                            Game1.mouseCursors,
                            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25),
                            1f)
                    );
                }
            }

            collection.Page = page;
            page++;
            widthUsed = 0;
        }
    }

    private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Manager.AchievementsAssetName))
        {
            e.LoadFrom(() => new Dictionary<string, AchievementCollection>(), AssetLoadPriority.Low);
        }
    }

    private static void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo(Manager.AchievementsAssetName))
            return;

        Manager.AchievementsData = Globals.GameContent.Load<Dictionary<string, AchievementCollection>>(Manager.AchievementsAssetName);
        Manager.ValidateAchievementsData(Manager.AchievementsData);
    }
}
