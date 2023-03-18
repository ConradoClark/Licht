using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

public class Transformer : BaseGameObject
{
    [field: SerializeField]
    public Transform CenterPivot { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 Scale { get; private set; }
    public Vector3 Position { get; private set; }

    private Quaternion _initialRotation;
    private Vector3 _initialScale;
    private Vector3 _initialPosition;

    [field: SerializeField]
    public Transform Target { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        _initialRotation = Target.localRotation;
        _initialScale = Target.localScale;
        _initialPosition = Target.localPosition;
    }

    private void ResetValues()
    {
        Rotation = Quaternion.identity;
        Position = Vector3.zero;
        Scale = Vector3.one;
    }

    public void ApplyRotation(Quaternion rotation)
    {
        Rotation *= rotation;
    }

    public void ApplyScale(Vector3 scale)
    {
        Scale += scale;
    }

    public void ApplyPosition(Vector3 position)
    {
        Position += position;
    }

    private void LateUpdate()
    {
        var rotation = _initialRotation * Rotation;
        var position = _initialPosition + Position;

        Target.localScale = Vector3.Scale(_initialScale, Scale);
        Target.localPosition = position;
        Target.localRotation = _initialRotation;
        Target.RotateAround(CenterPivot.position, Vector3.forward, rotation.eulerAngles.z);

        ResetValues();
    }
}
