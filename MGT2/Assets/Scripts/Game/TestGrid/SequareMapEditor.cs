using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SequareMapEditor : MonoBehaviour
{
    public Color[] colors;
    public Toggle[] toggles;
    public SequareGrid hexGrid;

    public Toggle toggleColor;
    public Toggle toggleElevation;
    private Color activeColor;
    private int activeElevation;
    void Awake()
    {
        toggles[0].Select();
    }

    void Update()
    {
        HandleInputDown();
        HandleInputDrag();
    }


    private void HandleInputDown()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            SequareCell currentCell = GetTouchCell();
            if (currentCell != null)
            {
                EditorCell(currentCell);
            }
        }
    }




    private SequareCell GetTouchCell()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return hexGrid.GetCell(hit.point);
        }
        return null;
    }

    private void EditorCell(SequareCell cell)
    {
        if (toggleColor.isOn)
        {
            cell.SetColor(activeColor);
        }
        if (toggleElevation.isOn)
        {
            cell.SetElevation(activeElevation);
        }


    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }






    public void OnClickToggleIndex(int index)
    {
        SelectColor(index);
    }

    public TMP_Dropdown dropdown;
    private SequareCell preCell;
    bool isDrag;
    private void HandleInputDrag()
    {

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDrag = true;
        }
        else
        {
            preCell = null;
            isDrag = false;
        }
        SequareCell cell = GetTouchCell();
        if (cell == null)
        {
            return;
        }
        if (GetRiverMode() == EnumOptionalToggle.No)
        {
            cell.RemoveRiver();
        }
        else if (isDrag && GetRiverMode() == EnumOptionalToggle.Yes)
        {
            if (preCell == cell)
            {
                return;
            }
            if (preCell != null)
            {
                int dir = cell.GetNeighborDirection(preCell);
                if (dir == -1)
                {
                    return;
                }
                EnumDirection optDir = (EnumDirection)HexMetrics.Opposite(dir);
                preCell.SetOutgoingRiver(optDir);

            }
            preCell = cell;
        }


    }

    private EnumOptionalToggle GetRiverMode()
    {
        return (EnumOptionalToggle)dropdown.value;
    }



}
