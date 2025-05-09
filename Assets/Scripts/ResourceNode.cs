using System;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    string resourceName;
    int remaining;
    bool collected = false;

    /// <summary>
    /// Called by MapGenerator to set up this node.
    /// </summary>
    public void Initialize(string name, int amount)
    {
        resourceName = name;
        remaining    = amount;
        // start hidden; MapTile.Reveal() will activate this GameObject
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Actually give the resource and destroy.
    /// </summary>
    void CollectAll()
    {
        if (collected) return;
        collected = true;

        if (resourceName.Equals("Coin", StringComparison.OrdinalIgnoreCase))
            GameManager.Instance.AddMoney(remaining);
        else
            ResourceManager.Instance.AddResource(resourceName, remaining);


        Destroy(gameObject);
    }


    /// <summary>
    /// Proximity‐based collection when the player enters this trigger.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[ResourceNode] {resourceName} collected by {other.name}");
            CollectAll();
        }
    }
}
