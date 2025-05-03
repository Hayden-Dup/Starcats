using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSet", menuName = "Faction/Building Set")]
public class BuildingSet : ScriptableObject
{
    public Sprite mainBuildingIcon;
    public Sprite cheapUnitBuildingIcon;
    public Sprite rangedUnitBuildingIcon;
    public Sprite mergeUnitBuildingIcon;
}
