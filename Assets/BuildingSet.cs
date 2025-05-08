using UnityEngine;
[CreateAssetMenu]
public class BuildingSet : ScriptableObject
{
    public int mainBuildingCost = 500;
    public int cheapUnitBuildingCost = 350;
    public int rangedUnitBuildingCost = 450;
    public int mergeUnitBuildingCost = 600;

    public Sprite mainBuildingIcon;
    public GameObject mainBuildingPrefab;

    public Sprite cheapUnitBuildingIcon;
    public GameObject cheapUnitBuildingPrefab;

    public Sprite rangedUnitBuildingIcon;
    public GameObject rangedUnitBuildingPrefab;

    public Sprite mergeUnitBuildingIcon;
    public GameObject mergeUnitBuildingPrefab;
}
