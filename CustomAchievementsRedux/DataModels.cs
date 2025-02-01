using Microsoft.Xna.Framework.Graphics;

namespace CustomAchievementsRedux;
public class Achievement
{
    public string Id = "";
    public string DisplayName = "";
    public string ObtainedDescription = "";
    public string UnobtainedDescription = "";
    public string RevealCondition = "";
    public string LockedDescription = "";
    public string Texture = "";
    public int? BaseSpriteIndex;
    public int? OverlaySpriteIndex;

    internal Texture2D? TextureSheet;
    internal bool Revealed;
    internal string FullId = "";
    internal AchievementCollection Collection;
    internal bool DrawCustomBase;
    internal bool DrawOverlay;

    internal void ShouldReveal()
    {
        Revealed = GameStateQuery.CheckConditions(RevealCondition) || Manager.DoesPlayerHaveAchievement(Game1.player, FullId);
    }
}

public class AchievementCollection
{
    public string DisplayName = "";
    public List<Achievement> Achievements = [];

    internal int Page;
}
