// functional class Weather, from PnET
// --> functions CalcTavg
//               CalcTday
//               CalcVaporPressure
//               CalcVaporPressureCurveSlope
//               CalcVPD

using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class Weather
    {
        /// <summary>
        /// Calculate average temperature (ºC)
        /// </summary>
        /// <param name="Tmin"></param>
        /// <param name="Tmax"></param>
        /// <returns></returns>
        public static double CalcTavg(double Tmin,
                                      double Tmax)
        {
            return (Tmin + Tmax) / 2.0;
        }

        /// <summary>
        /// Calculate daytime average temperature (ºC)
        /// </summary>
        /// <param name="Tavg"></param>
        /// <param name="Tmax"></param>
        /// <returns></returns>
        public static double CalcTday(double Tavg,
                                      double Tmax)
        {
            return (Tavg + Tmax) / 2.0;
        }

        /// <summary>
        /// Calculates vapor pressure at temperature (T) via the Tetens
        /// equations for water and ice   
        ///     see https://en.wikipedia.org/wiki/Tetens_equation
        /// </summary>
        /// <param name="T">Air temperature (°C)</param>
        public static double CalcVaporPressure(double T)
        {
            double es;
            if (T >= 0.0)  // above freezing point -- vapor pressure over water
                es = 0.61078 * Math.Exp(17.26939 * T / (T + 237.3));
            else  // below freezing point -- vapor pressure over ice
                es = 0.61078 * Math.Exp(21.87456 * T / (T + 265.5));
            return es;
        }

        /// <summary>
        /// Slope of curve of water pressure and air temp
        ///     Cabrera et al. 2016 (Table 1)
        /// </summary>
        /// <param name="T">Temperature (C)</param>
        /// <returns></returns>
        public static double CalcVaporPressureCurveSlope(double T)
        {
            double Slope = 4098.0 * CalcVaporPressure(T) / Math.Pow(T + 237.3, 2);
            return Slope;
        }

        /// <summary>
        /// Calculate vapor pressure deficit for daytime temperature
        /// </summary>
        public static double CalcVPD(double Tday,
                                     double Tmin)
        {
            double es = CalcVaporPressure(Tday);
            double emean = CalcVaporPressure(Tmin);
            return es - emean;
        }
    }
}
