using System.Text;
using System.Text.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.MapManager.Dumpers;

/// <summary>
/// MonoBehaviour that lives for the lifetime of the process and handles
/// keybind input + optional auto-dump on scene load.
/// </summary>
public static class JsonDump
{
    public static void DumpScene(Scene scene)
    {
        try
        {
            string filePath = Helper.EnsureFilePathAndDir(scene.name, "SceneDumps");
            var dumpData = new Dictionary<string, object>
            {
                ["header"] = CreateHeader(scene),
                ["rootObjects"] = new List<object>()
            };

            GameObject[] roots = scene.GetRootGameObjects();
            ((List<object>)dumpData["rootObjects"]).AddRange(CreateRootObjects(roots));

            string json = JsonSerializer.Serialize(dumpData, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(filePath, json, Encoding.UTF8);
            Plugin.Log?.LogInfo($"Scene dumped: {filePath} [code_file:1]");
        }
        catch (Exception ex)
        {
            Plugin.Log?.LogError($"SceneDumper error: {ex}");
        }
    }

    private static Dictionary<string, object> CreateHeader(Scene scene)
    {
        return new Dictionary<string, object>
        {
            ["title"] = "BepInEx Il2Cpp Scene Dump",
            ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            ["sceneName"] = scene.name,
            ["scenePath"] = scene.path,
            ["buildIndex"] = scene.buildIndex,
            ["isLoaded"] = scene.isLoaded,
            ["rootObjectCount"] = 0  // Filled later
        };
    }

    private static List<object> CreateRootObjects(GameObject[] roots)
    {
        var rootList = new List<object>();
        foreach (GameObject root in roots)
        {
            rootList.Add(CreateGameObject(root, 0));
        }
        return rootList;
    }

    private static Dictionary<string, object> CreateGameObject(GameObject go, int depth)
    {
        var layerStr = LayerMask.LayerToName(go.layer);
        if (string.IsNullOrEmpty(layerStr)) layerStr = go.layer.ToString();

        var goData = new Dictionary<string, object>
        {
            ["depth"] = depth,
            ["name"] = go.name,
            ["active"] = go.activeSelf,
            ["layer"] = layerStr,
            ["tag"] = go.tag,
            ["transform"] = CreateTransform(go.transform),
            ["components"] = new List<object>(),
            ["children"] = new List<object>()
        };

        // Components
        var comps = new Il2CppSystem.Collections.Generic.List<Component>();
        go.GetComponents(comps);
        foreach (Component c in comps)
        {
            if (c == null) continue;
            string typeName = c.GetIl2CppType()?.FullName ?? c.GetType().FullName ?? "???";
            var compData = new Dictionary<string, object>
            {
                ["type"] = typeName,
                ["details"] = CreateComponentDetails(c)
            };
            ((List<object>)goData["components"]).Add(compData);
        }

        // Children
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            ((List<object>)goData["children"]).Add(CreateGameObject(t.GetChild(i).gameObject, depth + 1));
        }

        return goData;
    }

    private static Dictionary<string, object> CreateTransform(Transform t)
    {
        return new Dictionary<string, object>
        {
            ["position"] = new { x = FormatFloat(t.position.x), y = FormatFloat(t.position.y), z = FormatFloat(t.position.z) },
            ["rotation"] = new { x = FormatFloat(t.eulerAngles.x), y = FormatFloat(t.eulerAngles.y), z = FormatFloat(t.eulerAngles.z) },
            ["scale"] = new { x = FormatFloat(t.localScale.x), y = FormatFloat(t.localScale.y), z = FormatFloat(t.localScale.z) }
        };
    }

    private static Dictionary<string, object> CreateComponentDetails(Component c)
    {
        var details = new Dictionary<string, object>();
        // Reuse your switch logic, but populate dict instead of StringBuilder
        switch (c)
        {
            case Camera cam:
                details["fieldOfView"] = FormatFloat(cam.fieldOfView);
                details["nearClipPlane"] = cam.nearClipPlane;
                details["farClipPlane"] = cam.farClipPlane;
                details["clearFlags"] = cam.clearFlags.ToString();
                details["cullingMask"] = cam.cullingMask;
                break;

            case Light light:
                details["type"] = light.type.ToString();
                details["color"] = new { r = light.color.r, g = light.color.g, b = light.color.b, a = light.color.a };
                details["intensity"] = FormatFloat(light.intensity);
                details["range"] = light.range;
                break;

            case Renderer rend:
                details["enabled"] = rend.enabled;
                details["shadowCastingMode"] = rend.shadowCastingMode.ToString();
                var matNames = new List<object>();
                if (rend.sharedMaterials != null)
                    foreach (var m in rend.sharedMaterials)
                        matNames.Add(m?.name ?? "null");
                details["materials"] = matNames;
                break;

            case Collider col:
                details["isTrigger"] = col.isTrigger;
                details["enabled"] = col.enabled;
                break;

            case Rigidbody rb:
                details["mass"] = rb.mass;
                details["isKinematic"] = rb.isKinematic;
                details["useGravity"] = rb.useGravity;
                break;

            case AudioSource audio:
                details["clip"] = audio.clip?.name ?? "null";
                details["volume"] = FormatFloat(audio.volume);
                details["pitch"] = FormatFloat(audio.pitch);
                details["loop"] = audio.loop;
                details["playOnAwake"] = audio.playOnAwake;
                break;
        }
        return details;
    }

    private static float FormatFloat(float f) => (float)Math.Round(f, 3);
}