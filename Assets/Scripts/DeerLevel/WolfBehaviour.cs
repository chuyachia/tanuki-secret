using Unity.VisualScripting;
using UnityEngine;

public class WolfBehaviour : TargetBasedSteerBehaviour
{
    [SerializeField] private float targetReachedSquaredDistance = 2f;

    public GameObject Target
    {
        set
        {
            target = value;
        }
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
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.Tags.Player))
        {
            EventManager.Instance.InvokeDeerLevelEvent(new GameObject[] { gameObject }, EventManager.DeerLevelEvent.PlayerKillWolf);
        }
    }

    protected override bool ShouldMove()
    {
        return target != null;
    }

    protected override bool ShouldJump()
    {
        return false;
    }
}