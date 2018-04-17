using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Player class
/// </summary>
public class PlayerScript : NetworkBehaviour
{
    public string playerName { get; private set; }
    public NetworkIdentity identity { get; private set; }
    public CellBehaviour.CellValue usedCellValue = CellBehaviour.CellValue.Zero;
    FieldManager manager;
    public CellBehaviour.CellValue UsedCellValue
    {
        get
        {
            return usedCellValue;
        }
        set
        {
            if (value == CellBehaviour.CellValue.Zero)
                throw new ArgumentException("Zero value not allowed for player");
            usedCellValue = value;
        }
    }

    void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
        manager = FindObjectOfType<FieldManager>();
        manager.OnCellClicked += CellClicked; //player input handle
        playerName = PlayerPrefs.GetString("name");
    }

    public void CellClicked(byte position)
    {
        if (isLocalPlayer)
            CmdChooseCell(position);
    }

    [Command]
    void CmdChooseCell(byte position)
    {
        manager.SetMove(new PlayerMove(position, UsedCellValue));
    }
}
