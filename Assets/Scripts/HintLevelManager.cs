using UnityEngine;
using UnityEngine.UI;

public class HintLevelManager : MonoBehaviour
{
    private int currentLevel;
    private Image Image;
    private Sprite levelSp;
    public Text currentLevelTxt;
    public GameObject HintLockPlate;
    public Button WatchAdsButton;
    private string levelFolderPath;


    void Start()
    {
        CheckForHint();

        if (ES3.KeyExists("toSaveCurrentLevel"))
        {
            currentLevel = ES3.Load<int>("toSaveCurrentLevel");
        }
        else
        {
            currentLevel = 1;
        }

        currentLevelTxt.text = "Level " + currentLevel.ToString();

        levelFolderPath = "Levels/" + currentLevel.ToString();
        levelSp = Resources.Load<Sprite>(levelFolderPath + "/1");

        Image = gameObject.GetComponent<Image>();
        Image.sprite = levelSp;
    }

    private void CheckForHint()
    {
        bool toSaveIsAlreadyHintShown = false;

        if (ES3.KeyExists("toSaveIsAlreadyHintShown"))
        {
            toSaveIsAlreadyHintShown = ES3.Load<bool>("toSaveIsAlreadyHintShown");
        }

        if (toSaveIsAlreadyHintShown == false)
            return;

        //set lock hint invisibile
        HintLockPlate.SetActive(false);

        WatchAdsButton.interactable = false;
    } // SAVE DATA

}
