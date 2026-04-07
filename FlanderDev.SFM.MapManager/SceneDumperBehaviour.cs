using ExposureUnnoticed2.Scripts.Mission;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlanderDev.SFM.MapManager;

/// <summary>
/// MonoBehaviour that lives for the lifetime of the process and handles
/// keybind input + optional auto-dump on scene load.
/// </summary>
public class SceneDumperBehaviour : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Auto-dump whenever a new scene finishes loading (optional – comment out if unwanted)
        SceneManager.sceneLoaded += (UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)OnSceneLoaded;
    }

    public void Update()
    {
        // Press F8 to manually trigger a dump of the active scene
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Plugin.Log.LogInfo($"Dump start {DateTime.Now}");
            DumpActiveScene();
            Plugin.Log.LogInfo($"Dump start {DateTime.Now}");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Plugin.Log.LogInfo($"Scene loaded: '{scene.name}' – auto-dumping…");
        DumpScene(scene);
    }

    // ─────────────────────────────────────────────────────────────────────
    //  Public entry-points
    // ─────────────────────────────────────────────────────────────────────

    public static void DumpActiveScene()
    {
        DumpScene(SceneManager.GetActiveScene());
    }

    public static void DumpScene(Scene scene)
    {
        try
        {
            string outputDir = Path.Combine(BepInEx.Paths.BepInExRootPath, "SceneDumps");
            Directory.CreateDirectory(outputDir);

            string safeName = MakeSafeFileName(scene.name);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = Path.Combine(outputDir, $"{safeName}_{timestamp}.txt");

            var sb = new StringBuilder();
            WriteHeader(sb, scene);

            GameObject[] roots = scene.GetRootGameObjects();
            sb.AppendLine($"Root object count : {roots.Length}");
            sb.AppendLine(new string('=', 80));

            foreach (GameObject root in roots)
                WriteGameObject(sb, root, 0);

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Plugin.Log.LogInfo($"Scene dumped → {filePath}");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"SceneDumper error: {ex}");
        }
    }

    private static void WriteHeader(StringBuilder sb, Scene scene)
    {
        sb.AppendLine("╔══════════════════════════════════════════════════════════════════════════════╗");
        sb.AppendLine("║                          BepInEx Il2Cpp Scene Dump                          ║");
        sb.AppendLine("╚══════════════════════════════════════════════════════════════════════════════╝");
        sb.AppendLine($"Timestamp  : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Scene name : {scene.name}");
        sb.AppendLine($"Scene path : {scene.path}");
        sb.AppendLine($"Build index: {scene.buildIndex}");
        sb.AppendLine($"Is loaded  : {scene.isLoaded}");
        sb.AppendLine();
    }

    private static void WriteGameObject(StringBuilder sb, GameObject go, int depth)
    {
        var indent = new string(' ', depth * 2);
        var activeStr = go.activeSelf ? string.Empty : " [INACTIVE]";
        var layerStr = LayerMask.LayerToName(go.layer);
        
        if (string.IsNullOrEmpty(layerStr)) 
            layerStr = go.layer.ToString();

        sb.AppendLine($"{indent}▸ {go.name}{activeStr}  (layer: {layerStr}, tag: {go.tag})");

        // Transform
        Transform t = go.transform;
        sb.AppendLine($"{indent}  Transform:");
        sb.AppendLine($"{indent}    position : {FormatVec3(t.position)}");
        sb.AppendLine($"{indent}    rotation : {FormatVec3(t.eulerAngles)}");
        sb.AppendLine($"{indent}    scale    : {FormatVec3(t.localScale)}");

        // Components
        Il2CppSystem.Collections.Generic.List<Component> comps = new();
        go.GetComponents(comps);

        if (comps.Count > 0)
        {
            sb.AppendLine($"{indent}  Components ({comps.Count}):");
            foreach (Component c in comps)
            {
                if (c == null) continue;
                string typeName = c.GetIl2CppType()?.FullName ?? c.GetType().FullName ?? "???";
                sb.Append($"{indent}    • {typeName}");
                AppendComponentDetails(sb, c, indent + "      ");
                sb.AppendLine();
            }
        }

        // Recurse into children
        for (int i = 0; i < t.childCount; i++)
            WriteGameObject(sb, t.GetChild(i).gameObject, depth + 1);
    }

    /// <summary>
    /// Appends human-readable details for well-known component types.
    /// Extend this switch to cover more component types as needed.
    /// </summary>
    private static void AppendComponentDetails(StringBuilder sb, Component c, string indent)
    {
        switch (c)
        {
            case Camera cam:
                sb.AppendLine();
                sb.AppendLine($"{indent}fieldOfView : {cam.fieldOfView:F2}");
                sb.AppendLine($"{indent}nearClipPlane : {cam.nearClipPlane}  farClipPlane : {cam.farClipPlane}");
                sb.Append($"{indent}clearFlags  : {cam.clearFlags}  cullingMask : {cam.cullingMask}");
                break;

            case Light light:
                sb.AppendLine();
                sb.AppendLine($"{indent}type      : {light.type}");
                sb.AppendLine($"{indent}color     : {light.color}");
                sb.AppendLine($"{indent}intensity : {light.intensity:F3}");
                sb.Append($"{indent}range     : {light.range}");
                break;

            case Renderer rend:
                sb.AppendLine();
                sb.Append($"{indent}enabled : {rend.enabled}  shadowCastingMode : {rend.shadowCastingMode}");
                var mats = rend.sharedMaterials;
                if (mats != null && mats.Length > 0)
                {
                    sb.AppendLine();
                    sb.Append($"{indent}materials : ");
                    var matNames = new List<string>();
                    foreach (var m in mats)
                        matNames.Add(m != null ? m.name : "null");
                    sb.Append(string.Join(", ", matNames));
                }
                break;

            case Collider col:
                sb.Append($"  isTrigger: {col.isTrigger}  enabled: {col.enabled}");
                break;

            case Rigidbody rb:
                sb.AppendLine();
                sb.AppendLine($"{indent}mass        : {rb.mass}");
                sb.AppendLine($"{indent}isKinematic : {rb.isKinematic}");
                sb.Append($"{indent}useGravity  : {rb.useGravity}");
                break;

            case AudioSource audio:
                sb.AppendLine();
                sb.AppendLine($"{indent}clip    : {(audio.clip != null ? audio.clip.name : "null")}");
                sb.AppendLine($"{indent}volume  : {audio.volume:F2}  pitch: {audio.pitch:F2}");
                sb.Append($"{indent}loop    : {audio.loop}  playOnAwake: {audio.playOnAwake}");
                break;

            default:
                
                break;
        }
    }

    private static string FormatVec3(Vector3 v) => $"({v.x:F3}, {v.y:F3}, {v.z:F3})";

    private static string MakeSafeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return string.IsNullOrWhiteSpace(name) ? "UnnamedScene" : name;
    }
}