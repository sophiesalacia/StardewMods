using Microsoft.Xna.Framework;

namespace Calcifer.Features.CGEventCommand;

public enum TweenState
{
    Running,
    Stopped
}

internal class TweenableFloat
{
    internal float StartTick;
    internal float EndTick;

    internal float StartValue;
    internal float EndValue;

    internal float Value;

    internal TweenState State;

    internal void Update(float elapsedTime)
    {
        if (State == TweenState.Stopped)
            return;

        if (elapsedTime >= EndTick)
        {
            Value = EndValue;
            State = TweenState.Stopped;
            return;
        }

        float progress = (elapsedTime - StartTick) / (EndTick - StartTick);
        Value = StartValue + (EndValue - StartValue) * progress;
    }

    internal void StartTween(float startTick, float endTick, float startVal, float endVal)
    {
        StartTick = startTick;
        EndTick = endTick;
        StartValue = startVal;
        EndValue = endVal;

        State = TweenState.Running;
    }

    internal void EndTween()
    {
        State = TweenState.Stopped;
    }
}

internal class TweenableVector2
{
    internal float StartTick;
    internal float EndTick;

    internal Vector2 StartValue;
    internal Vector2 EndValue;

    internal Vector2 Value;

    internal TweenState State;

    internal void Update(float elapsedTime)
    {
        if (State == TweenState.Stopped)
            return;

        if (elapsedTime >= EndTick)
        {
            Value = EndValue;
            State = TweenState.Stopped;
            return;
        }

        float progress = (elapsedTime - StartTick) / (EndTick - StartTick);
        Value = Vector2.Lerp(StartValue, EndValue, progress);
    }

    internal void StartTween(float startTick, float endTick, Vector2 startVal, Vector2 endVal)
    {
        StartTick = startTick;
        EndTick = endTick;
        StartValue = startVal;
        EndValue = endVal;

        State = TweenState.Running;
    }

    internal void EndTween()
    {
        State = TweenState.Stopped;
    }
}

internal class TweenableColor
{
    internal float StartTick;
    internal float EndTick;

    internal Color StartValue;
    internal Color EndValue;

    internal Color Value;

    internal TweenState State;

    internal void Update(float elapsedTime)
    {
        if (State == TweenState.Stopped)
            return;

        if (elapsedTime >= EndTick)
        {
            Value = EndValue;
            State = TweenState.Stopped;
            return;
        }

        float progress = (elapsedTime - StartTick) / (EndTick - StartTick);
        Value = Color.Lerp(StartValue, EndValue, progress);
    }

    internal void StartTween(float startTick, float endTick, Color startVal, Color endVal)
    {
        StartTick = startTick;
        EndTick = endTick;
        StartValue = startVal;
        EndValue = endVal;

        State = TweenState.Running;
    }

    internal void EndTween()
    {
        State = TweenState.Stopped;
    }
}
