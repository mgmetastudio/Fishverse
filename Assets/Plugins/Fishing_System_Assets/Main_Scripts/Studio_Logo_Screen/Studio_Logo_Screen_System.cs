using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Studio_Logo_Screen_System : MonoBehaviour
{
    [Header("Enter Scene")]
    public string Scene_To_Enter;
    public string SceneToUnload;
    public bool canEnterScene;
    [Header("Canvas")]
    public GameObject Canvas;

    public void Start()
    {
        StartCoroutine(Enter_Scene_After_Seconds());
    }

    IEnumerator Enter_Scene_After_Seconds()
    {
        yield return new WaitForSeconds(2);
        Canvas.GetComponent<Animator>().enabled = false;
        Load_Scene();
    }

    public void Load_Scene()
    {
        StartCoroutine(load());
    }

    IEnumerator load()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Scene_To_Enter);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {

            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
                SceneManager.UnloadSceneAsync(SceneToUnload);
            }

            yield return null;
        }
    }
}
