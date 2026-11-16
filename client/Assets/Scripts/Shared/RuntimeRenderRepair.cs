using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Repairs legacy scene data that can make every Canvas element render at zero
// scale on Android while audio and the rest of the scene continue running.
public static class RuntimeRenderRepair
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ConfigureAndroidRenderPipeline()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        // The device still reports a URP NRE before the first scene camera
        // renders. The game is 2D/UI-driven, so use Unity's built-in path on
        // Android until the URP asset chain is repaired independently.
        GraphicsSettings.defaultRenderPipeline = null;
        QualitySettings.renderPipeline = null;
        Debug.Log("RuntimeRenderRepair selected the built-in Android render pipeline.");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RepairActiveScene()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera camera in cameras)
            {
                if (camera.enabled)
                {
                    mainCamera = camera;
                    break;
                }
            }
        }

        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            RectTransform rectTransform = canvas.transform as RectTransform;
            if (rectTransform != null && rectTransform.localScale == Vector3.zero)
            {
                rectTransform.localScale = Vector3.one;
                Debug.LogWarning($"RuntimeRenderRepair restored zero-scale Canvas: {canvas.name}");
            }

            if (mainCamera != null &&
                canvas.renderMode == RenderMode.ScreenSpaceCamera &&
                canvas.worldCamera == null)
            {
                canvas.worldCamera = mainCamera;
            }
        }

        if (mainCamera == null)
        {
            Debug.LogError("RuntimeRenderRepair could not find an enabled camera.");
        }

        RuntimeTouchBridge.Install();
    }
}

// The public scene data contains an InputSystemUIInputModule, but its serialized
// action asset does not deliver pointer events on the target Android build. Keep
// the existing Button bindings and feed them through Unity's normal event chain
// from the touch device directly.
internal sealed class RuntimeTouchBridge : MonoBehaviour
{
    private static RuntimeTouchBridge instance;
    private static readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    private PointerEventData pointerData;
    private EventSystem pointerEventSystem;
    private GameObject pressedObject;
    private int pointerId = -1000;
    private float nextAssetRepairTime;

    public static void Install()
    {
        if (instance == null)
        {
            var bridgeObject = new GameObject("RuntimeTouchBridge");
            DontDestroyOnLoad(bridgeObject);
            instance = bridgeObject.AddComponent<RuntimeTouchBridge>();
            SceneManager.sceneLoaded += (_, __) => instance.RefreshSceneInput();
            Debug.Log("RuntimeTouchBridge installed.");
        }

        instance.RefreshSceneInput();
    }

    private void Update()
    {
        if (Time.unscaledTime >= nextAssetRepairTime)
        {
            RuntimeLocationCardRepair.Apply();
            nextAssetRepairTime = Time.unscaledTime + 1f;
        }

        if (!TryReadPointerFrame(out var position, out var pressed, out var released, out var currentPointerId))
        {
            return;
        }

        if (pressed)
        {
            BeginPointer(position, currentPointerId);
        }

        if (released)
        {
            EndPointer(position);
        }
    }

    private void RefreshSceneInput()
    {
        var eventSystems = FindObjectsOfType<EventSystem>();
        foreach (var eventSystem in eventSystems)
        {
            var inputModules = eventSystem.GetComponents<BaseInputModule>();
            foreach (var inputModule in inputModules)
            {
                if (inputModule.enabled)
                {
                    inputModule.enabled = false;
                    Debug.Log($"RuntimeTouchBridge disabled {inputModule.GetType().Name} on {eventSystem.name}.");
                }
            }
        }

        var canvases = FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.isActiveAndEnabled && canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"RuntimeTouchBridge added GraphicRaycaster to {canvas.name}.");
            }
        }

        RuntimeLocationCardRepair.Apply();
    }

    private bool TryReadPointerFrame(out Vector2 position, out bool pressed, out bool released, out int currentPointerId)
    {
        position = default;
        pressed = false;
        released = false;
        currentPointerId = pointerId;

        var touchscreen = Touchscreen.current;
        if (touchscreen != null)
        {
            var touch = touchscreen.primaryTouch;
            position = touch.position.ReadValue();
            pressed = touch.press.wasPressedThisFrame;
            released = touch.press.wasReleasedThisFrame;
            if (pressed || touch.press.isPressed || released)
            {
                currentPointerId = -1000 - touch.touchId.ReadValue();
                return true;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            position = mouse.position.ReadValue();
            pressed = mouse.leftButton.wasPressedThisFrame;
            released = mouse.leftButton.wasReleasedThisFrame;
            if (pressed || mouse.leftButton.isPressed || released)
            {
                currentPointerId = -1;
                return true;
            }
        }

        // Keep a legacy fallback for builds whose PlayerSettings still select
        // the old input backend. Unity throws when this API is unavailable, so
        // only use it when the legacy backend is actually active.
        try
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                position = touch.position;
                pressed = touch.phase == UnityEngine.TouchPhase.Began;
                released = touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled;
                currentPointerId = -1000 - touch.fingerId;
                return true;
            }

            position = Input.mousePosition;
            pressed = Input.GetMouseButtonDown(0);
            released = Input.GetMouseButtonUp(0);
            if (pressed || Input.GetMouseButton(0) || released)
            {
                currentPointerId = -1;
                return true;
            }
        }
        catch (System.InvalidOperationException)
        {
            // The new input backend is active and already covered above.
        }

        return false;
    }

    private void BeginPointer(Vector2 position, int currentPointerId)
    {
        var eventSystem = FindActiveEventSystem();
        if (eventSystem == null)
        {
            Debug.LogWarning("RuntimeTouchBridge could not find an active EventSystem.");
            return;
        }

        pointerId = currentPointerId;
        pointerEventSystem = eventSystem;
        pointerData = new PointerEventData(eventSystem)
        {
            pointerId = currentPointerId,
            position = position,
            pressPosition = position,
            button = PointerEventData.InputButton.Left,
            eligibleForClick = true,
            clickCount = 1
        };

        var currentObject = Raycast(eventSystem, pointerData);
        if (currentObject == null)
        {
            Debug.Log($"RuntimeTouchBridge pointer down at {position} hit nothing.");
            return;
        }

        pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
        pointerData.rawPointerPress = currentObject;
        pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy(
            currentObject, pointerData, ExecuteEvents.pointerDownHandler);
        if (pointerData.pointerPress == null)
        {
            pointerData.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);
        }

        pressedObject = pointerData.pointerPress;
        pointerData.pointerEnter = currentObject;
        Debug.Log($"RuntimeTouchBridge pointer down at {position}: {currentObject.name}, press={pressedObject?.name ?? "none"}.");
    }

    private void EndPointer(Vector2 position)
    {
        if (pointerData == null)
        {
            return;
        }

        pointerData.position = position;
        var eventSystem = pointerEventSystem != null && pointerEventSystem.isActiveAndEnabled
            ? pointerEventSystem
            : FindActiveEventSystem();
        if (eventSystem == null)
        {
            pointerData = null;
            pressedObject = null;
            pointerEventSystem = null;
            return;
        }

        var currentObject = Raycast(eventSystem, pointerData);
        var clickObject = currentObject == null
            ? null
            : ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        if (pressedObject != null)
        {
            ExecuteEvents.Execute(pressedObject, pointerData, ExecuteEvents.pointerUpHandler);
        }

        if (pressedObject != null && pressedObject == clickObject && pointerData.eligibleForClick)
        {
            ExecuteEvents.Execute(pressedObject, pointerData, ExecuteEvents.pointerClickHandler);
            Debug.Log($"RuntimeTouchBridge click at {position}: {pressedObject.name}.");
        }
        else
        {
            Debug.Log($"RuntimeTouchBridge release at {position}: down={pressedObject?.name ?? "none"}, up={clickObject?.name ?? "none"}.");
        }

        pointerData = null;
        pressedObject = null;
        pointerEventSystem = null;
    }

    private static GameObject Raycast(EventSystem eventSystem, PointerEventData eventData)
    {
        raycastResults.Clear();
        eventSystem.RaycastAll(eventData, raycastResults);
        var firstRaycast = FindFirstRaycast(raycastResults);
        eventData.pointerCurrentRaycast = firstRaycast;
        return firstRaycast.gameObject;
    }

    private static RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        for (var index = 0; index < results.Count; index++)
        {
            if (results[index].gameObject != null)
            {
                return results[index];
            }
        }

        return default;
    }

    private static EventSystem FindActiveEventSystem()
    {
        if (EventSystem.current != null && EventSystem.current.isActiveAndEnabled)
        {
            return EventSystem.current;
        }

        var eventSystems = FindObjectsOfType<EventSystem>();
        foreach (var eventSystem in eventSystems)
        {
            if (eventSystem.isActiveAndEnabled)
            {
                return eventSystem;
            }
        }

        return null;
    }
}

// Public builds do not include the original Asset Store location sprites. Use
// the project-owned replacement only for the six 350x350 location buttons.
internal static class RuntimeLocationCardRepair
{
    private const string LocationResourcePath = "UI/afk_location_card";
    private const string PanelResourcePath = "UI/afk_ui_panel";
    private const string ButtonResourcePath = "UI/afk_ui_button";
    private const string IconResourcePath = "UI/afk_ui_icon";
    private static Sprite locationCardSprite;
    private static Sprite panelSprite;
    private static Sprite buttonSprite;
    private static Sprite iconSprite;
    private static bool attemptedLoad;

    public static void Apply()
    {
        if (!attemptedLoad)
        {
            attemptedLoad = true;
            locationCardSprite = LoadSprite(LocationResourcePath);
            panelSprite = LoadSprite(PanelResourcePath);
            buttonSprite = LoadSprite(ButtonResourcePath);
            iconSprite = LoadSprite(IconResourcePath);

            if (locationCardSprite == null && panelSprite == null && buttonSprite == null && iconSprite == null)
            {
                Debug.LogWarning("RuntimeLocationCardRepair could not load generated UI resources.");
                return;
            }
        }

        var repaired = 0;
        foreach (var image in Object.FindObjectsOfType<Image>())
        {
            if (image.sprite != null)
            {
                continue;
            }

            var rectTransform = image.rectTransform;
            var size = rectTransform.rect.size;
            var parentButton = image.GetComponentInParent<Button>();
            var parentButtonRect = parentButton == null ? null : parentButton.transform as RectTransform;
            var isLocationCard = parentButtonRect != null &&
                Mathf.Abs(parentButtonRect.rect.width - 350f) <= 2f &&
                Mathf.Abs(parentButtonRect.rect.height - 350f) <= 2f;

            var replacement = isLocationCard
                ? locationCardSprite
                : SelectGenericSprite(image.gameObject.name, size);
            if (replacement == null)
            {
                continue;
            }

            image.sprite = replacement;
            repaired++;
        }

        if (repaired > 0)
        {
            Debug.Log($"RuntimeLocationCardRepair restored {repaired} missing UI sprite(s).");
        }
    }

    private static Sprite SelectGenericSprite(string objectName, Vector2 size)
    {
        var name = objectName.ToLowerInvariant();
        if (name.Contains("icon") || name.Contains("rank") || name.Contains("faction") ||
            (Mathf.Abs(size.x - size.y) < Mathf.Max(16f, size.x * 0.15f) && size.x <= 220f))
        {
            return iconSprite ?? panelSprite;
        }

        if (size.x > size.y * 1.8f || name.Contains("button") || name.Contains("bar") || name.Contains("separator"))
        {
            return buttonSprite ?? panelSprite;
        }

        return panelSprite ?? buttonSprite ?? iconSprite;
    }

    private static Sprite LoadSprite(string resourcePath)
    {
        var sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
        {
            return sprite;
        }

        var texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect);
    }
}
