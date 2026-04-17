using BepInEx.Configuration;
using BepInEx.Logging;
using ExposureUnnoticed2.Object3D.Player.Scripts;
using FlanderDev.SFM.General.ShortcutFramework;
using UnityEngine;

namespace FlanderDev.SFM.General.Business;

public static class Helper
{
    public static ManualLogSource? Logger { get; set; }

    public const double GoldenRatio = 1.61803398874989484820458683436;
    public static KeyShortcut CreateShortCut(
        this ConfigFile config,
        KeyConfig defaultShortcut,
        string keyName,
        string description = "",
        string section = "Shortcuts")
        => new(config, defaultShortcut, keyName, description, section);

    public static void Log(this string text, LogLevel logLevel =
#if DEBUG
        LogLevel.Fatal // I want to see everything, and fatal basiclly never get used anyway.
#else
        LogLevel.Debug
#endif
        ) => Logger?.Log(logLevel, text);

    public static void MovePlayer(PlayerController playerController, Vector3 destination)
    {
        if (!playerController)
            return;

        playerController.enabled = false;
        playerController.transform.position = destination;
        playerController.enabled = true;

        $"Teleported {nameof(PlayerController)} to {destination}".Log();
    }
}