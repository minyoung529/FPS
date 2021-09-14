using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyGenerator : MonoBehaviour
{
    public enum ZombieType
    {
        Basic,
        Strong
    }

    public static TutorialEnemyGenerator Instance;

    [SerializeField] private GameObject zombiePref;
    [SerializeField] private GameObject strongZombiePref;
    [SerializeField] private Vector3 spawnPos;

    [SerializeField] private GameObject ground;
    [SerializeField] private Bounds groundBounds;
    [SerializeField] private float minXPos;
    [SerializeField] private float minZPos;
    [SerializeField] private float maxXPos;
    [SerializeField] private float maxZPos;

    private float spawnDelay;
    private int basicMaxCount = 10;
    private int strongMaxCount = 2;

    private int basicCnt = 0;
    private int strongCnt = 0;

    private Queue<GameObject> basicQueue = new Queue<GameObject>();
    private Queue<GameObject> strongQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Bounds bounds = ground.GetComponent<MeshRenderer>().bounds;

        minXPos = bounds.min.x;
        minZPos = bounds.min.z;
        maxXPos = bounds.max.x;
        maxZPos = bounds.max.z;

        spawnDelay = 3f;
        StartCoroutine(GenerateZombieCoroutine());
    }

    private IEnumerator GenerateZombieCoroutine()
    {
        while (true)
        {
            Vector3 spawnPos = new Vector3(Random.Range(minXPos + 1, maxXPos - 1), 1, Random.Range(minZPos + 1, maxZPos - 1));

            if (basicCnt <= basicMaxCount)
            {
                GenerateZombie(zombiePref, spawnPos, ZombieType.Basic);
            }

            if (strongCnt <= strongMaxCount)
            {
                GenerateZombie(strongZombiePref, spawnPos, ZombieType.Strong);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void GenerateZombie(GameObject pref, Vector3 pos, ZombieType type)
    {
        Queue<GameObject> queue;

        switch (type)
        {
            case ZombieType.Basic:
                queue = basicQueue;
                basicCnt++;
                break;

            case ZombieType.Strong:
                queue = strongQueue;
                strongCnt++;
                break;

            default:
                queue = new Queue<GameObject>();
                break;
        }


        if (basicQueue.Count == 0)
        {
            GameObject zombie = Instantiate(pref, pos, Quaternion.Euler(0, Random.Range(-180, 180), 0));
            basicQueue.Enqueue(zombie);
        }
        else
        {
            GameObject zombie = basicQueue.Dequeue();
            zombie.transform.position = pos;
            zombie.transform.rotation = Quaternion.Euler(0, Random.Range(-180, 180), 0);
            zombie.SetActive(true);
        }
    }

    public static void EnqueueZombie(ZombieType type, GameObject zombie)
    {
        Queue<GameObject> queue;

        switch (type)
        {
            case ZombieType.Basic:
                queue = Instance.basicQueue;
                Instance.basicCnt--;
                Debug.Log("Sdf");
                break;

            case ZombieType.Strong:
                queue = Instance.strongQueue;
                Instance.strongCnt--;
                Debug.Log("Sdf");
                break;

            default:
                queue = new Queue<GameObject>();
                break;
        }

        queue.Enqueue(zombie);
    }
}