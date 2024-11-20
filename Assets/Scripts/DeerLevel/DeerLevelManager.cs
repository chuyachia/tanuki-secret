using System;
using System.Collections.Generic;
using UnityEngine;

public class DeerLevelManager : MonoBehaviour
{

    [SerializeField] private GameObject deerPrefab;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private List<GameObject> waypoints;
    [SerializeField] private int numberOfDeers = 5;
    [SerializeField] private float spacingHorizontal = 6f;
    [SerializeField] private float spacingVertical = 6f;
    [SerializeField] private GameObject player;
    [SerializeField] private float targetReachedSquaredDistance = 4f;
    [SerializeField] private float maxSquaredDistanceFromDeerGroup = 400f;
    [SerializeField] private float wolfAttackInterval = 2f;
    [SerializeField] private float wolfAttackProbability = 0.5f;
    [SerializeField] private float wolfAppearDistance = 12f;

    private GameObject leaderDeer;
    private Dictionary<int, GameObject> runningDeers;
    private Dictionary<int, GameObject> runningWolves;
    private List<GameObject> deadDeers;
    private List<GameObject> eatingWolves;

    private Vector3 playerTriggerPositon = Vector3.zero;
    private bool journeyStarted = false;
    private ObjectPool wolfPool;
    private ObjectPool deerPool;
    private float wolfAttackTimer = 0f;
    private int deerTargetId;

    void Start()
    {
        runningDeers = new Dictionary<int, GameObject>();
        runningWolves = new Dictionary<int, GameObject>();
        deadDeers = new List<GameObject>();
        eatingWolves = new List<GameObject>();
        journeyStarted = false;
        wolfPool = new ObjectPool(wolfPrefab);
        deerPool = new ObjectPool(deerPrefab);
        PlaceDeersInPosition();
        deerTargetId = numberOfDeers - 1;
        EventManager.Instance.RegisterDeerLevelEventListener(HandleEvent);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterDeerLevelEvenListener(HandleEvent);
    }

    void HandleEvent(GameObject[] targets, EventManager.DeerLevelEvent eventType)
    {
        switch (eventType)
        {
            case EventManager.DeerLevelEvent.WolfCatchDeer:
                {
                    WolfEatDeer(targets[0], targets[1]);
                    break;
                }
            case EventManager.DeerLevelEvent.PlayerKillWolf:
                {
                    wolfPool.Reclaim(targets[0]);
                    runningWolves.Remove(targets[0].GetInstanceID());
                    break;
                }
        }
    }

    void WolfEatDeer(GameObject wolf, GameObject deer)
    {
        deer.GetComponent<FollowerDeerBehaviour>().Leader = null;
        runningDeers.Remove(deer.GetInstanceID());
        runningWolves.Remove(wolf.GetInstanceID());
        deadDeers.Add(deer);
        eatingWolves.Add(wolf);
    }

    void Update()
    {
        if (!journeyStarted)
        {
            if (Utils.DistanceToTargetWithinThreshold(player.transform.position, playerTriggerPositon, targetReachedSquaredDistance))
            {
                StartDeersJoruney();
            }
        }
        else
        {
            AddWolf();
            SetWolfTargetDeer();
            if (Utils.DistanceToTargetAboveThreshold(player.transform.position, leaderDeer.transform.position, maxSquaredDistanceFromDeerGroup))
            {
                Debug.Log("Strayed away from the group");
                ResetGame();
            }
            if (runningDeers.Count == 0)
            {
                Debug.Log("Only one deer left");
                ResetGame();
            }
        }
    }

    void ResetGame()
    {
        foreach (GameObject follower in runningDeers.Values)
        {
            deerPool.Reclaim(follower);
        }
        foreach (GameObject deer in deadDeers)
        {
            deerPool.Reclaim(deer);
        }
        deerPool.Reclaim(leaderDeer);
        foreach (GameObject wolf in runningWolves.Values)
        {
            wolfPool.Reclaim(wolf);
        }
        foreach (GameObject wolf in eatingWolves)
        {
            wolfPool.Reclaim(wolf);
        }
        leaderDeer = null;
        runningDeers.Clear();
        runningWolves.Clear();
        PlaceDeersInPosition();
        player.transform.position = new Vector3(playerTriggerPositon.x, player.transform.position.y, playerTriggerPositon.z - 5f);
        journeyStarted = false;
    }

    void StartDeersJoruney()
    {
        journeyStarted = true;
        LeaderDeerBehaviour leaderDeerBehaviour = leaderDeer.GetComponent<LeaderDeerBehaviour>();
        leaderDeerBehaviour.StartMove(waypoints);
        foreach (GameObject follower in runningDeers.Values)
        {
            FollowerDeerBehaviour followerDeerBehaviour = follower.GetComponent<FollowerDeerBehaviour>();
            followerDeerBehaviour.Leader = leaderDeer;
        }
    }

    void SetWolfTargetDeer()
    {
        if (!runningDeers.ContainsKey(deerTargetId) || !runningDeers[deerTargetId].activeSelf)
        {
            List<int> deerId = new List<int>(runningDeers.Keys);
            if (deerId.Count > 0)
            {
                deerTargetId = deerId[UnityEngine.Random.Range(0, deerId.Count)];
            }
            else
            {
                return; // no more deers to assign as target, exist early
            }
        }
        foreach (GameObject wolf in runningWolves.Values)
        {
            wolf.GetComponent<WolfBehaviour>().Target = runningDeers[deerTargetId];
        }
    }

    void AddWolf()
    {
        if (wolfAttackTimer > 0)
        {
            wolfAttackTimer -= Time.fixedDeltaTime;
        }

        if (wolfAttackTimer <= 0 && UnityEngine.Random.value < wolfAttackProbability)
        {
            GameObject wolf = wolfPool.Get();
            runningWolves.Add(wolf.GetInstanceID(), wolf);
            wolf.transform.position = Utils.JitterPosition(leaderDeer.transform.position + Vector3.left * wolfAppearDistance, 5f);
            wolf.transform.parent = transform;
            wolfAttackTimer = wolfAttackInterval;
        }
    }

    GameObject SpawnDeer(Vector3 position)
    {
        GameObject newDeer = deerPool.Get();
        newDeer.transform.parent = transform;
        newDeer.transform.position = position;
        return newDeer;
    }

    void PlaceDeersInPosition()
    {
        int remainingObjects = numberOfDeers;
        int row = 1;
        Vector3 originPosition = transform.position;
        while (remainingObjects > 0)
        {
            int objectsInRow = row;
            int objectsToPlace = Math.Min(objectsInRow, remainingObjects);

            for (int i = 0; i < objectsToPlace; i++)
            {
                Vector3 position = originPosition - new Vector3(i * spacingHorizontal - (objectsInRow - 1) * spacingHorizontal / 2f, 0f, (row - 1) * spacingVertical);
                GameObject deer = SpawnDeer(position);
                if (row == 1)
                {
                    leaderDeer = deer;
                    LeaderDeerBehaviour leaderDeerBehaviour = deer.GetComponent<LeaderDeerBehaviour>();
                    leaderDeerBehaviour.enabled = true;
                    deer.GetComponent<FollowerDeerBehaviour>().enabled = false;
                    leaderDeerBehaviour.ResetMoveState();
                }
                else
                {
                    runningDeers.Add(deer.GetInstanceID(), deer);
                    FollowerDeerBehaviour followerDeerBehaviour = deer.GetComponent<FollowerDeerBehaviour>();
                    followerDeerBehaviour.enabled = true;
                    followerDeerBehaviour.Leader = null;
                    followerDeerBehaviour.deerPositionParamters = new DeerPositionParamters(row, i, objectsInRow, spacingHorizontal, spacingVertical);
                    deer.GetComponent<LeaderDeerBehaviour>().enabled = false;
                }
            }

            remainingObjects -= objectsToPlace;
            if (remainingObjects == 0)
            {
                playerTriggerPositon = originPosition - new Vector3(objectsToPlace * spacingHorizontal - (objectsInRow - 1) * spacingHorizontal / 2f, 0f, (row - 1) * spacingVertical);
            }
            row++;
        }
    }
}
