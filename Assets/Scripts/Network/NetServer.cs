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

    private Socket socket;

    private string localIP;

    private TcpListener server;
    private bool serverStarted = false;

    private List<ClientToken> clients;

    private string ip;
    private string port;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        clients = new List<ClientToken>();
        GetLocalIP();
    }

    void GetLocalIP()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(IPAddress ip in host.AddressList)
        {
            if(ip.AddressFamily == AddressFamily.InterNetwork)
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
        clients.Add(new ClientToken(listner.EndAcceptTcpClient(ar), "Player" + clients.Count));
        StartListening();
    }

    private void ChangeStatus(bool status)
    {
        serverStarted = status;
        ChatManager.Instance?.OnChangeServerStatus(serverStarted);

    }

    private void Update()
    {
        //im really sober

        if (!serverStarted) return;

        foreach (ClientToken client in clients)
        {
            NetworkStream stream = client.tcp.GetStream();

            if (stream.DataAvailable)
            {
                string data = new StreamReader(stream, true).ReadLine();

                if (data != null)
                {
                    Broadcast(data, client.clientName);
                }
            }
        }
    }

    //Server -> All Clients
    private void Broadcast(string data, string clientName)
    {
        foreach (ClientToken client in clients)
        {
            try
            {
                string newData = clientName + '&' + data;
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                writer.WriteLine(newData);
                writer.Flush();
            }

            catch (SocketException e)
            {
                Debug.Log(e);
            }
        }
    }
}

public class ClientToken
{
    public TcpClient tcp;
    public string clientName;

    public ClientToken(TcpClient tc, string name)
    {
        tcp = tc;
        clientName = name;
    }
}