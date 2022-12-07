using UnityEngine;

public class ButtonsGroup : MonoBehaviour
{
    private RectTransform thisRect;
    private float screenWidth;
    [SerializeField] private RectTransform RestartBTN;
    [SerializeField] private RectTransform LevelsBTN;
    [SerializeField] private RectTransform DonateBTN;
    [SerializeField] private RectTransform HintBTN;

    void Start()
    {
        thisRect = GetComponent<RectTransform>();
        screenWidth = thisRect.rect.size.x;

        SetButtonsPositions(screenWidth);
    }

    private void SetButtonsPositions(float _screenWidth)
    {
        float widthOffset = _screenWidth / 4;

        // RestartBTN
        RestartBTN.SetRight(widthOffset*3);
        // LevelsBTN
        LevelsBTN.SetRight(widthOffset * 2);
        LevelsBTN.SetLeft(widthOffset);
        // DonateBTN
        DonateBTN.SetRight(widthOffset);
        DonateBTN.SetLeft(widthOffset * 2);
        // HintBTN
        HintBTN.SetLeft(widthOffset * 3);
    }

}

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
