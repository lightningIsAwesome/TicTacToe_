using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ShowStats : MonoBehaviour
{
    [SerializeField] ScrollRect scroll;
    [SerializeField] GameDataMessage message;

    void OnEnable()
    {
        for (int i = scroll.content.childCount; i < scroll.content.childCount; i++)
        {
            Destroy(scroll.content.GetChild(i));
        }

        foreach (var save in GameData.GetSaves())
        {
            Instantiate(message, scroll.content).Init(save);
        }
    }

}
