using System.Diagnostics;

namespace FlanDev.SFM.PatchAndRun;

// NOTE:
// This program is intended as a helper tto optimize devloper workflow.
// The project has a build-event, copying the program into the solution dir.

internal static class Program
{
    public enum EnvVars
    {
        FlanDev_SFM_ROOT_DIR,
        FlanDev_SFM_STEAM_ID
    }

    private static void Main()
    {
        if (GetEnvVar(EnvVars.FlanDev_SFM_ROOT_DIR) is not { } gameRootDir)
            return;

        if (OperatingSystem.IsWindows())
        {
            var gameExeutable = Path.Combine(gameRootDir, "SecretFlasherManaka.exe");
            try
            {
                var result = Process.Start(gameExeutable);
            }
            catch (Exception ex)
            {
                PrintError(ex.Message);
            }
        }
        else if (OperatingSystem.IsLinux()) // Using Steam Proton. Chage the ID accordingly, since it's per user.
        {
            if (GetEnvVar(EnvVars.FlanDev_SFM_STEAM_ID) is not { } customGameId)
                return;

            try
            {
                var processs = Process.Start($"steam steam://rungameid/{customGameId}");

            }
            catch (Exception ex)
            {
                PrintError(ex.Message);
            }
        }
        else
            throw new PlatformNotSupportedException("Compiled for unssupported operating system.");


        static string? GetEnvVar(EnvVars envVars)
        {
            var stringVar = envVars.ToString();
            var envVar = Environment.GetEnvironmentVariable(stringVar);
            if (envVar != null)
                return envVar;

            PrintError(stringVar);
            return null;
        }

        static void PrintError(string text)
        {
            var priviousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Enviornment variable '{text}' is not set.");
            Console.ForegroundColor = priviousColor;
        }
    }
}
