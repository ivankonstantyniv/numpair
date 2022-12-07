using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelGrid : MonoBehaviour
{
    public float cellWidth = 150.0f;
    public float cellHeight = 150.0f;
    public float offset = 0.0f;
    public float maxWidthHeight;
    public float bgOffset = 0.0f;
    public float cellScale = 1.0f;
    public GameObject Cell, BgGrid;
    public Vector2 startPos = new Vector2(0.0f, 0.0f);
    public static List<GameObject> cells = new List<GameObject>();
    public static Level GameLevel;
    public Color BlockColor;
    public Text textLevel;
    public GameObject CanvasGame;
    public GameObject GameGrid;
    public GameObject ButtonGroup;
    public GameObject RestartWindow;
    public GameObject ExitWindow;
    public GameObject ConfettiGroup;
    public GameObject DonateWindow;
    public GameObject HintWindow;
    public GameObject PurchasedWindow;
    public GameObject SettingsScreens;
    public GameObject BlockUIPanel;
    public GameObject LevelsScreen;

    void Start() // SAVE DATA
    {
        SetGlobalVariables();

        maxWidthHeight = 900f + (5f * offset);

        cells.Clear();

        CheckForFirstLaunch();
        CheckForLevelComplete();

        // INIT GAME LEVEL
        if (ES3.KeyExists("toSaveCurrentLevel"))
        {
            GameLevel = new Level(ES3.Load<int>("toSaveCurrentLevel"));
        }
        else
        {
            GameLevel = new Level(1);
        }

        textLevel.text = "Level " + GameLevel.currentLevel;

        CreateLevel();
        //START ANIMATIONS
        DOT_AnimateButtonGroup(0.5f);
    }

    private void CreateLevel()
    {
        List<string[,]> levelStep;

        if (ES3.KeyExists("toSaveLevelStep") == true)
        {
            levelStep = ES3.Load<List<string[,]>>("toSaveLevelStep");
        } else
        {
            levelStep = new List<string[,]>();
        }


        if (levelStep.Count > 0)
        {
            GameLevel.grid = levelStep[levelStep.Count - 1];
            Debug.Log("Saved Level");
        }
        else
        {
            levelStep.Add(GameLevel.grid);
            ES3.Save("toSaveLevelStep", levelStep);
            Debug.Log("Default Level");
        }


        CreateGrid();
        ChangeGridPosition();
        ChangeBgGridPosition();
    } // CREATE LEVEL

    private void CheckForFirstLaunch()
    {
        bool isFirstLaunch = true;

        if (ES3.KeyExists("toSaveIsFirstLaunch"))
        {
            isFirstLaunch = ES3.Load<bool>("toSaveIsFirstLaunch");
        }

        if (isFirstLaunch)
        {
            ES3.Save<bool>("toSaveIsFirstLaunch", false);
        }
    } // SAVE DATA

    public void RefreshGrid() // REFRESH LEVEL
    {
        cells.Clear();

        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        List<string[,]> levelStep = ES3.Load<List<string[,]>>("toSaveLevelStep");

        if (levelStep.Count > 0)
        {
            GameLevel.grid = levelStep[levelStep.Count - 1];

            Debug.Log("Saved Step");
        }
        else
        {
            Debug.Log("Default Step");
        }

        Debug.Log("Create Grid");

        CreateGrid();
    }

    private void SetGlobalVariables()
    {
        Global.Instance.CanvasGame = CanvasGame;
        Global.Instance.RestartWindow = RestartWindow;
        Global.Instance.ExitWindow = ExitWindow;
        Global.Instance.ConfettiGroup = ConfettiGroup;
        Global.Instance.DonateWindow = DonateWindow;
        Global.Instance.HintWindow = HintWindow;
        Global.Instance.PurchasedWindow = PurchasedWindow;
        Global.Instance.SettingsScreens = SettingsScreens;
        Global.Instance.BlockUIPanel = BlockUIPanel;
        Global.Instance.LevelsScreen = LevelsScreen;
    }

    public void DOT_AnimateButtonGroup(float duration)
    {
        var ButtonGroupRect = ButtonGroup.transform as RectTransform;

        ButtonGroupRect.DOAnchorPos(new Vector2(ButtonGroupRect.localPosition.x, 140f), duration);
    }

    private void CreateGrid()
    {
        for (int row = 0; row < GameLevel.rows; row++)
        {
            for (int elem = 0; elem < GameLevel.elements; elem++)
            {
                GameObject thisCell;
                thisCell = Instantiate(Cell, transform);
                cells.Add(thisCell);
                thisCell.transform.localScale = new Vector3(cellScale, cellScale, cellScale);
                thisCell.transform.rotation = Quaternion.identity;
                thisCell.name = $"Cell {row}, {elem}";

                var thisRectCell = thisCell.transform as RectTransform;
                thisRectCell.sizeDelta = new Vector2(cellWidth, cellHeight);

                if (elem == 0)
                    SetCellSize(thisCell);

                
                thisRectCell.anchoredPosition = new Vector2(startPos.x + elem * (cellWidth + offset), startPos.y + row * (cellHeight + offset));

                SetNumberData(thisCell, (GameLevel.rows-1)-row, elem);

                if (elem == 0)
                {
                    GameLevel.cellWidth = thisRectCell.rect.width;
                    GameLevel.cellHeight = thisRectCell.rect.height;
                    GameLevel.width = (GameLevel.elements - 1) * offset + GameLevel.elements * GameLevel.cellWidth;
                    GameLevel.height = (GameLevel.rows - 1) * offset + GameLevel.rows * GameLevel.cellHeight;
                }
            }
        }
    }

    private void SetCellSize(GameObject _cell)
    {
        var thisRectCell = _cell.transform as RectTransform;
        float maxAmount = GetMaxValue(GameLevel.rows, GameLevel.elements);
        float outerGridSize = maxAmount * thisRectCell.rect.width + (maxAmount - 1) * offset;

        if (outerGridSize > maxWidthHeight)
        {
            cellWidth = Mathf.FloorToInt( (maxWidthHeight - ((maxAmount - 1) * offset)) / maxAmount );
            cellHeight = cellWidth;
        }

        thisRectCell.sizeDelta = new Vector2(cellWidth, cellHeight);
    }

    private void ChangeGridPosition()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-GameLevel.width / 2 + GameLevel.cellWidth / 2, -GameLevel.height / 2 + GameLevel.cellHeight / 2);
    }

    private void ChangeBgGridPosition()
    {
        var BgGridRect = BgGrid.transform as RectTransform;
        BgGridRect.sizeDelta = new Vector2(GameLevel.width + bgOffset, GameLevel.height + bgOffset);
    }

    private void SetNumberData(GameObject cell, int row, int elem)
    {
        string textValue = GameLevel.grid[row, elem];
        var cellComponent = cell.GetComponent<Cell>();
        int mVal = 0;

        cellComponent.a = row;
        cellComponent.b = elem;

        /*  Types of numbers:
         * 1) empty - "0"
         * 2) block - "block"
         * 3) simple - 1, 2...
         * 4) double - 1*, 2*...
         * 5) additional - 1', 2'...
         */

        string GetRangeOfString(string str, int from, int to)
        {
            string result = "";

            for (int i = 0; i < str.Length; i++)
            {
                if (i >= from && i <= to)
                {
                    result = result + str[i];
                }
            }

            return result;
        }


        if (textValue == "b")
        {
            cellComponent.type = "block";
            cell.GetComponent<Image>().color = BlockColor;
            return;
        }

        if (textValue[0].ToString() == "m")
        {
            mVal = 1;
            cellComponent.isHighlighted = true;
        }

        if (textValue[textValue.Length - 1].ToString() == "0")
        {
            cellComponent.type = "empty";
        }
        else if (textValue[textValue.Length - 1].ToString() == "*")
        {
            int numVal = Int32.Parse(GetRangeOfString(textValue, mVal, textValue.Length - 2));
            string textVal = GetRangeOfString(textValue, mVal, textValue.Length - 1);

            cellComponent.type = "double";
            cellComponent.number = numVal;
            cellComponent.SetDisplayText(textVal);
            cellComponent.interactable = true;
        }
        else if (textValue[textValue.Length - 1].ToString() == "'")
        {
            int numVal = Int32.Parse(GetRangeOfString(textValue, mVal, textValue.Length - 2));
            string textVal = GetRangeOfString(textValue, mVal, textValue.Length - 1);

            cellComponent.type = "additional";
            cellComponent.number = numVal;
            cellComponent.SetDisplayText(textVal);
            cellComponent.interactable = true;
        }
        else
        {
            string textVal = GetRangeOfString(textValue, mVal, textValue.Length - 1);

            cellComponent.type = "simple";
            cellComponent.number = Int32.Parse(textVal);
            cellComponent.SetDisplayText(textVal);
            cellComponent.interactable = true;
        }
    }

    private float GetMaxValue(float num1, float num2)
    {
        if (num1 > num2)
        {
            return num1;
        }
        else
        {
            return num2;
        }
    }

    public static void LevelPassed(bool reloadScene)
    {
        List<string[,]> levelStep = new List<string[,]>();

        ES3.Save("toSaveLevelStep", levelStep);

        int currentLevel = 1;

        if (ES3.KeyExists("toSaveCurrentLevel"))
        {
            currentLevel = ES3.Load<int>("toSaveCurrentLevel");
        }

        if (ES3.KeyExists("toSaveLevelDataList"))
        {
            List<LevelData> levelDataList = ES3.Load<List<LevelData>>("toSaveLevelDataList");
            levelDataList[currentLevel].IsUnlocked = true;
            ES3.Save("toSaveLevelDataList", levelDataList);
        }

        if (currentLevel != 100)
        {
            ES3.Save("toSaveCurrentLevel", currentLevel + 1);
        }
        
        Debug.Log("Level's passed!");

        if (reloadScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    } // SAVE DATA

    private void CheckForLevelComplete()
    {
        if (ES3.KeyExists("toSaveLevelStep") == false)
            return;

        List<string[,]> levelStep = ES3.Load<List<string[,]>>("toSaveLevelStep");

        if (levelStep.Count > 0)
        {
            int count = 0;

           foreach (String str in levelStep[levelStep.Count - 1])
           {
                if (str == "0")
                    continue;

                if (str == "b")
                    continue;

                if (str == "m0")
                    continue;

                count++;
           }

            if (count <= 1)
            {
                LevelPassed(false);
            }

        }
    } // SAVE DATA

    public struct Level
    {
        public int rows;
        public int elements;
        public int currentLevel;
        public string[,] grid;
        public float width;
        public float height;
        public float cellWidth;
        public float cellHeight;

        /*
        * 0 - empty cell
        * 1, 2, 3... - simple nums
        * b - block cell
        * 1*, 2*, 4*... - multiply or divide num cell
        * 1', 2', 3'... - add or subtract num cell
        * m1, m2... - hint num
        */

        public Level(int levelCount) : this()
        {
            this.currentLevel = levelCount;

            if (levelCount == 1)
            {
                this.rows = 2;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"0", "m2"},
                    {"1", "m1"}
                };
            }
            else if (levelCount == 2)
            {
                this.rows = 3;
                this.elements = 3;
                this.grid = new string[,]
                {
                    {"0", "m2", "2"},
                    {"0", "m2", "0"},
                    {"1", "1", "0"}
                };
            }
            else if (levelCount == 3)
            {
                this.rows = 3;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"m4", "1", "0", "0"},
                    {"m2", "1", "2", "4"},
                    {"0", "2", "0", "0"}
                };
            }
            else if (levelCount == 4)
            {
                this.rows = 3;
                this.elements = 3;
                this.grid = new string[,]
                {
                    {"1", "m2", "4"},
                    {"1", "m1", "4"},
                    {"1", "2", "0"}
                };
            }
            else if (levelCount == 5)
            {
                this.rows = 4;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"1", "1", "2", "0"},
                    {"1", "1", "1", "1"},
                    {"0", "m4", "m2", "0"},
                    {"0", "0", "1", "1"}
                };
            }
            else if (levelCount == 6)
            {
                this.rows = 4;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"0", "2", "m2", "0"},
                    {"0", "1", "2", "1"},
                    {"0", "2", "0", "b"},
                    {"1", "1", "m2", "2"}
                };
            }
            else if (levelCount == 7)
            {
                this.rows = 3;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"1", "1", "2", "1"},
                    {"1", "2", "m1", "1"},
                    {"0", "2", "m2", "2"}
                };
            }
            else if (levelCount == 8)
            {
                this.rows = 4;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"2", "1", "2", "0"},
                    {"1", "0", "b", "1"},
                    {"2", "0", "2", "0"},
                    {"m1", "1", "m2", "1"}
                };
            }
            else if (levelCount == 9)
            {
                this.rows = 4;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"4", "0", "0", "0"},
                    {"m1", "2", "1", "2"},
                    {"m2", "1", "0", "0"},
                    {"1", "1", "1", "0"}
                };
            }
            else if (levelCount == 10)
            {
                this.rows = 4;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"m2", "0", "4", "0"},
                    {"1", "1", "0", "0"},
                    {"2", "0", "b", "2"},
                    {"m1", "1", "1", "1"}
                };
            }
            else if (levelCount == 11)
            {
                this.rows = 4;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0", "0", "m2", "1", "0", "1"},
                    {"1", "0", "0", "1", "0", "1"},
                    {"1", "b", "1", "1", "b", "2"},
                    {"2", "0", "m2", "0", "0", "0"}
                };
            }
            else if (levelCount == 12)
            {
                this.rows = 4;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0", "2", "0", "m1", "2", "m1"},
                    {"0", "0", "0", "0", "0", "1"},
                    {"0", "1", "0", "1", "0", "1"},
                    {"1", "1", "b", "2", "0", "2"}
                };
            }
            else if (levelCount == 13)
            {
                this.rows = 5;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"0", "1", "1", "0", "0"},
                    {"1", "1", "0", "b", "1"},
                    {"1", "m2", "0", "2", "m1"},
                    {"0", "0", "0", "b", "1"},
                    {"0", "0", "1", "2", "1"}
                };
            }
            else if (levelCount == 14)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"m1", "2", "1", "1", "1", "m2"},
                    {"0", "b", "b", "0", "0", "0"},
                    {"1", "0", "1", "0", "0", "1"},
                    {"0", "1", "0", "0", "0", "1"},
                    {"1", "0", "1", "0", "1", "0"}
                };
            }
            else if (levelCount == 15)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0", "m2", "0", "0", "m2", "1"},
                    {"0", "1", "0", "0", "0", "1"},
                    {"1", "1", "0", "b", "1", "0"},
                    {"0", "2", "b", "0", "1", "0"},
                    {"0", "1", "0", "0", "1", "1"}
                };
            }
            else if (levelCount == 16)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1", "1", "1", "1", "0", "m1"},
                    {"1", "0", "0", "0", "1", "1"},
                    {"b", "0", "0", "0", "0", "0"},
                    {"0", "0", "0", "1", "0", "0"},
                    {"1", "2", "0", "1", "1", "m2"}
                };
            }
            else if (levelCount == 17)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"2", "1", "m1", "m2", "1", "1"},
                    {"0", "0", "0", "0", "b", "0"},
                    {"0", "0", "1", "2", "1", "0"},
                    {"1", "0", "0", "0", "1", "0"},
                    {"0", "1", "0", "0", "0", "1"}
                };
            }
            else if (levelCount == 18)
            {
                this.rows = 5;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0", "2", "1", "0", "1", "m1", "0"},
                    {"1", "0", "0", "0", "0", "1", "0"},
                    {"0", "0", "0", "0", "b", "0", "0"},
                    {"1", "1", "0", "b", "1", "0", "1"},
                    {"0", "0", "1", "0", "0", "m2", "2"}
                };
            }
            else if (levelCount == 19)
            {
                this.rows = 6;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"2", "0", "1", "b", "1"},
                    {"0", "1", "0", "0", "0"},
                    {"0", "1", "0", "0", "1"},
                    {"1", "1", "0", "0", "0"},
                    {"1", "0", "1", "0", "1"},
                    {"m1", "0", "0", "1", "m2"}
                };
            }
            else if (levelCount == 20)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1", "1", "0", "m2", "0", "0"},
                    {"1", "0", "0", "1", "0", "1"},
                    {"0", "0", "1", "1", "b", "1"},
                    {"0", "b", "0", "0", "0", "0"},
                    {"1", "1", "1", "m1", "1", "1"}
                };
            }
            else if (levelCount == 21)
            {
                this.rows = 6;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1", "0", "1", "0", "1", "m2"},
                    {"1", "0", "0", "0", "0", "0"},
                    {"0", "1", "0", "0", "0", "1"},
                    {"1", "0", "0", "0", "0", "1"},
                    {"b", "0", "1", "b", "1", "0"},
                    {"1", "1", "0", "1", "0", "m1"}
                };
            }
            else if (levelCount == 22)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"m2", "0", "0", "1", "1", "1"},
                    {"0", "0", "1", "0", "0", "b"},
                    {"1", "0", "0", "0", "0", "1"},
                    {"0", "1", "0", "0", "0", "1"},
                    {"m1", "1", "1", "1", "1", "1"}
                };
            }
            else if (levelCount == 23)
            {
                this.rows = 5;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"1", "0", "0", "1", "m1", "0", "1"},
                    {"1", "0", "0", "0", "0", "1", "b"},
                    {"b", "0", "0", "0", "2", "0", "0"},
                    {"1", "0", "b", "0", "1", "0", "1"},
                    {"1", "1", "0", "0", "m1", "1", "1"}
                };
            }
            else if (levelCount == 24)
            {
                this.rows = 8;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"1", "b", "0", "2"},
                    {"0", "0", "1", "0"},
                    {"1", "1", "0", "m1"},
                    {"1", "0", "1", "m1"},
                    {"0", "0", "0", "1"},
                    {"1", "0", "0", "0"},
                    {"b", "0", "1", "1"},
                    {"1", "0", "0", "1"}
                };
            }
            else if (levelCount == 25)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"m1", "1", "0", "1", "0", "m1"},
                    {"1", "0", "1", "0", "0", "0"},
                    {"0", "b", "0", "0", "0", "2*"},
                    {"0", "0", "0", "1", "0", "0"},
                    {"2", "1", "1", "b", "2", "1"}
                };
            }
            else if (levelCount == 26)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1", "1", "1", "m2", "1", "m1"},
                    {"0", "0", "0", "0", "0", "1"},
                    {"0", "0", "0", "0", "0", "2"},
                    {"1", "0", "2*", "0", "0", "0"},
                    {"0", "1", "0", "0", "1*", "1"}
                };
            }
            else if (levelCount == 27)
            {
                this.rows = 6;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1", "1*", "1", "0", "0", "0"},
                    {"0", "0", "1", "0", "0", "1"},
                    {"m1", "0", "1", "1", "0", "m1"},
                    {"1", "0", "0", "0", "2*", "0"},
                    {"0", "1", "b", "0", "0", "1"},
                    {"0", "0", "0", "0", "1", "1"}
                };
            }
            else if (levelCount == 28)
            {
                this.rows = 5;
                this.elements = 4;
                this.grid = new string[,]
                {
                    {"m1", "1", "1", "m1"},
                    {"1", "0", "1*", "1"},
                    {"2", "0", "0", "4*"},
                    {"0", "0", "0", "1"},
                    {"0", "0", "1", "1"}
                };
            }
            else if (levelCount == 29)
            {
                this.rows = 4;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"m1", "1", "1", "1*", "1", "0"},
                    {"0", "0", "b", "0", "0", "0"},
                    {"m2*", "0", "0", "4*", "0", "1"},
                    {"1", "1", "1", "0", "0", "1"}
                };
            }
            else if (levelCount == 30)
            {
                this.rows = 4;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"1","1","m1","0","1","m1","1","1"},
                    {"1","0","0","2*","0","0","0","1"},
                    {"0","0","0","1","0","1","0","0"},
                    {"0","0","1","1*","0","1","0","0"}
                };
            }
            else if (levelCount == 31)
            {
                this.rows = 6;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1","0","1","0","m1","2"},
                    {"0","0","b","0","0","0"},
                    {"1*","0","1","0","0","0"},
                    {"1","0","0","0","0","1"},
                    {"1*","1*","0","0","m2*","0"},
                    {"1","0","0","b","1","1"}
                };
            }
            else if (levelCount == 32)
            {
                this.rows = 5;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"1","0","0","1*","m1*"},
                    {"1","1","0","0","0"},
                    {"0","1","1","0","m1"},
                    {"1","b","2*","1*","1"},
                    {"1","b","1","0","1"}
                };
            }
            else if (levelCount == 33)
            {
                this.rows = 6;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0","0","0","1","0","1"},
                    {"1*","0","0","0","1*","0"},
                    {"0","0","0","b","0","0"},
                    {"1","0","1*","2*","1*","0"},
                    {"0","0","1","0","0","1"},
                    {"m1*","1","1","1","0","m1"}
                };
            }
            else if (levelCount == 34)
            {
                this.rows = 5;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0","1","1","1","0","0"},
                    {"1","2*","0","1","0","0"},
                    {"0","0","b","0","0","0"},
                    {"2*","0","1","0","b","1"},
                    {"1","1","m1","m1","0","1"}
                };
            }
            else if (levelCount == 35)
            {
                this.rows = 6;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"m1","1*","0","0","0","0","1"},
                    {"1","0","1*","0","1","b","1"},
                    {"m1","0","0","1","1","0","0"},
                    {"0","0","0","b","0","0","0"},
                    {"1","0","0","1","0","0","1"},
                    {"0","1","1","0","1","0","0"}
                };
            }
            else if (levelCount == 36)
            {
                this.rows = 8;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"1","b","1","0","0"},
                    {"0","1","1*","0","0"},
                    {"0","0","0","1","1"},
                    {"m1*","0","1*","0","m1*"},
                    {"2*","0","0","1","0"},
                    {"0","0","0","b","0"},
                    {"1","0","0","1","0"},
                    {"0","1","0","0","1"}
                };
            }
            else if (levelCount == 37)
            {
                this.rows = 6;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"m1","0","1*","1","0","2"},
                    {"0","0","0","0","0","b"},
                    {"0","0","0","b","1","1"},
                    {"m1*","1","0","1","1","0"},
                    {"1*","0","0","0","0","1*"},
                    {"0","1","1","1","0","0"}
                };
            }
            else if (levelCount == 38)
            {
                this.rows = 6;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0","0","1*","0","1*","b","1*"},
                    {"1","b","1","0","0","0","0"},
                    {"1","b","0","0","1","0","0"},
                    {"1","0","1","b","1","0","0"},
                    {"m1*","0","m2*","0","0","0","1*"},
                    {"1*","0","0","0","1*","0","0"}
                };
            }
            else if (levelCount == 39)
            {
                this.rows = 5;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"m1*", "0", "0", "1*", "0", "1*","m1*"},
                    {"b", "0", "1", "b", "1", "b", "0"},
                    {"0", "1", "0", "0", "0", "1", "1*"},
                    {"1", "0", "0", "0", "1*", "0", "1*"},
                    {"1", "b", "1", "0", "0", "1", "1*"}
                };
            }
            else if (levelCount == 40)
            {
                this.rows = 5;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"1","0","1","m1","1","0","0"},
                    {"1","1*","0","1*","0","0","1"},
                    {"0","1*","0","0","0","1","0"},
                    {"0","0","1","m1*","0","0","1"},
                    {"1","0","0","0","1*","1","0"}
                };
            }
            else if (levelCount == 41)
            {
                this.rows = 7;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0","1*","0","1","0","0","0"},
                    {"0","0","1","b","1","1","0"},
                    {"m1","0","0","m1*","0","0","1"},
                    {"0","0","0","0","b","0","b"},
                    {"0","1*","0","1*","0","0","0"},
                    {"0","0","0","1*","1","1*","0"},
                    {"0","0","1","1*","0","0","1*"}
                };
            }
            else if (levelCount == 42)
            {
                this.rows = 7;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0","1","b","1","0","0"},
                    {"1*","0","1","0","0","1"},
                    {"0","0","b","0","0","0"},
                    {"2","0","1","0","0","1"},
                    {"0","m1*","1*","m1*","0","0"},
                    {"1*","0","1*","0","0","0"},
                    {"0","0","1*","0","0","1*"}
                };
            }
            else if (levelCount == 43)
            {
                this.rows = 5;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0","1","b","1","0","0","1"},
                    {"1","0","m1*","0","b","1","1*"},
                    {"b","1","0","1","0","0","0"},
                    {"0","0","m1*","0","1","0","0"},
                    {"1","1*","1*","1*","0","0","1*"}
                };
            }
            else if (levelCount == 44)
            {
                this.rows = 6;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"1","1","b","1","0","1","0"},
                    {"0","0","0","1","0","0","0"},
                    {"1*","0","1*","0","b","0","1"},
                    {"0","0","1","0","0","1*","1*"},
                    {"0","1","0","1","0","0","0"},
                    {"m1","0","0","1*","0","0","m1*"}
                };
            }
            else if (levelCount == 45)
            {
                this.rows = 5;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"1","1","b","1","1"},
                    {"0","1","1","0","0"},
                    {"m1*","0","2'","0","m1*"},
                    {"1","b","1","0","0"},
                    {"1","0","b","1","2*"}
                };
            }
            else if (levelCount == 46)
            {
                this.rows = 6;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"1'","0","0","0","0","1","0"},
                    {"1","0","1","m1","1","0","0"},
                    {"b","1","0","0","0","0","1"},
                    {"1","0","1","0","0","1","0"},
                    {"0","1'","0","m1","0","0","1"},
                    {"1*","0","0","0","0","0","1'"}
                };
            }
            else if (levelCount == 47)
            {
                this.rows = 5;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0", "1", "1", "0", "1", "0", "2*"},
                    {"0", "0", "b", "1", "0", "1*", "0"},
                    {"m1'", "0", "1", "0", "m1'", "0", "0"},
                    {"0", "0", "1*", "0", "0", "0", "1*"},
                    {"1*", "1", "1*", "0", "0", "1*", "0"},
                };
            }
            else if (levelCount == 48)
            {
                this.rows = 7;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"0","0","1","0","0"},
                    {"0","1*","0","1*","0"},
                    {"1","0","m1'","m1*","0"},
                    {"b","2'","0","0","1"},
                    {"0","0","0","0","b"},
                    {"1","0","1","0","1'"},
                    {"1","1*","0","0","1"}
                };
            }
            else if (levelCount == 49)
            {
                this.rows = 8;
                this.elements = 5;
                this.grid = new string[,]
                {
                    {"1","1'","1","0","0"},
                    {"1'","0","1","0","0"},
                    {"0","0","b","0","0"},
                    {"0","1","1","1","0"},
                    {"0","0","0","1","0"},
                    {"m1","0","0","0","m1"},
                    {"0","0","1","0","1'"},
                    {"0","1","0","1'","1"}
                };
            }
            else if (levelCount == 50)
            {
                this.rows = 7;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"0","0","1'","0","1","0"},
                    {"1","b","m1","1","1'","0"},
                    {"0","0","0","b","0","0"},
                    {"1","0","0","0","0","1"},
                    {"0","0","m1","1'","0","1"},
                    {"1'","0","0","1","0","1*"},
                    {"0","0","0","0","1'","1"}
                };
            }
            else if (levelCount == 51)
            {
                this.rows = 7;
                this.elements = 6;
                this.grid = new string[,]
                {
                    {"1","1'","0","0","0","0"},
                    {"0","0","1","0","0","1"},
                    {"0","0","1","0","1*","0"},
                    {"m1*","1","0","1","m1*","0"},
                    {"1","0","1","0","0","0"},
                    {"0","1","1","0","0","0"},
                    {"0","0","0","0","1","1'"}
                };
            }
            else if (levelCount == 52)
            {
                this.rows = 6;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0","0","0","1","0","1","0"},
                    {"1","0","1","0","0","0","0"},
                    {"1","0","0","m1'","0","0","1"},
                    {"0","1","0","1","b","1","1"},
                    {"0","b","0","0","0","0","0"},
                    {"1","1","1","m1'","0","1","0"}
                };
            }
            else if (levelCount == 53)
            {
                this.rows = 6;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"1","b","1","0","1","0","0","0"},
                    {"0","0","0","1*","0","1*","0","0"},
                    {"0","0","0","m1'","1*","0","1*","m1'"},
                    {"1","0","0","0","0","1*","0","0"},
                    {"0","0","1*","0","0","1*","0","0"},
                    {"1*","0","0","0","1","0","1*","0"}
                };
            }
            else if (levelCount == 54)
            {
                this.rows = 7;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0","0","0","1","0","1*","0"},
                    {"0","1","b","1","1","0","1*"},
                    {"1","0","0","0","1","0","0"},
                    {"0","0","1","0","0","0","0"},
                    {"1'","m1*","0","0","0","1*","1*"},
                    {"1*","0","1*","0","0","0","0"},
                    {"0","m1'","0","0","0","0","0"}
                };
            }
            else if (levelCount == 55)
            {
                this.rows = 8;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"1","0","1","1","0","0","0"},
                    {"0","0","b","0","0","0","0"},
                    {"1","0","1'","0","0","0","0"},
                    {"1","0","0","0","0","0","1"},
                    {"0","1","1*","0","1","b","1"},
                    {"0","0","0","1","0","0","0"},
                    {"0","m1'","1","1*","m1'","0","1"},
                    {"1","0","0","0","1","0","0"}
                };
            }
            else if (levelCount == 56)
            {
                this.rows = 6;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"0","1","1","2","0","1*","0","1"},
                    {"1","0","0","0","0","0","0","1"},
                    {"0","1","b","1","b","m1'","b","1"},
                    {"1'","0","0","0","0","1","0","0"},
                    {"0","1'","0","0","0","m1*","0","1"},
                    {"0","0","0","1","0","2","0","0"}
                };
            }
            else if (levelCount == 57)
            {
                this.rows = 8;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"m1'","1","1","0","0","0","1","m1'"},
                    {"1","0","1*","0","0","0","0","1"},
                    {"0","0","0","0","2*","1","0","0"},
                    {"0","1","0","1","0","0","0","0"},
                    {"1","1","0","0","1","0","1","0"},
                    {"0","0","1*","0","0","0","1","0"},
                    {"0","0","b","0","0","0","0","0"},
                    {"1*","0","1","1","b","1'","0","1"}
                };
            }
            else if (levelCount == 58)
            {
                this.rows = 8;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"0","0","1","0","1*","0","1"},
                    {"0","0","0","0","0","1","1"},
                    {"1","1","1","0","0","0","1"},
                    {"0","0","0","0","0","0","b"},
                    {"0","0","m1'","0","1*","0","1"},
                    {"0","0","0","0","0","0","b"},
                    {"1","0","m1'","b","1","0","1"},
                    {"0","1","1*","0","0","1*","1"}
                };
            }
            else if (levelCount == 59)
            {
                this.rows = 7;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"m1'","0","0","2","0","0","m1'","1"},
                    {"1","1","0","0","1","1","0","0"},
                    {"0","0","0","0","0","0","b","0"},
                    {"0","1","1","1'","0","0","2*","0"},
                    {"0","0","1","1","1*","0","0","0"},
                    {"0","1","0","0","0","0","2*","1"},
                    {"1","0","0","0","0","0","1","0"}
                };
            }
            else if (levelCount == 60)
            {
                this.rows = 8;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"1","0","0","0","1","1","0","0","1"},
                    {"0","1","1","0","0","0","1","0","0"},
                    {"0","1*","0","1*","b","1*","0","1*","0"},
                    {"1","0","m1'","0","1","0","1","0","1"},
                    {"0","b","1","0","0","0","1","0","0"},
                    {"0","1","1","0","b","0","1","0","0"},
                    {"0","1","0","0","0","0","1","0","1"},
                    {"1","b","m1'","1*","0","0","0","1*","0"}
                };
            }
            else if (levelCount == 61)
            {
                this.rows = 6;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"1","1*","0","0","0","1","0","0","1"},
                    {"0","1","b","m1'","1","0","0","0","m2"},
                    {"0","0","1","0","0","0","0","1","0"},
                    {"1","0","1","1","0","1","b","0","1"},
                    {"0","b","1","0","1","0","0","0","1"},
                    {"1","1","0","0","0","0","0","1'","1"}
                };
            }
            else if (levelCount == 62)
            {
                this.rows = 8;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"0","0","1","0","0","1","1","0"},
                    {"1'","0","0","0","1","1","0","0"},
                    {"1","0","0","0","0","0","0","1"},
                    {"1","b","m1'","0","1","1","0","2"},
                    {"b","1","0","0","0","0","0","1"},
                    {"1","b","m1'","2","0","0","0","2"},
                    {"1","0","1","0","0","0","1","0"},
                    {"1","0","0","2*","1","1","0","0"}
                };
            }
            else if (levelCount == 63)
            {
                this.rows = 7;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","0","1","0","0","0","0","0","0","0"},
                    {"0","1","m1","0","0","4*","0","0","0","m1'"},
                    {"0","0","0","1","0","1","0","0","0","1"},
                    {"1","0","2","0","1","1","b","1","0","0"},
                    {"0","1*","0","1","0","0","1","1*","0","1*"},
                    {"1","0","0","0","1","1","0","0","0","0"},
                    {"0","1","0","0","0","1","0","0","b","1"}
                };
            }
            else if (levelCount == 64)
            {
                this.rows = 8;
                this.elements = 8;
                this.grid = new string[,]
                {
                    {"1","0","0","1","0","1","0","0"},
                    {"0","2","0","m1'","0","0","2","1"},
                    {"1","0","1","0","1","0","0","1"},
                    {"b","1","0","0","0","1","1","0"},
                    {"1","0","0","0","0","0","1","1"},
                    {"0","1","b","m1'","0","1*","0","0"},
                    {"1","0","1","1","1'","0","0","0"},
                    {"0","0","1","0","0","0","1","0"}
                };
            }
            else if (levelCount == 65)
            {
                this.rows = 6;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"1","1","0","1","1","m4","0","0","0","0"},
                    {"0","0","1","b","1","0","1","1","0","1"},
                    {"1","2","0","1","0","0","0","0","1","0"},
                    {"0","0","0","1","0","m1","1","0","0","2"},
                    {"0","0","1","0","1","0","b","1","0","1"},
                    {"1","0","1","0","0","2","0","0","0","1"}
                };
            }
            else if (levelCount == 66)
            {
                this.rows = 9;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"0","0","2","0","1","2","0","0","0"},
                    {"0","1","0","1","0","1","1","0","1"},
                    {"0","0","0","0","1","b","0","0","0"},
                    {"0","1","m2","1","0","0","0","1","0"},
                    {"0","b","0","0","1","0","0","0","0"},
                    {"0","0","1","0","0","1","0","1","0"},
                    {"0","1","1","1","0","0","0","0","0"},
                    {"1","0","m1","0","0","2","0","0","2"},
                    {"0","0","2","0","1","0","0","0","0"}
                };
            }
            else if (levelCount == 67)
            {
                this.rows = 7;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"1","1","0","0","1","0","2","0","0","0"},
                    {"0","1","1","0","0","0","1","0","4","0"},
                    {"0","b","0","1","0","0","1","0","0","1"},
                    {"0","1","1","0","1","1","b","0","0","0"},
                    {"0","0","0","0","0","1","0","0","1","0"},
                    {"0","1","0","1","m1","0","0","1","m2","1"},
                    {"1","0","0","0","1","0","1","0","0","1"}
                };
            }
            else if (levelCount == 68)
            {
                this.rows = 7;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"m1","2","1","0","0","0","0","2","1","m1"},
                    {"1","0","0","0","0","b","1","0","1","1"},
                    {"0","0","0","1","0","0","1","0","0","0"},
                    {"0","1","0","0","0","0","0","0","1","0"},
                    {"2","b","1","0","1","0","0","0","0","1"},
                    {"1","0","0","1","1","0","0","0","0","0"},
                    {"2","0","1","1","0","b","1","0","1","2"}
                };
            }
            else if (levelCount == 69)
            {
                this.rows = 10;
                this.elements = 7;
                this.grid = new string[,]
                {
                    {"1","1","2","1","b","0","0"},
                    {"1","0","0","0","0","0","0"},
                    {"1","1","0","0","0","1","0"},
                    {"0","0","0","0","0","1","0"},
                    {"m2","0","2","0","1","m2","0"},
                    {"0","0","0","0","0","0","1"},
                    {"1","0","1","0","1","0","0"},
                    {"0","1","0","0","0","1","0"},
                    {"1","0","0","1","b","0","0"},
                    {"2","0","b","1","2","1","1"}
                };
            }
            else if (levelCount == 70)
            {
                this.rows = 7;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","2","0","0","0","2","0","0","0","1"},
                    {"1","m1","1","0","0","0","0","0","1","m1"},
                    {"1","0","0","0","0","1","1","0","1","2"},
                    {"0","1","0","1","b","2","0","1","0","1"},
                    {"0","0","0","1","0","0","0","0","0","1"},
                    {"0","1","0","1","b","0","1","0","0","1"},
                    {"0","2","1","0","0","1","0","0","0","0"}
                };
            }
            else if (levelCount == 71)
            {
                this.rows = 8;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"1","0","1","0","1","0","0","0","0"},
                    {"b","1","2","0","m1","1","0","0","2"},
                    {"1","0","0","0","0","1","2","b","1"},
                    {"0","1","b","0","0","0","1","1","0"},
                    {"1","0","0","0","1","0","0","2","0"},
                    {"0","0","0","2","m1","0","1","0","1"},
                    {"0","0","0","0","0","0","b","0","0"},
                    {"0","1","0","0","1","1","1","b","1"}
                };
            }
            else if (levelCount == 72)
            {
                this.rows = 9;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","1","0","0","0","0","1","0","0","0"},
                    {"1","0","0","0","0","0","1","0","0","0"},
                    {"1","1","0","1","2","b","1","0","1","1"},
                    {"0","0","0","0","0","1","0","0","0","1"},
                    {"0","1","0","0","0","1","0","1","0","b"},
                    {"1","0","1","0","m1","0","m1","1","0","1"},
                    {"b","0","0","1","0","b","1","0","0","0"},
                    {"2","0","0","0","1","0","0","0","0","1"},
                    {"1","0","0","0","1","0","1","0","0","0"}
                };
            }
            else if (levelCount == 73)
            {
                this.rows = 8;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"0","0","0","0","b","1","b","0","1","0","1"},
                    {"1","1","1","0","0","0","0","1","0","0","0"},
                    {"1","0","0","0","1","0","0","0","0","0","0"},
                    {"1","0","0","1","0","1","0","m1","2","b","2"},
                    {"b","0","0","0","0","0","0","0","b","0","0"},
                    {"2","0","0","0","0","1","0","m2","1","1","1"},
                    {"0","1","1","0","0","0","0","1","0","0","0"},
                    {"0","1","0","0","0","1","0","0","0","1","1"}
                };
            }
            else if (levelCount == 74)
            {
                this.rows = 9;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","1","0","0","1","0","0","0","0","0"},
                    {"0","1","1","b","1","1","0","1","0","0"},
                    {"0","0","0","0","1","0","0","0","0","0"},
                    {"1","0","0","0","0","1","0","1","1","0"},
                    {"0","0","0","1","0","0","1","0","0","0"},
                    {"1","0","1","0","m4","0","m1","0","1","1"},
                    {"0","0","0","0","0","0","1","0","0","1"},
                    {"0","0","0","1","0","0","2","0","0","0"},
                    {"1","1","0","0","0","0","1","1","0","0"}
                };
            }
            else if (levelCount == 75)
            {
                this.rows = 10;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"0","1","0","0","0","1","2","0","0"},
                    {"0","0","0","0","0","0","0","0","1"},
                    {"1","1","1","0","1","0","m2","0","0"},
                    {"0","0","0","1","b","0","1","0","1"},
                    {"0","0","1","0","1","0","0","0","0"},
                    {"0","1","0","0","0","0","1","0","0"},
                    {"1","b","0","1","1","0","m2","1","1"},
                    {"0","0","1","0","0","1","0","0","0"},
                    {"1","0","0","0","2","0","0","0","0"},
                    {"1","b","0","0","0","0","0","1","0"}
                };
            }
            else if (levelCount == 76)
            {
                this.rows = 7;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","1","0","0","0","0","0","0","0","0"},
                    {"0","m1*","0","1*","2","0","0","0","2","0"},
                    {"1","1","b","1*","0","1","1","0","0","2"},
                    {"b","m1","0","0","4*","0","0","2","0","0"},
                    {"1","0","0","0","1","0","b","1","1","0"},
                    {"0","1","1","b","1","0","0","1","0","1"},
                    {"0","1","0","0","0","0","0","0","0","1"}
                };
            }
            else if (levelCount == 77)
            {
                this.rows = 9;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"0","0","0","0","1","0","1","1","0"},
                    {"1","1","0","1","0","2","0","m1","1"},
                    {"0","0","0","0","0","b","1","1","0"},
                    {"1","0","1*","0","0","1","b","2","0"},
                    {"b","0","0","1","0","0","0","0","0"},
                    {"2","0","1","0","0","1","0","m1","0"},
                    {"0","1","0","0","0","0","1","0","0"},
                    {"0","0","2","b","1","1","0","1","0"},
                    {"1","0","0","0","0","0","0","0","1"}
                };
            }
            else if (levelCount == 78)
            {
                this.rows = 8;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"1","1","2","0","0","0","m2","1","1","0","0"},
                    {"1","0","0","0","0","0","1","0","0","0","0"},
                    {"0","1","b","1","1","0","1*","0","0","0","1"},
                    {"0","0","1","0","0","0","1","0","0","1","0"},
                    {"1","0","0","1","b","1","m2","0","0","1","2"},
                    {"0","0","0","0","0","0","0","0","1","1","0"},
                    {"0","0","0","0","0","1","0","1","0","0","0"},
                    {"0","0","1","0","0","1","0","0","0","0","0"}
                };
            }
            else if (levelCount == 79)
            {
                this.rows = 10;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"1","0","0","0","1*","0","0","0","1*"},
                    {"2","0","1","1*","m1*","b","2","1","0"},
                    {"0","1","1","0","0","0","0","1","0"},
                    {"0","1","0","1","2","0","0","0","0"},
                    {"0","0","1","b","1","0","0","0","0"},
                    {"1","0","0","0","0","0","1","0","0"},
                    {"b","b","b","0","0","0","0","0","0"},
                    {"0","1*","0","0","0","0","0","0","1*"},
                    {"2","0","0","1","m1*","0","1","0","0"},
                    {"0","1","1","0","1","0","0","0","0"}
                };
            }
            else if (levelCount == 80)
            {
                this.rows = 10;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"0","2","0","0","0","0","0","1","0"},
                    {"0","0","0","0","0","2","b","1","0"},
                    {"1","0","1","0","0","0","0","0","1"},
                    {"0","1","0","0","1*","1","0","0","b"},
                    {"1","1","0","0","0","m1","0","1*","1"},
                    {"0","0","1","0","1*","1","0","0","0"},
                    {"1","b","1","0","0","m2","b","2","0"},
                    {"0","1","0","0","0","1","0","1","0"},
                    {"0","0","1","b","1","0","0","0","0"},
                    {"0","0","0","0","1","0","0","0","0"}
                };
            }
            else if (levelCount == 81)
            {
                this.rows = 9;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","0","1","b","0","0","1","0","0","1"},
                    {"0","2","1","1","b","1","0","0","2","0"},
                    {"0","1","0","0","0","0","1*","1","0","0"},
                    {"1","0","m1","0","1","1","0","0","m2","1"},
                    {"0","0","b","0","0","0","0","0","0","0"},
                    {"0","0","0","0","0","0","0","1","0","1"},
                    {"0","0","0","0","0","0","0","1","0","0"},
                    {"0","1","0","1'","0","1","0","0","1","0"},
                    {"1","0","0","1","0","0","b","1","2","0"}
                };
            }
            else if (levelCount == 82)
            {
                this.rows = 9;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"0","1","0","0","2","0","0","0","0","0","0"},
                    {"0","1*","0","0","0","1","0","0","0","0","0"},
                    {"1","0","1","0","0","0","0","0","0","0","0"},
                    {"1","1","0","0","1","m2","0","0","0","0","0"},
                    {"0","0","1'","0","0","0","0","0","0","0","1"},
                    {"2","b","0","2","2*","m1","0","1","1","1","1"},
                    {"0","0","1","0","0","1","0","0","0","0","0"},
                    {"0","0","0","0","0","0","1","1","0","0","0"},
                    {"1","0","0","1","b","1","0","0","0","0","0"}
                };
            }
            else if (levelCount == 83)
            {
                this.rows = 8;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","2*","1","b","0","0","0","1","0","0"},
                    {"0","1","0","b","0","1","0","1*","0","1"},
                    {"1","m1*","0","0","0","0","0","1","0","1"},
                    {"1","0","0","b","0","1","0","1","0","0"},
                    {"0","2","0","0","1","0","b","2","0","0"},
                    {"0","0","0","0","0","1","0","0","0","1*"},
                    {"2","m1","0","1*","0","1","0","0","b","1"},
                    {"1","0","0","1","1","0","0","1","0","0"}
                };
            }
            else if (levelCount == 84)
            {
                this.rows = 8;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"1","0","0","0","0","0","0","1","0","2","0"},
                    {"1","b","1","0","1","0","1","0","0","m1","0"},
                    {"0","0","0","1","0","0","0","0","0","2*","1"},
                    {"0","2","1*","0","1*","0","0","0","0","m1","1"},
                    {"0","0","0","1","2","b","1","0","0","0","1"},
                    {"1","1'","0","0","0","0","0","1","0","0","0"},
                    {"0","0","1","b","1","0","0","0","1","0","0"},
                    {"0","1","0","0","0","0","0","0","1","0","0"}
                };
            }
            else if (levelCount == 85)
            {
                this.rows = 10;
                this.elements = 9;
                this.grid = new string[,]
                {
                    {"0","1","0","1","1'","0","0","1","0"},
                    {"0","0","0","0","0","0","0","b","0"},
                    {"0","0","1","0","0","1","1","1","0"},
                    {"0","b","b","0","0","0","0","0","0"},
                    {"0","1","0","m4*","0","0","0","m1","2"},
                    {"1","0","0","0","1","0","0","0","0"},
                    {"0","0","1","1*","0","1","0","0","0"},
                    {"0","0","1","0","0","b","1*","1","1"},
                    {"1","0","0","1","b","1","0","1","0"},
                    {"1","0","0","0","0","0","0","2","0"}
                };
            }
            else if (levelCount == 86)
            {
                this.rows = 10;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","1","0","0","1","b","1","0","0","1"},
                    {"2","0","1","1","0","0","0","0","0","0"},
                    {"0","0","b","1","0","1","0","0","0","0"},
                    {"0","1","1","0","0","0","0","1","1","0"},
                    {"0","1","1*","m1","0","0","1","0","m1","0"},
                    {"0","0","0","2","0","0","0","0","0","0"},
                    {"1","0","0","0","0","0","0","0","1*","1*"},
                    {"b","0","0","0","0","0","0","0","0","0"},
                    {"1","0","1*","b","1","0","0","0","2","0"},
                    {"1","0","0","0","1","0","0","0","0","1"}
                };
            }
            else if (levelCount == 87)
            {
                this.rows = 10;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","0","0","0","0","2","0","0","0","0"},
                    {"0","1","1","0","0","0","0","0","0","1"},
                    {"0","0","0","0","1","0","b","0","1","0"},
                    {"0","0","0","0","1","0","0","1","0","0"},
                    {"0","1","0","0","1","0","0","0","0","0"},
                    {"2","0","1","b","0","1","1","0","0","0"},
                    {"0","1","1*","0","0","1","0","0","0","0"},
                    {"0","b","0","1","1","1","0","0","0","0"},
                    {"0","1","m2*","0","0","m1","0","1","1","1"},
                    {"0","0","1*","0","1*","0","b","0","0","1"}
                };
            }
            else if (levelCount == 88)
            {
                this.rows = 10;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"0","0","1","1","0","0","2*","0","0","0"},
                    {"0","0","0","0","1","0","0","0","0","1"},
                    {"1","1","0","1","0","0","1","1","m1*","0"},
                    {"0","1","0","0","0","0","0","0","0","0"},
                    {"0","0","0","0","1'","0","0","0","1*","0"},
                    {"0","0","0","0","0","1","0","0","0","1"},
                    {"1","0","0","0","1","0","b","2*","0","0"},
                    {"1","0","0","2*","b","1","0","0","0","0"},
                    {"0","1","1","0","0","1","0","1","m1","0"},
                    {"0","0","0","0","0","0","0","1","0","1"}
                };
            }
            else if (levelCount == 89)
            {
                this.rows = 9;
                this.elements = 10;
                this.grid = new string[,]
                {
                    {"2","1*","0","0","1","0","0","1*","0","0"},
                    {"0","1*","0","0","m1*","1","2","0","0","0"},
                    {"1","0","0","0","1*","0","0","0","1","0"},
                    {"0","b","1","1'","m2","b","0","1","0","0"},
                    {"0","0","1'","0","0","1","0","0","0","1"},
                    {"b","0","0","0","1","0","0","0","1","0"},
                    {"1","0","0","1","0","0","0","1","1","0"},
                    {"0","0","0","1","0","1","0","b","0","0"},
                    {"0","0","1","0","0","0","0","1","0","1"}
                };
            }
            else if (levelCount == 90)
            {
                this.rows = 8;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"0","1","0","0","0","0","1","b","1","0","0","0"},
                    {"0","0","1","b","0","0","1","0","0","0","0","1"},
                    {"0","1","0","0","0","0","0","0","1*","0","1","1"},
                    {"1","0","0","0","0","1","m1*","0","m1*","b","1","0"},
                    {"0","1","0","1","b","0","0","0","2","1","0","0"},
                    {"0","0","1","1","1","0","0","0","1","0","0","0"},
                    {"1","b","0","0","0","0","0","0","0","1","0","1"},
                    {"1","0","1","0","0","b","2","0","0","0","1","0"}
                };
            }
            else if (levelCount == 91)
            {
                this.rows = 11;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"1","0","1","0","0","1","0","0","0","0","1"},
                    {"0","0","0","0","0","0","1","0","0","0","0"},
                    {"0","1","0","0","1","0","0","1","0","0","0"},
                    {"0","0","m1'","0","2","0","0","0","0","0","0"},
                    {"0","b","0","0","0","b","0","0","0","0","0"},
                    {"1","0","2","0","0","0","0","1","0","0","0"},
                    {"b","1","0","1","1","0","0","0","0","1","0"},
                    {"1","0","m1*","0","0","1","0","0","2","0","1"},
                    {"0","0","2","0","0","0","1","b","0","0","0"},
                    {"0","1","0","0","0","0","0","0","1","0","0"},
                    {"1","0","0","0","0","0","0","1","0","0","1"}
                };
            }
            else if (levelCount == 92)
            {
                this.rows = 10;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"2*","1","0","0","0","1","0","0","0","0","1","0"},
                    {"0","0","1","1","b","1","0","1","1","0","0","1"},
                    {"0","1","0","0","0","0","0","0","0","1","0","0"},
                    {"0","0","0","0","0","0","0","0","0","0","0","1"},
                    {"0","0","0","1","2","m1'","0","2*","0","0","0","0"},
                    {"0","0","0","0","0","0","0","1","0","0","0","1"},
                    {"0","0","1","0","0","1","0","0","2*","0","0","0"},
                    {"1","1","0","0","0","1","0","0","0","2*","0","0"},
                    {"0","1","0","0","0","0","0","0","b","0","0","0"},
                    {"0","0","0","1","1","m1*","0","0","1","0","1*","0"}
                };
            }
            else if (levelCount == 93)
            {
                this.rows = 11;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"1","0","0","0","0","0","1","1","0","0","1"},
                    {"0","4*","0","0","1*","0","0","0","0","1","0"},
                    {"0","0","1","b","0","1","0","0","0","0","0"},
                    {"0","0","0","1","0","0","0","0","0","0","0"},
                    {"0","0","0","0","1","1","0","0","0","1","0"},
                    {"0","1","0","0","0","0","0","0","0","0","1"},
                    {"0","0","1*","0","0","m1*","0","0","0","2","0"},
                    {"0","0","1","0","0","0","0","0","0","0","0"},
                    {"1","1","0","1*","0","m1'","0","0","0","0","0"},
                    {"0","0","2","b","0","2","0","1","b","1","0"},
                    {"0","1","0","0","0","2","1","0","0","0","1"}
                };
            }
            else if (levelCount == 94)
            {
                this.rows = 11;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"0","0","1*","0","0","0","0","1*","0","b","1","2"},
                    {"0","0","0","0","1","2","0","0","0","0","0","0"},
                    {"0","1","0","0","0","0","1","0","0","0","0","0"},
                    {"1","0","0","0","0","0","0","0","0","0","0","1*"},
                    {"0","0","0","0","0","0","0","1","1","0","0","0"},
                    {"b","0","0","0","0","1","0","0","0","0","0","1*"},
                    {"1","0","1","b","1","0","0","0","0","0","0","0"},
                    {"0","1","0","0","0","1*","0","0","0","1","0","1*"},
                    {"2","0","m1'","0","0","m1'","0","0","0","0","0","0"},
                    {"0","b","1*","0","0","0","1*","0","1*","0","1","0"},
                    {"1","1","0","0","0","0","0","0","0","0","0","1"}
                };
            }
            else if (levelCount == 95)
            {
                this.rows = 10;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"1","0","0","0","0","1","b","1","0","1","b","1"},
                    {"b","1","0","0","0","0","0","0","0","0","0","1"},
                    {"1","0","2*","b","0","1","0","0","b","1*","0","0"},
                    {"2","0","0","0","0","1*","0","2","0","0","0","1"},
                    {"b","2","0","0","0","0","0","0","0","m1'","1*","0"},
                    {"1","0","0","0","0","0","0","0","0","0","1","0"},
                    {"0","0","0","1","0","0","0","1","0","0","0","b"},
                    {"0","0","0","0","0","2","0","1","0","m1'","0","0"},
                    {"0","1","0","0","0","1","0","0","0","1","0","1"},
                    {"0","1","1","0","b","1","0","1","0","2","0","0"}
                };
            }
            else if (levelCount == 96)
            {
                this.rows = 12;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"1","0","0","1*","b","1'","0","0","2","0","1"},
                    {"1","b","1","1","0","0","0","0","1","0","0"},
                    {"b","0","0","m1'","0","1","m1*","0","0","0","0"},
                    {"1'","0","0","1","0","0","0","0","0","2","0"},
                    {"0","0","1","2","0","2","0","b","2*","0","1"},
                    {"1","0","0","0","0","0","0","0","1","0","0"},
                    {"0","0","0","2","0","b","0","0","0","0","0"},
                    {"0","0","0","0","0","1","0","0","2*","1","0"},
                    {"1","0","1","0","0","0","0","0","0","b","0"},
                    {"0","0","0","0","0","0","0","0","4","0","0"},
                    {"0","1","0","1","0","0","0","0","2","b","1"},
                    {"0","0","0","0","0","0","1*","0","1*","0","0"}
                };
            }
            else if (levelCount == 97)
            {
                this.rows = 12;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"1","0","0","0","0","0","0","0","0","0","0","1"},
                    {"0","1","0","0","0","0","0","0","0","0","0","2"},
                    {"0","1","b","1","0","2","0","1","1","1","0","0"},
                    {"0","0","1","1","b","2","0","0","0","0","0","0"},
                    {"0","b","0","b","0","0","0","0","0","0","1","0"},
                    {"0","0","0","1*","1","m1'","0","0","0","0","0","m1*"},
                    {"0","1","0","0","0","0","1","1","0","0","1","0"},
                    {"1","0","0","2*","0","0","1*","0","0","0","0","0"},
                    {"0","0","1","0","0","0","0","0","0","0","0","1"},
                    {"0","0","0","0","2","0","0","0","0","0","1","0"},
                    {"0","0","0","0","0","0","0","0","1","0","0","1"},
                    {"1","1","0","0","0","2","b","2","0","1","0","0"}
                };
            }
            else if (levelCount == 98)
            {
                this.rows = 12;
                this.elements = 11;
                this.grid = new string[,]
                {
                    {"1","1","1*","0","1","b","1","0","b","1","1"},
                    {"0","b","0","0","1","0","0","1","0","0","0"},
                    {"1","0","1*","1","0","0","0","0","1","0","0"},
                    {"0","0","0","0","b","0","0","0","0","0","1"},
                    {"0","0","1","0","0","0","1","0","0","m2","0"},
                    {"0","0","0","0","0","1","0","0","1","0","0"},
                    {"0","0","2","0","1","0","0","0","0","1","1"},
                    {"0","2","1","0","0","0","0","0","0","2","0"},
                    {"1","1","0","0","1","0","0","0","0","1","0"},
                    {"b","0","0","2","0","0","0","0","0","1*","0"},
                    {"1","1","b","0","0","0","0","1*","1","m1'","0"},
                    {"1","0","1","0","0","0","0","0","0","1","0"}
                };
            }
            else if (levelCount == 99)
            {
                this.rows = 12;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"1","2","m1*","1","1*","0","1","0","1","1","0","1*"},
                    {"0","0","0","0","0","0","1","1","0","0","0","0"},
                    {"0","0","0","1","2","b","1","0","0","0","0","0"},
                    {"0","1","0","0","0","0","0","0","0","0","0","0"},
                    {"1","0","0","0","0","0","0","0","0","0","0","0"},
                    {"b","1'","0","0","0","1","1","0","0","0","1*","0"},
                    {"2","1","0","0","0","0","0","0","b","1","0","1"},
                    {"0","b","0","0","0","0","0","0","0","0","1","0"},
                    {"0","1","0","0","1","0","0","b","1","0","1","1"},
                    {"1","0","m1'","0","0","0","0","0","1*","0","b","0"},
                    {"0","0","1","0","0","0","0","0","0","0","1","1"},
                    {"0","1","0","0","0","0","0","0","0","0","0","1"}
                };
            }
            else if (levelCount == 100)
            {
                this.rows = 12;
                this.elements = 12;
                this.grid = new string[,]
                {
                    {"0","1*","0","0","0","0","0","0","0","1","1","0"},
                    {"1","0","0","0","0","0","1","0","b","0","0","2"},
                    {"0","0","0","2","0","1*","0","1*","1","0","0","1*"},
                    {"0","1","0","0","0","0","0","1","0","0","0","2"},
                    {"2*","0","1","0","0","0","0","0","0","1","1","0"},
                    {"0","b","0","1","0","0","0","0","0","0","0","1"},
                    {"0","1","0","0","b","1","0","0","2","0","0","0"},
                    {"0","0","0","0","1","0","0","b","0","0","0","0"},
                    {"1","1*","0","0","0","0","m1'","0","1","1*","0","m1*"},
                    {"0","0","0","1","0","0","0","0","0","0","1","0"},
                    {"2","0","0","0","1","0","1","0","0","0","0","2"},
                    {"0","1*","0","0","0","0","0","2","b","2","0","1"}
                };
            }
            else
            {
                this.rows = 0;
                this.elements = 0;
                this.grid = new string[,] {};
            }
        }
    }
}
