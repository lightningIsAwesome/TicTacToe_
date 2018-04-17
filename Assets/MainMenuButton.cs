using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

[RequireComponent(typeof(Button))]
public class MainMenuButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ToMainMenu);
    }

    void ToMainMenu()
    {
        SceneManager.LoadScene("menu");
        NetworkManager.singleton.StopHost();
        NetworkManager.singleton.StopClient();
    }

}
