using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("Minimap Setup")]
    public Camera        minimapCamera;   // drag in your MinimapCamera (Target Texture = MinimapRT)
    public RawImage      minimapImage;    // drag in your MinimapRawImage (RawImage component)

    [Header("Player Indicator")]
    public RectTransform playerMarker;    // drag in the RectTransform of your PlayerMarker (UI Image)
    public Transform     playerTransform; // drag in your Player GameObject

    [Header("Map Data")]
    public MapGenerator  mapGen;          // drag in your MapRoot (the GameObject with MapGenerator)

    void Start()
    {
        if (minimapCamera == null || mapGen == null)
        {
            Debug.LogError("MinimapController: Missing Camera or MapGenerator reference!");
            enabled = false;
            return;
        }

        // 1) Make the minimap camera see the entire map:
        float worldW = mapGen.width  * 1f;
        float worldH = mapGen.height * 1f;
        minimapCamera.orthographicSize = Mathf.Max(worldW, worldH) * 0.5f;

        // 2) Center it on the map’s center:
        //    MapGenerator spawns tiles around mainCamera.position, so reuse that:
        Vector3 mainCam = Camera.main.transform.position;
        minimapCamera.transform.position =
            new Vector3(mainCam.x, mainCam.y, minimapCamera.transform.position.z);

        // 3) Hook up the RenderTexture if you haven’t in the Inspector:
        if (minimapImage != null && minimapCamera.targetTexture != null && minimapImage.texture == null)
            minimapImage.texture = minimapCamera.targetTexture;
    }

    void Update()
    {
        // Don’t bother if we’re missing pieces
        if (playerTransform == null || playerMarker == null || minimapCamera == null || minimapImage == null)
            return;

        // Project player world→viewport (x,y in [0..1])
        Vector3 vp = minimapCamera.WorldToViewportPoint(playerTransform.position);

        // Only show the marker if the player is in front of the minimap camera
        bool onMap = vp.z >= 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;
        playerMarker.gameObject.SetActive(onMap);
        if (!onMap) return;

        // Convert viewport [0..1] → local UI coords
        Rect rect = minimapImage.rectTransform.rect;
        float x = (vp.x - 0.5f) * rect.width;
        float y = (vp.y - 0.5f) * rect.height;
        playerMarker.anchoredPosition = new Vector2(x, y);
    }
}
