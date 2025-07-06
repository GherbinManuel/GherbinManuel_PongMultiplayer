using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : UIBase
{
    public override UIManager.UITypes UIType => UIManager.UITypes.MainMenu;

    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;

    public override void IsActive(bool isActive)
    {
        base.IsActive(isActive);
        _hostButton.interactable = isActive;
        _joinButton.interactable = isActive;
    }

    public void Host()
    {
        UIManager.Instance.SetUI(UIManager.UITypes.Lobby);
        NetworkManager.singleton.StartHost();

    }

    public void Connect()
    {
        UIManager.Instance.SetUI(UIManager.UITypes.Lobby);
        _joinButton.interactable = false;
        NetworkManager.singleton.networkAddress = _ipInputField.text;
        NetworkManager.singleton.StartClient();

    }
}
