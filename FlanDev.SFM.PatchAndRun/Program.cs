using System.Diagnostics;

namespace FlanDev.SFM.PatchAndRun;

// NOTE:
// This program is intended as a helper to optimize devloper workflow.

internal static class Program
{
    public enum EnvVars
    {
        FlanDev_SFM_PATCH_DLL,
        FlanDev_SFM_ROOT_DIR,
        FlanDev_SFM_STEAM_ID
    }


    /// <summary>
    /// Patches the game and runs is after.
    /// </summary>
    /// <remarks>
    /// Operating system agnostic.
    /// Requires the entries in <see cref="EnvVars"/> to be set es enviornmment variables.
    /// On linux I use proton on steam.
    /// </remarks>
    /// <exception cref="PlatformNotSupportedException">Thrown if the application is run on an unsupported operating system.</exception>
    private static void Main()
    {
        if (GetEnvVar(EnvVars.FlanDev_SFM_ROOT_DIR) is not { } gameRootDir || GetEnvVar(EnvVars.FlanDev_SFM_PATCH_DLL) is not { } patchDll)
            return;

        if (!File.Exists(patchDll))
        {
            PrintError($"Patch file not found at: '{patchDll}'");
            return;
        }

        var copyPatchDll = Path.Combine(gameRootDir, "BepInEx", "plugins", Path.GetFileName(patchDll));
        File.Copy(patchDll, copyPatchDll, true);

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
