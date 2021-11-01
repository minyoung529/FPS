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

    private TcpListener server;
    private bool serverStarted = false;

    private List<ClientToken> clients;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        clients = new List<ClientToken>();
        //InitializeServer();
    }
    public void InitializeServer()
    {
        int port = 3333;

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
        //���� Ŭ���̾�Ʈ�� ��ٸ�
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        //listner�� ��� Ŭ���̾�Ʈ�� ���� ����
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

        foreach(ClientToken client in clients)
        {
            NetworkStream stream = client.tcp.GetStream();

            if(stream.DataAvailable)
            {
                string data = new StreamReader(stream, true).ReadLine();

                if(data != null)
                {
                    Broadcast(data);
                }
            }
        }
    }

    //Server -> All Clients
    private void Broadcast(string data)
    {
        foreach(ClientToken client in clients)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }

            catch(SocketException e)
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