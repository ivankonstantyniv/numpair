using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeControl : MonoBehaviour
{
    public GameObject ExitWindow;

    private void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape) && Global.Instance.CanShowHideSettings && SceneManager.GetActiveScene().name == "Main")
		{
            if (Global.Instance.IsSettingsOpened)
            {
                Global.Instance.DOT_FlyOutSettingsScreen(Global.Instance.FlyInOutSpeed);
            }
            else if (Global.Instance.IsAboutOpened)
            {
                Global.Instance.DOT_FlyOutBackAboutScreen(Global.Instance.FlyInOutSpeed);
            }
            else if (Global.Instance.IsLevelsScreenOpened)
            {
                Global.Instance.DOT_FlyOutLevelsScreen(Global.Instance.FlyInOutSpeed);
            }
            else if (Global.Instance.IsRestartOpened)
            {
                Global.Instance.CloseRestartWindow();
            }
            else if (Global.Instance.IsDonateOpened)
            {
                Global.Instance.CloseDonateWindow();
            }
            else if (Global.Instance.IsExitOpened)
            {
                Global.Instance.CloseExitWindow();
            }
            else if (Global.Instance.IsHintWindowOpened)
            {
                Global.Instance.CloseHintWindow();
            }
            else if (Global.Instance.IsPurchasedWindowOpened)
            {
                Global.Instance.ClosePurchasedWindow();
            }
            else
            {
                ShowExitWindow(ExitWindow);
                Global.Instance.IsExitOpened = true;
            }
			
		}

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Tutorial")
        {
            if (Global.Instance.IsSkipTutorOpened)
            {
                Global.Instance.CloseSkipTutorWindow();
            } 
            else
            {
                Global.Instance.ShowSkipTutorWindow();
            }
        }

    }

    private void ShowExitWindow(GameObject obj)
    {
        if (obj == null)
            return;

        obj.SetActive(true);
    }
}
