using UnityEngine;

// Repairs legacy scene data that can make every Canvas element render at zero
// scale on Android while audio and the rest of the scene continue running.
public static class RuntimeRenderRepair
{
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
    }
}
