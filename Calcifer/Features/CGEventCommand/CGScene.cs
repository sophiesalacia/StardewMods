using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Calcifer.Features.CGEventCommand;

internal class CGScene : ICustomEventScript
{
    internal List<KeyFrame> KeyFrames;
    internal List<CGElement> Elements;

    private float BgOpacity;
    private int CurrentFrame;

    public CGScene(float bgOpacity)
    {
        KeyFrames = new List<KeyFrame>();
        BgOpacity = bgOpacity;
    }

    public void AddKeyFrame(int frame, Action action)
    {
        KeyFrames.Add(new KeyFrame(frame, action));
    }

    public bool update(GameTime time, Event e)
    {
        foreach (KeyFrame keyFrame in KeyFrames)
        {
            if (CurrentFrame == keyFrame.Frame)
            {
                keyFrame.Action.Invoke();
                continue;
            }

            if (CurrentFrame < keyFrame.Frame)
            {
                break;
            }

            // all keyframes passed - done
            return true;
        }

        foreach (CGElement element in Elements)
        {
            element.Update(CurrentFrame);
        }

        CurrentFrame++;
        return false;
    }

    public void draw(SpriteBatch b)
    {
        throw new NotImplementedException();

        // draw background rectangle with specified BgOpacity
    }

    public void drawAboveAlwaysFront(SpriteBatch b)
    {
        throw new NotImplementedException();
    }
}

internal class KeyFrame
{
    internal int Frame;
    internal Action Action;

    public KeyFrame(int frame, Action action)
    {
        Frame = frame;
        Action = action;
    }
}

internal class CGElement
{
    private TweenableFloat Opacity;
    private TweenableFloat Scale;
    private TweenableVector2 Position;
    private TweenableColor BlendColor;

    internal void Update(int frame)
    {

    }

    public void Draw(SpriteBatch b)
    {
        throw new NotImplementedException();
    }

    public void DrawAboveAlwaysFront(SpriteBatch b)
    {
        throw new NotImplementedException();
    }
}

internal class GraphicElement : CGElement { }
internal class TextElement : CGElement { }
