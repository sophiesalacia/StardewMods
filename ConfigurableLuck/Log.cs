using StardewModdingAPI;

namespace ConfigurableLuck;

// Credit to spacechase0
internal class Log
{
    public static IMonitor Monitor;

    public static void Verbose(string str) => Monitor.VerboseLog(str);

    public static void Trace(string str) => Monitor.Log(str, LogLevel.Trace);

    public static void Debug(string str) => Monitor.Log(str, LogLevel.Debug);

    public static void Info(string str) => Monitor.Log(str, LogLevel.Info);

    public static void Warn(string str) => Monitor.Log(str, LogLevel.Warn);

    public static void Error(string str) => Monitor.Log(str, LogLevel.Error);
}
