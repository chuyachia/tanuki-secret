using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject cranePrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private int numberOfCrane = 5;
    [SerializeField] private float craneCircleRadius = 6f;
    [SerializeField] private float beatTime = 1.2f;
    [SerializeField] private float bufferTimeBeforeNextMove = 0.2f;
    [SerializeField] private float sideDistance = 2f;
    [SerializeField] private float playerAllowedDistacneFromDancePosition = 64f;

    private List<GameObject> cranes;
    private List<List<DanceCommand>> dances;
    private bool isDancing = false;
    private Vector3 playerPositionInDanceCircle;
    private int playerCorrectMoves = 0;
    private int currentDance = 0;
    private int nextDanseCommandIndex = 0;
    private float timerToExitCurrentMove = 0f;
    private float timerToNextMove = 0f;
    private float danceMoveDuration;
    private DanceMove currentTargetMove;
    private DanceMove currentPlayerMove;
    private bool shouldTrackPlayerMove;
    private bool levelCompleted;

    void Start()
    {
        cranes = new List<GameObject>();
        float anglePerObject = 360f / (numberOfCrane + 1);
        List<Vector3> positions = Utils.GetCircleAroundTargetPosition(numberOfCrane + 1, transform.position, 0f, 360f - anglePerObject, craneCircleRadius);
        for (int i = 0; i < numberOfCrane; i++)
        {
            GameObject crane = Instantiate(cranePrefab);
            crane.transform.parent = transform;
            crane.transform.position = positions[i];
            cranes.Add(crane);
        }
        playerPositionInDanceCircle = positions[positions.Count - 1];
        InitializeDanceCommands();
    }

    void InitializeDanceCommands()
    {
        dances = new List<List<DanceCommand>>();
        List<DanceCommand> tutorialBodyUpDown = new List<DanceCommand>(){
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.UP), 2),
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 2),
        };
        dances.Add(tutorialBodyUpDown);
        List<DanceCommand> tutorialBodyLeftRight = new List<DanceCommand>(){
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.LEFT), 2),
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.RIGHT), 2),
        };
        dances.Add(tutorialBodyLeftRight);
        List<DanceCommand> tutorialWingDeployedRoll = new List<DanceCommand>(){
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.NEUTRAL), 2),
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.NEUTRAL), 2),
        };
        dances.Add(tutorialWingDeployedRoll);
        List<DanceCommand> complextDance = new List<DanceCommand>(){
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.LEFT), 1),
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.LEFT), 1),
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2),

            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.RIGHT), 1),
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.RIGHT), 1),
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2),

            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 1),
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 1),
            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 1),
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 1),

            new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 2),
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2)
        };
        dances.Add(complextDance);
    }

    void Update()
    {
        if (!isDancing)
        {
            if (Utils.DistanceToTargetWithinThreshold(player.transform.position, playerPositionInDanceCircle, 4f))
            {
                EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.StartDance);
                player.transform.position = playerPositionInDanceCircle;
                isDancing = true;
                currentPlayerMove = DanceMove.NoMove;
            }
        }
        else if (Utils.DistanceToTargetAboveThreshold(player.transform.position, playerPositionInDanceCircle, playerAllowedDistacneFromDancePosition))
        {
            EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.PlayerTooFarFromCranes);
            StartCoroutine(ResetPlayerPosition());
        }
        else if (!levelCompleted)
        {
            List<DanceCommand> danceCommands = dances[currentDance];
            if (nextDanseCommandIndex >= danceCommands.Count)
            {
                nextDanseCommandIndex = 0;
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
                DanceCommand danceCommand = danceCommands[nextDanseCommandIndex];
                timerToNextMove = beatTime * danceCommand.Beat;
                danceMoveDuration = timerToNextMove - bufferTimeBeforeNextMove;
                timerToExitCurrentMove = danceMoveDuration;
                currentTargetMove = danceCommand.Move;
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
                playerPositionInDanceCircle += targetDirection * sideDistance;
                nextDanseCommandIndex++;
            }
            else if (timerToExitCurrentMove <= 0)
            {
                foreach (GameObject crane in cranes)
                {
                    CraneBehaviour craneBehaviour = crane.GetComponent<CraneBehaviour>();
                    craneBehaviour.Dance(DanceMove.NoMove);
                }
            }
            DanceMove playerMove = DanceMove.FromUserInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKey(KeyCode.Space));
            if (playerMove.Equals(DanceMove.NoMove))
            {
                shouldTrackPlayerMove = true;
            }
            else if (shouldTrackPlayerMove)
            {
                if (playerMove.Equals(currentTargetMove))
                {
                    playerCorrectMoves++;
                    Debug.Log("correct move");
                    if (playerCorrectMoves == danceCommands.Count)
                    {
                        Debug.Log("Next dance");
                        playerCorrectMoves = 0;
                        nextDanseCommandIndex = 0;
                        currentDance++;
                        if (currentDance == dances.Count)
                        {
                            LevelCompleted();
                        }
                    }
                }
                else
                {
                    playerCorrectMoves = 0;
                    Debug.Log("wrong move");
                }
                shouldTrackPlayerMove = false;

            }
        }
    }

    IEnumerator ResetPlayerPosition()
    {
        yield return new WaitForSeconds(1f);
        player.transform.position = playerPositionInDanceCircle;
    }

    void LevelCompleted()
    {
        Debug.Log("Level completed");
        levelCompleted = true;
        foreach (GameObject crane in cranes)
        {
            CraneBehaviour craneBehaviour = crane.GetComponent<CraneBehaviour>();
            craneBehaviour.Dance(DanceMove.NoMove);
        }
    }
}
