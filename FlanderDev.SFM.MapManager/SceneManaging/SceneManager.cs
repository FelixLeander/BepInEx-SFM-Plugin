using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.MapManager.SceneManaging;

public static class SceneController
{
    // Define our file paths (Defaulting to the BepInEx/plugins folder)
    private static string BundlePath => Helper.EnsureFilePathAndDir("BundleFile", "MapDump");
    private static string DumpPath => Helper.EnsureFilePathAndDir("DumpFile", "MapDump");

    public static void LoadSceneFromBundle()
    {
        if (!File.Exists(BundlePath))
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogError($"Bundle not found at: {BundlePath}");
            return;
        }

        BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogInfo("Loading AssetBundle...");

        // Load the AssetBundle from disk
        AssetBundle bundle = AssetBundle.LoadFromFile(BundlePath);
        if (bundle == null)
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogError("Failed to load AssetBundle!");
            return;
        }

        // Assuming there is only one scene in the bundle, find its path
        string[] scenePaths = bundle.GetAllScenePaths();
        if (scenePaths.Length == 0)
        {
            BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogError("No scenes found in the AssetBundle!");
            bundle.Unload(false);
            return;
        }

        BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogInfo($"Loading Scene: {scenePaths[0]}");

        // Load the scene natively
        SceneManager.LoadScene(scenePaths[0], LoadSceneMode.Single);

        // Unload the compressed bundle data from memory (false means keep the loaded objects intact)
        bundle.Unload(false);
    }

    public static void DumpSceneHierarchy(Scene currentScene)
    {
        BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogInfo("Dumping current scene hierarchy...");

        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        var sb = new StringBuilder();
        sb.AppendLine($"=== SCENE DUMP: {currentScene.name} ===");
        sb.AppendLine($"Dumped at: {DateTime.Now}");
        sb.AppendLine("=========================================\n");

        foreach (GameObject rootObj in rootObjects)
        {
            DumpGameObject(rootObj, sb, string.Empty);
        }

        File.WriteAllText(DumpPath, sb.ToString());
        BepInEx.Logging.Logger.CreateLogSource("SceneManager").LogInfo($"Scene successfully dumped to: {DumpPath}");
    }

    private static void DumpGameObject(GameObject obj, StringBuilder sb, string indent)
    {
        // Append the GameObject name and Active state
        string activeState = obj.activeSelf ? "[+]" : "[-]";
        sb.AppendLine($"{indent}{activeState} {obj.name} (Layer: {LayerMask.LayerToName(obj.layer)})");

        // Optional: Dump attached components
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null)
            {
                sb.AppendLine($"{indent}  -> {comp.GetIl2CppType().Name}");
            }
        }

        // Recursively dump children
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            DumpGameObject(obj.transform.GetChild(i).gameObject, sb, indent + "    ");
        }
    }
}