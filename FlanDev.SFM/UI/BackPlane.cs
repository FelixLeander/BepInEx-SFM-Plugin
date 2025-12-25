using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM.UI;

public sealed class BackPlane : MonoBehaviour
{
    public GameObject? Self;
    public GameObject? Parent;
    public readonly List<ConnectButton> ConnectButtons = [];

    public void AddButton(ConnectButton button)
    {
        ConnectButtons.Add(button);
    }

    public void Awake()
    {
        var go = new GameObject(nameof(BackPlane));
        var rect = go.AddComponent<RectTransform>();
        Self = rect.gameObject;
        go.transform.SetParent(Parent.transform);

        var x = Parent.GetComponent<RectTransform>().rect.width / 4;
        var y = x * Helper.GoldenRatio;

        rect.sizeDelta = new Vector2(x, y);
        var half = rect.sizeDelta / 2;
        rect.position = new Vector2(half.x + 8, half.y + 8);

        var image = Self.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        image.pixelsPerUnitMultiplier = 1f;
    }
}