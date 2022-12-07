using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TMP_About : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI mainText;

    private void Start()
    {
        mainText = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int index = TMP_TextUtilities.FindIntersectingLink(mainText, Input.mousePosition, Camera.main);

        if (index > -1)
        {
            Application.OpenURL(mainText.textInfo.linkInfo[index].GetLinkID());
        }
    }
}
