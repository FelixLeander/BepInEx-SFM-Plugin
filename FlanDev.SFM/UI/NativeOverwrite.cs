using BepInEx.Logging;
using ExposureUnnoticed2.Object3D.IngameManager;
using ExposureUnnoticed2.ObjectUI.OptionMenu;
using ExposureUnnoticed2.ObjectUI.SystemMenu;
using HadakaCoat.ObjectsUi.Common.Button;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM.UI;

public sealed class NativeOverwrite : MonoBehaviour
{
    private bool _postInitalFrame;
    private const float GoldenRatio = 20 / 9;

    public GameObject? SystemMenuButton;
    public GameObject? BackPlane;
    public List<GameObject> Buttons = [];
    internal ManualLogSource Log = new(nameof(MinimalTest));


    public ButtonView? OptionsButton;
    public void Update_B()
    {
        if (SystemMenuView.Instance is not { } instance)
            return;

        if (InGameUiManager.Instance.gameObject.GetComponentInChildren<OptionMenuView>() is not { } optionMenuView)
            return;

        if (optionMenuView.GetComponentInChildren<ButtonGroupManager>() is not { } buttonGroupManager)
            return;

        if (OptionsButton == null)
        {
            OptionsButton = Instantiate(buttonGroupManager.buttons[0].Cast<ButtonView>(), buttonGroupManager.transform);
            DestroyImmediate(OptionsButton.GetComponent<Button>());
            var button = OptionsButton.gameObject.AddComponent<Button>();
            button.onClick.AddListener(new Action(() => Log.LogInfo("Custom option-button Clicked!")));

            var iButton = OptionsButton.Cast<ButtonGroupManager.IButtonView>();
            buttonGroupManager.buttons.AddItem(iButton);

            Log.LogInfo("Instanziated OptionsButton");
        }
    }

    public void Update()
    {
        if (SystemMenuView.Instance == null)
            return;

        if (SystemMenuButton != null)
        {
            if (_postInitalFrame)
            {
                _postInitalFrame = false;
                SystemMenuButton.gameObject.GetComponentInChildren<TextMeshProUGUI>()?.SetText("IrlVibs");
            }

            return;
        }

        CreateBackPlane();
        if (BackPlane == null)
            throw new Exception("BackPlane creation failed");

        Buttons.Clear();
        for (int i = 0; i < 3; i++)
        {
            var parent = BackPlane;

            var go = new GameObject($"ButtonConnectToy{i}");
            var rect = go.AddComponent<RectTransform>();
            go.transform.SetParent(parent.transform);

            var pRect = parent.GetComponent<RectTransform>();
            var x = pRect.rect.width * 0.95f;

            rect.sizeDelta = new Vector2(x, x / 8);
            rect.anchoredPosition = new Vector3(0, (pRect.rect.height / 2) - (rect.sizeDelta.y / 2) - 8 - (i * (rect.sizeDelta.y + 8)));

            //var image = go.AddComponent<Image>();
            //image.color = new Color(255, 20, 150, 0.5f);
            //image.pixelsPerUnitMultiplier = 1f;

            var butttonView = go.AddComponent<ButtonView>();
            butttonView.transform.SetParent(rect);
            butttonView.text = go.AddComponent<TextMeshProUGUI>();
            butttonView.text.font = TMP_Settings.defaultFontAsset;
            butttonView.text.color = Color.magenta;
            butttonView.text.fontSize = rect.sizeDelta.y * 0.8f;
            butttonView.text.text = $"Connect to {i}";

            butttonView.button = go.AddComponent<Button>();
            butttonView.button.onClick.AddListener(new Action(() => Log.LogInfo($"Clicked button {i}")));

            Buttons.Add(go);
        }

        CreateSystemMenuButtton();
    }

    private void CreateBackPlane()
    {
        var parent = SystemMenuView.Instance.gameObject;

        var go = new GameObject("MyPlane");
        var rect = go.AddComponent<RectTransform>();
        BackPlane = rect.gameObject;
        go.transform.SetParent(parent.transform);

        var x = parent.GetComponent<RectTransform>().rect.width / 4;
        var y = x * GoldenRatio;

        rect.sizeDelta = new Vector2(x, y);
        var half = rect.sizeDelta / 2;
        rect.position = new Vector2(half.x + 8, half.y + 8);

        var image = BackPlane.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        image.pixelsPerUnitMultiplier = 1f;
    }

    private void CreateSystemMenuButtton()
    {
        var templateMenuEntry = SystemMenuView.Instance.buttonGroupManager.buttons[3].GameObject();
        var menuTransform = SystemMenuView.Instance.buttonGroupManager.gameObject.transform;
        SystemMenuButton = Instantiate(templateMenuEntry, menuTransform);
        SystemMenuButton.name = "IrlVibes";

        // I am confusion
        var buttonView = SystemMenuButton.GetComponent<ButtonView>();
        DestroyImmediate(buttonView.GetComponent<Button>());
        var button = buttonView.gameObject.AddComponent<Button>();
        button.onClick.AddListener(new Action(() => Log.LogInfo("Custom Button Clicked!")));

        _postInitalFrame = true;
        Log.LogInfo("Custom Button Successfully Created and Parented!");
    }
}

public sealed class BackPlane : MonoBehaviour
{
    private GameObject? GameObject;

    private void Update()
    {
        if (GameObject != null)
            return;

        var parent = SystemMenuView.Instance.gameObject;

        var go = new GameObject("MyPlane");
        var rect = go.AddComponent<RectTransform>();
        GameObject = rect.gameObject;
        go.transform.SetParent(parent.transform);

        var x = parent.GetComponent<RectTransform>().rect.width / 4;
        var y = x * GoldenRatio;

        rect.sizeDelta = new Vector2(x, y);
        var half = rect.sizeDelta / 2;
        rect.position = new Vector2(half.x + 8, half.y + 8);

        var image = BackPlane.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        image.pixelsPerUnitMultiplier = 1f;
    }
}