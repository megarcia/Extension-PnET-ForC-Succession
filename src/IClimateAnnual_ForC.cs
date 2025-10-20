namespace Landis.Extension.Succession.PnETForC
{
    public interface IClimateAnnual : ITimeInput
    {
        double ClimateAnnualTemp { get; }
    }
}