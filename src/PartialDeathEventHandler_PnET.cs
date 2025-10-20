
namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// Represents a method that handles cohort-death events.
    /// </summary>
    public delegate void PartialDeathEventHandler<TDeathEventArgs>(object sender, TDeathEventArgs eventArgs);
}
