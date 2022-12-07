using UnityEngine;

public class HintScreenSizeChanger : MonoBehaviour
{
    public GameObject[] ScreenObjects;
    private float screenHeight;
    private RectTransform objRectTrans;


    void Start()
    {
        screenHeight = 15f + 25f + 40f; // offsets

        foreach (GameObject obj in ScreenObjects)
        {
            screenHeight = screenHeight + obj.GetComponent<RectTransform>().rect.height;
        }

        objRectTrans = gameObject.GetComponent<RectTransform>();
        objRectTrans.sizeDelta = new Vector2(objRectTrans.sizeDelta.x, screenHeight);

    }

}
