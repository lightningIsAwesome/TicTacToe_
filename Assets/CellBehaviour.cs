using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Game field cell
/// </summary>
[RequireComponent(typeof(Button))]
public class CellBehaviour : MonoBehaviour
{
    public enum CellValue
    {
        None,
        Cross,
        Zero
    }
    public byte position { get; private set; }
    public Button button { get; private set; }
    public CellValue currentValue;
    public event Action<byte> OnClicked;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (OnClicked != null)
            OnClicked(position);
    }

    public void Init(byte position)
    {
        this.position = position;
    }
}
