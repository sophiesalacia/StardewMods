using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SophieShared.EventHooks;

internal static class EventHookHandler
{
    public static void InitHooks()
    {
        Log.Info("Applying event hooks.");

        try
        {
		    List<Type> types = new StackTrace().GetFrame(1)?.GetMethod()?.ReflectedType?.Assembly.GetTypes().ToList() ?? [];

            // priority is ordered low to high
            foreach (Type t in types.Where(t => t.GetCustomAttribute<HasEventHooksAttribute>() is not null).OrderBy(type => type.GetCustomAttribute<HasEventHooksAttribute>()!.Priority))
            {
                MethodInfo? m = t.GetMethod("InitHooks", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                m?.Invoke(null, null);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed to initialize event hooks: \n{e}");
        }
	}
}
