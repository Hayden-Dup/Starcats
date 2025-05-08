using UnityEngine;
[CreateAssetMenu]
public class BuildingSet : ScriptableObject
{
    public Sprite mainBuildingIcon;
    public GameObject mainBuildingPrefab;

    public Sprite cheapUnitBuildingIcon;
    public GameObject cheapUnitBuildingPrefab;

    public Sprite rangedUnitBuildingIcon;
    public GameObject rangedUnitBuildingPrefab;

    public Sprite mergeUnitBuildingIcon;
    public GameObject mergeUnitBuildingPrefab;
}
