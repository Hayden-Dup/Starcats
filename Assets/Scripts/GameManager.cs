using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Building Buttons")]
    public Button mainBuildingButton;
    public Button cheapUnitButton;
    public Button rangedUnitButton;
    public Button mergeUnitButton;

    [Header("Building Sets")]
    public BuildingSet catBuildings;
    public BuildingSet alienmalBuildings;

    private BuildingType buildingToPlace = BuildingType.None;
    private GameObject ghostPreview;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<string> validScenes = new() { "Game", "Battle" };
        if (validScenes.Contains(scene.name))
        {
            SetupBuildingButtons();
        }
    }

    void SetupBuildingButtons()
    {
        if (Selector.Instance == null)
        {
            Debug.LogWarning("No Selector found.");
            return;
        }

        BuildingSet currentSet = Selector.Instance.Choice switch
        {
            1 => catBuildings,
            2 => alienmalBuildings,
            _ => null
        };

        if (currentSet == null)
        {
            Debug.LogWarning("No BuildingSet assigned for current faction.");
            return;
        }

        mainBuildingButton.image.sprite = currentSet.mainBuildingIcon;
        cheapUnitButton.image.sprite = currentSet.cheapUnitBuildingIcon;
        rangedUnitButton.image.sprite = currentSet.rangedUnitBuildingIcon;
        mergeUnitButton.image.sprite = currentSet.mergeUnitBuildingIcon;

        mainBuildingButton.onClick.RemoveAllListeners();
        cheapUnitButton.onClick.RemoveAllListeners();
        rangedUnitButton.onClick.RemoveAllListeners();
        mergeUnitButton.onClick.RemoveAllListeners();

        mainBuildingButton.onClick.AddListener(() => SelectBuilding(BuildingType.Main));
        cheapUnitButton.onClick.AddListener(() => SelectBuilding(BuildingType.Cheap));
        rangedUnitButton.onClick.AddListener(() => SelectBuilding(BuildingType.Ranged));
        mergeUnitButton.onClick.AddListener(() => SelectBuilding(BuildingType.Merge));
    }

    void SelectBuilding(BuildingType type)
    {
        buildingToPlace = type;

        if (ghostPreview != null)
            Destroy(ghostPreview);

        GameObject prefab = GetBuildingPrefab(type);
        if (prefab != null)
        {
            ghostPreview = Instantiate(prefab);
            SpriteRenderer sr = ghostPreview.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(1f, 1f, 1f, 0.5f);

            Collider2D col = ghostPreview.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }

    void Update()
    {
        if (buildingToPlace == BuildingType.None) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 snappedPos = SnapToGrid(mousePos);

        if (ghostPreview != null)
        {
            ghostPreview.transform.position = snappedPos;

            SpriteRenderer sr = ghostPreview.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = IsValidPlacement(snappedPos) ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
            }
        }

        if (Input.GetMouseButtonDown(0) && IsValidPlacement(snappedPos))
        {
            PlaceBuildingAt(snappedPos);
        }
    }

    void PlaceBuildingAt(Vector2 position)
    {
        GameObject prefabToPlace = GetBuildingPrefab(buildingToPlace);
        if (prefabToPlace == null)
        {
            Debug.LogWarning("No prefab assigned for selected building.");
            return;
        }

        Instantiate(prefabToPlace, position, Quaternion.identity);
        buildingToPlace = BuildingType.None;

        if (ghostPreview != null)
        {
            Destroy(ghostPreview);
            ghostPreview = null;
        }
    }

    bool IsValidPlacement(Vector2 position)
    {
        Vector2 boxSize = new Vector2(0.9f, 0.9f);
        return Physics2D.OverlapBox(position, boxSize, 0f, LayerMask.GetMask("Buildings")) == null;
    }

    GameObject GetBuildingPrefab(BuildingType type)
    {
        BuildingSet currentSet = Selector.Instance.Choice switch
        {
            1 => catBuildings,
            2 => alienmalBuildings,
            _ => null
        };

        return type switch
        {
            BuildingType.Main => currentSet?.mainBuildingPrefab,
            BuildingType.Cheap => currentSet?.cheapUnitBuildingPrefab,
            BuildingType.Ranged => currentSet?.rangedUnitBuildingPrefab,
            BuildingType.Merge => currentSet?.mergeUnitBuildingPrefab,
            _ => null
        };
    }

    Vector2 SnapToGrid(Vector2 rawPosition)
    {
        float x = Mathf.Round(rawPosition.x);
        float y = Mathf.Round(rawPosition.y);
        return new Vector2(x, y);
    }
}

public enum BuildingType
{
    None,
    Main,
    Cheap,
    Ranged,
    Merge
}
