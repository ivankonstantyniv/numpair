using UnityEngine;

public class GlobalSounds : MonoBehaviour
{
    public static GlobalSounds Instance { get; private set; }

    public AudioClip ButtonSound, ChooseSound, PairSound, WinSound;
    private AudioSource AS;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    public void PlaySound(string type)
    {
        bool switchSoundStateOn = true;

        if (ES3.KeyExists("toSaveSwitchSoundStateOn"))
        {
            switchSoundStateOn = ES3.Load<bool>("toSaveSwitchSoundStateOn");
        }

        if (switchSoundStateOn)
        {
            if (type == "button")
            {
                AS.PlayOneShot(ButtonSound);
            }
            else if (type == "choose")
            {
                AS.PlayOneShot(ChooseSound);
            }
            else if (type == "pair")
            {
                AS.PlayOneShot(PairSound);
            }
            else if (type == "win")
            {
                AS.PlayOneShot(WinSound);
            }
            else
            {
                Debug.LogError("Error! Wrong type of sound!");
            }
        }
    }

}
