using BepInEx.Logging;
using ExposureUnnoticed2.ObjectUI.InGame.VIbeStatePanel;
using ExposureUnnoticed2.ObjectUI.SystemMenu;
using ExposureUnnoticed2.Scripts.InGame;
using UnityEngine;

namespace FlanderDev.SFM.IrlVibes.Business;

/// <summary>
/// Intended as a global singleton manager.
/// </summary>
public sealed class IrlVibsSetup : MonoBehaviour
{
    public static IrlVibsSetup? Instance { get; set; }

    public void Awake()
    {
        $"AWAKEN: {nameof(IrlVibsSetup)}".Log();

        if (Instance) // If already exists: keep old, destroy new.
        {
            $"A second {nameof(IrlVibsSetup)} tired to be created.".Log(LogLevel.Warning);
            Destroy(gameObject);
            return;
        }

        $"{nameof(IrlVibsSetup)} initialized.".Log();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if (!SystemMenuView.Instance)
            return;

        var vibeStatePanelView = InGameManager.Instance.GetComponentInChildren<VibeStatePanelView>();
        if (vibeStatePanelView == null)
            $"{nameof(vibeStatePanelView)} not initialized.".Log();
        else
            $"VibeState: {vibeStatePanelView.currentVibeType}".Log();
    }

    private void HandleFlyMovement()
    {
        // Try to orient movement relative to the main camera.
        // If there's no camera, fall back to world space.
        Transform reference = Camera.main != null ? Camera.main.transform : transform;

        float horizontal = Input.GetAxis("Horizontal");   // A / D
        float vertical = Input.GetAxis("Vertical");       // W / S
        float upDown = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) upDown = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) upDown = -1f;

        // Build a movement vector from the camera's forward/right directions,
        // but strip the Y component from forward so W/S doesn't tilt up/down
        // when the camera is angled. Players expect W to go forward, not skyward.
        Vector3 moveDir =
            reference.forward * vertical +    // Forward / back (camera-relative, flat)
            reference.right * horizontal +    // Strafe left / right
            Vector3.up * upDown;              // Absolute vertical

        // Normalize to prevent diagonal movement being faster,
        // then only normalize if magnitude > 1 to keep slow analog input working.
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        // Directly translate the Transform — this bypasses all physics.
        InGameManager.Instance.transform.position += moveDir * 10f * Time.deltaTime;
        InGameManager.Instance.transform.position.ToString().Log();
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