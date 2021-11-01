using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    public Button btnServerStart;
    public Button btnServerClose;

    public Button btnClientConnect;
    public Button btnClientDisconnect;

    public Text txtServerStatus;
    public Text txtClientStatus;

    public InputField inputChat;

    public VerticalLayoutGroup contentView;
    public Text chatPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inputChat.onEndEdit.AddListener(_ =>
        {
            SendChat();
        });
    }
    public void SendChat()
    {
        string chat = inputChat.text;
        if (chat == "") return;
        NetClient.Instance.SendString(chat);
        inputChat.text = "";
    }
    public void OnChangeServerStatus(bool serverStarted)
    {
        btnServerStart.interactable = !serverStarted;
        btnServerClose.interactable = serverStarted;

        string status = serverStarted ? "OPEN" : "CLOSED";
        txtServerStatus.text = $"Status: {status}";
    }

    public void OnChangeClientStatus(bool clientConnected)
    {
        btnClientConnect.interactable = !clientConnected;
        btnClientDisconnect.interactable = clientConnected;

        string status = clientConnected ? "CONNECT" : "DISCONNECT";
        txtClientStatus.text = $"Status: {status}";
    }

    public void OnReadChat(string data)
    {
        Text newChat = Instantiate(chatPrefab, contentView.transform);
        newChat.text = data;
    }
}
