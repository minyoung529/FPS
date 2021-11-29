using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance;
    private NetClient netClient;

    public GameObject playerPref;
    private int playerCount;
    private List<PlayerController> playerList = new List<PlayerController>();

    public Transform[] spawnPositions;
    private int interpolateConstant;

    private int listOrder = -1;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        netClient = NetClient.Instance;

        Invoke("StartRound", 3f);
    }

    private void StartRound()
    {
        //player 수, 생성위치, 누가 나인지
        playerCount = netClient.clients.Count;

        for (int i = 0; i < playerCount; i++)
        {
            GeneratePlayer(i);

            playerList[i].StartGame(listOrder == i);
        }

        listOrder = netClient.listOrder;
    }

    private void GeneratePlayer(int index)
    {
        GameObject player = Instantiate(playerPref, spawnPositions[index].position, Quaternion.identity);
        playerList.Add(player.GetComponent<PlayerController>());
    }

    public void NetPlayersMove(List<string[]> transformList)
    {
        for (int i = 0; i < transformList.Count; i++)
        {
            if (i == listOrder) continue;

            string[] trans = transformList[i];
            string[] posStrs = trans[0].Split(':');
            string[] rotStrs = trans[1].Split(':');

            if (trans[0] == null)
                continue;

            Vector3 pos = new Vector3(float.Parse(posStrs[0]), float.Parse(posStrs[1]), float.Parse(posStrs[2]));
            Quaternion rot = Quaternion.Euler(float.Parse(rotStrs[0]), float.Parse(rotStrs[1]), float.Parse(rotStrs[2]));

            Vector3 offset = pos - playerList[i].transform.position;

            if(offset.magnitude > interpolateConstant)
            {
                playerList[i].UpdateMoveDirection(offset);
            }
            else
            {
                playerList[i].UpdateMoveDirection(Vector3.zero);
            }
        }
    }
}
