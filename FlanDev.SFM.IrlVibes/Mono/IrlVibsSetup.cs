using BepInEx.Logging;
using Buttplug.Client;
using ExposureUnnoticed2.Master.AdultGoods;
using ExposureUnnoticed2.ObjectUI.InGame.VIbeStatePanel;
using ExposureUnnoticed2.Scripts.InGame;
using FlanDev.SFM.IrlVibes.Business;
using System.Linq;
using UnityEngine;

namespace FlanDev.SFM.IrlVibes.Mono;

public sealed class IrlVibsSetup : MonoBehaviour
{
    private readonly BpManager BpManager = new();

    public async void Update()
    {
        try
        {
            if (!InGameManager.Instance)
                return;

            var vibeStatePanelView = InGameManager.Instance.GetComponentInChildren<VibeStatePanelView>();
            if (vibeStatePanelView == null)
            {
                Plugin.manualLogSource.Log(LogLevel.Warning, "no vibe panel.");
                return;
            }

            await BpManager.ActivateAsync(vibeStatePanelView.currentVibeType);
        }
        catch (System.Exception ex)
        {
            Plugin.manualLogSource.LogError(ex.Message);
        }
    }
}
