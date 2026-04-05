using System.Diagnostics;

namespace FlanderDev.SFM.PatchAndRun;

// NOTE:
// This program is intended as a helper to optimize devloper workflow.
// It requires specifc envionmnet variables to be set.

internal static class Program
{
    public enum EnvVars
    {
        FlanderDev_SFM_PATCH_DLL,
        FlanderDev_SFM_ROOT_DIR,
        FlanderDev_SFM_STEAM_ID
    }

    private static void Main()
    {
        try
        {
            PatchAndRun();

            Console.WriteLine("Done. Clean exit.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error occurred:");
            PrintError(ex.Message);
            Console.WriteLine("Press enter to exit.");
            Console.WriteLine($"Redirected: {Console.IsInputRedirected}");
            Console.ReadLine();
        }
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
    private static void PatchAndRun()
    {
        if (GetEnvVar(EnvVars.FlanderDev_SFM_ROOT_DIR) is not { } gameRootDir || GetEnvVar(EnvVars.FlanderDev_SFM_PATCH_DLL) is not { } patchDll)
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
            Process.Start(gameExeutable);

        }
        else if (OperatingSystem.IsLinux()) // Using Steam Proton. Chage the ID accordingly, since it's per user.
        {
            if (GetEnvVar(EnvVars.FlanderDev_SFM_STEAM_ID) is not { } customGameId)
                return;
            Process.Start($"steam steam://rungameid/{customGameId}");
        }
        else
            throw new PlatformNotSupportedException("Compiled for unssupported operating system.");
    }

    private static string? GetEnvVar(EnvVars envVars)
    {
        var stringVar = envVars.ToString();
        var envVar = Environment.GetEnvironmentVariable(stringVar);
        if (envVar != null)
            return envVar;

        PrintError($"Enviornment variable '{stringVar}' is not set.");
        return null;
    }

    private static void PrintError(string text)
    {
        var priviousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ForegroundColor = priviousColor;
    }
}
