#nullable disable

using BepInEx;
using ExposureUnnoticed2.Object3D.IngameManager;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace FlanDev.SFM;

internal static class PluginFonts
{
    private static readonly TMP_FontAsset sans;

    private static readonly TMP_FontAsset serif;

    private static readonly GameObject g_sans;

    private static readonly GameObject g_serif;

    public static TMP_FontAsset Sans => sans ?? AssetReferencer.Instance.defaultFont;

    public static TMP_FontAsset Serif => serif ?? AssetReferencer.Instance.defaultFont;

    static PluginFonts()
    {
        List<string> countyCodes = ["", "JP", "SC", "TC", "KR"];
        List<string> fontFormats = ["ttf", "otf"];
        if (sans == null)
        {
            foreach (string cc in countyCodes)
            {
                foreach (string ext in fontFormats)
                {
                    var fileName = $"NotoSans{cc}-Regular.{ext}";
                    string filePath = Path.Combine(Paths.GameRootPath, fileName);
                    if (!File.Exists(filePath))
                    {
                        continue;
                    }
                    Font font = new(filePath);
                    TMP_FontAsset tMP_FontAsset = TMP_FontAsset.CreateFontAsset(font);
                    Object.DontDestroyOnLoad(tMP_FontAsset);
                    if (sans == null)
                    {
                        sans = tMP_FontAsset;
                        break;
                    }
                    sans.fallbackFontAssetTable ??= new Il2CppSystem.Collections.Generic.List<TMP_FontAsset>();
                    sans.fallbackFontAssetTable.Add(tMP_FontAsset);
                    break;
                }
            }
            g_sans = new GameObject();
            Object.DontDestroyOnLoad(g_sans);
            TextMeshProUGUI textMeshProUGUI = g_sans.AddComponent<TextMeshProUGUI>();
            textMeshProUGUI.font = sans;
        }

        if (serif != null)
            return;

        foreach (string cc in countyCodes)
        {
            foreach (string ext in fontFormats)
            {
                string text2 = Path.Combine(Paths.GameRootPath, $"NotoSerif{cc}-Regular.{ext}");
                if (!File.Exists(text2))
                {
                    continue;
                }
                Font font2 = new(text2);
                TMP_FontAsset tMP_FontAsset2 = TMP_FontAsset.CreateFontAsset(font2);
                Object.DontDestroyOnLoad(tMP_FontAsset2);
                if (serif == null)
                {
                    serif = tMP_FontAsset2;
                    break;
                }
                serif.fallbackFontAssetTable ??= new Il2CppSystem.Collections.Generic.List<TMP_FontAsset>();
                serif.fallbackFontAssetTable.Add(tMP_FontAsset2);
                break;
            }
        }

        g_serif = new GameObject();
        Object.DontDestroyOnLoad(g_serif);
        TextMeshProUGUI textMeshProUGUI2 = g_serif.AddComponent<TextMeshProUGUI>();
        textMeshProUGUI2.font = serif;
    }

    public static void AddFallbackFontAssets(TMP_FontAsset font, TMP_FontAsset fallback)
    {
        font.fallbackFontAssetTable ??= new Il2CppSystem.Collections.Generic.List<TMP_FontAsset>();

        if (!font.fallbackFontAssetTable.Contains(fallback))
            font.fallbackFontAssetTable.Add(fallback);
    }

    public static void AddFallbackFontAssets()
    {
        if (serif == null)
            return;

        AddFallbackFontAssets(AssetReferencer.Instance.defaultFont, serif);
        AddFallbackFontAssets(AssetReferencer.Instance.fontSc, serif);
        AddFallbackFontAssets(AssetReferencer.Instance.fontTc, serif);
        AddFallbackFontAssets(AssetReferencer.Instance.fontKr, serif);
    }
}
