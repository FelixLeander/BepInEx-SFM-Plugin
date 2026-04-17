using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FlanderDev.SFM.IrlVibes.Mono;
using HarmonyLib;

namespace FlanderDev.SFM.IrlVibes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    /// <summary>
    /// Creating and holdign a reference to it, so that it won't be destroyed and <see cref="IrlVibsSetup.Update"/> will be called.
    /// </summary>
    internal static IrlVibsSetup? IrlVibsSetup { get; set; }
    internal static ManualLogSource manualLogSource = new(nameof(FlanderDev));
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        _harmony.PatchAll();
        IrlVibsSetup = AddComponent<IrlVibsSetup>();
        manualLogSource = Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is unladoing...");
        _harmony.UnpatchSelf();
        return base.Unload();
    }
}
