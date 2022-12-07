using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLaunch : MonoBehaviour
{
    AsyncOperation loadingScene;

    private void Start()
    {
        StartCoroutine(LoadGame(2f));
    }

    IEnumerator LoadGame(float delay)
    {
        yield return new WaitForSeconds(delay);

        bool isFirstLaunch = true;

        if (ES3.KeyExists("toSaveIsFirstLaunch"))
        {
            isFirstLaunch = ES3.Load<bool>("toSaveIsFirstLaunch");
        }

        if (isFirstLaunch)
        {
            loadingScene = SceneManager.LoadSceneAsync("Tutorial");
        }
        else
        {
            loadingScene = SceneManager.LoadSceneAsync("Main");
        }
    }
}
