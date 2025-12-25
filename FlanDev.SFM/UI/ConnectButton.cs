using System;
using BepInEx.Logging;
using HadakaCoat.ObjectsUi.Common.Button;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM.UI;

public sealed class ConnectButton(GameObject parent, string gameObjectName = nameof(ConnectButton), int posFromTop = 0) : MonoBehaviour
{
    private GameObject? GameObject;

    public void Awake()
    {
        GameObject = new GameObject(gameObjectName);
        var rect = GameObject.AddComponent<RectTransform>();
        GameObject.transform.SetParent(parent.transform);

        var pRect = parent.GetComponent<RectTransform>();
        var x = pRect.rect.width * 0.95f;

        rect.sizeDelta = new Vector2(x, x / 8);
        var calculatedPos = (pRect.rect.height / 2) - (rect.sizeDelta.y / 2) - 8 - posFromTop * (rect.sizeDelta.y + 8);
        rect.anchoredPosition = new Vector3(0, calculatedPos);

        var image = GameObject.AddComponent<Image>();
        image.color = new Color(255, 20, 150, 0.5f);
        image.pixelsPerUnitMultiplier = 1f;

        var butttonView = GameObject.AddComponent<ButtonView>();
        butttonView.transform.SetParent(rect);
        butttonView.text = GameObject.AddComponent<TextMeshProUGUI>();
        butttonView.text.font = TMP_Settings.defaultFontAsset;
        butttonView.text.color = Color.magenta;
        butttonView.text.fontSize = rect.sizeDelta.y * 0.8f;
        butttonView.text.text = gameObjectName;

        butttonView.button = GameObject.AddComponent<Button>();
        butttonView.button.onClick.AddListener(new Action(() => $"Clicked button {gameObjectName}".Log()));
    }

    public void Update()
    {
        if (!GameObject)
            $"{nameof(ConnectButton)} is null.".Log(LogLevel.Warning);
    }
}