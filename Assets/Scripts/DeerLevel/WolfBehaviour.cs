using System.Collections;
using UnityEngine;

public class WolfBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance = 2f;

    private Animator animator;

    public GameObject Target
    {
        set
        {
            target = value;
        }
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (target == null || !target.activeSelf)
        {
            return;
        }
        if (Utils.DistanceToTargetWithinThreshold(transform.position, target.transform.position, targetReachedSquaredDistance))
        {
            EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { gameObject, target }, EventManager.DeerLevelEvent.WolfCatchDeer);
            target = null;
            animator.SetBool(Constants.AnimatorState.IsAttacking, true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tags.Player))
        {
            EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { gameObject }, EventManager.DeerLevelEvent.PlayerKillWolf);
        }
    }

    protected override bool ShouldJump()
    {
        return false;
    }
}