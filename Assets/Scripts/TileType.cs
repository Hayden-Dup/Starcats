using UnityEngine;

[CreateAssetMenu(fileName = "NewTileType", menuName = "RTS/Tile Type")]
public class TileType : ScriptableObject
{
    [Header("Identification")]
    public string typeName;            // e.g. “Grass”

    [Header("Visuals")]
    [Tooltip("Legacy single‐sprite; will be ignored if Variant Sprites has any entries")]
    public Sprite sprite;

    [Tooltip("If you assign any here, one will be picked at random for each tile")]
    public Sprite[] variantSprites;

    [Header("Gameplay")]
    public bool isWalkable = true;
    public int movementCost = 1;
    public bool blocksVision = false;

    [Header("Resources (optional)")]
    public bool providesResource = false;
    public string resourceName;       // ← new field
    public int    resourceAmount = 0;

    // new: what fraction of these tiles actually get a coin
    [Range(0f, 1f)]
    public float resourceSpawnChance = 0.2f;  // 20% by default

    public GameObject resourcePrefab; // a one-off prefab (could just be an icon)

}
