using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ThankYouCA
{
    internal class ThankYouMenu : JunimoNoteMenu
    {
        public ThankYouMenu(bool fromGameMenu, int area = 1, bool fromThisMenu = false) : base(fromGameMenu, area, fromThisMenu) { }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black);
            b.Draw(noteTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(0, 0, 320, 180), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
            SpriteText.drawStringHorizontallyCenteredAt(b, "To ConcernedApe:", xPositionOnScreen + width / 2 + 16, yPositionOnScreen + 12, 999999, -1, 99999, 0.88f);

            string parsed = Game1.parseText(GetThankYouText(), Game1.smallFont, 1060); //(int)Game1.tinyFont.MeasureString(ThankYouCa).X);
            //SpriteText.drawStringHorizontallyCenteredAt(b, parsed, xPositionOnScreen + width / 2 + 16, yPositionOnScreen + 120);

            int stringWidth = (int)Game1.smallFont.MeasureString(parsed).X;
            
            Vector2 pos = new(xPositionOnScreen + width / 2 - stringWidth / 2, yPositionOnScreen + 106);

            b.DrawString(Game1.smallFont, parsed, pos, new Color(86, 22, 12), 0f, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0.12f);
            b.DrawString(Game1.smallFont, parsed, pos + new Vector2(0, 1), new Color(86, 22, 12) * 0.33f, 0f, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0.12f);
            b.DrawString(Game1.smallFont, parsed, pos + new Vector2(1, 1), new Color(86, 22, 12) * 0.33f, 0f, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0.12f);

            SpriteText.drawStringWithScrollCenteredAt(b, "from sophie", xPositionOnScreen + width / 2, Math.Min(yPositionOnScreen + height + 20, Game1.uiViewport.Height - 64 - 8));


        }

        public string GetThankYouText()
        {
            return "     Thank you for everything you do for the Stardew Valley community. It's clear that this amazing thing you've created " +
                   "will always hold a special place in your heart, as it does for so many thousands of players - you're under no obligation to keep " +
                   "expanding it and refining it, and yet you've continued to do so, all without asking a single extra cent." +
                   "\n\n" +
                   "    It has been a delight to be able to take part in the 1.6 beta, both as a player and as an aspiring game developer. To see the " +
                   "thought process behind implementing certain features and to be able to contribute suggestions, feedback, and bug reports; " +
                   "to have a place where I could go to talk about the fun and exciting new things happening in mine and my partner's game; to have " +
                   "changefrogs to look forward to every night; and to see the love and attention which you put into your work. Being able to witness and " +
                   "take part in the creative process has rekindled my love of my own creative endeavours and inspired me to work harder on my projects." +
                   "\n\n" +
                   "    I've also had the opportunity to meet some amazing people through the beta and create lasting friendships and that is something " +
                   "for which I'm incredibly grateful." +
                   "\n\n" +
                   "Thank you, ConcernedApe and the rest of the team! I can't wait to see what you do next :)";
        }
    }
}
