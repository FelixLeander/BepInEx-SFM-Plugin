using ExposureUnnoticed2.ObjectUI.InGameMenu;
using ExposureUnnoticed2.Scripts.InGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM.FucktIt;

public sealed class UiHelper : MonoBehaviour
{
    public GameObject GameGui { get; set; } = new("tmp");
    public static float Scale { get; set; } = 2f;
    public static readonly float TopBarHeight = 10f;

    public static UiHelper? UiHelperInstance { get; private set; }

    private readonly List<Window> _windows = [];

    public void Setup(GameObject parent)
    {
        if (UiHelperInstance != null)
            return;

        UiHelperInstance = this;

        CreateWindow("MyTest", Color.red, this.gameObject, out _);

        GameGui ??= new GameObject(nameof(UiHelper));
        DontDestroyOnLoad(GameGui);
        var canvas = GameGui.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        GraphicRaycaster graphicRaycaster = GameGui.AddComponent<GraphicRaycaster>();
        graphicRaycaster.enabled = true;

        if (parent != null)
            GameGui.transform.SetParent(parent.transform);

        UpdateGUIScale();
        GameObject gameObject = OldHelper.CreateRect("OuterCase", GameGui, new Vector2(Scale * 200f, Scale * 190f), new Color(0f, 0.1f, 0.8f));
        OldHelper.SetPosition(gameObject, new Vector2(0f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
    }


    public void Update()
    {
        var gameMenuView = InGameMenuView.Instance;
        if (gameMenuView != null)
        {
            if (gameMenuView.gameObject != null)
                Setup(gameMenuView.gameObject);

            if (GameGui != null)
                GameGui.active = gameMenuView.currentState == InGameMenuView.State.Show && !gameMenuView.isOpenChild;
        }

        foreach (var item in _windows)
            item.Update();

        return;

        //void SetActive(GameObject parent, bool _active)
        //{
        //    if (_active)
        //        EnusreGuiCreated(parent);

        //    if ((bool)GameGui)
        //        GameGui.active = _active;
        //}
    }

    public void UpdateGUIScale()
    {
        Scale = GameGui.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width;
        if (GameState.OptionData.AspectRatioIndex == 0)
            Scale /= 1920f;
        else
            Scale /= 2560f;

        Scale *= 3f;
    }

    public void CreateWindow(string name, Color color, GameObject parent, out Window window)
    {
        window = new Window(name, color, parent);
        _windows.Add(window);
    }

    public sealed class Window
    {
        public GameObject GameObject { get; set; }

        public Window(string name, Color backgroundcolor, GameObject parent)
        {
            GameObject = OldHelper.CreateRect($"{name}App", parent, new Vector2(Scale * -2f, Scale * (-2f - TopBarHeight)), backgroundcolor);
            GameObject.active = false;
            OldHelper.SetPosition(GameObject, new Vector2(Scale * 1f, Scale * 1f), null, new Vector2(0f, 0f), new Vector2(1f, 1f));
            //Helper.SetBorder(GameObject, scale * roundcorners);
            //Helper.CreateMenuBar(GameObject, name, menubarheight);
        }

        // Looks wrong, but works as intended.
        public void Update()
        {
            if (GameObject != null)
                return;

            UiHelperInstance?._windows.Remove(this);
            Destroy(GameObject);
        }
    }
}

public static class OldHelper
{
    public static readonly Sprite DefaultSprite = Sprite.Create(Texture2D.redTexture, new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
    public static GameObject CreateRect(string name, GameObject parent, Vector2 size, Color color, Sprite? sprite = null)
    {
        var gameObject = new GameObject(name);
        Object.DontDestroyOnLoad(gameObject);
        gameObject.transform.parent = parent.transform;
        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.pivot = new Vector2(0f, 0f);
        if (color.a <= 0f)
            return gameObject;

        var image = gameObject.AddComponent<Image>();
        image.color = color;
        if (sprite == null)
        {
            image.sprite = DefaultSprite;
            return gameObject;
        }

        image.sprite = sprite;
        if (image.hasBorder)
        {
            image.type = Image.Type.Sliced;
        }
        image.pixelsPerUnitMultiplier = 1f;
        return gameObject;
    }
    public static void SetPosition(GameObject o, Vector2? anchoredPosition, Vector2? pivot = null, Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        var component = o.GetComponent<RectTransform>();

        if (anchorMin.HasValue)
            component.anchorMin = anchorMin.Value;

        if (anchorMax.HasValue)
            component.anchorMax = anchorMax.Value;

        if (pivot.HasValue)
            component.pivot = pivot.Value;

        if (anchoredPosition.HasValue)
            component.anchoredPosition = anchoredPosition.Value;
    }
}
