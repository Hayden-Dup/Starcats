using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Custom/Unit Set")]
public class UnitSet : ScriptableObject
{
    public List<UnitData> units;
}

[System.Serializable]
public class UnitData
{
    public string unitName;
    public Sprite icon;
    public GameObject prefab;
    public int unitCost;
    public float damage;
    public float health;
    public float moveSpeed;
    public float range;
    public float productionTime;
}

