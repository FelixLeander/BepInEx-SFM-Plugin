using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.MapManager;
using HarmonyLib;

namespace FlanderDev.SFM.MapManager;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public static ManualLogSource Log { get; private set; }
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo("SceneDumper loaded. Press F8 to dump the current scene.");
        _harmony.PatchAll();

        // Register a component that handles input & scene-load events
        AddComponent<SceneDumperBehaviour>();
    }
}
