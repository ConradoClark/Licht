using System.Linq;
using Licht.Impl.Generation;
using UnityEngine;

namespace Licht.Unity.Generation
{
    public class Grid2DGenObject :ProcGenObject
    {
        [field:SerializeField]
        public override string[] Tags { get; protected set; }
        
        [field:SerializeField]
        public override float Weight { get; set; }
        
        public override bool HasTags(params string[] tag)
        {
            return Tags.Intersect(tag).Count() == tag.Length;
        }

        public override void SetTags(params string[] tag)
        {
            Tags = tag.ToArray();
        }

        public override void AddTags(params string[] tag)
        {
            Tags = Tags.Union(tag).ToArray();
        }

        public override void RemoveTags(params string[] tag)
        {
            Tags = Tags.Except(tag).ToArray();
        }
    }
}