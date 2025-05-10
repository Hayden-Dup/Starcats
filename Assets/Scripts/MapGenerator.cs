using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Singleton instance
    public static MapGenerator Instance { get; private set; }

    [Header("Map Size")]
    [HideInInspector] public Vector2 origin;
    public int width  = 100;
    public int height = 100;

    [Header("Tile Data")]
    [Tooltip("Your TileType ScriptableObjects")]
    public TileType[] tileTypes;
    [Tooltip("Prefab with MapTile script, SpriteRenderer, + child named 'FogOverlay'")]
    public GameObject tilePrefab;

    [Header("Resource Setup")]
    [Tooltip("Prefab that has a ResourceNode component (and visuals/collider)")]
    public GameObject resourcePrefab;

    [Header("Spawn Safety")]
    [Tooltip("Drag in your Player GameObject here, or leave blank to auto-find by Tag.")]
    public Transform playerTransform;
    [Tooltip("No resources will spawn within this radius of the player’s start.")]
    public float resourceSafeRadius = 5f;

    [HideInInspector]
    public MapTile[,] tileGrid;

    void Awake()
    {
        // Initialize singleton
        if (Instance == null)
        {
            Instance = this;
            // Optionally persist across scenes:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// Generates the tile grid with fog and optional resources.
    /// </summary>
    public void GenerateMap()
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

        tileGrid = new MapTile[width, height];

        // center on camera
        Vector3 camPos = Camera.main.transform.position;
        float startX = camPos.x - width  / 2f + 0.5f;
        float startY = camPos.y - height / 2f + 0.5f;
        origin = new Vector2(startX, startY);

        // spawn tiles
        for (int x = 0; x < width; x++)
        {
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
    }

    /// <summary>
    /// Attempts to spawn a resource node on the given tile, respecting safe radius and spawn chance.
    /// </summary>
    void TrySpawnResource(MapTile mt)
    {
        // ① Safety check: skip tiles too close to the player's start
        if (playerTransform == null)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                playerTransform = playerGO.transform;
        }

        // ② Don’t spawn near player
        if (playerTransform != null)
        {
            float dist = Vector2.Distance(mt.transform.position, (Vector2)playerTransform.position);
            if (dist <= resourceSafeRadius)
                return;
        }

        // ③ Only if the tile provides a resource
        if (!mt.data.providesResource)
            return;

        // ④ Roll for spawn chance
        if (Random.value > mt.data.resourceSpawnChance)
            return;

        // instantiate the resource node
        GameObject nodeGO = Instantiate(resourcePrefab, mt.transform.position, Quaternion.identity, mt.transform);
        var node = nodeGO.GetComponent<ResourceNode>();
        node.Initialize(mt.data.resourceName, mt.data.resourceAmount);
        nodeGO.SetActive(false);
        mt.resourceNode = node;
    }

    /// <summary>
    /// Converts a world-space position into tileGrid indices.
    /// </summary>
    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x - origin.x);
        int y = Mathf.RoundToInt(worldPos.y - origin.y);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Converts tileGrid indices back into the world-space center of that tile.
    /// </summary>
    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        return new Vector2(origin.x + gridPos.x, origin.y + gridPos.y);
    }
}
