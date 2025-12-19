using BepInEx;
using BepInEx.Unity.IL2CPP;
using FlanDev.SFM.FuckIt;
using FlanDev.SFM.UI;
using HarmonyLib;
using UnityEngine;

namespace FlanDev.SFM;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BasePlugin
{
    /// <summary>
    /// Creating and holdign a reference to it, so that it won't be destroyed and <see cref="NativeOverwrite.Update"/> will be called.
    /// </summary>
    internal static NativeOverwrite? InplaceRewrite { get; set; }

    // internal static UiTemplate? UiTemplate { get; set; }

    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    public override void Load()
    {
        _harmony.PatchAll();

        //UiTemplate = AddComponent<UiTemplate>();
        //UiTemplate.Log = Log;

        InplaceRewrite = AddComponent<NativeOverwrite>();
        InplaceRewrite.Log  = Log;
        //MinimalTest.hideFlags = HideFlags.HideAndDontSave;

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    public override bool Unload()
    {
        _harmony.UnpatchSelf();
        return base.Unload();
    }
}
