using UnityEngine;

public class HintLevelImg : MonoBehaviour
{
    private RectTransform rectTrans;

    void Start()
    {
        rectTrans = gameObject.GetComponent<RectTransform>();

        float imgWidth = rectTrans.rect.width;
        rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, imgWidth);
    }

}
