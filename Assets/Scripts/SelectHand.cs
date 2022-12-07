using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectHand : MonoBehaviour
{
    public HowToPlay HTP;
    private RectTransform RT;
    public static Image IMG;
    public float xPosStep;
    private Vector2 startPosition;
    public float speedOfShowing = 0.25f;
    public float speedOfTapping = 0.25f;

    private void Start()
    {
        Global.Instance.isHandAnimating = true;

        RT = GetComponent<RectTransform>();
        IMG = GetComponent<Image>();
        RT.localScale = new Vector3(0f, 0f, 0f);
        xPosStep = HTP.cellWidth + HTP.offset;
        startPosition = new Vector2(RT.anchoredPosition.x, RT.anchoredPosition.y);

        if (Global.Instance.HTP_StepCount > 1 )
        {
            SetHandPosition(startPosition.x + xPosStep / 2, startPosition.y);
            startPosition = new Vector2(RT.anchoredPosition.x, RT.anchoredPosition.y);

            ChangeHandPositions(RT);
        }
        else
        {
            SetHandPosition(startPosition.x - xPosStep, startPosition.y - xPosStep / 2);
            startPosition = new Vector2(RT.anchoredPosition.x, RT.anchoredPosition.y);
        }

        StartCoroutine(DOT_ShowUpHand(RT, 0.5f, speedOfShowing));
    }

    private void SetHandPosition(float _x, float _y)
    {
        RT.anchoredPosition = new Vector2(_x, _y);
    }

    public static void SetAlpha(Image img, float value)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, value);
    }

    private void ChangeHandPositions(RectTransform RT)
    {
        if (Global.Instance.HTP_StepCount == 1)
        {
            if (RT.anchoredPosition.y == startPosition.y)
            {
                RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, startPosition.y + xPosStep);
            }
            else
            {
                RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, startPosition.y);
            }
        }
        else
        {
            if (RT.anchoredPosition.x == startPosition.x)
            {
                RT.anchoredPosition = new Vector2(startPosition.x - xPosStep, RT.anchoredPosition.y);

            }
            else
            {
                RT.anchoredPosition = new Vector2(startPosition.x, RT.anchoredPosition.y);
            }
        }
    }

    IEnumerator DOT_ShowUpHand(RectTransform objRect, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        void onComplete()
        {
            StartCoroutine(DOT_AnimateDownHand(RT, 0.25f, speedOfTapping));
        }

        if (Global.Instance.isHandAnimating && objRect != null)
            objRect.transform.DOScale(new Vector3(1f, 1f, 1f), duration).OnComplete(onComplete);
    }

    IEnumerator DOT_HideDownHand(RectTransform objRect, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        void onComplete()
        {
            ChangeHandPositions(RT);
            StartCoroutine(DOT_ShowUpHand(RT, 0.5f, speedOfShowing));
        }

        if (objRect != null)
            objRect.transform.DOScale(new Vector3(0f, 0f, 0f), duration).OnComplete(onComplete);
    }

    IEnumerator DOT_AnimateDownHand(RectTransform objRect, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        void onComplete()
        {
            StartCoroutine(DOT_AnimateUpHand(RT, 0f, speedOfTapping));
        }

        if (objRect != null)
            objRect.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), duration).OnComplete(onComplete);
    }

    IEnumerator DOT_AnimateUpHand(RectTransform objRect, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        void onComplete()
        {
            StartCoroutine(DOT_HideDownHand(RT, 0.5f, speedOfShowing));
        }

        if (objRect != null)
            objRect.transform.DOScale(new Vector3(1f, 1f, 1f), duration).OnComplete(onComplete);
    }
}
