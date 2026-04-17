using BepInEx.Configuration;

namespace FlanderDev.SFM.Common.ShortcutFramework;

/// <summary>
/// Binds a <see cref="XXXKeyConfig"/> to a BepInEx config entry so users
/// can remap it by editing the .cfg toml file.
///
/// Usage:
///   var openMenu = new ConfigShortcut(Config, "Shortcuts", "Open Menu",
///                      new KeyboardShortcut(KeyCode.F1),
///                      "Opens the plugin menu.");
///
///   // In Update():
///   if (openMenu.IsDown()) { ... }
/// </summary>
/// <param name="config">Your plugin's ConfigFile (pass <c>Config</c> from BasePlugin).</param>
/// <param name="section">Config section, e.g. "Shortcuts".</param>
/// <param name="keyName">Entry name shown in the .cfg file.</param>
/// <param name="defaultShortcut">Fallback when the user hasn't customized the entry.</param>
/// <param name="description">Comment written above the entry in the .cfg file.</param>
public readonly struct XXXKeyShortcut(
    ConfigFile config,
    XXXKeyConfig defaultShortcut,
    string keyName,
    string description = "",
    string section = "Shortcuts"
    )
{
    public readonly XXXKeyConfig Value =
        XXXKeyConfig.TryParse(config.Bind(section, keyName, defaultShortcut.ToString(), description).Value, out var shortcut)
        ? shortcut
        : defaultShortcut;

    public bool IsDown() => Value.IsDown();
    public bool IsPressed() => Value.IsPressed();
    public bool IsUp() => Value.IsUp();
}
