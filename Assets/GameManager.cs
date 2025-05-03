using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Selector;
    public int FactionChoice;
    void Start()
    {
        Debug.Log("HIHOFAWF");
        OnSceneLoad();
    }
    void OnSceneLoad()
    {
        GameObject selectorGO = GameObject.FindWithTag("SceneSelector"); // FindWithTag returns one object
        Selector selectorScript = selectorGO.GetComponent<Selector>();
        FactionChoice = selectorScript.Choice;
        if(FactionChoice == 1)
        {
            Debug.Log("Choice be 1");
        }
        if(FactionChoice == 2)
        {
            Debug.Log("Choice be 2");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
