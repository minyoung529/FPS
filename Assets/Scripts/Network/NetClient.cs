using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using UnityEngine.UI;
using System.Text;

public class NetClient : MonoBehaviour
{
    public static NetClient Instance;

    private bool connected = false;

    public ClientToken clientToken;
    public NetworkStream stream;

    private byte[] data = new byte[1024];

    private void Awake()
    {
        Instance = this;
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
                NetPacket packet = new NetPacket(readData);
                OnReadData(packet);
                Debug.Log("sdf");

            }
        }
    }
    //데이터 파씽
    private void OnReadData(NetPacket packet)
    {
        //지나ㅉ 거짓말 안 치고 하나도 모르겠ㅇ요 진짜 하나도 진짜 하나도ㅑ 진짜 진짜 진짜 하나도 진짜 하나도

        Debug.Log("sdf");

        switch (packet.protocol)
        {
            case NetProtocol.RES_NICKNAME:

                //if (IsChatScene())
                //{
                Debug.Log("sdf");

                string message = packet.PopString() + "님이 입장하셨습니다!";
                    ChatManager.Instance?.OnUserJoin(message);
                //}

                break;

            case NetProtocol.RES_CHAT:
                Debug.Log("e=dif");

                //nickname & chat
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
        }
    }

    private bool IsChatScene()
    {
        return ChatManager.Instance != null;
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
            clientToken = new ClientToken(new TcpClient(ip, int.Parse(port)), nickName);
            //socket = new TcpClient(ip, int.Parse(port));
            stream = clientToken.tcp.GetStream();

            //쓸 때는 writer를 받을 때는 reader를 씀
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
        if (clientToken == null) return;

        clientToken.tcp.Close();
        clientToken = null;

        ChangeStatus(false);
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
        SendData(NetProtocol.REQ_NICKNAME, clientToken.clientName);

    }

    private void SendData(int protocol, string data)
    {
        if (!connected) return;

        NetPacket packet = new NetPacket(protocol, data);

        stream.Write(packet.packetData, 0, packet.packetData.Length);
        stream.Flush();
    }
    #endregion
}
