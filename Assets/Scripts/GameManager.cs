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

        // Update button icons
        mainBuildingButton.image.sprite = currentSet.mainBuildingIcon;
        cheapUnitButton.image.sprite = currentSet.cheapUnitBuildingIcon;
        rangedUnitButton.image.sprite = currentSet.rangedUnitBuildingIcon;
        mergeUnitButton.image.sprite = currentSet.mergeUnitBuildingIcon;

        // Clear previous listeners
        mainBuildingButton.onClick.RemoveAllListeners();
        cheapUnitButton.onClick.RemoveAllListeners();
        rangedUnitButton.onClick.RemoveAllListeners();
        mergeUnitButton.onClick.RemoveAllListeners();

        // Assign correct functionality depending on faction
        if (Selector.Instance.Choice == 1) // Cats
        {
            mainBuildingButton.onClick.AddListener(BuildCatMain);
            cheapUnitButton.onClick.AddListener(BuildCatCheap);
            rangedUnitButton.onClick.AddListener(BuildCatRanged);
            mergeUnitButton.onClick.AddListener(BuildCatMerge);
        }
        else if (Selector.Instance.Choice == 2) // Alienmals
        {
            mainBuildingButton.onClick.AddListener(BuildAlienMain);
            cheapUnitButton.onClick.AddListener(BuildAlienCheap);
            rangedUnitButton.onClick.AddListener(BuildAlienRanged);
            mergeUnitButton.onClick.AddListener(BuildAlienMerge);
        }
    }

    // CAT building actions
    void BuildCatMain() { Debug.Log("Cat Main Building selected"); }
    void BuildCatCheap() { Debug.Log("Cat Cheap Unit Building selected"); }
    void BuildCatRanged() { Debug.Log("Cat Ranged Unit Building selected"); }
    void BuildCatMerge() { Debug.Log("Cat Merge Building selected"); }

    // ALIEN building actions
    void BuildAlienMain() { Debug.Log("Alien Main Building selected"); }
    void BuildAlienCheap() { Debug.Log("Alien Cheap Unit Building selected"); }
    void BuildAlienRanged() { Debug.Log("Alien Ranged Unit Building selected"); }
    void BuildAlienMerge() { Debug.Log("Alien Merge Building selected"); }
}
