using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.IrlVibes.Business;
using FlanderDev.SFM.IrlVibes.UnityObjects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.IrlVibes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log.LogInfo($"Initialzing.");
        Helper.Logger = Log;
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
