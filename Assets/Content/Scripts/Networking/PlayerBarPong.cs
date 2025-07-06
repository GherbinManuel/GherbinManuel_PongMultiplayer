using Mirror;
using Mirror.Examples.Common;
using System;
using UnityEngine;

public class PlayerBarPong : NetworkBehaviour
{

    public float speed = 30;
    public Rigidbody2D rigidbody2d;
    #region Player Index
    [SyncVar] private int _playerIndex;
    public void SetPlayerIndex(int playerIndex) => _playerIndex = playerIndex;
    #endregion
    #region Player Points
    [SyncVar(hook = nameof(OnPointChangeHandle))] private int _playerPoints = 0;
    [Server] public void SetPlayerPoints(int playerPoints) => _playerPoints = playerPoints;
    public int PlayerPoints => _playerPoints;
    public void OnPointChangeHandle(int oldPoints, int newPoints) => OnPointchangeEvent?.Invoke(_playerIndex, newPoints);
    #endregion
    #region Display Name
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] private string _displayName;
    [Server] public void SetDisplayName(string name) => _displayName = name;
    public string DisplayName => _displayName;
    private void ClientHandleDisplayNameUpdated(string oldPoints, string newPoints) => ClientOnInfoUpdated?.Invoke();
    #endregion

    #region Spawnpoint
    [SyncVar] private Vector3 _spawnPoint;
    [Server] public void SetSpawnPoint(Vector3 spawnPoint) => _spawnPoint = spawnPoint;
    #endregion



    #region EVENTS
    public static event Action ClientOnInfoUpdated;
    public static event Action<int, int> OnPointchangeEvent;
    public static event Action<int> ClientOnGameWin;
    public static event Action ClientAuthorityDisconnected;
    #endregion


    #region Server
    [Command]
    public void CmdStartGame()
    {
        ((PongNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Command]
    public void CmdBackToLobby()
    {
        RpcBackToLobby();
    }
    [Command]
    public void CmdChangeUI(UIManager.UITypes uiType)
    {
        RpcChangeUI(uiType);
    }

    #endregion

    #region Client
    [ClientRpc]
    public void RpcGameStart()
    {
        UIManager.Instance.SetUI(UIManager.UITypes.Gameplay);
        transform.position = _spawnPoint;
        SetPlayerPoints(0);
    }
    [ClientRpc]
    public void WinGame()
    {
        ClientOnGameWin?.Invoke(_playerIndex);
        UIManager.Instance.SetUI(UIManager.UITypes.Gameover);
    }
    [ClientRpc]
    private void RpcBackToLobby()
    {
        UIManager.Instance.SetUI(UIManager.UITypes.Lobby);
    }
    [ClientRpc]
    private void RpcChangeUI(UIManager.UITypes uiType)
    {
        if (!isOwned)
            return;
        UIManager.Instance.SetUI(uiType);
    }




    // need to use FixedUpdate for rigidbody
    void FixedUpdate()
    {
        if (!isOwned)
            return;
            Vector3 vel = speed * Time.fixedDeltaTime * new Vector2(0, Input.GetAxisRaw("Vertical"));
#if UNITY_6000_0_OR_NEWER
        rigidbody2d.linearVelocity = vel;
#else
            rigidbody2d.velocity = vel;
#endif
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) return;

        ((PongNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) return;

        ((PongNetworkManager)NetworkManager.singleton).Players.Remove(this);
        
        if (!isOwned) return;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
    }

    #endregion
}