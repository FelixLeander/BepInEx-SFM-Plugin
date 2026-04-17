using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.MapManager.SceneManaging;
using HarmonyLib;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.MapManager;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    public static new ManualLogSource? Log { get; private set; }
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("SceneDumper loaded. Press F8 to dump the current scene.");
        _harmony.PatchAll();

        AddComponent<DumpTrigger>();
    }
}

public sealed class DumpTrigger : MonoBehaviour
{
    public void Awake() => DontDestroyOnLoad(gameObject);

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
            DumpScene();
    }

    // SceneManager.sceneLoaded += (UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => DumpScene(scene);
    private static void DumpScene() => DumpScene(SceneManager.GetActiveScene());
    private static void DumpScene(Scene scene)
    {
        var sw = Stopwatch.StartNew();
        Plugin.Log?.LogInfo("======== DUMP START ========");

        SceneController.DumpSceneHierarchy(scene);

        sw.Stop();
        Plugin.Log?.LogInfo($"Dump duration: {sw.Elapsed}");
        Plugin.Log?.LogInfo("======== DUMP END ========");
    }
}
