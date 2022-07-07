using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class LichtPhysicsSemiSolid : MonoBehaviour
{
    public Collider2D Collider;
    private LichtPhysics _physics;
    private void Awake()
    {
        _physics = this.GetLichtPhysics();
        _physics.AddSemiSolid(Collider);
    }

    private void OnDestroy()
    {
        _physics.RemoveSemiSolid(Collider);
    }
}
