// from ForC-Succession extension

namespace Landis.Extension.Succession.PnETForC
{
    public interface ILight
    {  
        byte ShadeClass { get; set; }
        double ProbSufficientLight0 { get; set; }
        double ProbSufficientLight1 { get; set; }
        double ProbSufficientLight2 { get; set; }
        double ProbSufficientLight3 { get; set; }
        double ProbSufficientLight4 { get; set; }
        double ProbSufficientLight5 { get; set; }
    }
}
