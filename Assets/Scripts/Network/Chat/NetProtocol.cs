using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetProtocol
{
    // req 보내는쪽
    // res 받는 쪽응답

    public const int SYS_CLIENT_LIST = -1;
    public const int SYS_CLIENT_DISCONNECT = -2;
    public const int SYS_SET_HOST = -3;

    public const int REQ_NICKNAME = 1;
    public const int REQ_CHAT = 2;
    public const int REQ_GAME_START = 3;

    public const int RES_CHAT = 51;
    public const int RES_NICKNAME = 52;
    public const int RES_GAME_START = 53;
}
