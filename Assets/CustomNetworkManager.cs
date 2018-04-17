using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] Server server;
    PlayerScript[] players;
    int playersConected; //amount of players on server

    void Start()
    {
        players = new PlayerScript[2];
        var host = StartHost(); //if host already started run as client
        if (host != null)
            return;
        StartClient();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (playerPrefab == null)
        {
            if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
            return;
        }

        if (playerPrefab.GetComponent<NetworkIdentity>() == null)
        {
            if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
            return;
        }
        players[playersConected] = Instantiate(playerPrefab).GetComponent<PlayerScript>(); ;
        NetworkServer.AddPlayerForConnection(conn, players[playersConected].gameObject, playerControllerId);
        if (++playersConected > 1)
            InitGame();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        server.ForceQuit("Server connection lost");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        server.ForceQuit("Client disconnected");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        ClientScene.Ready(conn);
        ClientScene.AddPlayer(0);
    }

    void InitGame()
    {
        int rand = new System.Random().Next(0, 2); //choose random player to start
        var connection = players[rand].GetComponent<NetworkIdentity>().connectionToClient;
        players[rand].UsedCellValue = CellBehaviour.CellValue.Cross;
        //init opposite players with their enemies
        server.TargetSetOpponent(players[0].identity.connectionToClient, players[1].playerName);
        server.TargetSetOpponent(players[1].identity.connectionToClient, players[0].playerName);
        server.EnabledFirstPlayer(connection);
        server.RpcGameStarted();
    }
}
