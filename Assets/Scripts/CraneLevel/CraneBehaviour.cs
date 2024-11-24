using System.Collections.Generic;
using UnityEngine;

public class CraneBehaviour : MonoBehaviour
{
    [SerializeField] private float beatTime = 1.2f;
    [SerializeField] private float bufferTimeBeforeNextMove = 0.2f;
    [SerializeField] private float sideDistance = 2f;
    private List<DanceCommand> danseCommands;
    private CraneDanceAnimator craneDanseAnimator;
    private int currentDanseMove = 0;
    private float timerToExitCurrentMove = 0f;
    private float timerToNextMove = 0f;
    private Vector3 targetPosition;
    private float danseMoveDuration;
    private bool isDanceMode;

    void Start()
    {
        craneDanseAnimator = new CraneDanceAnimator(transform.GetChild(0).gameObject);
        danseCommands = new List<DanceCommand>();
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.SIDE_LEFT), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.SIDE_LEFT), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2));

        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.SIDE_RIGHT), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.SIDE_RIGHT), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2));

        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 1));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 1));

        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.ROLLED, BodyPosition.DOWN), 2));
        danseCommands.Add(new DanceCommand(new DanceMove(WingPosition.DEPLOYED, BodyPosition.UP), 2));
        EventManager.Instance.RegisterCraneLevelEventListener(HandleEvent);
    }

    void OnDestroy()
    {
        EventManager.Instance.UnregisterCraneLevelEvenListener(HandleEvent);
    }

    public void HandleEvent(GameObject[] gameObjects, EventManager.CraneLevelEvent eventType)
    {
        if (eventType == EventManager.CraneLevelEvent.StartDance)
        {
            isDanceMode = true;
        }
    }

    void Update()
    {
        if (!isDanceMode)
        {
            return;
        }

        if (currentDanseMove >= danseCommands.Count)
        {
            currentDanseMove = 0;
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
            DanceCommand danseCommand = danseCommands[currentDanseMove];
            craneDanseAnimator.Danse(danseCommand.Move);
            timerToNextMove = beatTime * danseCommand.Beat;
            danseMoveDuration = timerToNextMove - bufferTimeBeforeNextMove;
            timerToExitCurrentMove = danseMoveDuration;
            Vector3 targetDirection = Vector3.zero;
            if (BodyPosition.SIDE_RIGHT == danseCommand.Move.BodyPosition)
            {
                targetDirection = Vector3.left;
            }
            else if (BodyPosition.SIDE_LEFT == danseCommand.Move.BodyPosition)
            {
                targetDirection = Vector3.right;
            }
            targetPosition = transform.position + targetDirection * sideDistance;
            currentDanseMove++;
        }
        else if (timerToExitCurrentMove <= 0)
        {
            craneDanseAnimator.Danse(DanceMove.NoMove);
        }
        else
        {
            float t = Mathf.Clamp01((danseMoveDuration - timerToExitCurrentMove) / danseMoveDuration);
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);
        }
    }
}
