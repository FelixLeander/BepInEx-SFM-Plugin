using BepInEx;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.General.Business;
using FlanderDev.SFM.General.ShortcutFramework;
using FlanderDev.SFM.General.UnityObjects;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.General;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
#pragma warning disable CS8618
    public static Plugin Instance;
#pragma warning restore CS8618
    public static KeyShortcut Shortcut => Instance.Config.CreateShortCut(new KeyConfig(KeyCode.RightArrow), nameof(KeyCode.RightArrow), "Debug: RightArrow");
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log.LogInfo($"Initialzing.");
        Instance = this;
        Helper.Logger = Log;
        _harmony.PatchAll();

        SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((scene, mode) =>
        {
            AddComponent<GeneralSetup>();
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
