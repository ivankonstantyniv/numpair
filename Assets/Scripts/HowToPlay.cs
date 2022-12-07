using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public static Steps Step;
    public GameObject Cell;
    public GameObject BgGrid;
    public static List<GameObject> Cells = new List<GameObject>();
    public float cellScale = 1.0f;
    public float cellWidth = 100.0f;
    public float cellHeight = 100.0f;
    public float offset = 0.0f;
    public float bgOffset = 0.0f;
    public float maxWidthHeight;
    public Vector2 startPos = new Vector2(0.0f, 0.0f);
    public Color BlockColor;

    void Start()
    {
        Cells.Clear();
        Step = new Steps(Global.Instance.HTP_StepCountGrid);
        maxWidthHeight = 600.0f + 5 * offset;

        CreateGrid();
        ChangeGridPosition();
        ChangeBgGridPosition();
    }

    private void CreateGrid()
    {
        for (int row = 0; row < Step.rows; row++)
        {
            for (int elem = 0; elem < Step.elements; elem++)
            {
                GameObject htpCell;
                htpCell = Instantiate(Cell) as GameObject;
                htpCell.transform.SetParent(transform);
                Cells.Add(htpCell);
                htpCell.transform.localScale = new Vector3(cellScale, cellScale, cellScale);

                var thisRectCell = htpCell.transform as RectTransform;
                thisRectCell.sizeDelta = new Vector2(cellWidth, cellHeight);

                if (elem == 0)
                    SetCellSize(htpCell);

                thisRectCell.anchoredPosition = new Vector2(startPos.x + elem * (thisRectCell.rect.width + offset), startPos.y + row * (thisRectCell.rect.height + offset));

                SetNumberData(htpCell, (Step.rows - 1) - row, elem);

                if (elem == 0)
                {
                    Step.cellWidth = thisRectCell.rect.width;
                    Step.cellHeight = thisRectCell.rect.height;
                    Step.width = (Step.elements - 1) * offset + Step.elements * Step.cellWidth;
                    Step.height = (Step.rows - 1) * offset + Step.rows * Step.cellHeight;
                }
            }
        }
    }

    private void ChangeGridPosition()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(-Step.width / 2 + Step.cellWidth / 2, -Step.height / 2 + Step.cellHeight / 2);
    }

    private void ChangeBgGridPosition()
    {
        var BgGridRect = BgGrid.transform as RectTransform;
        BgGridRect.sizeDelta = new Vector2(Step.width + bgOffset, Step.height + bgOffset);
        BgGridRect.anchoredPosition = new Vector2(BgGridRect.anchoredPosition.x, startPos.y);
    }

    private void SetCellSize(GameObject _cell)
    {
        var thisRectCell = _cell.transform as RectTransform;
        float maxAmount = GetMaxValue(Step.rows, Step.elements);
        float outerGridSize = maxAmount * thisRectCell.rect.width + (maxAmount - 1) * offset;

        if (outerGridSize > maxWidthHeight)
        {
            cellWidth = (maxWidthHeight - ((maxAmount - 1) * offset)) / maxAmount;
            cellHeight = cellWidth;
        }

        thisRectCell.sizeDelta = new Vector2(cellWidth, cellHeight);
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

    private void SetNumberData(GameObject cell, int row, int elem)
    {
        string textValue = Step.grid[row, elem];
        var cellComponent = cell.GetComponent<HTP_Cell>();
        int textLen = textValue.Length;
        int mVal = 0;

        cellComponent.a = row;
        cellComponent.b = elem;
        cellComponent.interactable = false;
        cellComponent.canInteract = true;

        if (textValue == "b")
        {
            cellComponent.type = "block";
            cell.GetComponent<Image>().color = BlockColor;
            return;
        }

        if (textValue[0].ToString() == "m")
        {
            mVal = 1;
            cellComponent.isInteractable = false;
        }

        if (textLen == 1 + mVal && Int32.Parse(textValue[0 + mVal].ToString()) == 0)
        {
            cellComponent.type = "empty";
            return;
        }
        else if (textLen == 1 + mVal && Int32.Parse(textValue[0 + mVal].ToString()) > 0)
        {
            string textVal = textValue[0 + mVal].ToString();

            cellComponent.type = "simple";
            cellComponent.number = Int32.Parse(textValue[0 + mVal].ToString());
            cellComponent.SetDisplayText(textVal);
        }
        else if (textLen == 2 + mVal && textValue[1 + mVal].ToString() == "*")
        {
            int numVal = Int32.Parse(textValue[0 + mVal].ToString());
            string textVal = textValue[0 + mVal].ToString() + textValue[1 + mVal].ToString();

            cellComponent.type = "double";
            cellComponent.number = numVal;
            cellComponent.SetDisplayText(textVal);
        }
        else if (textLen == 2 + mVal && textValue[1 + mVal].ToString() == "'")
        {
            int numVal = Int32.Parse(textValue[0 + mVal].ToString());
            string textVal = textValue[0 + mVal].ToString() + textValue[1 + mVal].ToString();

            cellComponent.type = "additional";
            cellComponent.number = numVal;
            cellComponent.SetDisplayText(textVal);
        }

    }

    public void SetCellInteractable(bool value, int a, int b)
    {
        foreach (GameObject cell in Cells)
        {
            var cellComp = cell.GetComponent<HTP_Cell>();

            if (cellComp.a == a && cellComp.b == b)
                cellComp.canInteract = value;
        }
    }

    public struct Steps
    {
        public int rows;
        public int elements;
        public string[,] grid;
        public float width;
        public float height;
        public float cellWidth;
        public float cellHeight;
        public int currentStep;

        public Steps(int step) : this()
        {
            this.currentStep = step;

            if (step == 1)
            {
                this.rows = 2;
                this.elements = 3;
                this.grid = new string[,]
                {
                    {"m1", "m2", "m4"},
                    {"1", "0", "0"},
                };
            }
            else if (step == 2)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"1", "m1*"}
                };
            }
            else if (step == 3)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"1*", "m1"}
                };
            }
            else if (step == 4)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"1*", "m1*"}
                };
            }
            else if (step == 5)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"2*", "m1*"}
                };
            }
            else if (step == 6)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"2", "m2*"}
                };
            }
            else if (step == 7)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"2*", "m2*"}
                };
            }
            else if (step == 8)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"4*", "m2"}
                };
            }
            else if (step == 9)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"4*", "m2*"}
                };
            }
            else if (step == 10)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"2'", "m1"}
                };
            }
            else if (step == 11)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"2'", "m1*"}
                };
            }
            else if (step == 12)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"2'", "m2*"}
                };
            }
            else if (step == 13)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"1", "m2'"}
                };
            }
            else if (step == 14)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"4*", "m4'"}
                };
            }
            else if (step == 15)
            {
                this.rows = 1;
                this.elements = 2;
                this.grid = new string[,]
                {
                    {"3'", "m2'"}
                };
            }
        }
    }
}
