using BepInEx.Logging;
using ExposureUnnoticed2.Object3D.Player.Scripts;
using UnityEngine;

namespace FlanderDev.SFM.IrlVibes.Business;

public static class Helper
{
    public static ManualLogSource? Logger { get; set; }
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