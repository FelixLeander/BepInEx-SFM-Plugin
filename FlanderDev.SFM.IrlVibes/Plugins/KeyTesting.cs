using BepInEx.Logging;
using FlanderDev.SFM.IrlVibes.Business;
using UnityEngine;


namespace FlanderDev.SFM.IrlVibes.Plugins;

public sealed class KeyTesting : MonoBehaviour
{
    public static int Counter = 1;
    public void Update()
    {
        if (Counter % 1000 == 0)
        {
            "============CHECK".Log(LogLevel.Warning);
            if (Counter > 100000)
                Counter = 1;
        }
        Counter++;

        if (BepInEx.Unity.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.F1))
        {
            "============Old System".Log(LogLevel.Warning);
        }

        if (UnityEngine.InputSystem.Keyboard.current.yKey.isPressed)
        {
            "============New System".Log(LogLevel.Warning);
        }
    }
}
