using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ExposureUnnoticed2.ObjectUI.SystemMenu;
using FlanDev.SFM.UI;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static ManualLogSource logSource;
    public static UiHelper Ui { get; set; }
    public override void Load()
    {
        logSource = base.Log;
        logSource.LogError($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        Ui = AddComponent<UiHelper>();
        Ui.hideFlags = HideFlags.HideAndDontSave;
    }

    [HarmonyPatch(typeof(SystemMenuView), nameof(SystemMenuView.Initialize))]
    [HarmonyPostfix]
    public static void InitializePatch(SystemMenuView __instance)
    {
        logSource.LogError($"Patch {InitializePatch} called.");

        var gameObject = new GameObject("Button");
        gameObject.transform.SetParent(__instance.transform);

        var button = gameObject.AddComponent<Button>();
        button.onClick.AddListener((Action) delegate
        {
            logSource.LogWarning("Button Clicked!");
        });

        __instance.buttonss.AddItem(button);
    }
}
