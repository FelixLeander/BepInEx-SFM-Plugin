using ExposureUnnoticed2.Object3D.Player.Scripts;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FlanderDev.SFM.IrlVibes.UnityObjects;

internal class PlayerAnimation : MonoBehaviour
{
    public void Awake()
    {
    }

    public void Update()
    {
        // Get a reference to PlayerClassAccesor
        var playerAccessor = PlayerController.Instance.GetComponent<PlayerClassAccessor>();
    }
}
