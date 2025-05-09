// Assets/Scripts/ResourceManager.cs
using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // key: resource name (e.g. "Wood"), value: how many units the player has
    private Dictionary<string, int> resources = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Add some amount of a resource (creates the entry if needed).
    /// </summary>
    public void AddResource(string resourceName, int amount)
    {
        if (amount <= 0) return;
        if (!resources.ContainsKey(resourceName))
            resources[resourceName] = 0;
        resources[resourceName] += amount;
        Debug.Log($"[ResourceManager] +{amount} {resourceName} → {resources[resourceName]}");
        // TODO: notify UI to update
    }

    /// <summary>
    /// Spend an amount, returns true if successful.
    /// </summary>
    public bool SpendResource(string resourceName, int amount)
    {
        if (amount <= 0 || !resources.ContainsKey(resourceName) || resources[resourceName] < amount)
            return false;
        resources[resourceName] -= amount;
        Debug.Log($"[ResourceManager] -{amount} {resourceName} → {resources[resourceName]}");
        // TODO: notify UI to update
        return true;
    }

    /// <summary>
    /// Get current amount (0 if none).
    /// </summary>
    public int GetResourceAmount(string resourceName)
    {
        if (!resources.TryGetValue(resourceName, out var amt)) return 0;
        return amt;
    }
}
