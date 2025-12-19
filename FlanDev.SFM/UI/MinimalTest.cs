using BepInEx.Logging;
using ExposureUnnoticed2.ObjectUI.InGameMenu;
using UnityEngine;
using UnityEngine.UI;

namespace FlanDev.SFM.UI;

internal sealed class MinimalTest : MonoBehaviour
{
    internal static float Scale { get; set; } = 100f;
    internal ManualLogSource Log = new(nameof(MinimalTest));
    private GameObject? _rootObject;

    //private readonly List<GameObject> _views = [];

    public void Update()
    {
        if (InGameMenuView.Instance is not { } gameMenuView)
            return;

        if (_rootObject == null) // Sadly doing this in 'Awake' fails.
        {
            Log.LogWarning($"Creating {nameof(_rootObject)} for {nameof(MinimalTest)}.");
            _rootObject = CreateRootObject(gameMenuView.gameObject);
            CreateUi(_rootObject);
        }

        var show = gameMenuView.currentState == InGameMenuView.State.Show && !gameMenuView.isOpenChild;
        Log.LogWarning(show ? "show" : "hide");
        _rootObject.active = show;
    }

    private static void CreateUi(GameObject parent)
    {
        parent.CreateRect("Main", new Vector2(Scale * 100f, Scale * 190f), Color.blue);
        parent.SetPosition(new Vector2(10, 10), new Vector2(5f, 5f), new Vector2(1f, 1f));
    }

    private static GameObject CreateRootObject(GameObject parent)
    {
        var rootObject = new GameObject(nameof(MinimalTest));
        DontDestroyOnLoad(rootObject);

        var canvas = rootObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        rootObject.AddComponent<GraphicRaycaster>().enabled = true;

        rootObject.transform.SetParent(parent.transform);
        return rootObject;
    }
}

public static class UiExtensions
{
    public static readonly Sprite DefaultSprite = Sprite.Create(Texture2D.redTexture, new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
    public static GameObject CreateRect(this GameObject parent, string name, Vector2 size, Color color, Sprite? sprite = null)
    {
        var gameObject = new GameObject(name);
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
        gameObject.active = true;
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
            image.type = Image.Type.Sliced;

        image.pixelsPerUnitMultiplier = 1f;
        return gameObject;
    }

    public static void SetPosition(this GameObject gameObject, Vector2? anchoredPosition, Vector2? pivot = null, Vector2? anchorMin = null, Vector2? anchorMax = null)
    {
        var component = gameObject.GetComponent<RectTransform>();

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
