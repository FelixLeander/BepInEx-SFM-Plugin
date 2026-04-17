using BepInEx.Logging;
using ExposureUnnoticed2.Object3D.Player.Scripts;
using ExposureUnnoticed2.Object3D.Player.Scripts.Other;
using FlanderDev.SFM.General.Business;
using UnityEngine;

namespace FlanderDev.SFM.General.UnityObjects;

public sealed class DebugFloat : MonoBehaviour
{
    private const KeyCode ToggleGravity = KeyCode.RightControl;
    private const KeyCode FloatUp = KeyCode.UpArrow;
    private const KeyCode FloatDown = KeyCode.DownArrow;
    private const KeyCode BoostFloat = KeyCode.RightShift;

    private bool Floating;

    public void Update()
    {
        if (Plugin.Shortcut.IsDown())
            Plugin.Instance.Log.LogFatal("YYYYYYYYYYYYYYYYYYYY");

        // if not floating, toggle floating and disable gravity and downhillCheckers
        if (!Floating && Input.GetKeyDown(ToggleGravity))
        {
            Floating = !Floating;
            $"ENABLE FLOAT: {Floating}".Log(LogLevel.Info);

            foreach (var downhillChecker in PlayerController.Instance.GetComponentsInChildren<DownhillChecker>())
                downhillChecker.enabled = false;

            var rb = PlayerController.Instance.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
        else if (Floating && Input.GetKeyDown(ToggleGravity)) // if not floating, toggle floating and enable gravity and downhillCheckers
        {
            Floating = !Floating;
            $"DISABLE FLOAT: {Floating}".Log(LogLevel.Info);

            foreach (var downhillChecker in PlayerController.Instance.GetComponentsInChildren<DownhillChecker>())
                downhillChecker.enabled = true;

            var rb = PlayerController.Instance.GetComponent<Rigidbody>();
            rb.useGravity = true;
        }

        var isUp = Input.GetKey(FloatUp);
        if (isUp || Input.GetKey(FloatDown))
        {
            var boostFloat = Input.GetKey(BoostFloat) ? 10f : 1f;
            var t = PlayerController.Instance.transform.position;
            Helper.MovePlayer(PlayerController.Instance, new Vector3(t.x, t.y += (isUp ? 0.5f : -0.5f) * boostFloat * Time.deltaTime, t.z));
        }
    }
}