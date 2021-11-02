using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// [   HEADER   ]
// [Protocol ] [ body Length] [ body ]
public class NetPacket
{
    public int protocol;
    public int bodyLength;
    public byte[] packetData; // [Protocol ] [ body Length] [ body ] �� �� ������ �ִ�

    public NetPacket() { }
    public NetPacket(int protocol, string data)
    {
        //int to byteArray
        byte[] protocolHeader = BitConverter.GetBytes(protocol);

        //string to byteArray
        byte[] body = StringToByteArray(data);

        //length of byte array
        int bodyLength = body.Length;
        byte[] bodyLengthHeader = BitConverter.GetBytes(bodyLength);

        packetData = new byte[protocolHeader.Length + bodyLength + bodyLengthHeader.Length];

        // packet Data = [ �� �� ]
        //      sourceArray [ 0 ] => ��ü�� ����
        // =>
        //      destinationButeArray [ 0 ]  protocolHeader �����ŭ [0]����
        //= packetData = [ protocolHeader ] [�� ��]

        //protocolHeader Length : 4 byte
        // packet Data = [][][][] <= Protocol [][][][]  <= length  [][][][][][]~~ <= Data
        Array.Copy(protocolHeader, 0, packetData, 0, protocolHeader.Length);

        Array.Copy(bodyLengthHeader, 0, packetData, protocolHeader.Length, bodyLengthHeader.Length);

        Array.Copy(body, 0, packetData, protocolHeader.Length + bodyLengthHeader.Length, body.Length);

    }

    //stream writer, reader�� �̷� ������ ��ġ�� ����
    //���� ������ byte array�θ� ����Ѵ�.
    //stream writer, reader��: string -> byte[], byte[] -> string
    private byte[] StringToByteArray(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    private string ByteArrayToString(byte[] bytes)
    {
        return Encoding.Default.GetString(bytes);
    }

    //����Ʈ ��̸� ��Ŷȭ
    public NetPacket(byte[] bytes)
    {
        packetData = bytes;
        //byte[] : 0~ 3   => int

        protocol = BitConverter.ToInt32(bytes, 0);
        bodyLength = BitConverter.ToInt32(bytes, /*sizeof(int)*/4); // 4
    }

    //���ϴ� �� �̴°�
    //packet Data: [Header](8) [Body]
    //             bodyLength
    //body         [Body]
    public string PopString()
    {
        byte[] body = new byte[bodyLength];

        Array.Copy(packetData, /*sizeof(int) * 2*/8, body, 0, bodyLength);
        return ByteArrayToString(body);
    }

}
