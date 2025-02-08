using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

// ReSharper disable InconsistentNaming

namespace Calcifer.Features.CGEventCommand;

internal static class CGEventCommand
{
    internal static CGScene? Scene;
    internal static int CurrentFrame;

    internal static void HandleCommand(Event ev, string[] args, EventContext context)
    {
        if (args.Length == 1)
        {
            context.LogErrorAndSkip($"The provided event command {string.Join(" ", args)} is incomplete. See Calcifer's documentation for more details on how to use this command.");
        }
        
        string[] commandArgs = args[1..];

        switch (commandArgs[0])
        {
            // need to make all this case-insensitive

            case "start":
                {
                    if (Scene is not null)
                    {
                        context.LogErrorAndSkip("A CG scene has already been started. See Calcifer's documentation for more details on how to use this command.");
                        return;
                    }

                    if (commandArgs.Length == 1 || !float.TryParse(commandArgs[1], out float opacity))
                    {
                        opacity = 0f;
                    }

                    Scene = new CGScene(opacity);
                    ev.CurrentCommand++;
                    break;
                }

            // everything after this point assumes Scene is not null, figure out how to make that work better
            // pull start out of switch maybe?

            case "pause":
                {
                    // get pause duration
                    //CurrentFrame += pauseDuration;
                    //ev.CurrentCommand++;
                    break;
                }

            case "add":
                {
                    // make sure args are actually supplied
                    string[] addArgs = commandArgs[1..];
                    Scene.AddKeyFrame(CurrentFrame, () => AddElement(addArgs));
                    ev.CurrentCommand++;
                    break;
                }

            case "move":
                {
                    // make sure args are actually supplied
                    string[] moveArgs = commandArgs[1..];
                    Scene.AddKeyFrame(CurrentFrame, () => MoveElement(moveArgs));
                    ev.CurrentCommand++;
                    break;
                }

            case "play":
            {
                //if ev.CurrentCustomEventScript == Scene
                //  if Scene.Update
                //      ev.CurrentCustomEventScript = null;
                //      CurrentCommand++;
                //else
                //  Scene.AddKeyFrame(CurrentFrame, () => MarkFinished());
                //  ev.CurrentCustomEventScript == Scene;
                // do whatever other stuff necessary to set up and run cutscene script
                break;
            }

            default:
                {
                    context.LogErrorAndSkip($"The provided event command {string.Join(" ", args)} is unrecognized. See Calcifer's documentation for more details on how to use this command.");
                    break;
                }
        }
        
    }

    private static void AddElement(string[] args)
    {
        throw new NotImplementedException();
        //Scene.Elements.Add(null);
    }

    private static void MoveElement(string[] moveArgs)
    {
        throw new NotImplementedException();
        // make sure scene.elements contains the provided id, then call
        // SelectedElement.Position.StartTween(CurrentFrame, moveDuration, currentPosition, endPosition);
        // if pause is false, CurrentFrame += duration;
    }

    private static bool TryStartScene(string[] commandArgs, Event ev, out string error)
    {
        if (ev.currentCustomEventScript is CGScene)
        {
            error = "A CG scene is already active. See Calcifer's documentation for more details on how to use this command.";
            return false;
        }

        if (commandArgs.Length == 1 || !float.TryParse(commandArgs[1], out float bgOpacity))
        {
            bgOpacity = 0f;
        }

        ev.currentCustomEventScript = new CGScene(bgOpacity);
        error = "";
        return true;
    }

    private static bool TryStopScene(string[] commandArgs, Event ev, out string error)
    {
        if (ev.currentCustomEventScript is not CGScene)
        {
            error = "No CG scene is active. See Calcifer's documentation for more details on how to use this command.";
            return false;
        }

        ev.currentCustomEventScript = null;
        error = "";
        return true;
    }
}

[HasEventHooks]
internal static class CGEventHooks
{
    internal static void InitHooks()
    {
        //Globals.EventHelper.GameLoop.GameLaunched += (_, _) => Event.RegisterCommand("sophie.Calcifer_CG", CGEventCommand.HandleCommand);
    }
}
