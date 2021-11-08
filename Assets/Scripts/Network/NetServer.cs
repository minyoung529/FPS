using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;

public class NetServer : MonoBehaviour
{
    public static NetServer Instance;

    private ClientToken socket;

    private string localIP;

    private TcpListener server;
    private bool serverStarted = false;

    private List<ClientToken> clients;
    private List<ClientToken> disconnectList;

    private string ip;
    private string port;

    private byte[] data = new byte[1024];

    private int nextId = 0;
    //보통 1024

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        clients = new List<ClientToken>();
        disconnectList = new List<ClientToken>();
        GetLocalIP();
    }

    void GetLocalIP()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                ChatManager.Instance?.SetLocalIP(localIP);

            }
        }
    }
    public void InitializeServer(int port)
    {
        this.port = port.ToString();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            StartListening();
            ChangeStatus(true);
        }

        catch (Exception e)
        {
            Debug.Log("Scket Start Error: " + e);
        }
    }

    public void CloseServer()
    {
        if (!serverStarted) return;

        clients.Clear();
        server.Stop();
        server = null;
        ChangeStatus(false);
    }

    private void StartListening()
    {
        //다음 클라이언트를 기다림
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        //listner이 상대 클라이언트에 대한 정보
        TcpListener listner = (TcpListener)ar.AsyncState;
        clients.Add(new ClientToken(listner.EndAcceptTcpClient(ar), "Player" + nextId++));
        StartListening();
    }

    private void ChangeStatus(bool status)
    {
        serverStarted = status;
        ChatManager.Instance?.OnChangeServerStatus(serverStarted);

    }

    private void Update()
    {
        if (!serverStarted) return;

        foreach (ClientToken client in clients)
        {
            if (!IsConnected(client.tcp))
            {
                disconnectList.Add(client);
                continue;
            }
            try
            {
                NetworkStream stream = client.tcp.GetStream();

                if (stream.DataAvailable)
                {
                    //string data = new StreamReader(stream, true).ReadLine();
                    int length = stream.Read(data, 0, data.Length);
                    //위엔 껍데기

                    byte[] readData = new byte[length];
                    //여기가 실제 데이터를 담을 곳
                    Array.Copy(data, 0, readData, 0, length);

                    if (readData != null)
                    {
                        NetPacket packet = new NetPacket(readData);
                        OnReadData(packet, client);
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.Log(e);
            }

            for (int i = 0; i < disconnectList.Count; i++)
            {
                clients.Remove(disconnectList[i]);
                Broadcast(new NetPacket());
            }

            if (disconnectList.Count > 0)
            {
                disconnectList.Clear();
            }
        }
    }

    private void OnReadData(NetPacket packet, ClientToken token)
    {
        //string[] datas = data.Split('&');
        //string broadCastData = "";
        //string protocol = "";

        NetPacket broadcastPacket = new NetPacket();
        switch (packet.protocol)
        {
            case NetProtocol.REQ_NICKNAME:
                string nickname = packet.PopString();
                token.clientName = nickname;
                broadcastPacket = new NetPacket(NetProtocol.RES_NICKNAME, nickname + "&" + token.index);
                SendClientList(token);
                break;

            case NetProtocol.REQ_CHAT:
                string chat = packet.PopString();
                string newData = token.clientName + "&" + chat;
                broadcastPacket = new NetPacket(NetProtocol.RES_CHAT, newData);
                break;
        }

        Broadcast(broadcastPacket);
    }

    private void SendClientList(ClientToken token)
    {
        string[] names = new string[clients.Count];
        for (int i = 0; i < clients.Count; i++)
        {
            names[i] = clients[i].clientName + "&" + clients[i].index;
        }
        SendData(token, new NetPacket(NetProtocol.SYS_CLIENT_LIST, names));

    }
    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        catch
        {
            return false;
        }
    }
    //Server -> All Clients
    private void Broadcast(NetPacket packet)
    {
        foreach (ClientToken client in clients)
        {
            //보낼 때 새로 만들어서 똑같이 보내ㅂ주면 되겠져
            try
            {
                NetworkStream stream = client.tcp.GetStream();
                stream.Write(packet.packetData, 0, packet.packetData.Length);
                stream.Flush();
            }

            catch (SocketException e)
            {
                Debug.Log(e);
            }
        }
    }

    private void SendData(ClientToken client, NetPacket packet)
    {
        try
        {
            NetworkStream stream = client.tcp.GetStream();
            stream.Write(packet.packetData, 0, packet.packetData.Length);
            stream.Flush();
        }

        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }
}

[Serializable]
public class ClientToken
{
    public TcpClient tcp;
    public int index = -1;
    public string clientName;

    public ClientToken(TcpClient tc, string name, int id = -1)
    {
        tcp = tc;
        clientName = name;
        index = id;
    }

    public ClientToken(string name, int id = -1)
    {
        clientName = name;
        index = id;
    }
}