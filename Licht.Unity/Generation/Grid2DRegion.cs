using System.Linq;
using Licht.Impl.Generation;
using Licht.Unity.Objects;
using Licht.Unity.PropertyAttributes;
using UnityEngine;

namespace Licht.Unity.Generation
{
    public abstract class Grid2DRegion<T> : BaseGameObject, IProcGenRegion<Vector2Int, T>  where T: ProcGenObject
    {
        [field:SerializeField]
        public ProcGenObjectStack<T>[] ObjectStack { get; private set; }

        [field:SerializeField]
        public int MinRadius { get; private set; }
        
        [field:SerializeField]
        public int MaxRadius { get; private set; }
        
        [field:SerializeField]
        public bool InfiniteMaxRadius { get; private set; }
        
        public T[][] PossibleObjectStacks { get; set; }

        public abstract bool IsInRegion(Vector2Int position);
    }
}