using UnityEngine;

[CreateAssetMenu(fileName = "UnitSet", menuName = "Faction/Unit Set")]
public class UnitSet : ScriptableObject
{
    // Cheap Units
    public Sprite cheapStandardIcon;
    public GameObject cheapStandardPrefab;

    public Sprite cheapTankyIcon;
    public GameObject cheapTankyPrefab;

    // Expensive Units
    public Sprite expensiveRangedIcon;
    public GameObject expensiveRangedPrefab;

    public Sprite expensiveFastIcon;
    public GameObject expensiveFastPrefab;

    // Merge Unit
    public Sprite mergeUnitIcon;
    public GameObject mergeUnitPrefab;

    // Optional: Add stats or metadata for balance/design tools
    public int cheapStandardCost;
    public int cheapTankyCost;
    public int expensiveRangedCost;
    public int expensiveFastCost;
    public int mergeUnitCost;
}
