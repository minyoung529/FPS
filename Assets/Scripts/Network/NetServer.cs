using System.Collections;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;

public class NetServer : MonoBehaviour
{
    private Socket socket;

    private TcpListener server;
    private bool serverStarted = false;

    private List<ClientToken> clients;

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
            serverStarted = true;
            StartListening();
        }

        catch (Exception e)
        {
            Debug.Log("Scket Start Error: " + e);
        }
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