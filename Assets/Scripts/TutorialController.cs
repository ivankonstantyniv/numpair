using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public GameObject[] StepTextArray;
    public GameObject stepsTXT;
    public GameObject NextBTN, FinishBTN;
    public GameObject SkipTutorWindow;

    private void Start()
    {
        Global.Instance.TutorNextBTN = NextBTN;
        Global.Instance.TutorFinishBTN = FinishBTN;
        Global.Instance.SkipTutorWindow = SkipTutorWindow;
        CheckForStepText();

        if (Global.Instance.HTP_StepCountGrid == 15)
        {
            FinishBTN.SetActive(true);
            NextBTN.SetActive(false);
        }
        else
        {
            NextBTN.SetActive(true);
            FinishBTN.SetActive(false);
        }


        stepsTXT.GetComponent<TextMeshProUGUI>().text = "<size=36><color=#C8C8C8>Step: " + Global.Instance.HTP_StepCountGrid.ToString() + "/15</color></size>";
    }

    private void CheckForStepText()
    {
        for (int i = 0; i < StepTextArray.Length; i++)
        {
            if (Global.Instance.HTP_StepCount - 1 == i)
            {
                StepTextArray[i].SetActive(true);
            }
            else
            {
                StepTextArray[i].SetActive(false);
            }
        }
    }

}
