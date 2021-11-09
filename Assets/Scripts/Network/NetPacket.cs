using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

// [   HEADER   ]
// [Protocol ] [ body Length] [ body ]
public class NetPacket
{
    public int protocol;
    public int bodyLength;
    public byte[] packetData; // [Protocol ] [ body Length] [ body ] ㅏ 다 가지고 있대

    public NetPacket() { }
    public NetPacket(int protocol)
    {
        this.protocol = protocol;
        MakePacketData();
    }
    public NetPacket(int protocol, string data)
    {
        this.protocol = protocol;

        //string to byteArray
        byte[] body = StringToByteArray(data);
        MakePacketData(body);

        // packet Data = [ 빈 값 ]
        //      sourceArray [ 0 ] => 전체를 보냄
        // =>
        //      destinationButeArray [ 0 ]  protocolHeader 사이즈만큼 [0]부터
        //= packetData = [ protocolHeader ] [빈 값]

        //protocolHeader Length : 4 byte
        // packet Data = [][][][] <= Protocol [][][][]  <= length  [][][][][][]~~ <= Data
    }

    //stream writer, reader는 이런 과정을 거치고 있음
    //원래 소켓은 byte array로만 통신한다.
    //stream writer, reader는: string -> byte[], byte[] -> string
    private byte[] StringToByteArray(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    private string ByteArrayToString(byte[] bytes)
    {
        return Encoding.Default.GetString(bytes);
    }

    //바이트 어레이를 패킷화
    public NetPacket(byte[] bytes)
    {
        packetData = bytes;
        //byte[] : 0 ~ 3   => int

        protocol = BitConverter.ToInt32(bytes, 0);
        bodyLength = BitConverter.ToInt32(bytes, /*sizeof(int)*/4); // 4
    }

    public NetPacket(int protocol, int data)
    {
        this.protocol = protocol;
        byte[] body = BitConverter.GetBytes(data);

        MakePacketData(body);
    }

    public NetPacket(int protocol, object obj)
    {
        //obj 우어어우어어우어어어우어우어우어
        this.protocol = protocol;

        byte[] body = ObjectToByteArray(obj);
        MakePacketData(body);
    }

    private void MakePacketData(byte[] body)
    {
        byte[] protocolHeader = BitConverter.GetBytes(protocol);

        int bodyLength = body.Length;
        byte[] bodyLengthHeader = BitConverter.GetBytes(bodyLength);

        packetData = new byte[protocolHeader.Length + bodyLengthHeader.Length + body.Length];
        Array.Copy(protocolHeader, 0, packetData, 0, protocolHeader.Length);
        Array.Copy(bodyLengthHeader, 0, packetData, protocolHeader.Length, bodyLengthHeader.Length);
        Array.Copy(body, 0, packetData, protocolHeader.Length + bodyLengthHeader.Length, body.Length);
    }

    private void MakePacketData()
    {
        byte[] protocolHeader = BitConverter.GetBytes(protocol);

        int bodyLength = 0;
        byte[] bodyLengthHeader = BitConverter.GetBytes(bodyLength);

        Array.Copy(protocolHeader, 0, packetData, 0, protocolHeader.Length);
        Array.Copy(bodyLengthHeader, 0, packetData, protocolHeader.Length, bodyLengthHeader.Length);
    }

    private byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    //원하는 걸 뽑는거
    //packet Data: [Header](8) [Body]
    //             bodyLength
    //body         [Body]
    public string PopString()
    {
        byte[] body = new byte[bodyLength];

        Array.Copy(packetData, /*sizeof(int) * 2*/8, body, 0, bodyLength);
        return ByteArrayToString(body);
    }

    public object PopObject()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        ms.Write(packetData, 8, bodyLength);
        ms.Seek(0, SeekOrigin.Begin);
        return bf.Deserialize(ms);
    }

    internal int PopInt()
    {
        return BitConverter.ToInt32(packetData, 0);
    }
}
