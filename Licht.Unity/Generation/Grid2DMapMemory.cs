using Licht.Impl.Generation;
using Licht.Unity.Objects;
using UnityEngine;

namespace Licht.Unity.Generation
{
    public class Grid2DMapMemory : BaseGameObject
    {
        private ProcGenMemory<Vector2Int> _procGenMemory;

        public override void Init()
        {
            base.Init();
            _procGenMemory = new ProcGenMemory<Vector2Int>();
        }
        
        public void SaveMemory(Vector2Int position, int layer, ProcGenParameters parameters)
        {
            _procGenMemory.SaveMemory(position, layer, parameters);
        }

        public bool TryGetMemory(Vector2Int position, int layer, out ProcGenParameters parameters)
        {
            return _procGenMemory.TryGetMemory(position, layer, out parameters);
        }

        public bool IsDestroyed(Vector2Int position, int layer)
        {
            return _procGenMemory.IsDestroyed(position, layer);
        }

        public void SetDestroyed(Vector2Int position, int layer, bool destroyed)
        {
            _procGenMemory.SetDestroyed(position, layer, destroyed);
        }
    }
}