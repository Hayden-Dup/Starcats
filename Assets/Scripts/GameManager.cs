using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("UI")]
    public GameObject unitPurchasePanel;
    public Transform unitButtonContainer;
    public GameObject unitButtonPrefab;

    [Header("Player State")]
    public int playerMoney = 1000;
    public TMPro.TextMeshProUGUI moneyText;

    private BuildingType buildingToPlace = BuildingType.None;
    private GameObject ghostPreview;
    private SelectableBuilding selectedBuilding;

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
            if (unitPurchasePanel != null)
                unitPurchasePanel.SetActive(false);
        }
    }

    void SetupBuildingButtons()
    {
        if (Selector.Instance == null) return;

        BuildingSet currentSet = GetCurrentSet();
        if (currentSet == null) return;

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

            GameObject labelObj = new GameObject("GhostLabel");
            labelObj.transform.SetParent(ghostPreview.transform);
            labelObj.transform.localPosition = new Vector3(0, -1.2f, 0);

            var label = labelObj.AddComponent<TMPro.TextMeshPro>();
            label.text = GetBuildingName(type);
            label.fontSize = 8;
            label.fontStyle = TMPro.FontStyles.Bold;
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.color = new Color(1f, 1f, 1f, 0.95f);
            label.rectTransform.sizeDelta = new Vector2(12f, 2.5f);
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
                bool canAfford = playerMoney >= GetBuildingCost(buildingToPlace);
                bool validPos = IsValidPlacement(snappedPos);
                sr.color = (canAfford && validPos) ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
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
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    void PlaceBuildingAt(Vector2 position)
    {
        GameObject prefabToPlace = GetBuildingPrefab(buildingToPlace);
        if (prefabToPlace == null) return;

        Instantiate(prefabToPlace, position, Quaternion.identity);
        CancelPlacement();
    }

    void CancelPlacement()
    {
        buildingToPlace = BuildingType.None;
        if (ghostPreview != null) Destroy(ghostPreview);
    }

    public void SelectBuildingObject(SelectableBuilding building)
    {
        if (selectedBuilding != null)
            selectedBuilding.Highlight(false);

        selectedBuilding = building;
        selectedBuilding.Highlight(true);

        ShowUnitPanel();
    }

    void ShowUnitPanel()
    {
        if (unitPurchasePanel == null || unitButtonPrefab == null || unitButtonContainer == null) return;
        unitPurchasePanel.SetActive(true);

        foreach (Transform child in unitButtonContainer)
            Destroy(child.gameObject);

        UnitSet unitSet = GetCurrentUnitSet();
        if (unitSet == null) return;

        foreach (var unit in unitSet.units)
        {
            GameObject btn = Instantiate(unitButtonPrefab, unitButtonContainer);
            btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{unit.unitName} - ${unit.unitCost}";
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (playerMoney >= unit.unitCost)
                {
                    playerMoney -= unit.unitCost;
                    UpdateMoneyUI();
                    StartCoroutine(SpawnUnitWithDelay(unit, selectedBuilding.transform.position + Vector3.right));
                    Debug.Log($"Spawning {unit.unitName} after {unit.productionTime}s");
                }
                else
                {
                    Debug.Log($"Not enough money to spawn {unit.unitName}");
                }
            });
        }
    }

    private IEnumerator SpawnUnitWithDelay(UnitData unit, Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(unit.productionTime);
        Instantiate(unit.prefab, spawnPosition, Quaternion.identity);
    }

    bool IsValidPlacement(Vector2 position)
    {
        GameObject prefab = GetBuildingPrefab(buildingToPlace);
        if (prefab == null) return false;
        Collider2D prefabCol = prefab.GetComponent<Collider2D>();
        if (prefabCol == null) return false;

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

    UnitSet GetCurrentUnitSet()
    {
        return Selector.Instance.Choice switch
        {
            1 => catBuildings?.catUnitSet,
            2 => alienmalBuildings?.alienUnitSet,
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
