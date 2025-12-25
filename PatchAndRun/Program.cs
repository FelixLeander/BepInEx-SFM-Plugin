using System.Diagnostics;

var manakaRootDir =Environment.GetEnvironmentVariable("FlanDev_SFM_Root_Dir");
if (manakaRootDir is null)
{
    if (OperatingSystem.IsWindows())
    {
    
    }
    else if (OperatingSystem.IsLinux())
    {
     
    }
    else
        throw new PlatformNotSupportedException("Compiled for wrong operating system.");
}

Process.Start("steam steam://rungameid/9579161763274817536");