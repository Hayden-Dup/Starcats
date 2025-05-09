using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    string resourceName;
    int remaining;
    bool collected = false;

    public void Initialize(string name, int amount)
    {
        resourceName = name;
        remaining    = amount;
        // start hidden; MapTile.Reveal() will activate this GameObject
        gameObject.SetActive(false);
    }

    public void CollectAll()
    {
        if (collected) return;
        collected = true;
        ResourceManager.Instance.AddResource(resourceName, remaining);
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        Debug.Log($"[ResourceNode] Clicked {resourceName}");
        CollectAll();
    }
}
