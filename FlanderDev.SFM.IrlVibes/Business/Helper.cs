using BepInEx.Logging;

namespace FlanderDev.SFM.IrlVibes.Business;

public static class Helper
{
    public const float GoldenRatio = 1.618f;
    public static ManualLogSource? Logger { get; set; }
    public static void Log(this string text, LogLevel logLevel =
#if DEBUG
        LogLevel.Fatal // I want to see everything, and fatal basiclly neverr get used anyway.
#else
        LogLevel.Debug
#endif
        ) => Logger?.Log(logLevel, text);
}