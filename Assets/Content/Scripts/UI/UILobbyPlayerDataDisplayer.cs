using TMPro;
using UnityEngine;

public class UILobbyPlayerDataDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerNameLabes;

    public void SetData(string playerName)
    {
        _playerNameLabes.text = playerName;
    }
}
