using BepInEx.Logging;
using Buttplug.Client;
using ExposureUnnoticed2.Master.AdultGoods;
using ExposureUnnoticed2.ObjectUI.InGame.VIbeStatePanel;
using ExposureUnnoticed2.Scripts.InGame;
using FlanderDev.SFM.IrlVibes.Business;
using LibCpp2IL.Elf;
using System.Linq;
using UnityEngine;

namespace FlanderDev.SFM.IrlVibes.Mono;

public sealed class IrlVibsSetup : MonoBehaviour
{
    private readonly BpManager BpManager = new();

    public static double Value = 0;

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


            if (Input.GetKey(KeyCode.KeypadPlus))
            {
                Value++;
                await BpManager.AllDevices(Value / 1000);
            }
            else if (Input.GetKey(KeyCode.KeypadMinus))
            {
                Value--;
                await BpManager.AllDevices(Value / 1000);
            }
        }
        catch (System.Exception ex)
        {
            Plugin.manualLogSource.LogError(ex.Message);
        }
    }
}
