using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

public class SpriteTransformer : BaseGameObject
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
    public SpriteRenderer SpriteRenderer { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        _initialRotation = SpriteRenderer.transform.localRotation;
        _initialScale = SpriteRenderer.transform.localScale;
        _initialPosition = SpriteRenderer.transform.localPosition;
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

        SpriteRenderer.transform.localScale = Vector3.Scale(_initialScale, Scale);
        SpriteRenderer.transform.localPosition = position;
        SpriteRenderer.transform.localRotation = _initialRotation;
        SpriteRenderer.transform.RotateAround(CenterPivot.position, Vector3.forward, rotation.eulerAngles.z);

        ResetValues();
    }
}
