using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlanderDev.SFM.General.ShortcutFramework;

/// <summary>
/// A keyboard shortcut with an optional set of modifier keys.
/// Intended as a drop-in for BepInEx Mono's KeyboardShortcut on Il2Cpp builds.
///
/// Config format (BepInEx Mono compatible):
///   "F1"
///   "LeftControl + S"
///   "LeftControl + LeftShift + F5"
/// </summary>
public readonly struct KeyConfig(KeyCode mainKey, params KeyCode[] modifiers) : IEquatable<KeyConfig>
{
    public static readonly KeyConfig Empty = new(KeyCode.None);
    public readonly KeyCode MainKey = mainKey;
    public readonly IReadOnlyList<KeyCode> Modifiers = modifiers ?? [];

    private static readonly KeyCode[] AllModifiers =
    {
        KeyCode.LeftShift,   KeyCode.RightShift,
        KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.LeftAlt,     KeyCode.RightAlt,
        KeyCode.LeftCommand, KeyCode.RightCommand,
    };

    public bool IsDown() => IsActive(Input.GetKeyDown);
    public bool IsPressed() => IsActive(Input.GetKey);
    public bool IsUp() => MainKey != KeyCode.None
                               && Input.GetKeyUp(MainKey)
                               && Modifiers.All(Input.GetKey);

    private bool IsActive(Func<KeyCode, bool> mainCheck) =>
        MainKey != KeyCode.None
        && mainCheck(MainKey)
        && Modifiers.All(Input.GetKey)
        && NoStrayModifiers();

    private bool NoStrayModifiers()
    {
        foreach (var mod in AllModifiers)
        {
            if (mod == MainKey || Modifiers.Contains(mod))
                continue;

            if (Input.GetKey(mod))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Parses BepInEx-style shortcut strings: "F1"  /  "LeftControl + S"  /  "LeftControl + LeftShift + F5"
    /// Returns false (and Empty) on failure.
    /// </summary>
    public static bool TryParse(string s, out KeyConfig result)
    {
        result = Empty;
        if (string.IsNullOrWhiteSpace(s) || s.Trim() == KeyCode.None.ToString())
            return true;

        var parts = s.Split('+');
        var codes = new KeyCode[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!Enum.TryParse(parts[i].Trim(), out codes[i]))
                return false;
        }

        // Last token = main key, everything before = modifiers
        result = new KeyConfig(codes[^1], codes[..^1]);
        return true;
    }

    public override string ToString()
    {
        if (MainKey == KeyCode.None)
            return KeyCode.None.ToString();
        return string.Join(" + ", Modifiers.Select(m => m.ToString()).Append(MainKey.ToString()));
    }

    #region Equality
    public bool Equals(KeyConfig other) =>
        MainKey == other.MainKey &&
        new HashSet<KeyCode>(Modifiers).SetEquals(other.Modifiers);

    public override bool Equals(object? obj) => obj is KeyConfig k && Equals(k);

    public override int GetHashCode()
    {
        int h = MainKey.GetHashCode();
        foreach (var m in Modifiers.OrderBy(x => x))
            h = HashCode.Combine(h, m);
        return h;
    }

    public static bool operator ==(KeyConfig a, KeyConfig b) => a.Equals(b);
    public static bool operator !=(KeyConfig a, KeyConfig b) => !a.Equals(b);
    #endregion
}
