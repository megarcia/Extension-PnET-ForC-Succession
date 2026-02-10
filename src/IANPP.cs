// from ForC-Succession extension

namespace Landis.Extension.Succession.PnETForC
{
    public interface IANPP : ITimeInput
    {
        double GramsPerMetre2Year { get; }
        double StdDev { get; }
    }
}