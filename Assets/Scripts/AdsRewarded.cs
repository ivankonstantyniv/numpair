using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsRewarded S;
    public GameObject HintLockPlate;

    [SerializeField] private string androidadUnitID = "Rewarded_Android";
    [SerializeField] private string iosadUnitID = "Rewarded_iOS";
    private string adUnitID;
    private Button SubmitButton;
    private bool isAdLoaded;

    void Awake()
    {
        S = this;

        adUnitID = (Application.platform == RuntimePlatform.IPhonePlayer) ? iosadUnitID : androidadUnitID;
    }

    private void Start()
    {
        SubmitButton = GetComponent<Button>();
        isAdLoaded = false;

        LoadAd();
    }

    public void LoadAd()
    {
        Advertisement.Load(adUnitID, this);
    }

    public void ShowAd()
    {
        if (isAdLoaded)
        {
            Advertisement.Show(adUnitID, this);
        }
        else
        {
            LoadAd();
        }
            
    }

    public void ShowHintScreen() // SAVE DATA
    {
        ES3.Save("toSaveIsAlreadyHintShown", true);

        HintLockPlate.SetActive(false);

        SubmitButton.interactable = false;
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(adUnitID) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Advertisement.Load(adUnitID, this);

            ShowHintScreen();
        }

        Debug.Log("OnUnityAdsAdCompleted");

    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("OnUnityAdsAdLoaded");
        isAdLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("OnUnityAdsFailedToLoad");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log("OnUnityAdsShowFailure");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("OnUnityAdsShowStart");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("OnUnityAdsShowClick");
    }
}
