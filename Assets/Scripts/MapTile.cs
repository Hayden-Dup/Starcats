using UnityEngine;

public class MapTile : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public bool       isExplored = false;
    [HideInInspector] public GameObject  fogOverlay;

    // This holds the ScriptableObject data assigned at spawn
    public TileType data;

    private SpriteRenderer _sr;

    void Awake()
    {
        // cache the SpriteRenderer we’ll change in Initialize()
        _sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called by MapGenerator right after Instantiate().
    /// </summary>
        public void Initialize(Vector2Int pos, TileType tileType)
    {
        gridPosition = pos;
        data = tileType;

        // pick the sprite
        Sprite chosen;
        if (tileType.variantSprites != null && tileType.variantSprites.Length > 0)
        {
            // random variant
            int i = Random.Range(0, tileType.variantSprites.Length);
            chosen = tileType.variantSprites[i];
        }
        else
        {
            // fallback to the old single‐sprite field
            chosen = tileType.sprite;
        }

        _sr.sprite = chosen;
    }


    /// <summary>
    /// Reveal this tile (turn off its fog overlay).
    /// </summary>
    public void Reveal(float fadeDuration = 0f)
    {
        if (isExplored)
            return;

        isExplored = true;

        if (fogOverlay == null)
            return;

        if (fadeDuration <= 0f)
        {
            // instant
            fogOverlay.SetActive(false);
        }
        else
        {
            // gradual fade
            StartCoroutine(FadeOutFog(fadeDuration));
        }
    }

    private System.Collections.IEnumerator FadeOutFog(float duration)
    {
        var sr = fogOverlay.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            fogOverlay.SetActive(false);
            yield break;
        }

        Color c = sr.color;
        float startA = c.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(startA, 0f, t / duration);
            sr.color = c;
            yield return null;
        }
        fogOverlay.SetActive(false);
    }
}
