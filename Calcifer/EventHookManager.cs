using Calcifer.Features;

namespace Calcifer;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        CategoryNameOverrideHooks.InitializeEventHooks();
    }
}
