namespace Licht.Interfaces.Generation
{
    public interface IGenerator<TSeed, out TValue> : ISeedable<TSeed>
    {
        TValue Generate();
    }
}
