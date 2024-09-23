using System;

namespace SophieShared.EventHooks;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
internal class ConsoleCommandAttribute : Attribute
{
    internal string Name;
    internal string Description;

    public ConsoleCommandAttribute(string name, string description = "")
    {
        Name = name;
        Description = description;
    }
}

