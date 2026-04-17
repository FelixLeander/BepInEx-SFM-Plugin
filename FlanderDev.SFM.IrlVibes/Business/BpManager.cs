using BepInEx.Logging;
using Buttplug.Client;
using Buttplug.Core;
using ExposureUnnoticed2.Master.AdultGoods;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlanderDev.SFM.IrlVibes.Business;

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

    public async Task AllDevices(double strength)
    {
        if (!_connector.Connected)
            await client.ConnectAsync(_connector);

        foreach (var device in client.Devices)
        {
            Plugin.manualLogSource.LogFatal($"{device.Name}: {strength}");
            await device.VibrateAsync(strength);
        }
    }

    public async Task GetDebugInfoAsync()
    {
        if (!_connector.Connected)
            await client.ConnectAsync(_connector);

        var json = JsonSerializer.Serialize(client.Devices, JsonSerializerOptions);
        Plugin.manualLogSource.LogFatal(Environment.NewLine + json);
    }

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
                Plugin.manualLogSource.Log(LogLevel.Error, $"{nameof(VibrationModeType.Random)} should never trigger.");
                return;
            }

            var devices = client.Devices;
            Plugin.manualLogSource.Log(LogLevel.Info, $"device count: {devices.Length}");
            foreach (var device in devices)
            {
                Plugin.manualLogSource.Log(LogLevel.Info, $"Setting {device.Name} to {strength}");
                _ = device.VibrateAsync(strength); // personal prefrence. Also not waiting for the async task to complete to avoid blocking.
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
