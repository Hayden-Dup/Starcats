using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [HideInInspector] public Vector2 origin;


    public int width  = 50;
    public int height = 30;

    [Header("Tile Data")]
    public TileType[] tileTypes;   // drag in your ScriptableObjects
    public GameObject tilePrefab;  // drag in the TilePrefab asset

    [HideInInspector] public MapTile[,] tileGrid;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        if (tileTypes == null || tileTypes.Length == 0)
        {
            Debug.LogError("No TileTypes assigned!");
            return;
        }
        if (tilePrefab == null)
        {
            Debug.LogError("No TilePrefab assigned!");
            return;
        }

        tileGrid = new MapTile[width, height];

        // center the grid on the main camera
        Vector3 cam = Camera.main.transform.position;
        float startX = cam.x - width  / 2f + 0.5f;
        float startY = cam.y - height / 2f + 0.5f;

        origin = new Vector2(startX, startY);


        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Vector3 pos = new Vector3(startX + x, startY + y, 0f);
            GameObject go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            go.name = $"Tile {x},{y}";

            var mt = go.GetComponent<MapTile>();
            var chosen = tileTypes[Random.Range(0, tileTypes.Length)];
            mt.Initialize(new Vector2Int(x, y), chosen);

            // connect the FogOverlay child
            var fog = go.transform.Find("FogOverlay");
            if (fog != null) mt.fogOverlay = fog.gameObject;
            else Debug.LogWarning($"Tile {x},{y} missing FogOverlay child!");

            tileGrid[x, y] = mt;
        }
    }
}
