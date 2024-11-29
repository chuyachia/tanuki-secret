using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    [SerializeField] private float correctMoveTolerance = 0.5f;
    [SerializeField] private float inputThrottleDuration = 0.5f;
    [SerializeField] private EndingManager endingManager;
    [SerializeField] private CinemachineVirtualCamera craneTrackingCamera;

    private List<GameObject> cranes;
    private List<List<DanceCommand>> dances;
    private bool isDancing = false;
    private Vector3 playerPositionInDanceCircle;
    private int playerCorrectMoves = 0;
    private int currentDance = 0;
    private int danseCommandIndex = 0;
    private float timerToExitCurrentMove = 0f;
    private float timerToNextMove = 0f;
    private float danceMoveDuration;
    private DanceMove currentTargetMove;
    private DanceMove nextTargetMove;
    private DanceMove throttledPlayerInput = DanceMove.NoMove;
    private bool shouldTrackPlayerMove;
    private bool levelCompleted;
    private float inputThrottleTimer;

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
            new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.UP), 2),
            new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.DOWN), 2),
        };
        dances.Add(tutorialBodyUpDown);
        List<DanceCommand> tutorialBodyLeftRight = new List<DanceCommand>(){
            new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.LEFT), 2),
            new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.RIGHT), 2),
        };
        dances.Add(tutorialBodyLeftRight);
        List<DanceCommand> tutorialWingDeployedRoll = new List<DanceCommand>(){
            new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2),
            new DanceCommand(new DanceMove(WingPosition.NEUTRAL, BodyPosition.NEUTRAL), 2),
        };
        dances.Add(tutorialWingDeployedRoll);
        List<DanceCommand> complextDance = new List<DanceCommand>(){
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
        dances.Add(complextDance);
    }

    void Update()
    {
        if (levelCompleted)
        {
            return;
        }

        if (!isDancing)
        {
            if (Utils.DistanceToTargetWithinThreshold(player.transform.position, playerPositionInDanceCircle, 4f))
            {
                EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.StartDance);
                player.transform.position = playerPositionInDanceCircle;
                isDancing = true;
                craneTrackingCamera.enabled = true;
            }
        }
        else if (Utils.DistanceToTargetAboveThreshold(player.transform.position, playerPositionInDanceCircle, playerAllowedDistacneFromDancePosition))
        {
            EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.PlayerTooFarFromCranes);
            StartCoroutine(ResetPlayerPosition());
        }
        else
        {
            List<DanceCommand> danceCommands = dances[currentDance];
            if (danseCommandIndex >= danceCommands.Count)
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
                DanceCommand danceCommand = danceCommands[danseCommandIndex];
                timerToNextMove = beatTime * danceCommand.Beat;
                danceMoveDuration = timerToNextMove - bufferTimeBeforeNextMove;
                timerToExitCurrentMove = danceMoveDuration;
                currentTargetMove = danceCommand.Move;
                nextTargetMove = danceCommands[(danseCommandIndex + 1) % danceCommands.Count].Move;
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
                    EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.OtherCranesMove);

                    if (targetDirection != Vector3.zero)
                    {
                        Vector3 targetPosition = crane.transform.position + targetDirection * sideDistance;
                        craneBehaviour.MoveTo(targetPosition, timerToNextMove - bufferTimeBeforeNextMove);
                    }
                }
                playerPositionInDanceCircle += targetDirection * sideDistance;
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
            DanceMove playerInput = DanceMove.FromUserInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKey(KeyCode.Space));
            if (playerInput.Equals(DanceMove.NoMove))
            {
                shouldTrackPlayerMove = true;
                inputThrottleTimer = 0f;
            }
            else if (shouldTrackPlayerMove)
            {
                if (inputThrottleTimer < inputThrottleDuration)
                {
                    inputThrottleTimer += Time.deltaTime;
                    throttledPlayerInput = throttledPlayerInput.merge(playerInput);
                }
                else
                {
                    if (throttledPlayerInput.Equals(currentTargetMove) || throttledPlayerInput.Equals(nextTargetMove) && timerToNextMove < correctMoveTolerance)
                    {
                        playerCorrectMoves++;
                        Debug.Log("correct move");
                        EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.CorrectMove);
                        throttledPlayerInput = DanceMove.NoMove;
                        if (playerCorrectMoves == danceCommands.Count)
                        {
                            Debug.Log("Next dance");
                            playerCorrectMoves = 0;
                            danseCommandIndex = 0;
                            throttledPlayerInput = DanceMove.NoMove;
                            currentDance++;
                            if (currentDance == dances.Count)
                            {
                                levelCompleted = true;
                                EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.LevelCompleted);
                                StartCoroutine(CleanUpAndLaunchEnding());
                            }
                        }
                    }
                    else
                    {
                        playerCorrectMoves = 0;
                        EventManager.Instance.InvokeCraneLevelEvent(new GameObject[] { }, EventManager.CraneLevelEvent.WrongMove);
                        throttledPlayerInput = DanceMove.NoMove;
                        Debug.Log("wrong move");
                    }
                    throttledPlayerInput = DanceMove.NoMove;
                    shouldTrackPlayerMove = false;
                }

            }
        }
    }

    IEnumerator ResetPlayerPosition()
    {
        yield return new WaitForSeconds(1f);
        player.transform.position = playerPositionInDanceCircle;
    }

    IEnumerator CleanUpAndLaunchEnding()
    {
        craneTrackingCamera.enabled = false; // check if necessary
        yield return new WaitForSeconds(1f);
        foreach (GameObject crane in cranes)
        {
            Destroy(crane);
        }
        Destroy(player);
        endingManager.ChooseEndingCutscene();
    }
}
