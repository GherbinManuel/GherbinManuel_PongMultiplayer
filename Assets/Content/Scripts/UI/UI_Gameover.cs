using Mirror;
using System;
using TMPro;
using UnityEngine;

public class UI_Gameover : UIBase
{
    public override UIManager.UITypes UIType => UIManager.UITypes.Gameover;

    [SerializeField] private TextMeshProUGUI _winnerLabel;

    public override void Init()
    {
        PlayerBarPong.ClientOnGameWin += UpdateWinnerText;

    }

    private void UpdateWinnerText(int winnerID)
    {
        string winnerName = ((PongNetworkManager)NetworkManager.singleton).Players[winnerID].DisplayName;
        _winnerLabel.text = winnerName + " Wins";
    }

    public void BackToLobby()
    {
        NetworkClient.connection.identity.GetComponent<PlayerBarPong>().CmdBackToLobby();
    }


    public void LeaveLobby()
    {
        ((PongNetworkManager)PongNetworkManager.singleton).LeaveLobby();
    }

    public void QuitGame()
    {
        LeaveLobby();
        Application.Quit();
    }
    public void RestartGane() => NetworkClient.connection.identity.GetComponent<PlayerBarPong>().CmdStartGame();

}
