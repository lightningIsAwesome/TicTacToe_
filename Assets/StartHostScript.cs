using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class StartHostScript : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        SceneManager.LoadScene("game");
    }

}
