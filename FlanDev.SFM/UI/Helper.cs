using BepInEx.Logging;

namespace FlanDev.SFM.UI;

public static class Helper
{
    public const float GoldenRatio = 1.618f;
    public static ManualLogSource? Logger { get; set; }
    public static void Log(this string text, LogLevel logLevel = LogLevel.Info) => Logger?.Log(logLevel, text);
}