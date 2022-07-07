namespace Licht.Interfaces.Generation
{
    public interface ISeedable<TSeed>
    {
        TSeed Seed { get; set; }
    }
}