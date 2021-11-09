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

    void Start()
    {
        netClient = NetClient.Instance;

        Invoke("StartRound", 3f);
    }

    private void StartRound()
    {
        //player 수, 생성위치, 누가 나인지
        playerCount = netClient.clients.Count;

        for(int i = 0; i<playerCount;i++)
        {
            GeneratePlayer(i);
        }
    }

    private void GeneratePlayer(int index)
    {
        GameObject player = Instantiate(playerPref, spawnPositions[index].position, Quaternion.identity);
        playerList.Add(player.GetComponent<PlayerController>());
    }
}
