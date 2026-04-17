using ExposureUnnoticed2.Object3D.Player.Scripts;
using UnityEngine;

namespace FlanderDev.SFM.General.UnityObjects;

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
