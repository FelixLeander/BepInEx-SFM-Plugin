using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.IrlVibes.Business;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;
using BepInEx.Configuration;



namespace FlanderDev.SFM.IrlVibes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    private ConfigEntry<BepInEx.Configuration.Ke>? _openMenuKey;
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log.LogInfo($"Initialzing.");
        Helper.Logger = Log;
        _harmony.PatchAll();

        Config.Bind("General", "OpenMenuKey", new KeyboardShortcut(KeyCode.F1), "The key to open the menu.");

        SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((scene, mode) =>
        {
            AddComponent<IrlVibsSetup>();
            AddComponent<TeleportOnTopPlugin>();
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
