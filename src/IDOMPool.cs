namespace Landis.Extension.Succession.PnETForC
{
    public interface IDOMPool
    {
        int ID { get; }
        string Name { get; }
        double Q10 { get; }
        double FracAir { get; }
    }
}
