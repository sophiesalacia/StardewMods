using Microsoft.Xna.Framework.Graphics;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;

namespace CustomAchievementsRedux;

public class Manager
{
    public static readonly IAssetName AchievementsAssetName = Globals.GameContent.ParseAssetName("sophie.CustomAchievementsRedux/Achievements");
    private static Dictionary<string, AchievementCollection> _achievementsData;

    internal static string GettingAchievement = "";
    internal static Dictionary<string, AchievementCollection> AchievementsData
    {
        get =>
            _achievementsData ??=
                Globals.GameContent.Load<Dictionary<string, AchievementCollection>>(AchievementsAssetName);
        set => _achievementsData = value;
    }

    public static void SetAchievement(Farmer player, string achievementId, bool obtained = true)
    {
        if (DoesPlayerHaveAchievement(player, achievementId))
            return;

        Achievement? achievement = GetAchievementFromId(achievementId);
        
        if (achievement is null)
        {
            Log.Warn($"Attempted to set achievement \"{achievementId}\" which could not be matched to an existing achievement.");
            return;
        }

        player.modData["sophie.CustomAchievementsRedux/Achievements/" + achievementId] = obtained.ToString();

        if (!obtained)
            return;

        string farmerName = Game1.player.Name;
        if (farmerName == "")
        {
            farmerName = TokenStringBuilder.LocalizedText("Strings\\UI:Chat_PlayerJoinedNewName");
        }
        Game1.Multiplayer.globalChatInfoMessage("Achievement", farmerName, achievement.DisplayName);

        Game1.playSound("achievement");

        Game1.addHUDMessage(HUDMessage.ForAchievement(achievement.DisplayName));
        Game1.player.autoGenerateActiveDialogueEvent("achievement_" + achievement.FullId.Replace('/', '_'));    // cant remember if slashes in CTs are problematic
        GettingAchievement = achievement.FullId;    // setting this for special query
        TriggerActionManager.Raise("sophie.CustomAchievementsRedux/AchievementObtained", [achievement.FullId], Game1.currentLocation, Game1.player);
        GettingAchievement = "";
    }

    public static bool DoesPlayerHaveAchievement(Farmer player, string achievementId)
    {
        return player.modData.TryGetValue("sophie.CustomAchievementsRedux/Achievements/" + achievementId, out string obtained) && bool.TryParse(obtained, out bool obtainedParsed) && obtainedParsed;
    }

    public static Achievement? GetAchievementFromId(string fullId)
    {
        string[] splitId = fullId.Split('/');
        
        if (splitId.Length != 2 || !AchievementsData.TryGetValue(splitId[0], out AchievementCollection? collection) || collection is null)
        {
            return null;
        }
        
        return collection.Achievements.Find(a => a.Id == splitId[1]);
    }

    internal static void ValidateAchievementsData(Dictionary<string, AchievementCollection> achievementsData)
    {
        foreach ((string collectionId, AchievementCollection collection) in achievementsData)
        {
            List<Achievement> toRemove = [];

            if (collection.DisplayName == "")
                collection.DisplayName = collectionId;

            foreach (Achievement achievement in collection.Achievements)
            {
                if (string.IsNullOrEmpty(achievement.Id))
                {
                    Log.Error($"Ignoring achievement with no ID in pack \"{collectionId}\".");
                    toRemove.Add(achievement);
                    continue;
                }

                if (achievement.DisplayName == "")
                {
                    achievement.DisplayName = achievement.Id;
                }

                achievement.FullId = collectionId + "/" + achievement.Id;

                achievement.DrawCustomBase = false;
                achievement.DrawOverlay = false;

                if (string.IsNullOrEmpty(achievement.Texture) || !TryLoadTexture(achievement.Id, achievement.Texture, out Texture2D texture) || texture is null)
                    continue;
                
                achievement.TextureSheet = texture;

                if (achievement.BaseSpriteIndex is not null)
                {
                    achievement.DrawCustomBase = true;
                }

                if (achievement.OverlaySpriteIndex is not null)
                {
                    achievement.DrawOverlay = true;
                }
            }

            collection.Achievements.RemoveAll(achievement => toRemove.Contains(achievement));
        }
    }


    private static bool TryLoadTexture(string achievementId, string textureName, out Texture2D? texture)
    {
        try
        {
            if (Game1.content.DoesAssetExist<Texture2D>(textureName))
            {
                texture = Game1.content.Load<Texture2D>(textureName);
                return true;
            }

            Log.Error($"Failed loading texture \"{textureName}\" for achievement \"{achievementId}\": asset doesn't exist.");
            texture = null;
            return false;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed loading texture \"{textureName}\" for achievement \"{achievementId}\": {ex}");
            texture = null;
            return false;
        }
    }

}
