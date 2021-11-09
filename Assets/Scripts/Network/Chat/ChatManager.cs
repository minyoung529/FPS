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
    public VerticalLayoutGroup userListContentView;
    public Text userCellPref;

    public Text txtClientStatus;

    public InputField inputIP;
    public InputField inputPort;
    public InputField inputNickName;

    public bool firstJoin = true;
    public Button btnGameStart;

    private List<int> userTextIndexList = new List<int>(); // Id, TextChildIndex
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
        userTextIndexList.Add(-1);
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
    public void OnUserJoin(int id, string userName)
    {

        string msg = userName + "님이 입장하셨습니다";
        Text newChat = Instantiate(chatPrefab, contentView.transform);
        newChat.text = msg;

        if (firstJoin)
        {
            firstJoin = false;
        }
        else
        {
            return;
        }
        AddUserToList(id, userName);
    }

    public void InteractbleStartButton()
    {
        btnGameStart.interactable = true;
    }
    public void OnUserDisconnect(int index, string userName)
    {
        int textIndex = userTextIndexList.FindIndex(clientIndex => clientIndex == index);
        userListContentView.transform.GetChild(textIndex);
        Transform userTextTransform = userListContentView.transform.GetChild(textIndex);
        Destroy(userTextTransform.gameObject);
        userTextIndexList.RemoveAt(textIndex);
        Text newChat = Instantiate(chatPrefab, contentView.transform);
        newChat.text = userName + "님이 나갔습니다!";

    }

    //name > index!
    public void ResetUI()
    {
        Text[] names = userListContentView.transform.GetComponentsInChildren<Text>();

        foreach (Text text in names)
        {
            Destroy(text.gameObject);
        }

        Text[] chats = contentView.GetComponentsInChildren<Text>();

        foreach (Text text in chats)
        {
            Destroy(text.gameObject);
        }
    }

    private void AddUserToList(int id, string userName)
    {
        Text clientName = Instantiate(userCellPref, userListContentView.transform);
        userTextIndexList.Add(id);
        clientName.text = userName;
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

        NetClient.Instance.ConnectToServer(ip, port, nickName);
    }

    public void StartServer()
    {
        try
        {
            int port = int.Parse(inputServerPort.text);
            NetServer.Instance.InitializeServer(port);
        }

        catch (Exception)
        {
            throw;
        }
    }

    public void HandleStartGame()
    {
        //끝도 보이지 않ㄷㄴ
        NetClient.Instance.SendStartGame();
        //NetClient.Instance.StartScene(NetClient.Scene.MultiplayScene);
    }
}
