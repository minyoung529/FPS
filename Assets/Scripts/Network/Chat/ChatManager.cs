using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    #region SERVER
    public Button btnServerStart;
    public Button btnServerClose;

    public Text txtServerStatus;
    public Text txtServerLocalIP;

    public InputField inputServerPort;
    #endregion

    #region CLIENT
    public Button btnClientConnect;
    public Button btnClientDisconnect;

    public Text txtClientStatus;

    public InputField inputIP;
    public InputField inputPort;
    public InputField inputNickName;
    #endregion

    #region CHAT
    public InputField inputChat;
    public VerticalLayoutGroup contentView;
    public Text chatPrefab;
    #endregion

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
        NetClient.Instance.SendChat(chat);
        inputChat.text = "";
    }
    public void OnChangeServerStatus(bool serverStarted)
    {
        btnServerStart.interactable = !serverStarted;
        btnServerClose.interactable = serverStarted;

        string status = serverStarted ? "OPEN" : "CLOSED";
        txtServerStatus.text = $"Status: {status}";
    }

    public void SetLocalIP(string ip)
    {
        txtServerLocalIP.text = ip;
    }
    public void OnChangeClientStatus(bool clientConnected)
    {
        btnClientConnect.interactable = !clientConnected;
        btnClientDisconnect.interactable = clientConnected;

        string status = clientConnected ? "CONNECT" : "DISCONNECT";
        txtClientStatus.text = $"Status: {status}";
    }
    public void OnUserJoin(string msg)
    {
        Debug.Log("sdf");
        Text newChat = Instantiate(chatPrefab, contentView.transform);
        newChat.text = msg;

    }
    public void OnReadChat(string name, StringBuilder chat)
    {
        Text newChat = Instantiate(chatPrefab, contentView.transform);
        newChat.text = name + ": " + chat;

    }

    public void ConnectToServer()
    {
        string ip = inputIP.text;
        string port = inputPort.text;
        string nickName = inputNickName.text;

        if (nickName == "")
        {
            Debug.Log("닉네임 입력하세여");
            return;
        }

        NetClient.Instance.ConnectToServer(ip, port,nickName);
    }

    public void StartServer()
    {
        try
        {
            int port = int.Parse(inputServerPort.text);
            NetServer.Instance.InitializeServer(port);
        }
        
        catch(Exception)
        {
            throw;
        }
    }
}
