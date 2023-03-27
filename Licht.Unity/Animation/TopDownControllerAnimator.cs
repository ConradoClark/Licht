using System;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using UnityEngine;

public class TopDownControllerAnimator : BaseGameObject
{
    [field:SerializeField]
    public LichtTopDownMoveController MoveController { get; private set; }

    [field: SerializeField]
    public Animator Animator { get; private set; }

    [field: SerializeField] public string WalkingProperty { get; private set; } = "Walking";
    [field: SerializeField] public string DirectionXProperty { get; private set; } = "DirectionX";
    [field: SerializeField] public string DirectionYProperty { get; private set; } = "DirectionY";

    [field: SerializeField] public bool TriggerWalking { get; private set; } = true;
    [field: SerializeField] public bool TriggerDirection { get; private set; } = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        MoveController.OnStartMoving += MoveController_OnStartMoving;
        MoveController.OnStopMoving+= MoveControllerOnOnStopMoving;
        MoveController.OnChangeDirection += MoveController_OnChangeDirection;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        MoveController.OnStartMoving -= MoveController_OnStartMoving;
        MoveController.OnStopMoving -= MoveControllerOnOnStopMoving;
        MoveController.OnChangeDirection -= MoveController_OnChangeDirection;
    }

    private void MoveController_OnChangeDirection(LichtTopDownMoveController.LichtTopDownMoveEventArgs obj)
    {
        if (!TriggerDirection) return;

        if (obj.Direction.x != 0) Animator.SetInteger(DirectionXProperty, Math.Sign(obj.Direction.x));
        if (obj.Direction.y != 0) Animator.SetInteger(DirectionYProperty, Math.Sign(obj.Direction.y));
    }

    private void MoveControllerOnOnStopMoving(LichtTopDownMoveController.LichtTopDownMoveEventArgs obj)
    {
        if (!TriggerWalking) return;
        Animator.SetBool(WalkingProperty, false);
    }

    private void MoveController_OnStartMoving(LichtTopDownMoveController.LichtTopDownMoveEventArgs obj)
    {
        if (!TriggerWalking) return;
        Animator.SetBool(WalkingProperty, true);
    }
}
