﻿using System;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

[AddComponentMenu("L. Animation: TopDown2D Animator")]
public class TopDownControllerAnimator : BaseGameObject
{
    [field:SerializeField]
    public LichtTopDownMoveController MoveController { get; private set; }

    [field: SerializeField]
    public Animator Animator { get; private set; }

    [field:CustomHeader("Triggers")]
    [field:CustomLabel("Select this if moving should set an Animator's param (Bool).")]
    [field: SerializeField] public bool SetBoolOnWalking { get; private set; } = true;
    [field:ShowWhen(nameof(SetBoolOnWalking))]
    [field: SerializeField] public string WalkingProperty { get; private set; } = "Walking";

    [field: CustomLabel("Select this if changing direction should set Animator's params for each axis XY(Int, Int).")]
    [field: SerializeField] public bool SetAxisOnDirectionChange { get; private set; } = false;
    [field: ShowWhen(nameof(SetAxisOnDirectionChange))]
    [field: SerializeField] public string DirectionXProperty { get; private set; } = "DirectionX";
    [field: ShowWhen(nameof(SetAxisOnDirectionChange))]
    [field: SerializeField] public string DirectionYProperty { get; private set; } = "DirectionY";

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
        if (!SetAxisOnDirectionChange) return;

        if (obj.Direction.x != 0) Animator.SetInteger(DirectionXProperty, Math.Sign(obj.Direction.x));
        if (obj.Direction.y != 0) Animator.SetInteger(DirectionYProperty, Math.Sign(obj.Direction.y));
    }

    private void MoveControllerOnOnStopMoving(LichtTopDownMoveController.LichtTopDownMoveEventArgs obj)
    {
        if (!SetBoolOnWalking) return;
        Animator.SetBool(WalkingProperty, false);
    }

    private void MoveController_OnStartMoving(LichtTopDownMoveController.LichtTopDownMoveEventArgs obj)
    {
        if (!SetBoolOnWalking) return;
        Animator.SetBool(WalkingProperty, true);
    }
}
