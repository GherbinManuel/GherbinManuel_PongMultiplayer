using Mirror;
using System.Net.Sockets;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UI_MainMenu : UIBase
{
    public override UIManager.UITypes UIType => UIManager.UITypes.MainMenu;

    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private TextMeshProUGUI _localIPLabel;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;

    public override void Init()
    {
        base.Init();
        _localIPLabel.text = GetLocalIPv4();
        StartCoroutine(GetPublicIP());

        GUIUtility.systemCopyBuffer = _localIPLabel.text;
    }

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
    private static string GetLocalIPv4()
    {
        string localIP = "";
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    private IEnumerator GetPublicIP()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.ipify.org");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("IP pubblico: " + www.downloadHandler.text);
            _localIPLabel.text = www.downloadHandler.text;
        }
        else
        {
            Debug.LogError("Errore nel recupero IP pubblico");
        }
    }
}
