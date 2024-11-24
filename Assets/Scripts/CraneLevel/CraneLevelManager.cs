using System.Collections.Generic;
using UnityEngine;

public class CraneLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject cranePrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private int numberOfCrane = 5;
    [SerializeField] private float craneCircleRadius = 6f;

    private Vector3 playerDanceTriggerPoint;
    void Start()
    {
        float anglePerObject = 360f / (numberOfCrane + 1);
        List<Vector3> positions = Utils.GetCircleAroundTargetPosition(numberOfCrane + 1, transform.position, 0f, 360f - anglePerObject, craneCircleRadius);
        for (int i = 0; i < numberOfCrane; i++)
        {
            GameObject crane = Instantiate(cranePrefab);
            crane.transform.parent = transform;
            crane.transform.position = positions[i];
        }
        playerDanceTriggerPoint = positions[positions.Count - 1];
    }

    void Update()
    {
        if (Utils.DistanceToTargetWithinThreshold(player.transform.position, playerDanceTriggerPoint, 4f))
        {
            EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.StartDance);
        }
    }
}
