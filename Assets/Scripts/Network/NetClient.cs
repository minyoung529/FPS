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

    public InputField inputIP;
    public InputField inputPort;

    public void ConnectToServer()
    {
        string ip = inputIP.text;
        string port = inputPort.text;

        TcpClient socket;
        NetworkStream stream;
        StreamWriter writer;
        StreamReader reader;

        if (ip == "" || port == "")
        {
            return;
        }

        try
        {
            socket = new TcpClient(ip, int.Parse(port));
            stream = socket.GetStream();

            //쓸 대는 writer를 받을 때는 reader를 씀
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
        }

        catch(Exception e)
        {
            Debug.Log("Error" + e);
        }
    }
}
