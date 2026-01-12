
namespace Landis.Extension.Succession.PnETForC
{
    public interface IPnETEcoregionVars
    {
        /// <summary>
        /// Photosynthetically active radiation, average daily during the month (umol/m2*s)
        /// </summary>
        double PAR0 { get; } 

        /// <summary>
        /// Precipitation (mm/mo)
        /// </summary>
        double Prec { get; } 

        /// <summary>
        /// Monthly average daytime air temp: (Tmax + Tavg)/2
        /// </summary>
        double Tday { get; } 

        /// <summary>
        /// Vapor pressure deficit
        /// </summary>
        double VPD { get; } 

        /// <summary>
        /// Decimal year and month
        /// </summary>
        double Time { get; } 

        int Year { get; }

        /// <summary>
        /// Number of days in the month
        /// </summary>
        double DaySpan { get; }

        /// <summary>
        /// Length of daylight in seconds
        /// </summary>
        double DayLength { get; } 

        /// <summary>
        /// Numeric month
        /// </summary>
        byte Month { get; }  

        /// <summary>
        /// Monthly average air temp: (Tmin + Tmax)/2
        /// </summary>
        double Tavg { get; } 

        /// <summary>
        /// Monthly min air temp
        /// </summary>
        double Tmin { get; } 

        /// <summary>
        /// Monthly max air temp
        /// </summary>
        double Tmax { get; } 

        /// <summary>
        /// Atmospheric CO2 concentration (ppm)
        /// </summary>
        double CO2 { get; } 

        /// <summary>
        /// Atmospheric O3 concentration, acumulated during growing season (AOT40) (ppb h)
        /// </summary>
        double O3 { get; } 

        /// <summary>
        /// SPEI
        /// </summary>
        double SPEI { get; }  

        PnETSpeciesVars this[string species] { get; }
    }
}
