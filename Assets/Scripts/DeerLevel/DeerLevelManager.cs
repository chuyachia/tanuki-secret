using System;
using System.Collections.Generic;
using UnityEngine;

// TODO Trigger start journey only when player get to position
// Detect if player falls out of group
// wolf attach
public class DeerLevelManager : MonoBehaviour
{

    [SerializeField] private GameObject leaderDeerPrefab;
    [SerializeField] private GameObject followerPrefab;
    [SerializeField] private List<GameObject> waypoints;
    [SerializeField] private int numberOfDeers = 5;
    [SerializeField] private float spacingHorizontal = 2f;
    [SerializeField] private float spacingVertical = 2f;
    [SerializeField] private GameObject player;
    [SerializeField] private float targetReachedSquaredDistance = 2f;
    [SerializeField] private float maxSquaredDistanceFromDeerGroup = 6f;

    private List<GameObject> deers;

    private Vector3 playerTriggerPositon = Vector3.zero;
    private bool journeyStarted = false;

    void Start()
    {
        deers = new List<GameObject>();
        PlaceDeersInPosition();
        journeyStarted = false;
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
            if (Utils.DistanceToTargetAboveThreshold(player.transform.position, deers[0].transform.position, maxSquaredDistanceFromDeerGroup))
            {
                RestartJourney();
            }
        }
    }

    void RestartJourney()
    {
        Debug.Log("Strayed away from the group");
        PlaceDeersInPosition();
        player.transform.position = new Vector3(playerTriggerPositon.x, player.transform.position.y, playerTriggerPositon.z - 5f);
        journeyStarted = false;
    }

    void StartDeersJoruney()
    {
        Debug.Log("Start the journey");
        journeyStarted = true;
        GameObject leaderDeer = null;

        for (int i = 0; i < deers.Count; i++)
        {
            if (i == 0)
            {
                leaderDeer = deers[i];
                LeaderDeerBehaviour leaderDeerBehaviour = leaderDeer.GetComponent<LeaderDeerBehaviour>();
                leaderDeerBehaviour.StartMove(waypoints);
            }
            else
            {
                FollowerDeerBehaviour followerDeerBehaviour = deers[i].GetComponent<FollowerDeerBehaviour>();
                followerDeerBehaviour.Leader = leaderDeer;
            }
        }
    }

    GameObject PlaceDeerOrInstantiateToPosition(int n, Vector3 position)
    {
        if (deers.Count > n)
        {
            deers[n].transform.position = position;
            return deers[n];
        }
        else
        {
            GameObject newDeer;
            if (n == 0)
            {
                newDeer = Instantiate(leaderDeerPrefab, position, Quaternion.identity);
            }
            else
            {
                newDeer = Instantiate(followerPrefab, position, Quaternion.identity);
            }
            newDeer.transform.parent = transform;
            deers.Add(newDeer);
            return newDeer;
        }
    }

    void PlaceDeersInPosition()
    {
        int remainingObjects = numberOfDeers;
        int row = 1;
        Vector3 originPosition = transform.position;
        int nDeersPlaced = 0;
        while (remainingObjects > 0)
        {
            int objectsInRow = row;
            int objectsToPlace = Math.Min(objectsInRow, remainingObjects);

            for (int i = 0; i < objectsToPlace; i++)
            {
                Vector3 position = originPosition - new Vector3(i * spacingHorizontal - (objectsInRow - 1) * spacingHorizontal / 2f, 0f, (row - 1) * spacingVertical);
                if (row == 1)
                {
                    GameObject leaderDeer = PlaceDeerOrInstantiateToPosition(nDeersPlaced, position);
                    LeaderDeerBehaviour leaderDeerBehaviour = leaderDeer.GetComponent<LeaderDeerBehaviour>();
                    leaderDeerBehaviour.ResetMoveState();
                    // GameObject leaderDeer = Instantiate(leaderDeerPrefab, position, Quaternion.identity);
                    // leaderDeer.transform.parent = transform;
                    // deers.Add(leaderDeer);
                }
                else
                {
                    GameObject followerDeer = PlaceDeerOrInstantiateToPosition(nDeersPlaced, position);
                    FollowerDeerBehaviour followerDeerBehaviour = followerDeer.GetComponent<FollowerDeerBehaviour>();
                    followerDeerBehaviour.Leader = null;
                    followerDeerBehaviour.deerPositionParamters = new DeerPositionParamters(row, i, objectsInRow, spacingHorizontal, spacingVertical);
                    // followerDeerBehaviour.transform.parent = transform;
                    // deers.Add(followerDeer);
                }
                nDeersPlaced++;
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
