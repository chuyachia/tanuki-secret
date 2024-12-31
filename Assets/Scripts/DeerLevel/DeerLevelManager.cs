using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using System.Numerics; // why was this here???
using Unity.VisualScripting;

public class DeerLevelManager : MonoBehaviour
{

    [SerializeField] bool wolfAppearInBothDirection;
    [SerializeField] private GameObject deerPrefab;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private GameObject bitingLogoPrefab;
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
    [SerializeField] private Vector3 biteLogoShowOffset = new Vector3(0f, 1f, -1f);
    [SerializeField] private float biteLogoDisappearIn = 2f;
    [SerializeField] private float wolfFleeDisappearIn = 5f;
    [SerializeField] private float speedDecrement = 1f;
    [SerializeField] private Door doorToNextLevel;
    [SerializeField] private CinemachineVirtualCamera deerTrackingCamera;
    [SerializeField] private float allowedDistanceBeforeStartJourney = 100f;
    [SerializeField] private GameObject startTriggerLocator;


    private GameObject leaderDeer;
    private Dictionary<int, GameObject> activeDeers;
    private Dictionary<int, GameObject> activeWolves;
    private List<GameObject> inactiveDeers;
    private List<GameObject> inactiveWolves;

    private Vector3 playerTriggerPositon = Vector3.zero;
    private bool journeyStarted;
    private bool arrivedAtDestination;
    private float wolfAttackTimer = 0f;
    private int deerTargetId;
    private Queue<KeyValuePair<GameObject[], EventManager.DeerLevelEvent>> eventsToProcess;

    void Start()
    {
        activeDeers = new Dictionary<int, GameObject>();
        activeWolves = new Dictionary<int, GameObject>();
        inactiveDeers = new List<GameObject>();
        inactiveWolves = new List<GameObject>();
        journeyStarted = false;
        eventsToProcess = new Queue<KeyValuePair<GameObject[], EventManager.DeerLevelEvent>>();
        PlaceDeersInPosition();
        PlaceTriggerLocator();
        deerTargetId = numberOfDeers - 1;
        EventManager.Instance.RegisterDeerLevelEventListener(QueueEvent);
    }

    private void PlaceTriggerLocator()
    {
        GameObject levelLocator = Instantiate(startTriggerLocator);
        levelLocator.transform.parent = transform;
        levelLocator.transform.position = new Vector3(playerTriggerPositon.x, playerTriggerPositon.y + 0.5f, playerTriggerPositon.z);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterDeerLevelEventListener(QueueEvent);
    }

    void QueueEvent(GameObject[] targets, EventManager.DeerLevelEvent eventType)
    {
        var eventToQueue = new KeyValuePair<GameObject[], EventManager.DeerLevelEvent>(targets, eventType);
        eventsToProcess.Enqueue(eventToQueue);
    }

    void ProcessEvent()
    {
        while (eventsToProcess.Count > 0)
        {
            KeyValuePair<GameObject[], EventManager.DeerLevelEvent> targetEventTypePair = eventsToProcess.Dequeue();
            GameObject[] targets = targetEventTypePair.Key;
            switch (targetEventTypePair.Value)
            {
                case EventManager.DeerLevelEvent.WolfCatchDeer:
                    {
                        WolfEatDeer(targets[0], targets[1]);
                        break;
                    }
                case EventManager.DeerLevelEvent.PlayerAttackWolf:
                    {
                        WolfFlee(targets[0]);
                        break;
                    }
                case EventManager.DeerLevelEvent.ArriveAtDestination:
                    {
                        doorToNextLevel.ToggleDoor(true);
                        arrivedAtDestination = true;
                        deerTrackingCamera.enabled = false;
                        foreach (GameObject deer in activeDeers.Values)
                        {
                            deer.GetComponent<FollowerDeerBehaviour>().Target = null;
                        }
                        foreach (GameObject deer in inactiveDeers)
                        {
                            Destroy(deer);
                        }
                        foreach (GameObject wolf in activeWolves.Values)
                        {
                            Destroy(wolf);
                        }
                        foreach (GameObject wolf in inactiveWolves)
                        {
                            Destroy(wolf);
                        }
                        break;
                    }
            }
        }
    }

    void WolfEatDeer(GameObject wolf, GameObject deer)
    {
        deer.GetComponent<FollowerDeerBehaviour>().CaughtByWolf();
        activeDeers.Remove(deer.GetInstanceID());
        activeWolves.Remove(wolf.GetInstanceID());
        inactiveDeers.Add(deer);
        inactiveWolves.Add(wolf);
        leaderDeer.GetComponent<LeaderDeerBehaviour>().DecreseSpeed(speedDecrement);
        foreach (GameObject follower in activeDeers.Values)
        {
            follower.GetComponent<FollowerDeerBehaviour>().DecreseSpeed(speedDecrement);
        }
        StartCoroutine(ShowBiteLogoCoroutine(wolf.transform.position + biteLogoShowOffset));
    }

    void WolfFlee(GameObject wolf)
    {
        activeWolves.Remove(wolf.GetInstanceID());
        inactiveWolves.Add(wolf);
        StartCoroutine(WolfFleeCoroutine(wolf));
    }

    IEnumerator ShowBiteLogoCoroutine(Vector3 position)
    {
        GameObject biteLogo = Instantiate(bitingLogoPrefab);
        biteLogo.transform.parent = transform;
        biteLogo.transform.position = position;
        yield return new WaitForSeconds(biteLogoDisappearIn);
        Destroy(biteLogo);
    }

    IEnumerator WolfFleeCoroutine(GameObject wolf)
    {
        yield return new WaitForSeconds(wolfFleeDisappearIn);
        Destroy(wolf);
    }

    void Update()
    {
        if (!Level.Deer.Equals(player.GetComponent<CharacterControlV2>().GetCurrentLevel()))
        {
            return;
        }
        ProcessEvent();
        if (!journeyStarted)
        {
            if (player.transform.position.x - playerTriggerPositon.x > allowedDistanceBeforeStartJourney)
            {
                EventManager.Instance.InvokeDeerLevelEvent(null, EventManager.DeerLevelEvent.PlayerTooFarFromDeers);
                StartCoroutine(ResetGame());
            }
            if (Utils.DistanceToTargetWithinThreshold(player.transform.position, playerTriggerPositon, targetReachedSquaredDistance))
            {
                StartDeersJoruney();
                EventManager.Instance.InvokeDeerLevelEvent(null, EventManager.DeerLevelEvent.StartJourney);
            }
        }
        else
        {
            if (!arrivedAtDestination)
            {
                WolfAppear();
                SetWolfTargetDeer();
                if (Utils.DistanceToTargetAboveThreshold(player.transform.position, leaderDeer.transform.position, maxSquaredDistanceFromDeerGroup))
                {
                    EventManager.Instance.InvokeDeerLevelEvent(null, EventManager.DeerLevelEvent.PlayerTooFarFromDeers);
                    StartCoroutine(ResetGame());
                }
                else if (activeDeers.Count == 0)
                {
                    EventManager.Instance.InvokeDeerLevelEvent(null, EventManager.DeerLevelEvent.NotEnoughDeersLeft);
                    StartCoroutine(ResetGame());
                }
            }
        }
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(1f);
        foreach (GameObject follower in activeDeers.Values)
        {
            Destroy(follower);
        }
        foreach (GameObject deer in inactiveDeers)
        {
            Destroy(deer);
        }
        Destroy(leaderDeer);
        foreach (GameObject wolf in activeWolves.Values)
        {
            Destroy(wolf);
        }
        foreach (GameObject wolf in inactiveWolves)
        {
            Destroy(wolf);
        }
        leaderDeer = null;
        activeDeers.Clear();
        inactiveDeers.Clear();
        activeWolves.Clear();
        inactiveWolves.Clear();
        PlaceDeersInPosition();
        player.transform.position = new Vector3(playerTriggerPositon.x, player.transform.position.y, playerTriggerPositon.z - 5f);
        journeyStarted = false;
        deerTrackingCamera.enabled = false;
    }

    void StartDeersJoruney()
    {
        journeyStarted = true;
        SetupCamera();
        LeaderDeerBehaviour leaderDeerBehaviour = leaderDeer.GetComponent<LeaderDeerBehaviour>();
        leaderDeerBehaviour.StartMove(waypoints);
        foreach (GameObject follower in activeDeers.Values)
        {
            FollowerDeerBehaviour followerDeerBehaviour = follower.GetComponent<FollowerDeerBehaviour>();
            followerDeerBehaviour.Target = leaderDeer;
        }
    }

    private void SetupCamera()
    {
        deerTrackingCamera.Follow = leaderDeer.transform;
        deerTrackingCamera.LookAt = leaderDeer.transform;
        deerTrackingCamera.enabled = true;
    }

    void SetWolfTargetDeer()
    {
        if (!activeDeers.ContainsKey(deerTargetId) || !activeDeers[deerTargetId].activeSelf)
        {
            List<int> deerId = new List<int>(activeDeers.Keys);
            if (deerId.Count > 0)
            {
                deerTargetId = deerId[UnityEngine.Random.Range(0, deerId.Count)];
            }
            else
            {
                return; // no more deers to assign as target, exist early
            }
        }
        foreach (GameObject wolf in activeWolves.Values)
        {
            wolf.GetComponent<WolfBehaviour>().DeerTarget = activeDeers[deerTargetId];
        }
    }

    void WolfAppear()
    {
        if (wolfAttackTimer > 0)
        {
            wolfAttackTimer -= Time.deltaTime;
        }

        if (wolfAttackTimer <= 0 && UnityEngine.Random.value < wolfAttackProbability)
        {
            GameObject wolf = Instantiate(wolfPrefab);
            activeWolves.TryAdd(wolf.GetInstanceID(), wolf);
            int randomSign = 1;
            if (wolfAppearInBothDirection)
            {
                randomSign = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            }
            EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { wolf }, EventManager.DeerLevelEvent.WolfAppear);
            wolf.transform.position = Utils.JitterPosition(leaderDeer.transform.position + randomSign * Vector3.left * wolfAppearDistance, 10f);
            wolf.transform.parent = transform;
            wolfAttackTimer = wolfAttackInterval;
        }
    }

    GameObject SpawnDeer(Vector3 position)
    {
        GameObject newDeer = Instantiate(deerPrefab);
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
                }
                else
                {
                    activeDeers.TryAdd(deer.GetInstanceID(), deer);
                    FollowerDeerBehaviour followerDeerBehaviour = deer.GetComponent<FollowerDeerBehaviour>();
                    followerDeerBehaviour.enabled = true;
                    followerDeerBehaviour.Target = null;
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
