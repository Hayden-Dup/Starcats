using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    [Tooltip("World object this bar follows")]
    public Transform target;
    [Tooltip("Lift offset in world units")]
    public Vector3 worldOffset = new Vector3(0, 1.5f, 0);
    [Tooltip("Your Screen-Space Overlay Canvas")]
    public Canvas parentCanvas;

    RectTransform _rt;

    void Awake() => _rt = GetComponent<RectTransform>();

    void LateUpdate()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + worldOffset);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPos, null, out Vector2 localPos
        );
        _rt.localPosition = localPos;
    }
}
