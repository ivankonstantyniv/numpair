using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelObject : Selectable, IPointerClickHandler
{
    public Text TextValue;
    private Vector2 levelCellSize;
    public int levelCount = 1;
    public bool isLevelActive = false;
    public bool isCurrentLevel = false;
    public Color32 colorUnlocked;
    public Color32 colorLocked;

    private new void Start()
    {
        if (isCurrentLevel)
        {
            TextValue.fontSize = (int)((int)levelCellSize.x / 2.75f);
            TextValue.fontStyle = FontStyle.Bold;
        }
        else
        {
            TextValue.fontSize = (int)((int)levelCellSize.x / 3.75f);
        }

        if (isLevelActive)
        {
            TextValue.color = colorUnlocked;
        }
        else
        {
            TextValue.color = colorLocked;
        }

            
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable())
            return;

        GlobalSounds.Instance.PlaySound("button");

        List<string[,]> levelStep = new List<string[,]>();
        ES3.Save("toSaveLevelStep", levelStep);

        ES3.Save("toSaveIsAlreadyHintShown", false);

        ES3.Save("toSaveCurrentLevel", levelCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } // SAVE DATA

    public void SetTextValue(string value)
    {
        TextValue.text = value;
    }

    public void SetLevelCellSize(float width, float height)
    {
        levelCellSize = new Vector2(width, height);
    }
}
