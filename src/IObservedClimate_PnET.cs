using System.Collections.Generic;

namespace Landis.Extension.Succession.PnETForC 
{
    public interface IObservedClimate: IEnumerable<ObservedClimate>  
    {
        /// <summary>
        /// Atmospheric O3 concentration, acumulated during growing season (AOT40) (ppb h)
        /// </summary>
        double O3 { get; }

        /// <summary>
        /// Atmospheric CO2 concentration (ppm)
        /// </summary>
        double CO2 { get; } 

        /// <summary>
        /// Photosynthetically active radiation, average daily during the month (W/m2)
        /// </summary>
        double PAR0 { get; }

        /// <summary>
        /// Precipitation (mm/mo)
        /// </summary>
        double Prec { get; }

        /// <summary>
        /// Maximum daily temperature
        /// </summary>
        double Tmax { get; } 

        /// <summary>
        /// Minimum daily temperature
        /// </summary>
        double Tmin { get; }

        /// <summary>
        /// SPEI
        /// </summary>
        double SPEI { get; } 
    }
}
