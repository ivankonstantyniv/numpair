using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Global : MonoBehaviour
{
    public static Global Instance { get; private set; }

    public bool CanShowHideSettings;
    public bool IsSettingsOpened;
    public bool IsAboutOpened;
    public bool IsExitOpened;
    public bool IsRestartOpened;
    public bool IsDonateOpened;
    public bool IsSkipTutorOpened;
    public bool IsHintWindowOpened;
    public bool isHandAnimating;
    public bool IsPurchasedWindowOpened;
    public bool IsLevelsScreenOpened;
    public float FlyInOutSpeed;
    public int HTP_StepCount;
    public int HTP_StepCountGrid;
    public Color UIColor1, UIColor2;
    public GameObject CanvasGame;
    public GameObject SettingsScreens;
    public GameObject RestartWindow;
    public GameObject ExitWindow;
    public GameObject ConfettiGroup;
    public GameObject DonateWindow;
    public GameObject SkipTutorWindow;
    public GameObject HintWindow;
    public GameObject TutorNextBTN;
    public GameObject TutorFinishBTN;
    public GameObject PurchasedWindow;
    public GameObject BlockUIPanel;
    public GameObject LevelsScreen;
    // Save Load Data
    public int toSaveCurrentLevel;
    public bool toSaveSwitchSoundStateOn;
    public bool toSaveSwitchVibroStateOn;
    public bool toSaveIsFirstLaunch;
    public bool toSaveCanShowPrivacyScreen;
    public bool toSaveIsAlreadyHighlighted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetStartVariables();
    }

    private void SetStartVariables()
    {
        FlyInOutSpeed = 0.25f;
        HTP_StepCount = 1;
        HTP_StepCountGrid = 1;

        CanShowHideSettings = true;
        IsSettingsOpened = false;
        IsAboutOpened = false;
        IsExitOpened = false;
        IsRestartOpened = false;
        IsDonateOpened = false;
        IsSkipTutorOpened = false;
        IsHintWindowOpened = false;
        isHandAnimating = true;
        IsPurchasedWindowOpened = false;
        IsLevelsScreenOpened = false;
    }

    public void DOT_FlyInSettingsScreen(float duration)
    {
        CanShowHideSettings = false;
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsSettingsOpened = true;
            CanvasGame.SetActive(false);
            BlockUIPanel.SetActive(false);
        }

        SettingsScreens.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, SettingsScreens.GetComponent<SettingsScreens>().screenHeight), duration).OnComplete(onComplete);
    }

    public void DOT_FlyOutSettingsScreen(float duration)
    {
        CanShowHideSettings = false;
        CanvasGame.SetActive(true);
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsSettingsOpened = false;
            BlockUIPanel.SetActive(false);
        }

        SettingsScreens.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), duration).OnComplete(onComplete);
    }

    public void DOT_FlyInAboutScreen(float duration)
    {
        CanShowHideSettings = false;
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsAboutOpened = true;
            IsSettingsOpened = false;
            BlockUIPanel.SetActive(false);
        }

        SettingsScreens.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1 * SettingsScreens.GetComponent<SettingsScreens>().screenWidth, SettingsScreens.GetComponent<RectTransform>().anchoredPosition.y), duration).OnComplete(onComplete);
    }

    public void DOT_FlyOutBackAboutScreen(float duration)
    {
        CanShowHideSettings = false;
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsAboutOpened = false;
            IsSettingsOpened = true;
            BlockUIPanel.SetActive(false);
        }

        SettingsScreens.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, SettingsScreens.GetComponent<RectTransform>().anchoredPosition.y), duration).OnComplete(onComplete);
    }

    public void DOT_CloseAllScreens(float duration)
    {
        CanvasGame.SetActive(true);
        CanShowHideSettings = false;
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsSettingsOpened = false;
            IsAboutOpened = false;
            BlockUIPanel.SetActive(false);
            SettingsScreens.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        }

        SettingsScreens.GetComponent<RectTransform>().DOAnchorPos(new Vector2(SettingsScreens.GetComponent<RectTransform>().anchoredPosition.x, 0f), duration).OnComplete(onComplete);
    }

    public void DOT_FlyInLevelsScreen(float duration)
    {
        CanShowHideSettings = false;
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsLevelsScreenOpened = true;
            CanvasGame.SetActive(false);
            BlockUIPanel.SetActive(false);
        }

        LevelsScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, 0f), duration).OnComplete(onComplete);
    }

    public void DOT_FlyOutLevelsScreen(float duration)
    {
        CanvasGame.SetActive(true);
        CanShowHideSettings = false;
        BlockUIPanel.SetActive(true);

        void onComplete()
        {
            CanShowHideSettings = true;
            IsLevelsScreenOpened = false;
            BlockUIPanel.SetActive(false);
        }

        LevelsScreen.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0f, -1 * LevelsScreen.GetComponent<LevelsScreen>().screenHeight), duration).OnComplete(onComplete);
    }


    public void ShowRestartWindow()
    {
        RestartWindow.SetActive(true);
        IsRestartOpened = true;
    }

    public void CloseRestartWindow()
    {
        RestartWindow.SetActive(false);
        IsRestartOpened = false;
    }

    public void CloseExitWindow()
    {
        ExitWindow.SetActive(false);
        IsExitOpened = false;
    }

    public void ShowDonateWindow()
    {
        DonateWindow.SetActive(true);
        IsDonateOpened = true;
    }

    public void CloseDonateWindow()
    {
        DonateWindow.SetActive(false);
        IsDonateOpened = false;
    }

    public void ShowSkipTutorWindow()
    {
        SkipTutorWindow.SetActive(true);
        IsSkipTutorOpened = true;
    }

    public void CloseSkipTutorWindow()
    {
        SkipTutorWindow.SetActive(false);
        IsSkipTutorOpened = false;
    }

    public void ShowHintWindow()
    {
        HintWindow.SetActive(true);
        IsHintWindowOpened = true;
    }

    public void CloseHintWindow()
    {
        HintWindow.SetActive(false);
        IsHintWindowOpened = false;
    }

    public void OnVibrate() // SAVE DATA
    {
        bool switchVibroStateOn = true;

        if (ES3.KeyExists("toSaveSwitchVibroStateOn"))
        {
            switchVibroStateOn = ES3.Load<bool>("toSaveSwitchVibroStateOn");
        }

        if (switchVibroStateOn)
            Handheld.Vibrate();
    }

    public void SetTutorButtonsInteractable()
    {
        TutorNextBTN.GetComponent<Button>().interactable = true;
        TutorFinishBTN.GetComponent<Button>().interactable = true;
    }

    public void ShowPurchasedWindow()
    {
        PurchasedWindow.SetActive(true);
        IsPurchasedWindowOpened = true;
    }

    public void ClosePurchasedWindow()
    {
        PurchasedWindow.SetActive(false);
        IsPurchasedWindowOpened = false;
    }
}
