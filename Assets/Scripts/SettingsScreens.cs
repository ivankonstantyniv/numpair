using UnityEngine;

public class SettingsScreens : MonoBehaviour
{
    public RectTransform RectSettings, RectAbout;
    public float screenWidth, screenHeight;
    private RectTransform thisRect;

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        screenWidth = thisRect.rect.size.x;
        screenHeight = thisRect.rect.size.y;

        RectSettings.anchoredPosition = new Vector2(0f, -1* screenHeight);
        RectAbout.anchoredPosition = new Vector2(screenWidth, -1 * screenHeight);
    }

}

