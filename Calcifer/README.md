# Calcifer
*by sophie* 

[NexusMods](https://www.nexusmods.com/stardewvalley/mods/20628)  
[GitHub](https://github.com/sophiesalacia/StardewMods/tree/main/Calcifer)

### Table of Contents

* [Custom Categories](#custom-categories)  
* [Furniture Actions](#furniture-actions)  
  * [Tile Actions](#tile-actions)  
  * [Trigger Actions](#trigger-actions)
* [Furniture Offsets](#furniture-offsets)  
* [NPC Swimming](#npc-swimming)  
* [Content Patcher Tokens](#content-patcher-tokens)
  * [Days Since Mod Installed](#days-since-mod-installed)
  * [Mines](#mines)
  * [Stats](#stats)
  * [Short Day Name](#short-day-name)
* [Triggers](#triggers)
  * [Machine Loaded](#machine-loaded)
  * [Machine Harvested](#machine-harvested)
  * [Stat Milestone Reached](#stat-milestone-reached)

## Custom Categories

This feature allows for items to have a custom category name displayed when looking at them in the inventory. Please note that this does not change the underlying category itself, only the text that is displayed on the tooltip. To use this feature, add the key `sophie.Calcifer/Category` to your object's CustomFields. It is recommended to i18n the value for ease of translation.

For example, the following patch adds an object "Pizza Bagel" with a custom category name of "Junk Food":

```json
{
    "LogName": "Add Pizza Bagel Object",
    "Action": "EditData",
    "Target": "Data/Objects",
    "Entries": {
        "MyMod_PizzaBagel": {
            "Name": "Pizza Bagel",
            // rest of object definition
            "CustomFields": {
                "sophie.Calcifer/Category": "Junk Food"
            }
        }
    }
}
```

## Furniture Actions

This feature allows tile actions and trigger actions to be performed when the player interacts with a piece of furniture. This is used most commonly to create catalogues or other pieces of furniture that open a shop when interacted with.

### Tile Actions

You can set furniture to perform one or more [tile actions](https://stardewvalleywiki.com/Modding:Maps#Action) when interacted with by editing the data asset `sophie.Calcifer/FurnitureActions`. The asset consists of a string → model lookup, where the key is the furniture's qualified item ID and the value is a model with one field, `TileActions`.

`TileActions` is a list of models with the following fields:

| Field | Description |
| --- | --- |
| Condition | A [game state query](https://stardewvalleywiki.com/Modding:Game_state_queries) that determines whether or not to run this tile action. Defaults to always true. |
| TileAction | A tile action string to perform as if the player clicked a tile on the map with this action. |
| StopProcessingActions | If true, will not attempt to perform any further actions. Defaults to true. |

Here is an example that adds two actions to a piece of furniture. The first one is conditional, but whether or not it is true, the second one will always run.

```json
{
    "Action": "EditData",
    "Target": "sophie.Calcifer/FurnitureActions",
    "Entries": {
        "(F)MyMod_MyFurniture": {
            "TileActions": [
                {
                    "Condition": "RANDOM 0.5",
                    "TileAction": "Dialogue Succeeded the 50/50 random chance!",
                    "StopProcessingActions": false
                },
                {
                    "TileAction": "OpenShop MyShop"
                }
            ]
        },
    }
}
```

### Trigger Actions

Furniture can also run [trigger actions](https://stardewvalleywiki.com/Modding:Trigger_actions). To do so, you edit the data asset `Data/Triggers` as described on the wiki page, using the custom trigger `sophie.Calcifer_FurnitureTriggered` as the trigger to run your action. To restrict the action to only being performed when a specific piece of furniture is interacted with, you use a game state query to check the ID of the Target. Use the `MarkActionApplied` field to set whether the action should be performed only once or if it should be performed every time the furniture is interacted with.

For example, this trigger action runs every time the piece of furniture with the fully qualified ID `(F)MyMod_TriggerFurniture` is interacted with:

```json
{
    "Action": "EditData",
    "Target": "Data/TriggerActions",
    "Entries": {
        "MyMod_TriggerExample": {
            "Id": "MyMod_TriggerExample",
            "Trigger": "sophie.Calcifer_FurnitureTriggered",
            "Condition": "ITEM_ID Target (F)MyMod_TriggerFurniture",
            "Action": "AddItem (O)64 1",
            "MarkActionApplied": false
        }
    }
},
```

## Furniture Offsets

This feature allows you to adjust the placement of items that are put on top of your furniture. This is helpful if, for example, you have a very tall or short piece of furniture and want to adjust the height of the item sitting on it so it doesn't appear to hover or sink in to the furniture. This is done by editing the data asset `sophie.Calcifer/FurnitureOffsets`, where the key is your furniture's fully qualified ID and the value is an offset to apply in pixels, in the format `x, y`. Negative numbers will shift the item to the left or up, and positive numbers will shift the item to the right or down.

For example, this patch edits the furniture with the fully qualified ID `(F)MyMod_OffsetFurniture` to shift the item placement right by 10 pixels and upwards by 20 pixels.

```json
{
    "Action": "EditData",
    "Target": "sophie.Calcifer/FurnitureOffsets",
    "Entries": {
        "(F)MyMod_OffsetFurniture": "10, -20"
    }
}
```

Getting the placement just right can take several tries; to make this easier, [patch reloading](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/troubleshooting.md#reload) your data will immediately reflect the changes in-game.

## NPC Swimming

This feature allows NPCs to swim as part of their schedule, instead of being restricted to events. The NPC's "swimming" state is achieved by drawing their sprite only from the neck upwards, with a white line across the bottom to indicate the surface of the water - this can be seen in Penny's 10-hearts event.

To use this feature, edit `Data/animationDescriptions` to add animations with the following keys based on the behavior you want to achieve:

| Key | Description |
| --- | --- |
| \<NPCInternalName>_startSwimming | The NPC will start swimming when this schedule point ends and the next begins. |
| \<NPCInternalName>_stopSwimming | The NPC will stop swimming when this schedule point ends and the next begins. |
| \<NPCInternalName>_startSwimmingNow | The NPC will enter their swimming state immediately, though they will still use this animation's frames until their next schedule point. |
| \<NPCInternalName>_stopSwimmingNow | The NPC will exit their swimming state immediately, though they will still use this animation's frames until their next schedule point. |

Then when writing the schedule, including the appropriate key will make the NPC start or stop swimming at that time. The startSwimming and stopSwimming keys will transition the NPC into the swimming state at the *end* of the schedule point, i.e. when they are ready to move to their next schedule point. The startSwimmingNow and stopSwimmingNow keys will immediately transition the NPC to the swimming state once they reach this schedule point.

For example, this pair of patches will make Pierre swim around neck deep in cobblestone in the town square for a couple hours on spring mornings, with some placeholder animations at the start and end:

```json
{
    "Action": "EditData",
    "Target": "Data/animationDescriptions",
    "Entries": {
        "Pierre_startSwimming": "16/17 18 19 18/16",
        "Pierre_stopSwimming": "23/23/23"
    }
},
{
    "Action": "EditData",
    "Target": "Characters/schedules/Pierre",
    "Entries": {
        "spring": "610 Town 30 60 Pierre_startSwimming/700 Town 30 70/800 Town 30 60/830 Town 30 61 Pierre_stopSwimming/900 SeedShop 20 20"
    }
}
```

## Content Patcher Tokens

Calcifer adds several [Content Patcher tokens](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md).

### Days Since Mod Installed

`sophie.Calcifer/DaysSinceModInstalled: <ModUniqueId>`

This token returns the number of in-game days on the current save that each mod has been installed. This allows you to define when things should happen relative to when the mod is installed, instead of relative to the start of the save, giving you better control over pacing when a mod is installed mid-save. This is most effectively utilized by using [query expressions](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide/tokens.md#query-expressions) to compare the given value.

For example, this patch adds a trigger action that sends a letter 5 days after the mod `sophie.ExampleMod` is installed:

```json
{
    "Action": "EditData",
    "Target": "Data/TriggerActions",
    "Entries": {
        "sophie.ExampleMod_SendIntroLetter": {
            "Id": "sophie.ExampleMod_SendIntroLetter",
            "Trigger": "DayStarted",
            "Condition": "{{Query: {{sophie.Calcifer/DaysSinceModInstalled}} >= 5}}",
            "Action": "AddMail Current sophie.ExampleMod_IntroLetter now"
        }
    }
}
```

### Mines

`sophie.Calcifer/InMines`

Returns whether or not the player is currently in the regular mines (levels 0-120).

`sophie.Calcifer/InQuarry`

Returns whether or not the player is currently in the quarry mine.

`sophie.Calcifer/InSkullCavern`

Returns whether or not the player is currently in the Skull Cavern.

`sophie.Calcifer/CurrentMineLevel`

Returns the current mine level number the player is on, if they are in the mines.

`sophie.Calcifer/DeepestMineLevel`

Returns the deepest mine level number that the player has accessed.

`sophie.Calcifer/IsHardModeActive`

Returns whether or not the mines are on hard mode, either because the Danger In The Deep quest is active or because the player has activated the Shrine of Challenge.

**NOTE: Due to the way Content Patcher uses [update rates](https://github.com/Pathoschild/StardewMods/blob/develop/ContentPatcher/docs/author-guide.md#how-often-are-patch-changes-applied), patches which rely on these tokens will need to update at a higher rate. This can be performance-heavy, especially when using `OnTimeChange`, so try to use them sparingly.**

### Stats

`sophie.Calcifer/Stat: \<stat name>`

This token will return the current player's current value for the given stat (see the [PLAYER_STAT game state query](https://stardewvalleywiki.com/Modding:Game_state_queries) for a list of stats tracked by vanilla).

### Short Day Name

`sophie.Calcifer/ShortDayName`

This token will return the short name for the current day in English, i.e. "Mon" for "Monday".


## Triggers

Calcifer adds several triggers in addition to the Furniture trigger.

### Machine Loaded

`sophie.Calcifer/MachineLoaded`

This fires whenever a player loads an item into a machine. The parameters are:

| Parameter | Description |
| --- | --- |
| Player | The player who loaded the item into the machine. |
| Location | The game location the machine is in. |
| Target Item | The machine itself. |
| Input Item | The item placed in the machine. |

### Machine Harvested

`sophie.Calcifer/MachineHarvested`

This fires whenever a player harvests output from a machine. The parameters are:

| Parameter | Description |
| --- | --- |
| Player | The player who collected the output from the machine. |
| Location | The game location the machine is in. |
| Target Item | The machine itself. |
| Input Item | The output item collected. |

It seems a bit weird to have the output considered input, but there's no other way to pass that data along so that it's accessible to trigger actions added through Content Patcher.

### Stat Milestone Reached

`sophie.Calcifer/StatMilestoneReached`

This fires whenever a stat is set to a certain amount, as defined by the data asset `sophie.Calcifer/StatMilestones`. The asset consists of a string → model lookup, where the key is a unique ID and the value is a model with the following fields:

| Field | Description |
| --- | --- |
| Stat | The stat key to set the milestone for (see the [PLAYER_STAT game state query](https://stardewvalleywiki.com/Modding:Game_state_queries) for a list of stat keys provided by vanilla). |
| Milestones | A list of numbers to set milestones for. |

The following example sets milestones for the stat "fishCaught" at 50, 100, and 200.

```json
{
    "Action": "EditData",
    "Target": "sophie.Calcifer/StatMilestones",
    "Entries": {
        "sophie.ExampleMod_FishCaughtMilestones": {
            "Stat": "fishCaught",
            "Milestones": [ 50, 100, 200 ]
        }
    }
}
```

When the trigger is fired, you can use the special query `sophie.Calcifer/CurrentStatMilestone <stat key> <milestone number>` to check the current stat and milestone being hit. **This query will not work in any circumstances other than the StatMilestoneReached trigger.**

For example, the following set of patches sends the player a piece of mail on the following day once they reach 500 items cooked, by first defining the milestone and then the corresponding trigger action:

```json
{
    "Action": "EditData",
    "Target": "sophie.Calcifer/StatMilestones",
    "Entries": {
        "sophie.ExampleMod_ItemsCookedMilestones": {
            "Stat": "itemsCooked",
            "Milestones": [ 500 ]
        }
    }
},
{
    "Action": "EditData",
    "Target": "Data/TriggerActions",
    "Entries": {
        "sophie.ExampleMod_500ItemsCooked": {
            "Id": "sophie.ExampleMod_500ItemsCooked",
            "Trigger": "sophie.Calcifer/StatMilestoneReached",
            "Condition": "sophie.Calcifer/CurrentStatMilestone itemsCooked 500",
            "Action": "AddMail Current sophie.ExampleMod_MasterChef"
        }
    }
}
```
