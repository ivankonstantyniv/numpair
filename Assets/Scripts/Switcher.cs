using UnityEngine;
using UnityEngine.UI;

public class Switcher : MonoBehaviour
{
    [SerializeField] private Color32 ColorOn, ColorOff;
    private RectTransform SwitchRoundRect;
    private Image SwitchBgImg;
    private float switchRoundPosValue = 0;
    private bool switchSoundStateOn;
    private bool switchVibroStateOn;

    public enum Type
    {
        Sound,
        Vibration
    }

    public Type type;

    private void Start()
    {
        switchSoundStateOn = true;
        switchVibroStateOn = true;

        if (ES3.KeyExists("toSaveSwitchSoundStateOn"))
        {
            switchSoundStateOn = ES3.Load<bool>("toSaveSwitchSoundStateOn");
        }

        if (ES3.KeyExists("toSaveSwitchVibroStateOn"))
        {
            switchVibroStateOn = ES3.Load<bool>("toSaveSwitchVibroStateOn");
        }

        SwitchRoundRect = transform.GetChild(0).GetComponent<RectTransform>();
        SwitchBgImg = GetComponent<Image>();
        switchRoundPosValue = SwitchRoundRect.rect.width / 2 + (((SwitchRoundRect.rect.width / 2) * 5) / 33);
        CheckSwitcherState(SwitchBgImg, SwitchRoundRect, switchRoundPosValue);
    }

    public void OnPointerClick()
    {
        GlobalSounds.Instance.PlaySound("button");

        bool _switchSoundStateOn = true;
        bool _switchVibroStateOn = true;

        if (ES3.KeyExists("toSaveSwitchSoundStateOn"))
        {
            _switchSoundStateOn = ES3.Load<bool>("toSaveSwitchSoundStateOn");
        }

        if (ES3.KeyExists("toSaveSwitchVibroStateOn"))
        {
            _switchVibroStateOn = ES3.Load<bool>("toSaveSwitchVibroStateOn");
        }

        if (type == Type.Sound)
        {
            if (_switchSoundStateOn)
            {
                ES3.Save("toSaveSwitchSoundStateOn", false);

                SwitchBgImg.color = ColorOff;
                SwitchRoundRect.anchoredPosition = new Vector2(-1 * switchRoundPosValue, SwitchRoundRect.anchoredPosition.y);
            }
            else
            {
                ES3.Save("toSaveSwitchSoundStateOn", true);

                SwitchBgImg.color = ColorOn;
                SwitchRoundRect.anchoredPosition = new Vector2(switchRoundPosValue, SwitchRoundRect.anchoredPosition.y);
            }
        }
        else if (type == Type.Vibration)
        {
            if (_switchVibroStateOn)
            {
                ES3.Save("toSaveSwitchVibroStateOn", false);

                SwitchBgImg.color = ColorOff;
                SwitchRoundRect.anchoredPosition = new Vector2(-1 * switchRoundPosValue, SwitchRoundRect.anchoredPosition.y);
            }
            else
            {
                ES3.Save("toSaveSwitchVibroStateOn", true);

                SwitchBgImg.color = ColorOn;
                SwitchRoundRect.anchoredPosition = new Vector2(switchRoundPosValue, SwitchRoundRect.anchoredPosition.y);
                Handheld.Vibrate();
            }
        }
    }

    private void CheckSwitcherState(Image SwitchImg, RectTransform RoundButtonRect, float roundPosition)
    {
        if (type == Type.Sound)
        {
            if (switchSoundStateOn)
            {
                SwitchImg.color = ColorOn;
                RoundButtonRect.anchoredPosition = new Vector2(roundPosition, RoundButtonRect.anchoredPosition.y);
            }
            else
            {
                SwitchImg.color = ColorOff;
                RoundButtonRect.anchoredPosition = new Vector2(-1 * roundPosition, RoundButtonRect.anchoredPosition.y);
            }
        }
        else if (type == Type.Vibration)
        {
            if (switchVibroStateOn)
            {
                SwitchImg.color = ColorOn;
                RoundButtonRect.anchoredPosition = new Vector2(roundPosition, RoundButtonRect.anchoredPosition.y);
            }
            else
            {
                SwitchImg.color = ColorOff;
                RoundButtonRect.anchoredPosition = new Vector2(-1 * roundPosition, RoundButtonRect.anchoredPosition.y);
            }
        }
    }

    
}
