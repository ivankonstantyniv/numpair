using UnityEngine;
using UnityEngine.UI;

public class DonateController : MonoBehaviour
{
    [SerializeField] private GameObject priceValuesGroup;
    [SerializeField] private Button arrowLeft, arrowRight;
    [SerializeField] private int activeChildVal = 0;
    private Button button;
    private string buttonName;
    private int childCount;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonName = button.transform.name;
        childCount = priceValuesGroup.transform.childCount;
    }

    public void ChangePriceValue()
    {
        GlobalSounds.Instance.PlaySound("button");
        activeChildVal = CheckForActive();

        if (buttonName == "Right Arrow BTN")
        {
            activeChildVal++;
        }
        else if (buttonName == "Left Arrow BTN")
        {
            activeChildVal--;
        }

        CheckForInteractable();

        for (int val = 0; val < childCount; val++)
        {
            var child = priceValuesGroup.transform.GetChild(val).gameObject;

            if (val == activeChildVal)
            {
                child.SetActive(true);
            }
            else
            {
                child.SetActive(false);
            }
        }
    }
    private int CheckForActive()
    {
        int childCount = priceValuesGroup.transform.childCount;
        int activeChild = 0;

        for (int val = 0; val < childCount; val++)
        {
            var child = priceValuesGroup.transform.GetChild(val).gameObject;
            bool isChildActive = child.activeSelf;

            if (isChildActive)
            {
                activeChild = val;
            }
        }

        return activeChild;
    }

    private void CheckForInteractable()
    {
        if (activeChildVal == 0)
        {
            arrowLeft.interactable = false;
        }
        else
        {
            arrowLeft.interactable = true;
        }

        if (activeChildVal == childCount - 1)
        {
            arrowRight.interactable = false;
        }
        else
        {
            arrowRight.interactable = true;
        }
    }
}
