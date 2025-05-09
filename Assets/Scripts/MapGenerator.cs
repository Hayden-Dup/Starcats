using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [HideInInspector] public Vector2 origin;
    public int width  = 50;
    public int height = 30;

    [Header("Tile Data")]
    [Tooltip("Your TileType ScriptableObjects")]
    public TileType[] tileTypes;
    [Tooltip("Prefab with MapTile script, SpriteRenderer, + child named 'FogOverlay'")]
    public GameObject tilePrefab;

    [Header("Resource Setup")]
    [Tooltip("Prefab that has a ResourceNode component (and visuals/collider)")]
    public GameObject resourcePrefab;

    [HideInInspector]
    public MapTile[,] tileGrid;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // sanity checks
        if (tileTypes == null || tileTypes.Length == 0)
        {
            Debug.LogError("MapGenerator: No TileTypes assigned!");
            return;
        }
        if (tilePrefab == null)
        {
            Debug.LogError("MapGenerator: No TilePrefab assigned!");
            return;
        }
        if (resourcePrefab == null)
        {
            Debug.LogWarning("MapGenerator: No ResourcePrefab assigned — skipping resource spawn.");
        }

        // prepare grid
        tileGrid = new MapTile[width, height];

        // center on camera
        Vector3 camPos = Camera.main.transform.position;
        float startX = camPos.x - width  / 2f + 0.5f;
        float startY = camPos.y - height / 2f + 0.5f;
        origin = new Vector2(startX, startY);

        // spawn tiles
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Vector3 pos = new Vector3(startX + x, startY + y, 0f);
            GameObject go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            go.name = $"Tile {x},{y}";

            // initialize MapTile
            MapTile mt = go.GetComponent<MapTile>();
            TileType tt = tileTypes[Random.Range(0, tileTypes.Length)];
            mt.Initialize(new Vector2Int(x, y), tt);

            // hook up fog
            Transform fog = go.transform.Find("FogOverlay");
            if (fog != null) mt.fogOverlay = fog.gameObject;
            else Debug.LogWarning($"MapGenerator: Tile {x},{y} missing FogOverlay child!");

            tileGrid[x, y] = mt;

            // try to spawn a resource if this TileType says so
            TrySpawnResource(mt);
        }
    }

    void TrySpawnResource(MapTile mt)
    {
        if (!mt.data.providesResource)
            return;

        // NEW: roll for spawn chance
        if (Random.value > mt.data.resourceSpawnChance)
            return;

        // instantiate as before…
        GameObject nodeGO = Instantiate(resourcePrefab, mt.transform.position, Quaternion.identity, mt.transform);
        var node = nodeGO.GetComponent<ResourceNode>();
        node.Initialize(mt.data.typeName, mt.data.resourceAmount);
        nodeGO.SetActive(false);
        mt.resourceNode = node;
    }
}
