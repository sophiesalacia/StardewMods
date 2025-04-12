using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Minigames;

namespace DesertBus;

internal class BusJourney : IMinigame
{
    private Vector2 pamOffset = new(0f, 29f);
    private Vector2 busPosition = new (25f, 25f);
    private Rectangle busSource = new (288, 1247, 128, 64);

    private Texture2D desertTexture;
    private Texture2D stuffTexture;

    private Random desertRandom;

    public BusJourney()
    {
        desertTexture = Globals.GameContent.Load<Texture2D>("sophie.DesertBus/Desert");
        stuffTexture = Globals.GameContent.Load<Texture2D>("sophie.DesertBus/Stuff");

        // seeded random using save so desert is consistent per save
        // wouldn't want it to be different each time, that might be less mind-numbingly boring ;)
        // ReSharper disable once PossibleLossOfFraction
        desertRandom = Utility.CreateRandom(Game1.uniqueIDForThisGame / 2UL);


    }

    public bool tick(GameTime time)
    {
        return false;
    }

    public bool doMainGameUpdates()
    {
        return true;
    }

    public void draw(SpriteBatch b)
    {
        b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
        b.Draw(
            Game1.staminaRect,
            new Rectangle(0, 0, 800, 960),
            null,
            Color.Sienna,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0f);



        b.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, new Vector2((int) busPosition.X, (int) busPosition.Y)),
            busSource,
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            (busPosition.Y + 192f) / 10000f);

        Game1.player.faceDirection(3);
        Game1.player.blinkTimer = -1000;
        Game1.player.FarmerRenderer.draw(b,
            new FarmerSprite.AnimationFrame(117, 99999, 0, secondaryArm: false, flip: true), 117,
            new Rectangle(48, 608, 16, 32),
            Game1.GlobalToLocal(new Vector2((int) (busPosition.X + 4f), (int) (busPosition.Y - 8f)) + pamOffset * 4f),
            Vector2.Zero,
            (busPosition.Y + 192f + 4f) / 10000f,
            Color.White,
            0f,
            1f,
            Game1.player);

        b.Draw(
            Game1.mouseCursors2,
            Game1.GlobalToLocal(Game1.viewport, new Vector2((int) busPosition.X, (int) busPosition.Y - 40) + pamOffset * 4f),
            new Rectangle(0, 0, 21, 41),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            (busPosition.Y + 192f + 8f) / 10000f);





        b.End();
    }

    public void changeScreenSize()
    {
        //TODO_IMPLEMENT_ME();
    }

    public void unload()
    {
        //TODO_IMPLEMENT_ME();
    }


    public string minigameId()
    {
        return "desertbus";
    }

    public bool forceQuit()
    {
        return true;
    }
    
    #region Mouse logic

    public void receiveLeftClick(int x, int y, bool playSound = true) { }

    public void leftClickHeld(int x, int y) { }

    public void receiveRightClick(int x, int y, bool playSound = true) { }

    public void releaseLeftClick(int x, int y) { }

    public void releaseRightClick(int x, int y) { }
    
    public bool overrideFreeMouseMovement()
    {
        return Game1.options.SnappyMenus;
    }

    #endregion
    
    #region Keypress logic

    public void receiveKeyPress(Keys k)
    {
        //TODO_IMPLEMENT_ME();
    }

    public void receiveKeyRelease(Keys k)
    {
        //TODO_IMPLEMENT_ME();
    }
    
    #endregion

    public void receiveEventPoke(int data) { }
}
