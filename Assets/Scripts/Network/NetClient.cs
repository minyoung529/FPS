using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class NetClient : MonoBehaviour
{
    public enum Scene
    {
        MultiplayScene = 2,
        SocketChat = 99
    }

    public static NetClient Instance;
    private Scene currentScene;
    private bool connected = false;

    public ClientToken serverToken;
    public NetworkStream stream;

    private byte[] data = new byte[1024];
    private bool isHost;
    public List<ClientToken> clients;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentScene = Scene.SocketChat;
        clients = new List<ClientToken>();
    }

    private void Update()
    {
        if (!connected) return;

        if (stream.DataAvailable)
        {
            //string data = reader.ReadLine();
            int length = stream.Read(data, 0, data.Length);

            byte[] readData = new byte[length];

            Array.Copy(data, 0, readData, 0, length);

            if (readData != null)
            {
                SplitBytesToPackets(readData);
                Debug.Log("sdf");

            }
        }
    }

    private void SplitBytesToPackets(byte[] readData)
    {
        int packetStartPos = 0;

        while (readData.Length > packetStartPos)
        {
            int bodyLength = BitConverter.ToInt32(readData, 4 + packetStartPos);
            int packetLength = bodyLength + 8;
            byte[] packetBytes = new byte[packetLength];
            Array.Copy(readData, packetStartPos, packetBytes, 0, packetLength);

            NetPacket packet = new NetPacket(packetBytes);
            OnReadData(packet);

            packetStartPos += packetLength;
        }
    }
    //µ¥ÀÌÅÍ ÆÄ½Ì ÆÄ½ÌÆÄ½ÌÁö³ª¤¹½Î¿ï·¡
    private void OnReadData(NetPacket packet)
    {
        switch (packet.protocol)
        {
            case NetProtocol.SYS_CLIENT_LIST:
                SceneEvent(packet);
                break;

            case NetProtocol.RES_NICKNAME:
                SceneEvent(packet);
                break;

            case NetProtocol.RES_CHAT:
                string[] datas = packet.PopString().Split('&');

                StringBuilder chat = new StringBuilder();

                for (int i = 1; i < datas.Length; i++)
                {
                    chat.Append(datas[i]);

                    if (i > 1)
                    {
                        chat.Append("&");
                    }
                }
                ChatManager.Instance?.OnReadChat(datas[0], chat);
                break;

            case NetProtocol.SYS_CLIENT_DISCONNECT:
                int disconnectedId = packet.PopInt();
                string dName = clients.Find(client => client.index == disconnectedId).clientName;

                SceneEvent(packet);
                ChatManager.Instance.OnUserDisconnect(disconnectedId, dName);
                break;

            case NetProtocol.RES_GAME_START:
                StartScene(Scene.MultiplayScene);
                break;

            case NetProtocol.SYS_SET_HOST:
                isHost = true;
                break;
        }
    }
    private void SceneEvent(NetPacket packet)
    {
        switch (currentScene)
        {
            case Scene.MultiplayScene:
                break;
            case Scene.SocketChat:
                switch (packet.protocol)
                {
                    case NetProtocol.SYS_CLIENT_LIST:
                        string[] clientsNames = (string[])packet.PopObject();

                        for (int i = 0; i < clientsNames.Length; i++)
                        {
                            if (i == clientsNames.Length - 1) break;
                            ClientToken client = AddClient(clientsNames[i]);

                            ChatManager.Instance.OnUserJoin(client.index, client.clientName);
                        }
                        break;

                    case NetProtocol.RES_NICKNAME: // user join
                        ClientToken t = AddClient(packet.PopString());
                        ChatManager.Instance.OnUserJoin(t.index, t.clientName);
                        break;

                    case NetProtocol.SYS_SET_HOST:
                        isHost = true;
                        ChatManager.Instance.InteractbleStartButton();
                        break;
                }
                break;
        }
    }

    public void SendStartGame()
    {
        SendData(NetProtocol.REQ_GAME_START);
    }
    private ClientToken AddClient(string cn)
    {
        string[] nameIndex = cn.Split('&');
        int id = int.Parse(nameIndex[nameIndex.Length - 1]);
        string clientName = "";

        for (int j = 0; j < nameIndex.Length - 1; j++)
        {
            clientName += nameIndex[j];
        }
        ClientToken token = new ClientToken(clientName, id);
        clients.Add(token);
        return token;
    }
    public void ConnectToServer(string ip, string port, string nickName)
    {
        if (ip == "" || port == "")
        {
            Debug.Log("sd");
            return;
        }

        try
        {
            serverToken = new ClientToken(new TcpClient(ip, int.Parse(port)), nickName, -1);
            //socket = new TcpClient(ip, int.Parse(port));
            stream = serverToken.tcp.GetStream();

            //¾µ ¶§´Â writer¸¦ ¹ÞÀ» ¶§´Â reader¸¦ ¾¸
            //writer = new StreamWriter(stream);
            //reader = new StreamReader(stream);

            OnConnected();
        }

        catch (Exception e)
        {
            Debug.Log("Error" + e);
        }
    }

    // Set Nick Name -> Connect To Server -> Send NickName
    private void OnConnected()
    {
        ChangeStatus(true);
        SendNickName();
    }

    public void DisconnectToServer()
    {
        if (serverToken == null) return;

        serverToken.tcp.Close();
        serverToken = null;
        isHost = false;

        ChangeStatus(false);

        ChatManager.Instance?.ResetUI();
    }


    private void ChangeStatus(bool status)
    {
        connected = status;
        ChatManager.Instance?.OnChangeClientStatus(connected);
    }

    #region Send
    public void SendChat(string msg)
    {
        SendData(NetProtocol.REQ_CHAT, msg);

    }

    public void SendNickName()
    {
        SendData(NetProtocol.REQ_NICKNAME, serverToken.clientName);

    }

    private void SendData(int protocol, string data)
    {
        if (!connected) return;

        NetPacket packet = new NetPacket(protocol, data);

        stream.Write(packet.packetData, 0, packet.packetData.Length);
        stream.Flush();
    }

    private void SendData(int protocol)
    {
        if (!connected) return;

        NetPacket packet = new NetPacket(protocol);

        stream.Write(packet.packetData, 0, packet.packetData.Length);
        stream.Flush();
    }


    public void StartScene(Scene scene)
    {
        currentScene = scene;

        SceneManager.LoadScene(scene.ToString());
    }

    #endregion
}
