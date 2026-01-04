using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ExposureUnnoticed2.Object3D.AdultGoods;
using ExposureUnnoticed2.ObjectUI.InGame.VIbeStatePanel;
using ExposureUnnoticed2.ObjectUI.OptionMenu.KeyConfigPanel;
using ExposureUnnoticed2.Scripts.Base;
using ExposureUnnoticed2.Scripts.InGame;
using FlanDev.SFM.UI;
using HarmonyLib;

namespace FlanDev.SFM;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    /// <summary>
    /// Creating and holdign a reference to it, so that it won't be destroyed and <see cref="IrlVibsSetup.Update"/> will be called.
    /// </summary>
    internal static IrlVibsSetup? IrlVibsSetup { get; set; }

    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        _harmony.PatchAll();

        IrlVibsSetup = AddComponent<IrlVibsSetup>();
        IrlVibsSetup.Log = Log;
        Helper.Logger = Log;

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} iss unladoing...");
        _harmony.UnpatchSelf();
        return base.Unload();
    }
}

//public class MyPatches
//{
//    [HarmonyPatch(typeof(VibeStatePanelView), nameof(VibeStatePanelView.OnChange))]
//    public void VibeStatePanelViewOnChangePatch(OptionChangeEvent evt)
//    {

//        var value = UnityEngine.Object.FindObjectsOfType<KeyConfigItemView>();
//        //CommonVibratorController.ForceSetVibrationMode();
//        "Changed xxxxxxxxxxxxxxxxx".Log(LogLevel.Warning);
//    }
//}