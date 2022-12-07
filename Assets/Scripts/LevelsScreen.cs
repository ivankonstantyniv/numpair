using UnityEngine;

public class LevelsScreen : MonoBehaviour
{
    private RectTransform thisRect;
    public float screenWidth, screenHeight;

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        screenWidth = thisRect.rect.size.x;
        screenHeight = thisRect.rect.size.y;

        thisRect.anchoredPosition = new Vector2(0f, -1 * screenHeight);
    }
}
