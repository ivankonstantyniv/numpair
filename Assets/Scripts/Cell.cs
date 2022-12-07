using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Cell : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    public Text numText;
    public bool isHighlighted = false;
    public int number;
    public bool isFocus = false;
    public bool isTarget = false;
    private bool isFading = false;
    public string type;
    public int a, b;
    public Color targetColor, focusColor, clearColor;
    public GameObject Frame;


    private new void Start()
    {
        numText.fontSize = (int)GetComponent<RectTransform>().rect.width / 2;
    }

    IEnumerator StartNewLevelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        LevelGrid.LevelPassed(true);
    }

    public void DOT_FadeOutColor(Text text, float duration)
    {
        isFading = true;

        void onComplete()
        {
            isFading = false;
        }

        text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 0f), duration).OnComplete(onComplete);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (type == "empty" || type == "block")
            return;

        if (isTarget)
        {
            PairCells();
            return;
        }

        CheckForFocus();

        if (isFocus)
        {
            GlobalSounds.Instance.PlaySound("choose");

            CheckForTarget();
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {

    }

    public void SetDisplayText(string value)
    {
        numText.text = value;
    }

    public void CheckForFocus()
    {
        foreach (GameObject cell in LevelGrid.cells)
        {
            var cellComponent = cell.GetComponent<Cell>();

            cellComponent.isTarget = false;

            if (cellComponent.type == "empty" || cellComponent.type == "block")
                continue;

            if (cellComponent.a == a && cellComponent.b == b)
            {
                if (cellComponent.isFocus)
                {
                    cellComponent.isFocus = false;
                    cellComponent.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    cellComponent.isFocus = true;
                    cellComponent.GetComponent<Image>().color = focusColor;
                }
            }
            else
            {
                cellComponent.isFocus = false;
                cellComponent.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void CheckForTarget()
    {
        List<int[]> targetCells = new List<int[]>();

        //Left Direction
        for (int i = b-1; i >= 0; i--)
        {
            var thisElem = LevelGrid.GameLevel.grid[a, i];

            if (thisElem == "b")
                break;

            if (thisElem == "0" || thisElem == "m0")
                continue;

            int[] cellCoords = new int[] { a, i };
            targetCells.Add(cellCoords);

            break;

        }

        //Right Direction
        for (int i = b+1; i < LevelGrid.GameLevel.elements; i++)
        {
            var thisElem = LevelGrid.GameLevel.grid[a, i];

            if (thisElem == "b")
                break;

            if (thisElem == "0" || thisElem == "m0")
                continue;

            int[] cellCoords = new int[] { a, i };
            targetCells.Add(cellCoords);

            break;
        }

        //Up Direction
        for (int i = a - 1; i >= 0; i--)
        {
            var thisElem = LevelGrid.GameLevel.grid[i, b];

            if (thisElem == "b")
                break;

            if (thisElem == "0" || thisElem == "m0")
                continue;

            int[] cellCoords = new int[] { i, b };
            targetCells.Add(cellCoords);

            break;
        }

        //Down Direction
        for (int i = a + 1; i < LevelGrid.GameLevel.rows; i++)
        {
            var thisElem = LevelGrid.GameLevel.grid[i, b];

            if (thisElem == "b")
                break;

            if (thisElem == "0" || thisElem == "m0")
                continue;

            int[] cellCoords = new int[] { i, b };
            targetCells.Add(cellCoords);

            break;
        }

        foreach (GameObject cell in LevelGrid.cells)
        {
            var cellComponent = cell.GetComponent<Cell>();

            foreach (int[] cellCoords in targetCells)
            {
                if (cellComponent.a == cellCoords[0] && cellComponent.b == cellCoords[1])
                {
                    CompareCells(cell);
                }
            }  
        }
    }

    public void MakeCellTarget(GameObject _targetCell)
    {
        _targetCell.GetComponent<Image>().color = targetColor;
        _targetCell.GetComponent<Cell>().isTarget = true;
    }

    public void CompareCells(GameObject targetCell)
    {
        var targetCellComp = targetCell.GetComponent<Cell>();

        if (targetCellComp == null)
            return;

        if (type == "simple") //FOR "SIMPLE" TYPE
        {
            if (targetCellComp.type == "simple" && targetCellComp.number == number)
            {
                MakeCellTarget(targetCell);
            }

            if (targetCellComp.type == "additional")
            {
                MakeCellTarget(targetCell);
            }

            if ((targetCellComp.type == "double") && targetCellComp.number == number)
            {
                MakeCellTarget(targetCell);
            }
        } else if (type == "double") //FOR "DOUBLE" TYPE
        {
            if (targetCellComp.type == "simple" && (targetCellComp.number == number / 2 || (targetCellComp.number == 1 && targetCellComp.number == number) ) )
            {
                MakeCellTarget(targetCell);
            }

            if (targetCellComp.type == "additional")
            {
                MakeCellTarget(targetCell);
            }

            if ((targetCellComp.type == "double") && (targetCellComp.number == number || targetCellComp.number == number / 2) )
            {
                MakeCellTarget(targetCell);
            }
        }
        else if (type == "additional") //FOR "ADDITIONAL" TYPE
        {
            if ( (targetCellComp.type == "simple" || targetCellComp.type == "double") && number >= targetCellComp.number)
            {
                MakeCellTarget(targetCell);
            } else if (targetCellComp.type == "additional")
            {
                MakeCellTarget(targetCell);
            }
        }
    }

    public void PairCells()
    {
        foreach (GameObject cell in LevelGrid.cells)
        {
            var cellComponent = cell.GetComponent<Cell>();

            if (cellComponent.isFocus)
            {
                CalcCells(cell);
            }
        }
    }

    public void CalcCells(GameObject _cell)
    {
        var focusCell = _cell.GetComponent<Cell>();
        int focusCellNum = focusCell.number;

        if (focusCell.type == "simple") //SIMPLE & ADDITIONAL
        {
            if (type == "simple" || type == "double")
            {
                focusCell.number = focusCell.number - number;
                number = number + focusCellNum;
            } 
            else if (type == "additional")
            {
                focusCell.number = 0;
                number = number + focusCellNum;
            }
            
        }
        else if (focusCell.type == "double") //DOUBLE
        {
            if (focusCell.number == number && type != "additional")
            {
                focusCell.number = focusCell.number - number;
                number = number + focusCellNum;
            }
            else
            {
                focusCell.number = focusCell.number / 2;

                if (focusCell.number == 0)
                {
                    number = number + focusCellNum;
                }
                else
                {
                    number = number + focusCellNum / 2;
                }
            }
        }
        else if (focusCell.type == "additional")
        {
            if (type == "additional")
            {
                focusCell.number = 0;
                number = number + focusCellNum;

                //focusCell.number = focusCell.number - 1;
                //number = number + 1;
            }
            else
            {
                focusCell.number = focusCell.number - number;
                number = number + number;
            }
            
        }

        if (type == "simple")
        {
            SetDisplayText(number.ToString());
        }
        else if (type == "double")
        {
            SetDisplayText(number.ToString() + "*");
        }
        else if (type == "additional")
        {
            SetDisplayText(number.ToString() + "'");
        }

        if (focusCell.number <= 0)
        {
            focusCell.type = "empty";

            DOT_FadeOutColor(focusCell.numText, 0.25f);
        } else
        {
            if (focusCell.type == "simple")
            {
                focusCell.SetDisplayText(focusCell.number.ToString());
            }
            else if (focusCell.type == "double")
            {
                focusCell.SetDisplayText(focusCell.number.ToString() + "*");
            }
            else if (focusCell.type == "additional")
            {
                focusCell.SetDisplayText(focusCell.number.ToString() + "'");
            }
        }

        foreach (GameObject cell in LevelGrid.cells)
        {
            var cellComp = cell.GetComponent<Cell>();

            if (cellComp.isFocus)
            {
                UpdateLevelGrid(cell);
            }
        }

        foreach (GameObject cell in LevelGrid.cells)
        {
            var cellComp = cell.GetComponent<Cell>();

            if (cellComp.type == "block")
                continue;

            ClearCellStates(cell);
        }

        //CHECK FOR WIN
        SaveGridStep();
        CheckForWin();
    }

    public void ClearCellStates(GameObject _cell)
    {
        _cell.GetComponent<Image>().color = clearColor;
        _cell.GetComponent<Cell>().isFocus = false;
        _cell.GetComponent<Cell>().isTarget = false;
    }

    public void UpdateLevelGrid(GameObject _focusCell)
    {
        for (int row = 0; row < LevelGrid.GameLevel.rows; row++)
        {
            for (int elems = 0; elems < LevelGrid.GameLevel.elements; elems++)
            {
                var focusCellComp = _focusCell.GetComponent<Cell>();

                if (focusCellComp == null)
                    break;

                if (focusCellComp.a == row && focusCellComp.b == elems)
                {
                    string strFocus = "";

                    if (focusCellComp.isHighlighted)
                    {
                        strFocus = strFocus + "m";
                    }

                    if (focusCellComp.number <= 0)
                    {
                        LevelGrid.GameLevel.grid[row, elems] = strFocus + "0";
                    } else
                    {
                        if (focusCellComp.type == "simple")
                        {
                            strFocus = strFocus + focusCellComp.number.ToString();
                        }
                        else if (focusCellComp.type == "double")
                        {
                            strFocus = strFocus + focusCellComp.number.ToString() + "*";
                        }
                        else if (focusCellComp.type == "additional")
                        {
                            strFocus = strFocus + focusCellComp.number.ToString() + "'";
                        }

                        LevelGrid.GameLevel.grid[row, elems] = strFocus;
                    }
                }

                if (a == row && b == elems)
                {
                    var str = "";

                    if (isHighlighted)
                    {
                        str = str + "m";
                    }

                    if (type == "simple")
                    {
                        str = str + number.ToString();
                    }
                    else if (type == "double")
                    {
                        str = str + number.ToString() + "*";
                    }
                    else if (type == "additional")
                    {
                        str = str + number.ToString() + "'";
                    }

                    LevelGrid.GameLevel.grid[row, elems] = str;
                }
            }
        }

        
    }

    public void DebugGridElements()
    {
        for (int row = 0; row < LevelGrid.GameLevel.rows; row++)
        {
            string str = "";

            for (int elems = 0; elems < LevelGrid.GameLevel.elements; elems++)
            {
                str = str + " " + LevelGrid.GameLevel.grid[row, elems];
            }

            Debug.Log(str);
        }
    }

    public void CheckForWin()
    {
        int amountOfNumbers = 0;

        foreach (GameObject cell in LevelGrid.cells)
        {
            var cellComp = cell.GetComponent<Cell>();

            if (cellComp.number == 0)
                continue;

            amountOfNumbers = amountOfNumbers + 1;
        }

        if (amountOfNumbers <= 1)
        {
            Global.Instance.OnVibrate();
            GlobalSounds.Instance.PlaySound("win");

            StartCoroutine(FadeOutLastNum());
            Global.Instance.ConfettiGroup.SetActive(true);
            StartCoroutine(StartNewLevelWithDelay(2f));

            ES3.Save("toSaveIsAlreadyHintShown", false);
        }
        else
        {
            GlobalSounds.Instance.PlaySound("pair");
        }
    } // SAVE DATA

    public void SaveGridStep()
    {
        Debug.Log("Save step");

        List<string[,]> levelStep = ES3.Load<List<string[,]>>("toSaveLevelStep");
        levelStep.Add(LevelGrid.GameLevel.grid);
 
        ES3.Save("toSaveLevelStep", levelStep);
        Debug.Log("levelStep COunt : " + levelStep.Count);

    } // SAVE DATA

    IEnumerator FadeOutLastNum()
    {
        while (isFading)
        {
            yield return null;
        }

        DOT_FadeOutColor(numText, 0.25f);
    }
}


