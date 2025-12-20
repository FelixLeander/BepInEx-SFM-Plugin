using BepInEx.Logging;
using ExposureUnnoticed2.ObjectUI.SystemMenu;
using HadakaCoat.ObjectsUi.Common.Button;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM.UI;

public sealed class IrlVibsSetup : MonoBehaviour
{
    private BackPlane? BackPlane;
    public void Update()
    {
        if (SystemMenuView.Instance == null)
            return;

        BackPlane ??= new BackPlane(SystemMenuView.Instance.gameObject);
    }
    //public void Update_B()
    //{
    //    if (SystemMenuView.Instance is not { } instance)
    //        return;

    //    if (InGameUiManager.Instance.gameObject.GetComponentInChildren<OptionMenuView>() is not { } optionMenuView)
    //        return;

    //    if (optionMenuView.GetComponentInChildren<ButtonGroupManager>() is not { } buttonGroupManager)
    //        return;

    //    if (OptionsButton == null)
    //    {
    //        OptionsButton = Instantiate(buttonGroupManager.buttons[0].Cast<ButtonView>(), buttonGroupManager.transform);
    //        DestroyImmediate(OptionsButton.GetComponent<Button>());
    //        var button = OptionsButton.gameObject.AddComponent<Button>();
    //        button.onClick.AddListener(new Action(() => Log.LogInfo("Custom option-button Clicked!")));

    //        var iButton = OptionsButton.Cast<ButtonGroupManager.IButtonView>();
    //        buttonGroupManager.buttons.AddItem(iButton);

    //        Log.LogInfo("Instanziated OptionsButton");
    //    }
    //}

    //private void CreateBackPlane()
    //{
    //    var parent = SystemMenuView.Instance.gameObject;

    //    var go = new GameObject("MyPlane");
    //    var rect = go.AddComponent<RectTransform>();
    //    BackPlane = rect.gameObject;
    //    go.transform.SetParent(parent.transform);

    //    var x = parent.GetComponent<RectTransform>().rect.width / 4;
    //    var y = x * GoldenRatio;

    //    rect.sizeDelta = new Vector2(x, y);
    //    var half = rect.sizeDelta / 2;
    //    rect.position = new Vector2(half.x + 8, half.y + 8);

    //    var image = BackPlane.AddComponent<Image>();
    //    image.color = new Color(0, 0, 0, 0.5f);
    //    image.pixelsPerUnitMultiplier = 1f;
    //}

    //private void CreateSystemMenuButtton()
    //{
    //    var templateMenuEntry = SystemMenuView.Instance.buttonGroupManager.buttons[3].GameObject();
    //    var menuTransform = SystemMenuView.Instance.buttonGroupManager.gameObject.transform;
    //    SystemMenuButton = Instantiate(templateMenuEntry, menuTransform);
    //    SystemMenuButton.name = "IrlVibes";

    //    // I am confusion
    //    var buttonView = SystemMenuButton.GetComponent<ButtonView>();
    //    DestroyImmediate(buttonView.GetComponent<Button>());
    //    var button = buttonView.gameObject.AddComponent<Button>();
    //    button.onClick.AddListener(new Action(() => Log.LogInfo("Custom Button Clicked!")));

    //    _postInitalFrame = true;
    //    Log.LogInfo("Custom Button Successfully Created and Parented!");
    //}
}

public static class Helper
{
    public const float GoldenRatio = 20 / 9;
    public static ManualLogSource? Logger { get; set; }
    public static void Log(this string text, LogLevel logLevel = LogLevel.Info) => Logger?.Log(logLevel, text);

}

public sealed class ConnectButton(GameObject parent, string gameObjectName = nameof(ConnectButton)) : MonoBehaviour
{
    private GameObject? GameObject;
    public void Update()
    {
        if (GameObject != null)
            return;

        GameObject = new GameObject(gameObjectName);
        var rect = GameObject.AddComponent<RectTransform>();
        GameObject.transform.SetParent(parent.transform);

        var pRect = parent.GetComponent<RectTransform>();
        var x = pRect.rect.width * 0.95f;

        rect.sizeDelta = new Vector2(x, x / 8);
        rect.anchoredPosition = new Vector3(0, (pRect.rect.height / 2) - (rect.sizeDelta.y / 2) - 8 - (i * (rect.sizeDelta.y + 8)));

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
}

public sealed class BackPlane(GameObject parent, string gameObjectName = nameof(BackPlane)) : MonoBehaviour
{
    private GameObject? GameObject;

    private readonly List<ConnectButton> ConnectButtons = [];

    public void AddButton(ConnectButton button)
    {
        ConnectButtons.Add(button);
    }

    public void Update()
    {
        if (GameObject != null)
            return;

        GameObject = new GameObject(gameObjectName);
        GameObject.transform.SetParent(parent.transform);

        var x = parent.GetComponent<RectTransform>().rect.width / 4;
        var y = x * Helper.GoldenRatio;

        var rect = GameObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(x, y);
        var half = rect.sizeDelta / 2;
        rect.position = new Vector2(half.x + 8, half.y + 8);

        var image = GameObject.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f);
        image.pixelsPerUnitMultiplier = 1f;
    }
}