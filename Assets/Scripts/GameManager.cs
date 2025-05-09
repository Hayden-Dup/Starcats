using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public Transform queueContainer;
    public GameObject queueItemPrefab;

    [Header("Player State")]
    public int playerMoney = 1000;
    public TextMeshProUGUI moneyText;

    private BuildingType buildingToPlace = BuildingType.None;
    private GameObject ghostPreview;
    private SelectableBuilding selectedBuilding;

    private Queue<QueuedUnit> unitQueue = new();
    private bool isProcessingQueue = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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

    private void Update()
    {
        if (buildingToPlace != BuildingType.None)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 snappedPos = SnapToGrid(mousePos);

            if (ghostPreview != null)
            {
                ghostPreview.transform.position = snappedPos;

                var sr = ghostPreview.GetComponent<SpriteRenderer>();
                bool canAfford = playerMoney >= GetBuildingCost(buildingToPlace);
                bool validPos = IsValidPlacement(snappedPos);
                sr.color = (canAfford && validPos) ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
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
                CancelPlacement();
        }
    }

    void SetupBuildingButtons()
    {
        if (Selector.Instance == null) return;

        BuildingSet set = GetCurrentSet();
        if (set == null) return;

        mainBuildingButton.image.sprite = set.mainBuildingIcon;
        cheapUnitButton.image.sprite = set.cheapUnitBuildingIcon;
        rangedUnitButton.image.sprite = set.rangedUnitBuildingIcon;
        mergeUnitButton.image.sprite = set.mergeUnitBuildingIcon;

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
        if (ghostPreview) Destroy(ghostPreview);

        GameObject prefab = GetBuildingPrefab(type);
        if (prefab != null)
        {
            ghostPreview = Instantiate(prefab);
            ghostPreview.name = "GhostPreview";
            ghostPreview.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            var col = ghostPreview.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            GameObject labelObj = new("GhostLabel");
            labelObj.transform.SetParent(ghostPreview.transform);
            labelObj.transform.localPosition = new Vector3(0, -1.2f, 0);

            var label = labelObj.AddComponent<TextMeshPro>();
            label.text = GetBuildingName(type);
            label.fontSize = 8;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(1f, 1f, 1f, 0.95f);
        }
    }

    void PlaceBuildingAt(Vector2 pos)
    {
        GameObject prefab = GetBuildingPrefab(buildingToPlace);
        if (prefab == null) return;

        Instantiate(prefab, pos, Quaternion.identity);
        CancelPlacement();
    }

    public void SelectBuildingObject(SelectableBuilding building)
    {
        if (selectedBuilding == building)
        {
            selectedBuilding.Highlight(false);
            selectedBuilding = null;
            unitPurchasePanel.SetActive(false);
            return;
        }

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

        UnitSet set = GetCurrentUnitSet();
        if (set == null) return;

        foreach (var unit in set.units)
        {
            GameObject btn = Instantiate(unitButtonPrefab, unitButtonContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"{unit.unitName} - ${unit.unitCost}";
            btn.GetComponent<Button>().onClick.AddListener(() => QueueUnit(unit));
        }
    }

    void QueueUnit(UnitData unit)
    {
        if (playerMoney < unit.unitCost || selectedBuilding == null)
        {
            Debug.Log("Cannot queue unit: not enough money or no building selected.");
            return;
        }

        playerMoney -= unit.unitCost;
        UpdateMoneyUI();

        GameObject ui = Instantiate(queueItemPrefab, queueContainer);
        Slider bar = ui.GetComponentInChildren<Slider>();

        QueuedUnit queued = new()
        {
            data = unit,
            progressBar = bar,
            uiObject = ui,
            targetBuilding = selectedBuilding
        };

        unitQueue.Enqueue(queued);

        if (!isProcessingQueue)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        isProcessingQueue = true;

        while (unitQueue.Count > 0)
        {
            QueuedUnit current = unitQueue.Peek();
            float t = 0f;
            float duration = current.data.productionTime;

            while (t < duration)
            {
                t += Time.deltaTime;
                if (current.progressBar != null)
                    current.progressBar.value = Mathf.Clamp01(t / duration);
                yield return null;
            }

                        // Replace inside ProcessQueue (within while loop after production time)
            if (current.targetBuilding != null)
            {
                Vector3 spawnOffset = new Vector3(3f, 1f, 0f); // Offsets right and slightly up
                Vector3 spawnPos = selectedBuilding.transform.position + spawnOffset;
                GameObject unit = Instantiate(current.data.prefab, spawnPos, Quaternion.identity);

                SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = 10;

                if (!unit.GetComponent<SelectableUnit>())
                    unit.AddComponent<SelectableUnit>(); // fallback in case prefab forgot to add
            }



            Destroy(current.uiObject);
            unitQueue.Dequeue();
        }

        isProcessingQueue = false;
    }

    void CancelPlacement()
    {
        buildingToPlace = BuildingType.None;
        if (ghostPreview != null) Destroy(ghostPreview);
    }

    bool IsValidPlacement(Vector2 position)
    {
        GameObject prefab = GetBuildingPrefab(buildingToPlace);
        if (prefab == null) return false;
        Collider2D col = prefab.GetComponent<Collider2D>();
        if (col == null) return false;

        return Physics2D.OverlapBox(position, col.bounds.size, 0f, LayerMask.GetMask("Buildings")) == null;
    }

    GameObject GetBuildingPrefab(BuildingType type)
    {
        BuildingSet set = GetCurrentSet();
        return type switch
        {
            BuildingType.Main => set.mainBuildingPrefab,
            BuildingType.Cheap => set.cheapUnitBuildingPrefab,
            BuildingType.Ranged => set.rangedUnitBuildingPrefab,
            BuildingType.Merge => set.mergeUnitBuildingPrefab,
            _ => null
        };
    }

    int GetBuildingCost(BuildingType type)
    {
        BuildingSet set = GetCurrentSet();
        return type switch
        {
            BuildingType.Main => set.mainBuildingCost,
            BuildingType.Cheap => set.cheapUnitBuildingCost,
            BuildingType.Ranged => set.rangedUnitBuildingCost,
            BuildingType.Merge => set.mergeUnitBuildingCost,
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
            1 => catBuildings.catUnitSet,
            2 => alienmalBuildings.alienUnitSet,
            _ => null
        };
    }

    Vector2 SnapToGrid(Vector2 raw)
    {
        return new Vector2(Mathf.Round(raw.x), Mathf.Round(raw.y));
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{playerMoney}";
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyUI();
    }
}

public enum BuildingType
{
    None, Main, Cheap, Ranged, Merge
}

public class QueuedUnit
{
    public UnitData data;
    public Slider progressBar;
    public GameObject uiObject;
    public SelectableBuilding targetBuilding;
}
