using System;

namespace SophieShared.EventHooks;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
internal class HasEventHooksAttribute : Attribute
{
    internal int Priority;

    public HasEventHooksAttribute(int priority) => Priority = priority;

    public HasEventHooksAttribute() : this(HookPriority.Medium) { }
}

internal class HookPriority
{
    internal const int High = -100;
    internal const int Medium = 0;
    internal const int Low = 100;
}
