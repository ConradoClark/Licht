using System.Collections.Generic;
using Licht.Interfaces.Generation;

namespace Licht.Impl.Generation
{
    public abstract class ProcGenObject : IWeighted<float>
    {
        public abstract string[] Tags { get; protected set; }
        public abstract float Weight { get; set; }
        public abstract bool HasTags(params string[] tag);
        public abstract void SetTags(params string[] tag);
        public abstract void AddTags(params string[] tag);
        public abstract void RemoveTags(params string[] tag);
    }
}