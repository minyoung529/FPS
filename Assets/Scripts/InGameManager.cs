using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    private NetClient netClient;

    public GameObject playerPref;
    private int playerCount;
    private List<PlayerController> playerList = new List<PlayerController>();

    public Transform[] spawnPositions;

    private int listOrder = -1;

    void Start()
    {
        netClient = NetClient.Instance;

        Invoke("StartRound", 3f);
    }

    private void StartRound()
    {
        //player ��, ������ġ, ���� ������
        playerCount = netClient.clients.Count;

        for (int i = 0; i < playerCount; i++)
        {
            GeneratePlayer(i);
        }

        listOrder = netClient.listOrder;

        playerList[listOrder].StartGame();
    }

    private void GeneratePlayer(int index)
    {
        GameObject player = Instantiate(playerPref, spawnPositions[index].position, Quaternion.identity);
        playerList.Add(player.GetComponent<PlayerController>());
    }
}
