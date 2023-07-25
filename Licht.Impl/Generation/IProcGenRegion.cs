using Licht.Interfaces.Generation;

namespace Licht.Impl.Generation
{
    public interface IProcGenRegion<in TPosition, TObject> where TObject: ProcGenObject
    {
        public TObject[][] PossibleObjectStacks { get; set; }
        public bool IsInRegion(TPosition position);
    }
}