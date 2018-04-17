using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Server class manages game state on network
/// </summary>
public class Server : NetworkBehaviour
{
    public enum GameState
    {
        WaitForPlayers,
        Started,
        Over
    }

    [Tooltip("Maximum tim for player turn")]
    [Range(1f, 100f)]
    [SerializeField] float maxTurnTime = 30f;
    [SerializeField] FieldManager fieldManager;
    [SyncVar] float totalRoundTime;
    [SyncVar] GameState gameState;
    [SyncVar] float timeLeft;
    [SyncVar] bool pickMade; //user pick

    public event Action OnGameStarted;
    public event Action<GameData> OnGameOver;
    public PlayerScript Player { get; private set; }
    public string OpponentName { get; private set; }
    public GameState State
    {
        get
        {
            return gameState;
        }
    }
    public FieldManager GameFieldManager
    {
        get
        {
            return fieldManager;
        }
    }
    public float TimeLeft
    {
        get
        {
            return timeLeft;
        }
    }

    public override void OnStartServer()
    {
        Network.maxConnections = 2;
        fieldManager.OnChanged += () => pickMade = true;
        fieldManager.OnFieldResultReceived += HandleGameFinished;
    }

    public void ForceQuit(string message)
    {
        if (State != GameState.Over)
        {
            Debug.LogError(message);
            NetworkClient.ShutdownAll();
            SceneManager.LoadScene("menu");
        }
    }

    [ClientRpc]
    public void RpcGameStarted()
    {
        var playerController = NetworkManager.singleton.client.connection.playerControllers[0];
        if (playerController == null || !playerController.IsValid)
            throw new InvalidOperationException("No player found");
        Player = playerController.gameObject.GetComponent<PlayerScript>();
        if (OnGameStarted != null)
            OnGameStarted();
    }

    [ClientRpc]
    public void RpcGameOver(bool somebodyWin)
    {
        GameData.GameResult result;
        if (!somebodyWin)
            result = GameData.GameResult.Tie;
        else if (!GameFieldManager.IsBlocked)
            result = GameData.GameResult.Win;
        else
            result = GameData.GameResult.Lose;

        var data = new GameData(result, OpponentName, totalRoundTime);
        data.Save();

        GameFieldManager.IsBlocked = true;

        if (OnGameOver != null)
            OnGameOver(data);
    }

    public void EnabledFirstPlayer(NetworkConnection firstPlayerConnection)
    {
        if (!isServer)
            return;
        TargetStartGame(firstPlayerConnection);
        StartCoroutine(TurnCountDown());
        gameState = GameState.Started;
    }

    public void ClientDisconnect()
    {
        if (gameState != GameState.Over)
            new GameData(GameData.GameResult.Lose, OpponentName, totalRoundTime).Save();
    }

    [TargetRpc]
    public void TargetSetOpponent(NetworkConnection conn, string opponent)
    {
        OpponentName = opponent;
    }

    [TargetRpc]
    public void TargetStartGame(NetworkConnection conn)
    {
        fieldManager.IsBlocked = false; 
    }

    [ClientRpc]
    void RpcInvertInputBlock()
    {
        fieldManager.IsBlocked = !fieldManager.IsBlocked;
    }

    void HandleGameFinished(bool somebodyWin)
    {
        if (!isServer)
            return;
        StopAllCoroutines();
        RpcGameOver(somebodyWin);
        gameState = GameState.Over;
    }

    IEnumerator TurnCountDown()
    {
        float startTime = Time.time;
        var endTime = startTime + maxTurnTime;
        while (endTime > Time.time && !pickMade)
        {
            timeLeft = (int)(endTime - Time.time);
            yield return null;
        }
        totalRoundTime += Time.time - startTime;
        pickMade = false;
        StartCoroutine(TurnCountDown());
        RpcInvertInputBlock();
    }
}
