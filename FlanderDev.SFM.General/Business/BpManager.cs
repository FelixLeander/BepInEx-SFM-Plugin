using BepInEx.Logging;
using Buttplug.Client;
using Buttplug.Core;
using ExposureUnnoticed2.Master.AdultGoods;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlanderDev.SFM.General.Business;

internal sealed class BpManager
{

    private readonly ButtplugClient client = new($"{nameof(FlanderDev)}.");
    private readonly ButtplugWebsocketConnector _connector = new(new("ws://127.0.0.1:12345"));

    private bool _active = false;
    private VibrationModeType _lastVibeMode = VibrationModeType.Off;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    };

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
                VibrationModeType.Random => -1, // The game dosent use this mode.
                _ => throw new NotImplementedException($"Unexpected {nameof(VibrationModeType)}."),
            };

            if (strength == -1)
            {
                $"{nameof(VibrationModeType.Random)} should never trigger.".Log(LogLevel.Error);
                return;
            }

            var devices = client.Devices;
            $"device count: {devices.Length}".Log();
            foreach (var device in devices)
            {
                $"Setting {device.Name} to {strength}".Log(LogLevel.Info);
                _ = device.VibrateAsync(strength); // personal prefrence. Also not waiting for the async task to complete to avoid blocking.
            }
        }
        catch (ButtplugClientConnectorException ex)
        {
            $"Please start Iniface-Central. Without it this mod dosen't work!{Environment.NewLine}{ex.Message}".Log(LogLevel.Warning);
        }
        catch (ButtplugHandshakeException ex)
        {
            $"Ensure Iniface-Central is running. If the problem persists, please contact the developer.{Environment.NewLine}{ex.Message}".Log(LogLevel.Warning);
        }
        catch (Exception ex)
        {
            $"Unexpected error, please contact the developer:{Environment.NewLine}{ex.Message}".Log(LogLevel.Warning);
        }
        finally
        {
            _lastVibeMode = vibrationModeType;
            _active = false;
        }
    }
}
