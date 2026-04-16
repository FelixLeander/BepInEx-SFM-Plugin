using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.Common.Business;
using FlanderDev.SFM.Common.ShortcutFramework;
using FlanderDev.SFM.IrlVibes.Business;
using FlanderDev.SFM.IrlVibes.UnityObjects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.IrlVibes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
#pragma warning disable CS8618
    public static Plugin Instance;
#pragma warning restore CS8618
    public static XXXKeyShortcut Shortcut => Instance.Config.CreateShortCut(new XXXKeyConfig(KeyCode.RightArrow), nameof(KeyCode.RightArrow), "Debug: RightArrow");
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log.LogInfo($"Initialzing.");
        Instance = this;
        XHelper.Logger = Log;
        _harmony.PatchAll();

        SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((scene, mode) =>
        {
            AddComponent<IrlVibsSetup>();
            AddComponent<DebugFloat>();
        }));

        Log.LogInfo($"Done Initialzing.");
    }

    public override bool Unload()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is unlooading...");
        _harmony.UnpatchSelf();
        return Unload();
    }
}
