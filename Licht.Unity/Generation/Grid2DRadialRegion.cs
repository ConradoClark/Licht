using System.Linq;
using Licht.Impl.Generation;
using UnityEngine;

namespace Licht.Unity.Generation
{
    public class Grid2DRadialRegion<T> : Grid2DRegion<T>  where T: ProcGenObject
    {
        public override bool IsInRegion(Vector2Int position)
        {
            PossibleObjectStacks = ObjectStack.Select(stack => stack.Objects).ToArray();
            
            var minRadiusSquared = MinRadius * MinRadius;
            var maxRadiusSquared = MaxRadius * MaxRadius;
            var sqrMagnitude = position.sqrMagnitude;

            return sqrMagnitude >= minRadiusSquared && (InfiniteMaxRadius || sqrMagnitude <= maxRadiusSquared);
        }
    }
}