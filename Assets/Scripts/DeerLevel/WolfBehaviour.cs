using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WolfBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance = 4f;
    [SerializeField] private float fleeDistance = 20f;

    private Animator animator;
    private bool targetIsDeer;
    private bool isFleeing;
    private bool caughtDeer;
    private GameObject target;

    public GameObject DeerTarget
    {
        set
        {
            target = value;
            if (value == null)
            {
                targetPosition = transform.position;
            }
            else
            {
                targetPosition = value.transform.position;
                targetIsDeer = true;
            }
        }
    }

    void OnEnable()
    {
        caughtDeer = false;
        isFleeing = false;
        targetIsDeer = false;
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Utils.DistanceToTargetWithinThreshold(transform.position, targetPosition, targetReachedSquaredDistance))
        {
            if (targetIsDeer && !caughtDeer && !isFleeing)
            {
                EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { gameObject, target }, EventManager.DeerLevelEvent.WolfCatchDeer);
                targetPosition = transform.position;
                animator.SetBool(Constants.AnimatorState.IsAttacking, true);
                caughtDeer = true;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tags.Player) && !isFleeing && !caughtDeer)
        {
            EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { gameObject }, EventManager.DeerLevelEvent.WolfFlee);
            isFleeing = true;
            Vector3 directionAwayFromCollider = transform.position - other.transform.position;
            directionAwayFromCollider.Normalize();
            targetPosition = directionAwayFromCollider * fleeDistance;
            targetIsDeer = false;
        }
    }

    protected override bool ShouldJump()
    {
        return false;
    }
}