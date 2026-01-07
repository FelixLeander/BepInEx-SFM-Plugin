using BepInEx.Logging;
using Buttplug.Client;
using Buttplug.Core;
using ExposureUnnoticed2.Master.AdultGoods;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlanDev.SFM.IrlVibes.Business;

internal sealed class BpManager
{

    private readonly ButtplugClient client = new($"{nameof(FlanDev)}.");
    private readonly ButtplugWebsocketConnector _connector = new(new("ws://127.0.0.1:12345"));

    private bool _active = false;
    private VibrationModeType _lastVibeMode = VibrationModeType.Off;
    public async Task ActivateAsync(VibrationModeType vibrationModeType)
    {
        try
        {
            if (_active)
                return;

            if (_lastVibeMode == vibrationModeType)
                return;

            _active = true;

            if (!_connector.Connected)
               await client.ConnectAsync(_connector);

            double strength = vibrationModeType switch
            {
                VibrationModeType.Off => 0,
                VibrationModeType.Low => 0.40,
                VibrationModeType.High => 0.80,
                VibrationModeType.Random => -1,
                _ => throw new NotImplementedException($"Unexpected {nameof(VibrationModeType)}."),
            };

            if (strength == -1)
            {
                Plugin.manualLogSource.Log(LogLevel.Error, $"{nameof(VibrationModeType.Random)} should never trigger.");
                return;
            }

            var devices = client.Devices;
            Plugin.manualLogSource.Log(LogLevel.Info, $"device count: {devices.Length}");
            foreach (var device in devices)
            {
                Plugin.manualLogSource.Log(LogLevel.Info, $"Setting {device.Name} to {strength}");
                //var strengths = Enumerable.Repeat(strength, 2);
                _ = device.VibrateAsync([strength / 4, strength]);
            }
        }
        catch (ButtplugClientConnectorException ex)
        {
            Plugin.manualLogSource.LogWarning("Please start Iniface-Central. Without it this mod dosen't work!");
            Plugin.manualLogSource.LogWarning(ex);
        }
        catch (ButtplugHandshakeException ex)
        {
            Plugin.manualLogSource.LogWarning("Ensure Iniface-Central is running. If the problem persists, please contact the developer");
            Plugin.manualLogSource.LogWarning(ex);
        }
        catch (Exception ex)
        {
            Plugin.manualLogSource.LogError("Unexpected error, please contact the developer:");
            Plugin.manualLogSource.LogError(ex);
        }
        finally
        {
            _lastVibeMode = vibrationModeType;
            _active = false; 
        }
    }
}
