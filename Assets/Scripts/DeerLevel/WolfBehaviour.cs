using UnityEngine;

public class WolfBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance = 4f;
    [SerializeField] private float fleeDistance = 20f;

    private Animator animator;
    private bool targetIsDeer;
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

    public Vector3 FleeTarget
    {
        set
        {
            targetPosition = value;
            targetIsDeer = false;
        }
    }

    void OnEnable()
    {
        targetIsDeer = false;
        caughtDeer = false;
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
            if (targetIsDeer && !caughtDeer)
            {
                EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { gameObject, target }, EventManager.DeerLevelEvent.WolfCatchDeer);
                targetPosition = transform.position;
                animator.SetBool(Constants.AnimatorState.IsAttacking, true);
                caughtDeer = true;
            }

        }
    }

    protected override bool ShouldJump()
    {
        return false;
    }
}