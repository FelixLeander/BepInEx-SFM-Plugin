using System.Diagnostics;

namespace FlanDev.SFM.PatchAndRun;


/// <remarks>
/// This program is intended as a helper to optimize devloper workflow.
/// It requires specifc envionmnet variables to be set. See <see cref="EnvVars"/>
/// </remarks>
internal static class Program
{
    public enum EnvVars
    {
        /// <summary>The full .DLL path containing the patch.</summary>
        FlanDev_SFM_PATCH_DLL,

        /// <summary>The full directory path containing the game executable.</summary>
        FlanDev_SFM_ROOT_DIR,

        /// <summary>The directory name inside the BepInEx/Plugin directoy, where the <see cref="FlanDev_SFM_PATCH_DLL"/> will be placed.</summary>
        FlanDev_SFM_PATCH_DIR,

        /// <summary>The steam id of the game.</summary>
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
        if (GetEnvVar(EnvVars.FlanDev_SFM_ROOT_DIR) is not { } gameRootPath || GetEnvVar(EnvVars.FlanDev_SFM_PATCH_DLL) is not { } patchDllPath || GetEnvVar(EnvVars.FlanDev_SFM_PATCH_DIR) is not { } patchDirPath)
            return;

        if (!File.Exists(patchDllPath))
        {
            PrintError($"Patch file not found at: '{patchDllPath}'");
            return;
        }

        var targetDirPath = Path.Combine(gameRootPath, "BepInEx", "plugins", patchDirPath);
        if (!Directory.Exists(targetDirPath))
            Directory.CreateDirectory(targetDirPath);

        var copyPatchDllPath = Path.Combine(targetDirPath, Path.GetFileName(patchDllPath));
        File.Copy(patchDllPath, copyPatchDllPath, true);

        if (OperatingSystem.IsWindows())
        {
            var gameExeutable = Path.Combine(gameRootPath, "SecretFlasherManaka.exe");
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
