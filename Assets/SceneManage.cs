using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public string sceneName;
    public void LoadSelectionScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
