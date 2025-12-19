#nullable disable

using BepInEx.Logging;
using ExposureUnnoticed2.Object3D.IngameManager;
using ExposureUnnoticed2.ObjectUI.InGameMenu;
using ExposureUnnoticed2.Scripts.InGame;
using System;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlanDev.SFM.FuckIt;

public sealed class InPlaceRewrite : MonoBehaviour
{
    internal ManualLogSource Log = new(nameof(InPlaceRewrite));

    private static readonly Sprite defaultSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
    public class Window
    {
        public GameObject gameObject;

        public Window previousWindow;

        private GameObject backbutton;

        public Window(string name, Color backgroundcolor)
        {
            gameObject = CreateRect($"{name}App", Approot, new Vector2(Scale * -2f, Scale * (-2f - topbarheight)), backgroundcolor);
            gameObject.active = false;
            SetPosition(gameObject, new Vector2(Scale * 1f, Scale * 1f), null, new Vector2(0f, 0f), new Vector2(1f, 1f));
            SetBorder(gameObject, Scale * roundcorners);
            //CreateMenuBar(gameObject, name, menubarheight);
            windows.Add(this);
        }

        private GameObject CreateMenuBar(GameObject window, string title, float height)
        {
            GameObject gameObject = CreateRect("MenuBar", window, new Vector2(0f, Scale * height), new Color(0.8f, 0.8f, 0.8f));
            SetPosition(gameObject, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f));
            GameObject gameObject2 = CreateText(title, gameObject, new Vector2((0f - Scale) * 2f * height, 0f), Color.black, height - 4f);
            SetPosition(gameObject2, new Vector2(Scale * height, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f));
            TextMeshProUGUI component = gameObject2.GetComponent<TextMeshProUGUI>();
            component.enableAutoSizing = true;
            component.fontSizeMax = Scale * (height - 4f);
            float num = height - 4f;
            backbutton = CreateRect("BackButton", gameObject, new Vector2(Scale * num, Scale * num), new Color(0.95f, 0.95f, 0.95f));
            SetPosition(backbutton, new Vector2(Scale * height / 2f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
            SetBorder(backbutton, Scale * num / 2f);
            Button button = backbutton.AddComponent<Button>();
            button.onClick.AddListener((Action)delegate
            {
                ShowPreviousWindow();
            });
            GameObject o = CreateRect("BackButtonIcon", backbutton, new Vector2(Scale * -2f, Scale * -2f), new Color(0f, 0f, 0f));
            SetPosition(o, new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(1f, 1f));
            return gameObject;
        }

        public virtual void OnShow()
        {
            backbutton.active = previousWindow != null;
        }

        public virtual void OnHide()
        {
        }

        public virtual void DoUpdate()
        {
        }

        public void Update()
        {
            if (gameObject != null)
            {
                if (gameObject.activeInHierarchy)
                {
                    DoUpdate();
                }
            }
            else
            {
                windows.Remove(this);
                Destroy();
            }
        }

        public void Destroy()
        {
            OnDestroy();
            UnityEngine.Object.Destroy(gameObject);
        }

        public virtual void OnDestroy()
        {
        }
    }

    public class MainMenu : Window
    {
        public GameObject messengerIconNotify;
        public MainMenu() : base("Menu", new Color(0.95f, 0.95f, 0.95f))
        {
        }

        public override void OnShow() => base.OnShow();
    }

    private static MainMenu mainMenu;

    private static InPlaceRewrite instance;

    public GameObject gameGui;

    private static Window currWindow;

    private static float Scale = 2f;

    private static readonly float topbarheight = 10f;

    private static readonly float roundcorners = 9f;

    private static GameObject Approot;

    private static readonly List<Window> windows = [];

    public InPlaceRewrite() => instance = this;

    public void UpdateGUIScale()
    {
        Scale = gameGui.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width;
        if (GameState.OptionData.AspectRatioIndex == 0)
            Scale /= 1920f;
        else
            Scale /= 2560f;

        Scale *= 3f;
    }

    public static GameObject CreateRect(string name, GameObject parent, Vector2 size, Color color, Sprite sprite = null)
    {
        var gameObject = new GameObject(name);
        DontDestroyOnLoad(gameObject);
        gameObject.transform.parent = parent.transform;
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);
        if (color.a > 0f)
        {
            Image image = gameObject.AddComponent<Image>();
            image.color = color;
            if (sprite != null)
            {
                image.sprite = sprite;
                if (image.hasBorder)
                {
                    image.type = Image.Type.Sliced;
                }
                image.pixelsPerUnitMultiplier = 1f;
            }
            else
            {
                image.sprite = defaultSprite;
            }
        }
        return gameObject;
    }

    private static GameObject CreateText(string caption, GameObject parent, Vector2 size, Color color, float fontsize, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
    {
        var gameObject = new GameObject();
        gameObject.transform.parent = parent.transform;
        TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = caption;
        textMeshProUGUI.alignment = alignment;
        textMeshProUGUI.fontSize = Scale * fontsize;
        textMeshProUGUI.font = PluginFonts.Sans;
        textMeshProUGUI.rectTransform.anchorMin = new Vector2(0f, 0f);
        textMeshProUGUI.rectTransform.anchorMax = new Vector2(1f, 1f);
        textMeshProUGUI.rectTransform.sizeDelta = size;
        textMeshProUGUI.rectTransform.pivot = new Vector2(0f, 0f);
        textMeshProUGUI.rectTransform.anchoredPosition = new Vector2(0f, 0f);
        textMeshProUGUI.color = color;
        return gameObject;
    }

    public static void SetPosition(GameObject o, Vector2? anchoredPosition, Vector2? pivot = null, Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        RectTransform component = o.GetComponent<RectTransform>();
        if (anchorMin.HasValue)
        {
            component.anchorMin = anchorMin.Value;
        }
        if (anchorMax.HasValue)
        {
            component.anchorMax = anchorMax.Value;
        }
        if (pivot.HasValue)
        {
            component.pivot = pivot.Value;
        }
        if (anchoredPosition.HasValue)
        {
            component.anchoredPosition = anchoredPosition.Value;
        }
    }

    public static void SetBorder(GameObject o, float border)
    {
        Image component = o.GetComponent<Image>();
        component.pixelsPerUnitMultiplier = border;
    }

    public static void AddEventListener(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        if (obj.GetComponent<EventTrigger>() == null)
            obj.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new() { eventID = type };
        entry.callback.AddListener(action);
        obj.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public static void AddEventListener(GameObject obj, EventTriggerType type, UnityAction<PointerEventData> action)
    {
        if (obj.GetComponent<EventTrigger>() == null)
            obj.AddComponent<EventTrigger>();


        EventTrigger.Entry entry = new() { eventID = type };
        entry.callback.AddListener((Action<BaseEventData>)delegate (BaseEventData data)
        {
            action.Invoke(ExecuteEvents.ValidateEventData<PointerEventData>(data));
        });
        obj.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    private static GameObject CreateTopBar(GameObject menu)
    {
        GameObject gameObject = CreateRect("TopBar", menu, new Vector2(Scale * -2f, Scale * topbarheight), new Color(0f, 0f, 0f));
        SetPosition(gameObject, new Vector2(Scale * 1f, Scale * -1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f));
        SetBorder(gameObject, Scale * 9f);
        GameObject o = CreateRect("Signal", gameObject, new Vector2(Scale * 4f * 8f, Scale * 8f), new Color(1f, 1f, 1f));
        SetPosition(o, new Vector2(Scale * -6f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
        return gameObject;
    }

    public static GameObject CreateCheckbox(GameObject parent, float size, bool active, bool enabled, UnityAction<bool> action)
    {
        GameObject gameObject = CreateRect("Outer", parent, new Vector2(Scale * size, Scale * size), new Color(0f, 0f, 0f));
        SetPosition(gameObject, null, new Vector2(0.5f, 0.5f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        SetBorder(gameObject, Scale * size / 4f);
        Button button = gameObject.AddComponent<Button>();
        GameObject gameObject2 = CreateRect("Outer", gameObject, new Vector2(Scale * -1f, Scale * -1f), new Color(1f, 1f, 1f));
        SetPosition(gameObject2, null, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(1f, 1f));
        SetBorder(gameObject2, Scale * ((size / 4f) - 1f));
        GameObject tick = CreateRect("Tick", gameObject2, new Vector2(Scale * -1f, Scale * -1f), new Color(0f, 0f, 0f));
        SetPosition(tick, null, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(1f, 1f));
        tick.active = active;
        if (enabled)
        {
            if (action != null)
            {
                button.onClick.AddListener((Action)delegate
                {
                    tick.active = !tick.active;
                    action.Invoke(tick.active);
                });
            }
            else
            {
                button.onClick.AddListener((Action)delegate
                {
                    tick.active = !tick.active;
                });
            }
        }
        return gameObject;
    }

    public static GameObject CreateButton(GameObject parent, string name, float size, Color color, string icon, UnityAction action)
    {
        GameObject gameObject = CreateRect($"{name}Button", parent, new Vector2(Scale * size, Scale * size), color);
        SetBorder(gameObject, Scale * 4f);
        if (action != null)
        {
            Button button = gameObject.AddComponent<Button>();
            button.onClick.AddListener(action);
        }
        GameObject o = CreateRect($"{name}Icon", gameObject, new Vector2(Scale * (size - 4f), Scale * (size - 4f)), Color.white);
        SetPosition(o, null, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        return gameObject;
    }

    private void EnusreGuiCreated(GameObject parent)
    {
        if (gameGui != null)
            return;

        gameGui = new GameObject("MyPhoneGUI");
        DontDestroyOnLoad(gameGui);

        var canvas = gameGui.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GraphicRaycaster graphicRaycaster = gameGui.AddComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = true;
        if (parent != null)
        {
            gameGui.transform.SetParent(parent.transform);
        }

        UpdateGUIScale();

        GameObject gameObject = CreateRect("OuterCase", gameGui, new Vector2(Scale * 100f, Scale * 190f), new Color(0f, 0.1f, 0.8f));
        SetPosition(gameObject, new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        SetBorder(gameObject, Scale * 15f);

        Approot = CreateRect("InnerCase", gameObject, new Vector2(Scale * -10f, Scale * -10f), new Color(255, 0, 0, 1));
        SetPosition(Approot, new Vector2(Scale * 5f, Scale * 5f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f));
        SetBorder(Approot, Scale * 10f);

        if (mainMenu == null || mainMenu.gameObject == null)
            mainMenu = new MainMenu();

        currWindow = null;
        ShowWindow(mainMenu);
    }

    public static void ShowWindow(Window newwindow)
    {
        if (instance.gameGui != null && instance.gameGui.active)
        {
            if (currWindow != null)
            {
                currWindow.gameObject.active = false;
                currWindow.OnHide();
            }
            newwindow.previousWindow = currWindow;
            newwindow.OnShow();
            newwindow.gameObject.active = true;
            currWindow = newwindow;
        }
    }

    public static void ShowPreviousWindow()
    {
        if (instance.gameGui != null && instance.gameGui.active && currWindow != null)
        {
            currWindow.gameObject.active = false;
            currWindow.OnHide();
            Window previousWindow = currWindow.previousWindow;
            if (previousWindow != null)
            {
                previousWindow.gameObject.active = true;
                previousWindow.OnShow();
            }
            currWindow = previousWindow;
        }
    }

    public void SetActive(GameObject parent, bool active)
    {
        if (active)
            EnusreGuiCreated(parent);

        if ((bool)gameGui)
            gameGui.active = active;
    }

    public void Update()
    {
        if (InGameMenuView.Instance == null)
            SetActive(null, false);
        else
        {
            var inGameMenuView = InGameMenuView.Instance;
            var active = inGameMenuView.currentState == InGameMenuView.State.Show && !inGameMenuView.isOpenChild;
            SetActive(inGameMenuView.gameObject, active);
        }

        foreach (var item in windows)
            item.Update();
    }
}
