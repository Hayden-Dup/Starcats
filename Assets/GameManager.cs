using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Building Buttons")]
    public Button mainBuildingButton;
    public Button cheapUnitBuildingButton;
    public Button expensiveUnitBuildingButton;
    public Button mergeBuildingButton;

    [Header("Unit Buttons")]
    public Button cheapStandardUnitButton;
    public Button cheapTankyUnitButton;
    public Button expensiveRangedUnitButton;
    public Button expensiveFastUnitButton;
    public Button mergeUnitButton;

    [Header("Building Sets")]
    public BuildingSet catBuildings;
    public BuildingSet alienmalBuildings;

    [Header("Unit Sets")]
    public UnitSet catUnits;
    public UnitSet alienmalUnits;

    private void Awake()
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
        if (scene.name == "Game")
        {
            SetupUI();
        }
    }

    // --- Faction Helpers ---

    BuildingSet GetCurrentBuildingSet()
    {
        return Selector.Instance?.Choice switch
        {
            1 => catBuildings,
            2 => alienmalBuildings,
            _ => null
        };
    }

    UnitSet GetCurrentUnitSet()
    {
        return Selector.Instance?.Choice switch
        {
            1 => catUnits,
            2 => alienmalUnits,
            _ => null
        };
    }

    // --- UI Setup ---

    void SetupUI()
    {
        SetupBuildingUI();
        SetupUnitUI();
    }

    void SetupBuildingUI()
    {
        BuildingSet set = GetCurrentBuildingSet();
        if (set == null) return;

        mainBuildingButton.image.sprite = set.mainBuildingIcon;
        cheapUnitBuildingButton.image.sprite = set.cheapUnitBuildingIcon;
        expensiveUnitBuildingButton.image.sprite = set.rangedUnitBuildingIcon;
        mergeBuildingButton.image.sprite = set.mergeUnitBuildingIcon;

        mainBuildingButton.onClick.RemoveAllListeners();
        cheapUnitBuildingButton.onClick.RemoveAllListeners();
        expensiveUnitBuildingButton.onClick.RemoveAllListeners();
        mergeBuildingButton.onClick.RemoveAllListeners();

        // Replace these with actual building placement logic
        mainBuildingButton.onClick.AddListener(() => Debug.Log("Main Building selected"));
        cheapUnitBuildingButton.onClick.AddListener(() => Debug.Log("Cheap Unit Building selected"));
        expensiveUnitBuildingButton.onClick.AddListener(() => Debug.Log("Expensive Unit Building selected"));
        mergeBuildingButton.onClick.AddListener(() => Debug.Log("Merge Building selected"));
    }

    void SetupUnitUI()
    {
        UnitSet set = GetCurrentUnitSet();
        if (set == null) return;

        cheapStandardUnitButton.image.sprite = set.cheapStandardIcon;
        cheapTankyUnitButton.image.sprite = set.cheapTankyIcon;
        expensiveRangedUnitButton.image.sprite = set.expensiveRangedIcon;
        expensiveFastUnitButton.image.sprite = set.expensiveFastIcon;
        mergeUnitButton.image.sprite = set.mergeUnitIcon;

        cheapStandardUnitButton.onClick.RemoveAllListeners();
        cheapTankyUnitButton.onClick.RemoveAllListeners();
        expensiveRangedUnitButton.onClick.RemoveAllListeners();
        expensiveFastUnitButton.onClick.RemoveAllListeners();
        mergeUnitButton.onClick.RemoveAllListeners();

        cheapStandardUnitButton.onClick.AddListener(() => SpawnUnit(set.cheapStandardPrefab));
        cheapTankyUnitButton.onClick.AddListener(() => SpawnUnit(set.cheapTankyPrefab));
        expensiveRangedUnitButton.onClick.AddListener(() => SpawnUnit(set.expensiveRangedPrefab));
        expensiveFastUnitButton.onClick.AddListener(() => SpawnUnit(set.expensiveFastPrefab));
        mergeUnitButton.onClick.AddListener(() => SpawnUnit(set.mergeUnitPrefab));
    }

    // --- Unit Spawning Logic ---

    void SpawnUnit(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPosition = GetMouseWorldPosition(); // Or predefined spawn point
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Distance from camera
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
