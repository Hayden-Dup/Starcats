using UnityEngine;
using System.Collections;

public class MapTile : MonoBehaviour
{
    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public bool       isExplored = false;
    [HideInInspector] public bool isWalkable = true;

    [HideInInspector] public GameObject  fogOverlay;

    // ScriptableObject data assigned at spawn
    public TileType data;

    // Resource node instantiated under this tile (if any)
    [HideInInspector] public ResourceNode resourceNode;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called by MapGenerator right after Instantiate().
    /// </summary>
    public void Initialize(Vector2Int pos, TileType tileType)
    {
        gridPosition = pos;
        data = tileType;

        isWalkable = data.isWalkable;  // from your SO

        // pick the sprite
        Sprite chosen;
        if (tileType.variantSprites != null && tileType.variantSprites.Length > 0)
        {
            int i = Random.Range(0, tileType.variantSprites.Length);
            chosen = tileType.variantSprites[i];
        }
        else
        {
            chosen = tileType.sprite;
        }

        _sr.sprite = chosen;
    }

    /// <summary>
    /// Reveal this tile (turn off its fog), then show any resource node.
    /// </summary>
    public void Reveal(float fadeDuration = 0f)
    {
        if (isExplored)
            return;

        isExplored = true;

        // remove fog
        if (fogOverlay != null)
        {
            if (fadeDuration <= 0f)
                fogOverlay.SetActive(false);
            else
                StartCoroutine(FadeOutFog(fadeDuration));
        }

        // show the resource node instead of auto-destroying it
        if (resourceNode != null)
        {
            resourceNode.gameObject.SetActive(true);

            // If you prefer auto-collect on reveal, uncomment this:
            // resourceNode.CollectAll();
        }
    }

    private IEnumerator FadeOutFog(float duration)
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
