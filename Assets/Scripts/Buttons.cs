using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;

public class Buttons : MonoBehaviour
{
    public LevelGrid LG;

    public void ResetDataButton()
    {
        SceneManager.LoadScene("Privacy");
        LevelGrid.cells.Clear();
        ES3.DeleteFile("SaveFile.es3");
    }

    public void ShowRestartWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.ShowRestartWindow();
    }

    public void CloseRestartindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.CloseRestartWindow();
    }

    public void CloseExitWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.CloseExitWindow();
    }

    public void ShowDonateWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.ShowDonateWindow();
    }

    public void CloseDonateWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.CloseDonateWindow();
    }

    public void ExitGame()
    {
        GlobalSounds.Instance.PlaySound("button");
        Application.Quit();
    }

    public void SubmitToRestartLevel() // SAVE DATA
    {
        GlobalSounds.Instance.PlaySound("button");

        List<string[,]> levelStep = new List<string[,]>();
        ES3.Save("toSaveLevelStep", levelStep);

        LevelGrid.cells.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowLevelsScreen()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");

            Global.Instance.DOT_FlyInLevelsScreen(Global.Instance.FlyInOutSpeed);
        }
    }

    public void HideLevelsScreen()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");
            Global.Instance.DOT_FlyOutLevelsScreen(Global.Instance.FlyInOutSpeed);
        }

    }

    public void ShowSettingsScreen()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");

            Global.Instance.DOT_FlyInSettingsScreen(Global.Instance.FlyInOutSpeed);
        }
    }

    public void HideSettingsScreen()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");
            Global.Instance.DOT_FlyOutSettingsScreen(Global.Instance.FlyInOutSpeed);
        }
            
    }

    public void ShowAboutScreen()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");
            Global.Instance.DOT_FlyInAboutScreen(Global.Instance.FlyInOutSpeed);
        }
            
    }

    public void HideAboutScreen()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");
            Global.Instance.DOT_FlyOutBackAboutScreen(Global.Instance.FlyInOutSpeed);
        }
  
    }

    public void CloseAllScreens()
    {
        if (Global.Instance.CanShowHideSettings)
        {
            GlobalSounds.Instance.PlaySound("button");
            Global.Instance.DOT_CloseAllScreens(Global.Instance.FlyInOutSpeed);
        }

    }

    public void TutorialNextStep()
    {
        GlobalSounds.Instance.PlaySound("button");

        if (Global.Instance.HTP_StepCountGrid == 1 || Global.Instance.HTP_StepCountGrid == 9)
        {
            Global.Instance.HTP_StepCount++;
        }

        Global.Instance.HTP_StepCountGrid++;
        DOTween.KillAll(false);
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenTutorialScene()
    {
        GlobalSounds.Instance.PlaySound("button");
        SceneManager.LoadScene("Tutorial");
    }
    public void OpenMainScene()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.HTP_StepCount = 1;
        Global.Instance.HTP_StepCountGrid = 1;

        DOTween.KillAll(false);
        SceneManager.LoadScene("Main");
    }

    public void ShowSkipTutorWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.ShowSkipTutorWindow();
    }

    public void CloseSkipTutorWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.CloseSkipTutorWindow();
    }

    public void ShowHintWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.ShowHintWindow();
    }

    public void CloseHintWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.CloseHintWindow();
    }

    public void ClosePurchasedWindow()
    {
        GlobalSounds.Instance.PlaySound("button");
        Global.Instance.ClosePurchasedWindow();
    }

    public void AcceptPrivacy() // SAVE DATA
    {
        GlobalSounds.Instance.PlaySound("button");
        ES3.Save("toSaveCanShowPrivacyScreen", false);
        SceneManager.LoadScene("FirstLaunch");
    }

    public void SendMail()
    {
        GlobalSounds.Instance.PlaySound("button");

        void SendEmail()
        {
            string email = "piwpawgames@gmail.com";
            string subject = MyEscapeURL("NumPair Support");
            string body = MyEscapeURL("Please, describe your issue below:\n\n-----------------------------------------\n\n");
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        SendEmail();
    }

    public void OpenPrivacyPolicy()
    {
        GlobalSounds.Instance.PlaySound("button");
        Application.OpenURL("https://github.com/piwpawgames/root/blob/main/NumPair/Privacy%20Policy");
    }

    public void OpenTerms()
    {
        GlobalSounds.Instance.PlaySound("button");
        Application.OpenURL("https://github.com/piwpawgames/root/blob/main/NumPair/Terms%20%26%20Conditions");
    }

    public void OpenSuperlogicPlayStore()
    {
        GlobalSounds.Instance.PlaySound("button");
        Application.OpenURL("https://play.google.com/store/apps/dev?id=6916256973280609239");
    }

    public void ShowRewardedAd()
    {
        GlobalSounds.Instance.PlaySound("button");
        AdsRewarded.S.ShowAd();
    }

    public void ReturnStep()
    {
        GlobalSounds.Instance.PlaySound("button");

        if (ES3.KeyExists("toSaveLevelStep"))
        {
            List<string[,]> levelStep = ES3.Load<List<string[,]>>("toSaveLevelStep");

            if (levelStep.Count > 1)
            {
                levelStep.RemoveAt(levelStep.Count - 1);
                ES3.Save("toSaveLevelStep", levelStep);
                LG.RefreshGrid();
                Debug.Log("Returned");
            }
        }

    }
}
