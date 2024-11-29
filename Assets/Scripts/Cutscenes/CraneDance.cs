using System.Collections.Generic;
using UnityEngine;

public class CraneDance : MonoBehaviour
{
    [SerializeField] private GameObject cranePrefab;
    [SerializeField] private int numberOfCrane = 5;
    [SerializeField] private float craneCircleRadius = 6f;
    [SerializeField] private float beatTime = 1.2f;
    [SerializeField] private float bufferTimeBeforeNextMove = 0.2f;
    [SerializeField] private float sideDistance = 2f;
    private List<GameObject> cranes = new List<GameObject>();
    private int danseCommandIndex;
    private float timerToExitCurrentMove = 0f;
    private float timerToNextMove = 0f;

    private List<DanceCommand> finalDance = new List<DanceCommand>(){
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.LEFT), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.LEFT), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.UP), 2),

        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.RIGHT), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.RIGHT), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.UP), 2),

        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.DOWN), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.UP), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.DOWN), 1),
        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.UP), 1),

        new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.DOWN), 2),
        new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2)
    };

    void Start()
    {
        float anglePerObject = 360f / numberOfCrane;
        List<Vector3> positions = Utils.GetCircleAroundTargetPosition(numberOfCrane, transform.position, 0f, 360f - anglePerObject, craneCircleRadius);
        for (int i = 0; i < numberOfCrane; i++)
        {
            GameObject crane = Instantiate(cranePrefab);
            crane.transform.parent = transform;
            crane.transform.position = positions[i];
            cranes.Add(crane);
        }
    }

    void Update()
    {
        if (danseCommandIndex >= finalDance.Count)
        {
            danseCommandIndex = 0;
        }
        if (timerToExitCurrentMove > 0)
        {
            timerToExitCurrentMove -= Time.deltaTime;
        }
        if (timerToNextMove > 0)
        {
            timerToNextMove -= Time.deltaTime;
        }
        if (timerToNextMove <= 0 && timerToExitCurrentMove <= 0)
        {
            DanceCommand danceCommand = finalDance[danseCommandIndex];
            timerToNextMove = beatTime * danceCommand.Beat;
            timerToExitCurrentMove = timerToNextMove - bufferTimeBeforeNextMove;
            Vector3 targetDirection = Vector3.zero;
            if (BodyPosition.RIGHT == danceCommand.Move.BodyPosition)
            {
                targetDirection = Vector3.right;
            }
            else if (BodyPosition.LEFT == danceCommand.Move.BodyPosition)
            {
                targetDirection = Vector3.left;
            }
            foreach (GameObject crane in cranes)
            {
                CraneBehaviour craneBehaviour = crane.GetComponent<CraneBehaviour>();
                craneBehaviour.Dance(danceCommand.Move);
                if (targetDirection != Vector3.zero)
                {
                    Vector3 targetPosition = crane.transform.position + targetDirection * sideDistance;
                    craneBehaviour.MoveTo(targetPosition, timerToNextMove - bufferTimeBeforeNextMove);
                }
            }
            danseCommandIndex++;
        }
        else if (timerToExitCurrentMove <= 0)
        {
            foreach (GameObject crane in cranes)
            {
                CraneBehaviour craneBehaviour = crane.GetComponent<CraneBehaviour>();
                craneBehaviour.Dance(DanceMove.NoMove);
            }
        }
    }
}
