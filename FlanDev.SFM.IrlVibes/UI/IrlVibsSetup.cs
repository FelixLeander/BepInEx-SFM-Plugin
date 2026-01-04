using BepInEx.Logging;
using ExposureUnnoticed2.ObjectUI.InGame.VIbeStatePanel;
using ExposureUnnoticed2.Scripts.InGame;
using UnityEngine;

namespace FlanDev.SFM.UI;

public sealed class IrlVibsSetup : MonoBehaviour
{
    public ManualLogSource Log = new(nameof(IrlVibsSetup));
    public BackPlane? rootObject;
    public void Awake()
    {
        rootObject ??= gameObject.AddComponent<BackPlane>();
    }

    public void Update()
    {
        if (!InGameManager.Instance)
            return;

        var vibeStatePanelView = InGameManager.Instance.GetComponentInChildren<VibeStatePanelView>();
        if (vibeStatePanelView == null)
            Log.LogError($"{nameof(vibeStatePanelView)} not initialized.");
        else
            Log.LogError($"VibeState: {vibeStatePanelView.currentVibeType}");

        if (rootObject)
            rootObject?.gameObject.SetActive(true);
        else
            Log.LogError($"{nameof(rootObject)} not initialized.");
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