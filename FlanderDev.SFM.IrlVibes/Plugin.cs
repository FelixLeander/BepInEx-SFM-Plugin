using BepInEx;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.IrlVibes.Business;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;


namespace FlanderDev.SFM.IrlVibes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    /// <summary>
    /// Creating and holdign a reference to it, so that it won't be destroyed and <see cref="IrlVibsSetup.Update"/> will be called.
    /// </summary>

    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log.LogInfo($"Initialzing.");
        Helper.Logger = Log;
        _harmony.PatchAll();

        SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((scene, mode) =>
        {
            $"SceneManager.add_sceneLoaded: {scene.name}".Log();

            //if (scene.name == "Title")
            AddComponent<IrlVibsSetup>();
        }));

        Log.LogInfo($"Done Initialzing.");
    }

    public override bool Unload()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is unladoing...");
        _harmony.UnpatchSelf();
        return Unload();
    }
}
