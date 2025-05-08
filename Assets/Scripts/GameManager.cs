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

    [Header("Player State")]
    public int playerMoney = 1000;
    public TMPro.TextMeshProUGUI moneyText;

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
            UpdateMoneyUI();
        }
    }

    void SetupBuildingButtons()
    {
        if (Selector.Instance == null)
        {
            Debug.LogWarning("No Selector found.");
            return;
        }

        BuildingSet currentSet = GetCurrentSet();
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
            ghostPreview.name = "GhostPreview";
            ghostPreview.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

            var col = ghostPreview.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            // Add label
            GameObject labelObj = new GameObject("GhostLabel");
            labelObj.transform.SetParent(ghostPreview.transform);
            labelObj.transform.localPosition = new Vector3(0, -1.2f, 0);

            var label = labelObj.AddComponent<TMPro.TextMeshPro>();
            label.text = GetBuildingName(type);
            label.fontSize = 3;
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.color = new Color(1f, 1f, 1f, 0.85f);
            label.rectTransform.sizeDelta = new Vector2(6f, 1f);
        }
    }

    void Update()
    {
        if (buildingToPlace == BuildingType.None)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 snappedPos = SnapToGrid(mousePos);

        if (ghostPreview != null)
        {
            ghostPreview.transform.position = snappedPos;

            var sr = ghostPreview.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = IsValidPlacement(snappedPos) ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
            }
        }

        if (Input.GetMouseButtonDown(0) && IsValidPlacement(snappedPos))
        {
            int cost = GetBuildingCost(buildingToPlace);
            if (playerMoney >= cost)
            {
                PlaceBuildingAt(snappedPos);
                playerMoney -= cost;
                UpdateMoneyUI();
                Debug.Log($"Placed {buildingToPlace}, Remaining Money: ${playerMoney}");
            }
            else
            {
                Debug.Log("Not enough money to place this building!");
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right-click to cancel
        {
            CancelPlacement();
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
        CancelPlacement();
    }

    void CancelPlacement()
    {
        buildingToPlace = BuildingType.None;

        if (ghostPreview != null)
        {
            Destroy(ghostPreview);
            ghostPreview = null;
        }
    }

    bool IsValidPlacement(Vector2 position)
    {
        GameObject prefab = GetBuildingPrefab(buildingToPlace);
        if (prefab == null)
            return false;

        Collider2D prefabCol = prefab.GetComponent<Collider2D>();
        if (prefabCol == null)
            return false;

        Vector2 boxSize = prefabCol.bounds.size;
        return Physics2D.OverlapBox(position, boxSize, 0f, LayerMask.GetMask("Buildings")) == null;
    }

    GameObject GetBuildingPrefab(BuildingType type)
    {
        BuildingSet currentSet = GetCurrentSet();
        return type switch
        {
            BuildingType.Main => currentSet?.mainBuildingPrefab,
            BuildingType.Cheap => currentSet?.cheapUnitBuildingPrefab,
            BuildingType.Ranged => currentSet?.rangedUnitBuildingPrefab,
            BuildingType.Merge => currentSet?.mergeUnitBuildingPrefab,
            _ => null
        };
    }

    int GetBuildingCost(BuildingType type)
    {
        BuildingSet set = GetCurrentSet();
        return type switch
        {
            BuildingType.Main => set?.mainBuildingCost ?? 0,
            BuildingType.Cheap => set?.cheapUnitBuildingCost ?? 0,
            BuildingType.Ranged => set?.rangedUnitBuildingCost ?? 0,
            BuildingType.Merge => set?.mergeUnitBuildingCost ?? 0,
            _ => 0
        };
    }

    string GetBuildingName(BuildingType type)
    {
        int cost = GetBuildingCost(type);

        string name = type switch
        {
            BuildingType.Main => "Main Building",
            BuildingType.Cheap => "Cheap Building",
            BuildingType.Ranged => "Expensive Building",
            BuildingType.Merge => "Merge Building",
            _ => ""
        };

        return $"{name} - ${cost}";
    }

    BuildingSet GetCurrentSet()
    {
        return Selector.Instance.Choice switch
        {
            1 => catBuildings,
            2 => alienmalBuildings,
            _ => null
        };
    }

    Vector2 SnapToGrid(Vector2 rawPosition)
    {
        float x = Mathf.Round(rawPosition.x);
        float y = Mathf.Round(rawPosition.y);
        return new Vector2(x, y);
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{playerMoney}";
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
