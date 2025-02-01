using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SophieShared.EventHooks;

internal static class ConsoleCommandHandler
{
    public static void InitConsoleCommands()
    {
        Log.Info("Registering console commands.");

        try
        {
		    List<Type> types = new StackTrace().GetFrame(1)?.GetMethod()?.ReflectedType?.Assembly.GetTypes().ToList() ?? [];
            
            foreach (Type t in types)
            {
                foreach (MethodInfo mi in t.GetMethods())
                {
                    var ccAttributes = mi.GetCustomAttributes<ConsoleCommandAttribute>();

                    foreach (ConsoleCommandAttribute ccAttr in ccAttributes)
                    {
                        if (string.IsNullOrEmpty(ccAttr.Name))
                        {
                            Log.Error($"Failed to register console command for {t.FullName}::{mi.Name}: no name provided.");
                            continue;
                        }
                        
                        try
                        {
                            Globals.CCHelper.Add(ccAttr.Name, ccAttr.Description, mi.CreateDelegate<Action<string, string[]>>());
                            Log.Trace($"Registered \"{ccAttr.Name}\" with description \"{ccAttr.Description}\"");
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Failed to register console command \"{ccAttr.Name}\": \n{e}");
                        }
                    }
                }
            }
            
        }
        catch (Exception e)
        {
            Log.Error($"Failed to register console commands: \n{e}");
        }
	}
}
