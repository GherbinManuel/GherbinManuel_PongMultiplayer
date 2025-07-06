using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
public class PongNetworkManager : NetworkManager
{
    public const int MAX_POINT = 7;
    public List<PlayerBarPong> Players => _players;
    private List<PlayerBarPong> _players = new(2);

    [SerializeField] private Transform[] _spawnPoints;
    GameObject ball;


    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerBarPong player = conn.identity.GetComponent<PlayerBarPong>();

        _players.Add(player);

        int playerID = _players.Count - 1;

        player.SetPlayerIndex(playerID);
        player.SetSpawnPoint(_spawnPoints[playerID].position);
        player.SetDisplayName($"Player {_players.Count}");

       //player.RpcSetActive(false);
    }

    #region Gameplay
    [Server]
    public void StartGame()
    {
        ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GM_PongBall"));
        NetworkServer.Spawn(ball);
        foreach(PlayerBarPong player in Players)
            player.RpcGameStart();
    }
    private void EndGame(int winnedPlayerID)
    {
        NetworkServer.Destroy(ball);
        Players[winnedPlayerID].WinGame();/*
        foreach (PlayerBarPong player in Players)
            player.RpcSetActive(false);*/
    }
    public void OnGoal()
    {
        float player0Distance = (ball.transform.position - _players[0].transform.position).sqrMagnitude;
        float player1Distance = (ball.transform.position - _players[1].transform.position).sqrMagnitude;

        int playerIDToGetPoint = player0Distance < player1Distance? 1 : 0;

        int currentPlayerPoints = _players[playerIDToGetPoint].PlayerPoints + 1;

        if(currentPlayerPoints == MAX_POINT)
        {
            EndGame(playerIDToGetPoint);
            return;
        }

        _players[playerIDToGetPoint].SetPlayerPoints(currentPlayerPoints);
        ball.transform.position = Vector3.zero;


    }
    #endregion

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);

        Debug.Log($"{Players.Count}");
        /*
        if (Players.Count <= 2) return;

        conn.Disconnect();
    */}
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        PlayerBarPong player = conn.identity.GetComponent<PlayerBarPong>();

        Players.Remove(player);

        if (ball != null)
            NetworkServer.Destroy(ball);

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        Players.Clear();
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Players.Clear();
    }
    public override void OnStopHost()
    {
        base.OnStopHost();
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        else
        {
            StopClient();

            PlayerBarPong currentPlayer = NetworkClient.connection.identity.GetComponent<PlayerBarPong>();
            foreach (var player in Players)
                player.CmdChangeUI(player == currentPlayer ? UIManager.UITypes.MainMenu : UIManager.UITypes.Lobby);
        }

    }
}
