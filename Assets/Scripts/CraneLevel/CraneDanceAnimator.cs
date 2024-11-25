using UnityEngine;

public partial class CraneDanceAnimator
{
    private const string WingModel = "Crane_wings";
    private const string BodyModel = "Crane_Body";
    private Vector3 craneWingOffSet = new Vector3(-0.2f, -0.7f, -0.7f);
    private Transform wing;
    private Vector3 initialWingPosition = Vector3.zero;

    private Animator wingAnimator;
    private Animator bodyAnimator;

    public CraneDanceAnimator(GameObject craneModel)
    {
        foreach (Transform child in craneModel.transform)
        {
            if (child.gameObject.name == WingModel)
            {
                wing = child;
                break;
            }
        }
        initialWingPosition = wing.localPosition;
        Animator[] animators = craneModel.GetComponentsInChildren<Animator>();
        foreach (Animator anim in animators)
        {
            if (anim.gameObject.name == WingModel)
            {
                wingAnimator = anim;
            }
            if (anim.gameObject.name == BodyModel)
            {
                bodyAnimator = anim;
            }
        }
    }

    public void Danse(DanceMove danseMove)
    {
        switch (danseMove.WingPosition)
        {
            case WingPosition.DEPLOYED:
                {
                    wingAnimator?.SetBool(Constants.AnimatorState.WingDeployed, true);
                    break;
                }
            case WingPosition.ROLLED:
                {
                    wingAnimator?.SetBool(Constants.AnimatorState.WingDeployed, false);
                    break;
                }
        }

        Vector3 desiredWingPosition = wing.localPosition;
        switch (danseMove.BodyPosition)
        {
            case BodyPosition.UP:
                {
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, true);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, false);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, false);
                    desiredWingPosition = initialWingPosition;
                    break;
                }
            case BodyPosition.DOWN:
                {
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, true);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, false);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, false);
                    desiredWingPosition = initialWingPosition + craneWingOffSet;
                    break;
                }
            case BodyPosition.SIDE_LEFT:
            case BodyPosition.SIDE_RIGHT:
                {
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, true);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, false);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, false);
                    desiredWingPosition = initialWingPosition;
                    break;
                }
            case BodyPosition.NEUTRAL:
                {
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, false);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, false);
                    bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, false);
                    desiredWingPosition = initialWingPosition;
                    break;
                }
        }
        if (wing.localPosition != desiredWingPosition)
        {
            wing.localPosition = desiredWingPosition;
        }
    }

    public void AnimateWing(bool inputSpace)
    {
        if (inputSpace)
        {
            wingAnimator?.SetBool(Constants.AnimatorState.WingDeployed, true);
        }
        else
        {
            wingAnimator?.SetBool(Constants.AnimatorState.WingDeployed, false);
        }
    }

    public void AnimateBodyAndMoveWing(float inputHorizontal, float inputVertical)
    {
        Vector3 desiredWingPosition;
        if (inputVertical > 0)
        {
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, true);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, false);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, false);
            desiredWingPosition = initialWingPosition;
        }
        else if (inputVertical < 0)
        {
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, true);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, false);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, false);
            desiredWingPosition = initialWingPosition + craneWingOffSet;

        }
        else if (inputHorizontal != 0)
        {
            bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, true);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, false);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, false);
            if (inputHorizontal > 0)
            {
                desiredWingPosition = initialWingPosition;
            }
            else
            {
                desiredWingPosition = initialWingPosition;
            }
        }
        else
        {
            bodyAnimator?.SetBool(Constants.AnimatorState.BodySide, false);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyDown, false);
            bodyAnimator?.SetBool(Constants.AnimatorState.BodyUp, false);
            desiredWingPosition = initialWingPosition;
        }
        if (wing.localPosition != desiredWingPosition)
        {
            wing.localPosition = desiredWingPosition;
        }
    }
}