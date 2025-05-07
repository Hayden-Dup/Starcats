using UnityEngine;

public class Selector : MonoBehaviour
{
     // Start() and Update() methods deleted - we don't need them right now

public static Selector Instance;
[SerializeField]
private int choice = 0;
public int Choice => choice;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void UpdateCat()
    {
        choice = 1;
    }
    public void UpdateAlienmal()
    {
        choice = 2;
    }
}
