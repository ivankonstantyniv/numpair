using UnityEngine;
using UnityEngine.SceneManagement;

public class Privacy : MonoBehaviour
{
    private void Start()
    {
        bool canShowPrivacyScreen = true;

        if (ES3.KeyExists("toSaveCanShowPrivacyScreen"))
        {
            canShowPrivacyScreen = ES3.Load<bool>("toSaveCanShowPrivacyScreen");
        }

        if (canShowPrivacyScreen)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("FirstLaunch");
        }
    }
}
