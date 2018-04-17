using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameField : MonoBehaviour
{
    [SerializeField] Button apply;
    [SerializeField] Button startGame;
    InputField inpField;

    void Awake()
    {
        inpField = GetComponent<InputField>();
        if (PlayerPrefs.HasKey("name"))
            inpField.text = PlayerPrefs.GetString("name");
        else
            startGame.interactable = false;
        apply.interactable = false;
        apply.onClick.AddListener(ChangeUserName);
        inpField.onValueChanged.AddListener(ChangeInput);
    }

    void ChangeInput(string newValue)
    {
        if (newValue != string.Empty)
            apply.interactable = true;
        else
            apply.interactable = false;
    }

    void ChangeUserName()
    {
        if (inpField.text != string.Empty)
        {
            PlayerPrefs.SetString("name", inpField.text);
            PlayerPrefs.Save();
            startGame.interactable = true;
        }
    }
}
