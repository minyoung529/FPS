using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine.UI;

public class NetClient : MonoBehaviour
{
    public static NetClient Instance;

    public InputField inputIP;
    public InputField inputPort;

    private bool connected = false;

    public TcpClient socket;
    public NetworkStream stream;
    public StreamWriter writer;
    public StreamReader reader;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!connected) return;

        if (stream.DataAvailable)
        {
            string data = reader.ReadLine();

            if (data != null)
            {
                OnReadData(data);
            }
        }
    }

    private void OnReadData(string data)
    {
        ChatManager.Instance?.OnReadChat(data);
    }
    public void ConnectToServer()
    {
        string ip = inputIP.text;
        string port = inputPort.text;

        if (ip == "" || port == "")
        {
            Debug.Log("sd");
            return;
        }

        try
        {
            socket = new TcpClient(ip, int.Parse(port));
            stream = socket.GetStream();

            //쓸 때는 writer를 받을 때는 reader를 씀
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            ChangeStatis(true);

        }

        catch (Exception e)
        {
            Debug.Log("Error" + e);
        }
    }

    public void DisconnectToServer()
    {
        if (socket == null) return;

        socket.Close();
        socket = null;

        ChangeStatis(false);
    }


    private void ChangeStatis(bool status)
    {
        connected = status;
        ChatManager.Instance?.OnChangeClientStatus(connected);
    }

    public void SendString(string msg)
    {
        if (!connected) return;

        writer.WriteLine(msg);
        writer.Flush();
    }
}
