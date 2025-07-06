using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : UIBase
{
    public override UIManager.UITypes UIType => UIManager.UITypes.Lobby;

    [SerializeField] private UILobbyPlayerDataDisplayer[] _playerNameLabes;
    [SerializeField] private TextMeshProUGUI _playersQtaLabel;
    [SerializeField] private Button _StartGameButton;

    public override void IsActive(bool isActive)
    {
        base.IsActive(isActive);

        if (isActive)
        {
            PlayerBarPong.ClientOnInfoUpdated += SetPlayerLabels;
            SetPlayerLabels();
        }
        else
        {
            PlayerBarPong.ClientOnInfoUpdated -= SetPlayerLabels;
        }
    }


    private void SetPlayerLabels()
    {
        List<PlayerBarPong> players = ((PongNetworkManager)NetworkManager.singleton).Players;

        int count = players.Count;

        _playersQtaLabel.text = $"PLayer: {count}/2";

        _StartGameButton.interactable = count == 2;

        for (int i = 0; i < players.Count; i++)
            _playerNameLabes[i].SetData($"Player {i}");

        for (int i = players.Count; i < _playerNameLabes.Length; i++)
            _playerNameLabes[i].SetData("Waiting for Player...");
    }

    public void LeaveLobby() => ((PongNetworkManager)PongNetworkManager.singleton).LeaveLobby();



    public void StartGame() => NetworkClient.connection.identity.GetComponent<PlayerBarPong>().CmdStartGame();
}
