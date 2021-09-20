namespace Licht.Interfaces.Memory
{
    public interface ICaterpillar<TObject>
    {
        int TailSize { get; set; }
        TObject Current { get; set; }
        TObject GetTrail(int index);
    }
}
