using BepInEx.Logging;
using ExposureUnnoticed2.ObjectUI.InGame.VIbeStatePanel;
using ExposureUnnoticed2.Scripts.InGame;
using FlanderDev.SFM.General.Business;
using System;
using UnityEngine;

namespace FlanderDev.SFM.General.Mono;

public sealed class GeneralSetup : MonoBehaviour
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
                "no vibe panel.".Log(LogLevel.Warning);
                return;
            }

            await BpManager.ActivateAsync(vibeStatePanelView.currentVibeType);
        }
        catch (Exception ex)
        {
            ex.Message.Log(LogLevel.Warning);
        }
    }
}
