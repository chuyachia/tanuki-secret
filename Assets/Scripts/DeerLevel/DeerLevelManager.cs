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

    void Start()
    {
        PlaceObjectsInPascalTriangle();
    }

    void Update()
    {

    }

    void PlaceObjectsInPascalTriangle()
    {
        int remainingObjects = numberOfDeers;
        int row = 1;
        Vector3 originPosition = transform.position;
        GameObject leaderDeer = null;

        while (remainingObjects > 0)
        {
            int objectsInRow = row;
            int objectsToPlace = Math.Min(objectsInRow, remainingObjects);

            for (int i = 0; i < objectsToPlace; i++)
            {
                Vector3 position = originPosition - new Vector3(i * spacingHorizontal - (objectsInRow - 1) * spacingHorizontal / 2f, 0f, (row - 1) * spacingVertical);
                if (row == 1)
                {
                    leaderDeer = Instantiate(leaderDeerPrefab, position, Quaternion.identity);
                    LeaderDeerBehaviour leaderDeerBehaviour = leaderDeer.GetComponent<LeaderDeerBehaviour>();
                    leaderDeerBehaviour.StartJourney(waypoints);
                    leaderDeer.transform.parent = transform;
                }
                else
                {
                    GameObject follwerDeer = Instantiate(followerPrefab, position, Quaternion.identity);
                    FollowerDeerBehaviour followerDeerBehaviour = follwerDeer.GetComponent<FollowerDeerBehaviour>();
                    followerDeerBehaviour.Leader = leaderDeer;
                    followerDeerBehaviour.deerPositionParamters = new DeerPositionParamters(row, i, objectsInRow, spacingHorizontal, spacingVertical);
                    followerDeerBehaviour.transform.parent = transform;
                }
            }

            remainingObjects -= objectsToPlace;
            row++;
        }
    }
}
