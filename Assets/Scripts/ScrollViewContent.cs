using System.Collections.Generic;
using UnityEngine;

public class ScrollViewContent : MonoBehaviour
{
    public GameObject levelCell;
    public static List<GameObject> levelsList = new List<GameObject>();
    public List<LevelData> levelDataList = new List<LevelData>();
    private RectTransform contentRect;
    public float contentWidth, contentHeight;
    public int columnsAmount = 5;
    public int rowsAmount;
    public int levelsAmount = 100;
    public int activeLevel = 1;
    private Vector2 cellSize;

    private void Start()
    {
        levelsList.Clear();

        contentRect = GetComponent<RectTransform>();
        contentWidth = contentRect.rect.size.x;
        contentHeight = contentRect.rect.size.y;
        rowsAmount = levelsAmount / columnsAmount;

        cellSize = new Vector2(contentWidth / columnsAmount, contentWidth / columnsAmount);
        contentRect.sizeDelta = new Vector2(contentRect.anchoredPosition.x, (cellSize.x * (rowsAmount + 1) ) - contentHeight);
        contentRect.SetRight(90f);

        SetLevelData();
        GenerateGrid();


        for (int i = 0; i < levelsList.Count; i++)
        {
            GameObject thisLevel = levelsList[i];
            LevelObject levelObj = thisLevel.GetComponent<LevelObject>();
            LevelData levelData;

            levelData = levelDataList[i];

            if (levelData.IsUnlocked)
            {
                levelObj.isLevelActive = true;
                Debug.Log("level " + i);

            }
            else
            {
                levelObj.interactable = false;
            }

            if (LevelGrid.GameLevel.currentLevel == levelData.LevelCount)
            {
                levelObj.isCurrentLevel = true;
            }
        }
    }

    private void GenerateGrid()
    {
        int levelCount = 1;

        for (int row = 0; row < rowsAmount; row++)
        {
            for (int column = 0; column < columnsAmount; column++)
            {
                GameObject thisCell;
                RectTransform thisCellRect;
                LevelObject thisLevelObject;

                thisCell = Instantiate(levelCell, transform);
                levelsList.Add(thisCell);
                thisCell.transform.rotation = Quaternion.identity;
                thisCell.name = $"Level {levelCount}";

                thisLevelObject = thisCell.GetComponent<LevelObject>();
                thisLevelObject.SetLevelCellSize(cellSize.x, cellSize.y);
                thisLevelObject.levelCount = levelCount;

                if (levelCount >= 75 && levelCount <= 89)
                {
                    thisLevelObject.SetTextValue(levelCount.ToString() + "*");
                }
                else if (levelCount >= 90)
                {
                    thisLevelObject.SetTextValue(levelCount.ToString() + "**");
                }
                else
                {
                    thisLevelObject.SetTextValue(levelCount.ToString());
                }

                thisCellRect = thisCell.GetComponent<RectTransform>();
                thisCellRect.sizeDelta = cellSize;
                thisCellRect.anchoredPosition = new Vector2(column * thisCellRect.rect.width + thisCellRect.rect.width/2, -1 * (row * thisCellRect.rect.height + thisCellRect.rect.height/2) );

                levelCount++;
            }
        }
    }

    private void SetLevelData()
    {
        for (int i = 1; i <= levelsAmount; i++)
        {
            LevelData thisData = new LevelData();

            if (i == 1 || i == 75)
            {
                thisData.SetData(i, true);
            }
            else
            {
                thisData.SetData(i, false);
            }

            levelDataList.Add(thisData);
        }

        if (ES3.KeyExists("toSaveLevelDataList"))
        {
            levelDataList = ES3.Load<List<LevelData>>("toSaveLevelDataList");

            levelDataList[0].IsUnlocked = true;
            levelDataList[74].IsUnlocked = true;

            Debug.Log("levelDataList LOADED!");

        }
        else
        {
            ES3.Save<List<LevelData>>("toSaveLevelDataList", levelDataList);
            Debug.Log("levelDataList SAVED!");
        }
    } // SAVE DATA
}

public class LevelData
{
    public int LevelCount;
    public bool IsUnlocked;

    public void SetData(int _levelCount, bool _isUnlocked)
    {
        this.LevelCount = _levelCount;
        this.IsUnlocked = _isUnlocked;
    }
}
