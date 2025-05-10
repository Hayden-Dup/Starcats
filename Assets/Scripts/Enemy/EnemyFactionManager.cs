// Assets/Scripts/EnemyFactionManager.cs
using UnityEngine;

public class EnemyFactionManager : MonoBehaviour
{
    [Tooltip("Prefabs of your enemy unit(s)")]
    public GameObject[] enemyUnitPrefabs;

    [Tooltip("Where enemy units will spawn")]
    public Transform[] spawnPoints;

    [Tooltip("Seconds between spawns")]
    public float spawnInterval = 15f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            SpawnEnemyUnit();
            _timer = 0f;
        }
    }

    void SpawnEnemyUnit()
    {
        if (enemyUnitPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        // pick a random spawn point and a random unit type
        var sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var prefab = enemyUnitPrefabs[Random.Range(0, enemyUnitPrefabs.Length)];

        Instantiate(prefab, sp.position, Quaternion.identity);
    }
}
