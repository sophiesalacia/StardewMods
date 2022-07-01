using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InventoryRandomizer;

internal class TimeManager
{
    private static int SecondsUntilRandomization = Globals.Config.SecondsUntilInventoryRandomization;

    private static readonly List<ChatMessage> ChatMessages =
        Globals.ReflectionHelper.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();

    internal static void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
    {
        // only count down if save is loaded and player has control
        if (!Context.IsWorldReady || !Game1.player.CanMove)
            return;

        SecondsUntilRandomization--;

        switch (SecondsUntilRandomization)
        {
            // send chat message on 60 second intervals and at 30 seconds
            case > 0 when SecondsUntilRandomization % 60 == 0:
            case 30:
                Game1.chatBox.addInfoMessage($"Randomizing inventory in {SecondsUntilRandomization} seconds...");
                ChatMessages[^1].timeLeftToDisplay = 120;
                return;

            // send chat messages on final 5 seconds
            case > 0 and < 6:
                // clear my chat messages as they show up, to avoid clogging the chat
                ChatMessages.RemoveAll(chatMessage =>
                    chatMessage.message[0].message.Contains("Randomizing inventory"));
                Game1.chatBox.addInfoMessage($"Randomizing inventory in {SecondsUntilRandomization} seconds...");
                ChatMessages[^1].timeLeftToDisplay = 120;
                return;

            // randomize and reset timer at 0
            case <= 0:
                // if sound config is turned on, play sound
                Game1.playSound("cowboy_powerup");

                ChatMessages.RemoveAll(chatMessage =>
                    chatMessage.message[0].message.Contains("Randomizing inventory"));
                Game1.chatBox.addInfoMessage("Inventory randomized!");
                ChatMessages[^1].timeLeftToDisplay = 150;

                InventoryRandomizer.RandomizeInventory();
                SecondsUntilRandomization = Globals.Config.SecondsUntilInventoryRandomization;
                break;

            default:
                return;
        }
    }
}