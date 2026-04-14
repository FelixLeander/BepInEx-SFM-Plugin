namespace FlanderDev.SFM.MapManager;

public static class Helper
{
    public static string EnsureFilePathAndDir(string name, string bepInExDir = "dump")
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        var safeName = string.IsNullOrWhiteSpace(name) ? "UnnamedScene" : name;
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        var outputDir = Path.Combine(BepInEx.Paths.BepInExRootPath, bepInExDir);
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        return Path.Combine(outputDir, $"{safeName}_{timestamp}.json");
    }
}
