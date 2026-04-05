using BepInEx;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ExposureUnnoticed2.Object3D.Player.Scripts;
using ExposureUnnoticed2.Object3D.Player.Scripts.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace FlanderDev.SFM.IrlVibes.Business;

/// <summary>
/// MonoBehaviour that runs every frame and listens for the hotkey.
/// We use a MonoBehaviour instead of Harmony patches because this is
/// runtime input polling — there is no game method to hook into.
/// </summary>
public sealed class TeleportOnTopPlugin : MonoBehaviour
{
    private const KeyCode ToggleGravity = KeyCode.F1;
    private const KeyCode FlyUp = KeyCode.UpArrow;
    private const KeyCode FlyDown = KeyCode.DownArrow;

    private bool Floating;

    public void Update()
    {
        // if not floating, toggle floating and disable gravity and downhillCheckers
        if (!Floating && Input.GetKeyDown(ToggleGravity))
        {
            Floating = !Floating;
            $"ENABLE FLOAT: {Floating}".Log();

            foreach (var downhillChecker in PlayerController.Instance.GetComponentsInChildren<DownhillChecker>())
                downhillChecker.enabled = false;

            var rb = PlayerController.Instance.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
        else if (Floating && Input.GetKeyDown(ToggleGravity)) // if not floating, toggle floating and enable gravity and downhillCheckers
        {
            Floating = !Floating;
            $"DISABLE FLOAT: {Floating}".Log();

            foreach (var downhillChecker in PlayerController.Instance.GetComponentsInChildren<DownhillChecker>())
                downhillChecker.enabled = true;

            var rb = PlayerController.Instance.GetComponent<Rigidbody>();
            rb.useGravity = true;
        }

        var isUp = Input.GetKey(FlyUp);
        if (isUp || Input.GetKey(FlyDown))
        {
            var t = PlayerController.Instance.transform.position;
            Helper.MovePlayer(PlayerController.Instance, new Vector3(t.x, t.y += (isUp ? 0.1f : -0.1f), t.z));
        }
    }
}