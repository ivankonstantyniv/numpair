using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameID = "4565785";
    [SerializeField] private string iosGameID = "4565784";
    [SerializeField] private bool testMode = false;
    private string gameID;

    private void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        gameID = (Application.platform == RuntimePlatform.IPhonePlayer) ? iosGameID : androidGameID;

        Advertisement.Initialize(gameID, testMode);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Initialization completed");

    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization failed: {error.ToString()} - {message}" );
    }
}