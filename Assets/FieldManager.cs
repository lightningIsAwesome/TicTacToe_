using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Managed field of cells that can handle player pick 
/// </summary>
[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class FieldManager : NetworkBehaviour
{
    [SerializeField] GameObject field;
    [SerializeField] GameObject blocker;
    [SerializeField] Sprite O;
    [SerializeField] Sprite X;
    [SyncVar(hook = "MoveMade")]
    PlayerMove move;
    [SerializeField] CellBehaviour[] cells;
    public event Action<byte> OnCellClicked;//callback for one of the cells cliked
    public event Action OnChanged;
    /// <summary>
    /// Event that triggered when somebody win [true] or field become full [false]
    /// </summary>
    public event Action<bool> OnFieldResultReceived;
    public int pickedCellsCount;
    public bool IsBlocked
    {
        get
        {
            return blocker.activeSelf;
        }
        set
        {
            blocker.SetActive(value);
        }
    }

    void Awake()
    {
        byte position = 0;
        foreach (var cell in cells)
        {
            cell.Init(position++);
            cell.OnClicked += CellClicked;
        }
    }

    public void SetMove(PlayerMove move)
    {
        this.move = move;
    }

    void CellClicked(byte position)
    {
        if (OnCellClicked != null)
            OnCellClicked(position);
    }

    void MoveMade(PlayerMove move)
    {
        cells[move.pos].button.interactable = false;
        cells[move.pos].currentValue = move.value;
        var cellImage = cells[move.pos].button.targetGraphic.GetComponent<Image>();
        cellImage.sprite = move.value == CellBehaviour.CellValue.Cross ? X : O;
        pickedCellsCount++;
        if (OnChanged != null)
            OnChanged();
        CheckWin();
    }

    void CheckWin()
    {
        if (pickedCellsCount < 3)
            return;
        if (CheckLines(1, 3) || CheckLines(3, 1) || CheckRightDiagonal() || CheckLeftDiagonal())
        {
            if (OnFieldResultReceived != null)
                OnFieldResultReceived(true);
            return;
        }

        if (pickedCellsCount == 9)
        {
            if (OnFieldResultReceived != null)
                OnFieldResultReceived(false);
        }
    }

    bool CheckLines(int iStep, int jStep)
    {
        bool foundedEqauls;
        for (int i = 0; i < ((iStep * 2) + 1); i += iStep)
        {
            foundedEqauls = false;
            for (int j = 0; j < (jStep + 1); j += jStep)
            {
                if (cells[i + j].currentValue == CellBehaviour.CellValue.None)
                    break;
                if (cells[i + j].currentValue == cells[i + j + jStep].currentValue)
                {
                    if (foundedEqauls)
                        return true;
                    else
                        foundedEqauls = true;
                    continue;
                }
                else
                    break;

            }
        }
        return false;
    }

    bool CheckRightDiagonal()
    {
        int step = 2;
        bool foundedEqauls = false;
        for (int j = step; j < (step * 2) + 1; j += step)
        {
            if (cells[j].currentValue == CellBehaviour.CellValue.None)
                return false;
            if (cells[j].currentValue != cells[j + step].currentValue)
                return false;
            else
                if (foundedEqauls)
                    return true;
                else
                    foundedEqauls = true;
        }
        return false;
    }

    bool CheckLeftDiagonal()
    {
        int step = 4;
        bool foundedEqauls = false;
        for (int j = 0; j < (step + 1); j += step)
        {
            if (cells[j].currentValue == CellBehaviour.CellValue.None)
                return false;
            if (cells[j].currentValue != cells[j + step].currentValue)
                return false;
            else
            {
                if (foundedEqauls)
                    return true;
                else
                    foundedEqauls = true;
            }
        }
        return false;
    }
}
