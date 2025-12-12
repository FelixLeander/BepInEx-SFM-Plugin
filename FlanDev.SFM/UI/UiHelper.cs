using ExposureUnnoticed2.Object3D.IngameManager;
using ExposureUnnoticed2.ObjectUI.InGameMenu;
using ExposureUnnoticed2.Scripts.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace FlanDev.SFM.UI;

public sealed class UiHelper : MonoBehaviour
{
    private static readonly Sprite defaultSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
    public class Window
    {
        public GameObject gameObject;

        public Window previousWindow;

        private GameObject backbutton;

        public Window(string name, Color backgroundcolor)
        {
            gameObject = CreateRect(name + "App", Approot, new Vector2(scale * -2f, scale * (-2f - topbarheight)), backgroundcolor);
            gameObject.active = false;
            SetPosition(gameObject, new Vector2(scale * 1f, scale * 1f), null, new Vector2(0f, 0f), new Vector2(1f, 1f));
            SetBorder(gameObject, scale * roundcorners);
            CreateMenuBar(gameObject, name, menubarheight);
            windows.Add(this);
        }

        private GameObject CreateMenuBar(GameObject window, string title, float height)
        {
            GameObject gameObject = CreateRect("MenuBar", window, new Vector2(0f, scale * height), new Color(0.8f, 0.8f, 0.8f));
            SetPosition(gameObject, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f));
            GameObject gameObject2 = CreateText(title, gameObject, new Vector2((0f - scale) * 2f * height, 0f), Color.black, height - 4f);
            SetPosition(gameObject2, new Vector2(scale * height, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f));
            TextMeshProUGUI component = gameObject2.GetComponent<TextMeshProUGUI>();
            component.enableAutoSizing = true;
            component.fontSizeMax = scale * (height - 4f);
            float num = height - 4f;
            backbutton = CreateRect("BackButton", gameObject, new Vector2(scale * num, scale * num), new Color(0.95f, 0.95f, 0.95f));
            SetPosition(backbutton, new Vector2(scale * height / 2f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
            SetBorder(backbutton, scale * num / 2f);
            Button button = backbutton.AddComponent<Button>();
            button.onClick.AddListener((Action)delegate
            {
                ShowPreviousWindow();
            });
            GameObject o = CreateRect("BackButtonIcon", backbutton, new Vector2(scale * -2f, scale * -2f), new Color(0f, 0f, 0f));
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

        public override void OnShow()
        {
            base.OnShow();
        }
    }

    private static MainMenu mainMenu;

    private static UiHelper instance;

    public GameObject gui;

    private static Window currWindow;

    private GameObject messengerGlobalIcon;

    private static float scale = 2f;

    private static float topbarheight = 10f;

    private static float roundcorners = 9f;

    private static float menubarheight = 14f;

    private readonly TMP_FontAsset font;

    private static GameObject Approot;

    private static List<Window> windows;

    public UiHelper()
    {
        instance = this;
        windows = [];
    }

    public void UpdateGUIScale()
    {
        scale = gui.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width;
        if (GameState.OptionData.AspectRatioIndex == 0)
        {
            scale /= 1920f;
        }
        else
        {
            scale /= 2560f;
        }
        scale *= 3f;
    }

    public static GameObject CreateRect(string name, GameObject parent, Vector2 size, Color color, Sprite sprite = null)
    {
        GameObject gameObject = new GameObject(name);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
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
        GameObject gameObject = new GameObject();
        gameObject.transform.parent = parent.transform;
        TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = caption;
        textMeshProUGUI.alignment = alignment;
        textMeshProUGUI.fontSize = scale * fontsize;
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
        {
            obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        obj.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public static void AddEventListener(GameObject obj, EventTriggerType type, UnityAction<PointerEventData> action)
    {
        if (obj.GetComponent<EventTrigger>() == null)
        {
            obj.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((Action<BaseEventData>)delegate (BaseEventData data)
        {
            action.Invoke(ExecuteEvents.ValidateEventData<PointerEventData>(data));
        });
        obj.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    private GameObject CreateTopBar(GameObject menu)
    {
        GameObject gameObject = CreateRect("TopBar", menu, new Vector2(scale * -2f, scale * topbarheight), new Color(0f, 0f, 0f));
        SetPosition(gameObject, new Vector2(scale * 1f, scale * -1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(1f, 1f));
        SetBorder(gameObject, scale * 9f);
        GameObject o = CreateRect("Signal", gameObject, new Vector2(scale * 4f * 8f, scale * 8f), new Color(1f, 1f, 1f));
        SetPosition(o, new Vector2(scale * -6f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
        return gameObject;
    }

    public static GameObject CreateCheckbox(GameObject parent, float size, bool active, bool enabled, UnityAction<bool> action)
    {
        GameObject gameObject = CreateRect("Outer", parent, new Vector2(scale * size, scale * size), new Color(0f, 0f, 0f));
        SetPosition(gameObject, null, new Vector2(0.5f, 0.5f), new Vector2(0f, 1f), new Vector2(0f, 1f));
        SetBorder(gameObject, scale * size / 4f);
        Button button = gameObject.AddComponent<Button>();
        GameObject gameObject2 = CreateRect("Outer", gameObject, new Vector2(scale * -1f, scale * -1f), new Color(1f, 1f, 1f));
        SetPosition(gameObject2, null, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(1f, 1f));
        SetBorder(gameObject2, scale * (size / 4f - 1f));
        GameObject tick = CreateRect("Tick", gameObject2, new Vector2(scale * -1f, scale * -1f), new Color(0f, 0f, 0f));
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

    public static GameObject CreateNotifyIcon(GameObject parent, float size)
    {
        GameObject gameObject = CreateRect("NotifyIcon", parent, new Vector2(scale * size, scale * size), Color.red);
        SetPosition(gameObject, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));
        SetBorder(gameObject, scale * size / 2f);
        GameObject o = CreateRect("NotifyIcon2", gameObject, new Vector2(0f, 0f), Color.white);
        SetPosition(o, null, null, new Vector2(0f, 0f), new Vector2(1f, 1f));
        return gameObject;
    }

    public static GameObject CreateButton(GameObject parent, string name, float size, Color color, string icon, UnityAction action)
    {
        GameObject gameObject = CreateRect(name + "Button", parent, new Vector2(scale * size, scale * size), color);
        SetBorder(gameObject, scale * 4f);
        if (action != null)
        {
            Button button = gameObject.AddComponent<Button>();
            button.onClick.AddListener(action);
        }
        GameObject o = CreateRect(name + "Icon", gameObject, new Vector2(scale * (size - 4f), scale * (size - 4f)), Color.white);
        SetPosition(o, null, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        return gameObject;
    }

    private void CreateGUI(GameObject parent)
    {
        if (gui == null)
        {
            gui = new GameObject("MyPhoneGUI");
            DontDestroyOnLoad(gui);
            var canvas = gui.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            GraphicRaycaster graphicRaycaster = gui.AddComponent<GraphicRaycaster>();
            graphicRaycaster.enabled = true;
            if (parent != null)
            {
                gui.transform.SetParent(parent.transform);
            }

            UpdateGUIScale();
            GameObject gameObject = CreateRect("OuterCase", gui, new Vector2(scale * 100f, scale * 190f), new Color(0f, 0.1f, 0.8f));
            SetPosition(gameObject, new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            SetBorder(gameObject, scale * 15f);
            GameObject o = CreateRect("OuterCaseSheen", gameObject, new Vector2(0f, 0f), new Color(1f, 1f, 1f, 0.25f));
            SetPosition(o, null, null, new Vector2(0f, 0f), new Vector2(1f, 1f));
            SetBorder(o, scale * 15f);
            GameObject o2 = CreateRect("OuterCaseShadow", gameObject, new Vector2(0f, 0f), new Color(0f, 0f, 0f, 2f));
            SetPosition(o2, null, null, new Vector2(0f, 0f), new Vector2(1f, 1f));
            SetBorder(o2, scale * 15f);
            Approot = CreateRect("InnerCase", gameObject, new Vector2(scale * -10f, scale * -10f), Color.black);
            SetPosition(Approot, new Vector2(scale * 5f, scale * 5f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(1f, 1f));
            SetBorder(Approot, scale * 10f);
            CreateTopBar(Approot);

            if (mainMenu == null || mainMenu.gameObject == null)
                mainMenu = new MainMenu();

            currWindow = null;
            ShowWindow(mainMenu);
        }
    }

    private void CreateMessengerGlobalIcon()
    {
        if (messengerGlobalIcon == null && InGameUiManager.Instance != null && InGameUiManager.Instance.middleCloneLayer != null)
        {
            GameObject parent = InGameUiManager.Instance.middleCloneLayer.gameObject;
            messengerGlobalIcon = CreateButton(parent, "MessengerNotify", 20f, new Color(0f, 0.9f, 0f), "messenger_icon", null);
            SetPosition(messengerGlobalIcon, new Vector2((0f - scale) * 360f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f));
            CreateNotifyIcon(messengerGlobalIcon, 8f);
            messengerGlobalIcon.active = false;
        }
    }

    public static void ShowWindow(Window newwindow)
    {
        if (instance.gui != null && instance.gui.active)
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
        if (instance.gui != null && instance.gui.active && currWindow != null)
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

    public void SetActive(GameObject parent, bool _active)
    {
        if (_active)
        {
            CreateGUI(parent);
        }
        if ((bool)gui)
        {
            gui.active = _active;
        }
    }

    public void Update()
    {
        if (InGameMenuView.Instance != null)
        {
            InGameMenuView inGameMenuView = InGameMenuView.Instance;
            GameObject parent = inGameMenuView.gameObject;
            SetActive(parent, inGameMenuView.currentState == InGameMenuView.State.Show && !inGameMenuView.isOpenChild);
        }
        else
        {
            SetActive(null, _active: false);
        }

        foreach (Window item in windows.ToList())
        {
            item.Update();
        }
        CreateMessengerGlobalIcon();
    }
}
