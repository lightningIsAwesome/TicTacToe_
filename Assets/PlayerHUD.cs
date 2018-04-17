using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// HUD that shows time left for turn, status and game over message
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Server server;
    [SerializeField] GameDataMessage gameOverMessage;
    [SerializeField] Text status;
    [SerializeField] Text timeLeft;
    [SerializeField] Text waitingForOtherPlayer;

    void Awake()
    {
        server.OnGameOver += CreateGameOverMessage;
        server.OnGameStarted += ShowGameHUD;
        timeLeft.gameObject.SetActive(false);
        status.gameObject.SetActive(false);
        enabled = false;
    }

    void CreateGameOverMessage(GameData data)
    {
        var message = Instantiate(gameOverMessage, transform);
        message.Init(data);
    }

    void ShowGameHUD()
    {
        enabled = true;
        waitingForOtherPlayer.gameObject.SetActive(false);
        timeLeft.gameObject.SetActive(true);
        status.gameObject.SetActive(true);

    }

    void Update()
    {
        timeLeft.text = server.TimeLeft.ToString();
    }
}
