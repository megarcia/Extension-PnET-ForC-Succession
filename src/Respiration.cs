// functional class Respiration, from PnET
// --> functions CalcBaseFolRespFrac_Wythers
//               CalcQ10_Wythers
//               CalcFQ10
//               CalcFTemp

using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class Respiration
    {
        /// <summary>
        /// Calculate base foliar respiration fraction via Wythers
        /// </summary>
        /// <param name="Tavg"></param>
        /// <returns></returns>
        public static double CalcBaseFolRespFrac_Wythers(double Tavg)
        {
            double BaseFolRespFrac = 0.138071 - 0.0024519 * Tavg;;
            return BaseFolRespFrac;
        }

        /// <summary>
        /// Calculate respiration Q10 factor via Wythers
        /// </summary>
        /// <param name="Tavg"></param>
        /// <param name="PsnTopt"></param>
        /// <returns></returns>
        public static double CalcQ10_Wythers(double Tavg,
                                             double PsnTopt)
        {
            // Midpoint between Tavg and optimal T for photosynthesis
            double Tmid = (Tavg + PsnTopt) / 2.0;
            double Q10 = 3.22 - 0.046 * Tmid;
            return Q10;
        }

        /// <summary>
        /// Calculate respiration Q10 reduction factor
        /// </summary>
        /// <param name="Q10"></param>
        /// <param name="Tday"></param>
        /// <param name="PsnTopt"></param>
        /// <returns></returns>
        public static double CalcFQ10(double Q10,
                                      double Tday,
                                      double PsnTopt)
        {
            double FQ10 = Math.Pow(Q10, (Tday - PsnTopt) / 10.0);
            return FQ10;
        }

        /// <summary>
        /// Calculate temperature-dependent factor for respiration
        /// </summary>
        /// <param name="Q10"></param>
        /// <param name="Tday"></param>
        /// <param name="Tmin"></param>
        /// <param name="PsnTopt"></param>
        /// <param name="DayLength"></param>
        /// <param name="NightLength"></param>
        /// <returns></returns>
        public static double CalcFTemp(double Q10,
                                       double Tday,
                                       double Tmin,
                                       double PsnTopt,
                                       double DayLength,
                                       double NightLength)
        {
            // Daytime maintenance respiration factor (scaling factor of actual vs potential respiration applied to daytime average temperature)
            double FTempDay = CalcFQ10(Q10, Tday, PsnTopt);
            // Night maintenance respiration factor (scaling factor of actual vs potential respiration applied to nighttime minimum temperature)
            double FTempNight = CalcFQ10(Q10, Tmin, PsnTopt);
            // Unitless respiration adjustment
            double FTemp = Math.Min(1.0, (FTempDay * DayLength + FTempNight * NightLength) / (DayLength + NightLength));
            return FTemp;
        }
    }
}
