using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_API : MonoBehaviour
{
    public GameObject loadingScreen;

    public void OpenScene(int buildIndex)
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(buildIndex);
    }

    public void OpenSceneByName(string scene_name)
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(scene_name);
    }
}
