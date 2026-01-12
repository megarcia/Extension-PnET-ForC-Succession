// functional class Snow, from PnET
// --> functions CalcDensity
//               CalcDepth
//               CalcThermalConductivity
//               CalcThermalDamping
//               CalcDampingRatio
//               CalcMaxSnowMelt
//               CalcMelt
//               CalcSnowFrac
//               CalcNewSnowDepth

using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class Snow
    {
        /// <summary>
        /// Calculate snow density (kg/m3, function of daysOfWinter) 
        /// </summary>
        /// <param name="DaysOfWinter"></param>
        /// <returns></returns>
        public static double CalcDensity(int DaysOfWinter)
        {
            return Constants.DensitySnow_intercept + (Constants.DensitySnow_slope * DaysOfWinter);
        }

        /// <summary>
        /// Calculate snow depth (m)
        /// </summary>
        /// <param name="DaysOfWinter"></param>
        /// <param name="Snowpack"></param>
        /// <returns></returns>
        public static double CalcDepth(double DensitySnow_kg_m3, double Snowpack)
        {
            return Constants.DensityWater * Snowpack / DensitySnow_kg_m3 / 1000F;
        }

        /// <summary>
        /// Calculate thermal conductivity of snow (kJ/m.d.K) 
        /// includes unit conversion from W to kJ
        /// based on CLM model - https://escomp.github.io/ctsm-docs/doc/build/html/tech_note/Soil_Snow_Temperatures/CLM50_Tech_Note_Soil_Snow_Temperatures.html#soil-and-snow-thermal-properties
        /// Eq. 85 in Jordan (1991)
        /// </summary>
        /// <param name="DensitySnow_kg_m3"></param>
        /// <returns></returns>
        public static double CalcThermalConductivity(double DensitySnow_kg_m3)
        {
            return (Constants.ThermalConductivityAir_Watts + ((0.0000775 * DensitySnow_kg_m3) + (0.000001105 * Math.Pow(DensitySnow_kg_m3, 2))) * (Constants.ThermalConductivityIce_Watts - Constants.ThermalConductivityAir_Watts)) * 3.6 * 24.0;
        }

        /// <summary>
        /// Calculate snow thermal damping coefficient
        /// based on CLM model - https://escomp.github.io/ctsm-docs/doc/build/html/tech_note/Soil_Snow_Temperatures/CLM50_Tech_Note_Soil_Snow_Temperatures.html#soil-and-snow-thermal-properties
        /// Eq. 85 in Jordan (1991)
        /// </summary>
        /// <param name="ThermalConductivity_Snow"></param>
        /// <returns></returns>
        public static double CalcThermalDamping(double ThermalConductivity_Snow)
        {
            return Math.Sqrt(Constants.omega / (2.0 * ThermalConductivity_Snow));
        }

        /// <summary>
        /// Thermal damping ratio for snow
        /// adapted from Kang et al. (2000) and Liang et al. (2014)
        /// based on CLM model - https://escomp.github.io/ctsm-docs/doc/build/html/tech_note/Soil_Snow_Temperatures/CLM50_Tech_Note_Soil_Snow_Temperatures.html#soil-and-snow-thermal-properties
        /// Eq. 85 in Jordan (1991)
        /// </summary>
        /// <param name="SnowDepth"></param>
        /// <param name="ThermalDamping"></param>
        /// <returns></returns>
        public static double CalcDampingRatio(double SnowDepth, double ThermalDamping)
        {
            return Math.Exp(-1.0F * SnowDepth * ThermalDamping);
        }

        /// <summary>
        /// Snowmelt rate can range between 1.6 to 6.0 mm/degree day, and default should be 2.74 according to NRCS Part 630 Hydrology National Engineering Handbook (Chapter 11: Snowmelt)
        /// </summary>
        /// <param name="Tavg"></param>
        /// <param name="DaySpan"></param>
        /// <returns></returns>
        public static double CalcMaxSnowMelt(double Tavg, double DaySpan)
        {
            return 2.74 * Math.Max(0.0, Tavg) * DaySpan;
        }

        /// <summary>
        /// Calculate actual snow melt
        /// </summary>
        /// <param name="Snowpack"></param>
        /// <param name="Tavg"></param>
        /// <param name="DaySpan"></param>
        /// <param name="Name"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double CalcMelt(double Snowpack, double Tavg, double DaySpan, string Name, string Location)
        {
            double Snowmelt = Math.Min(Snowpack, CalcMaxSnowMelt(Tavg, DaySpan)); // mm
            if (Snowmelt < 0.0)
                throw new Exception("Error, snowmelt = " + Snowmelt + "; ecoregion = " + Name + "; site = " + Location);
            return Snowmelt;
        }

        /// <summary>
        /// Snow fraction of ground cover
        /// </summary>
        /// <param name="Tavg"></param>
        /// <returns></returns>
        public static double CalcSnowFrac(double Tavg)
        {
            return Math.Max(0.0, Math.Min(1F, (Tavg - 2.0) / -7.0));
        }

        // Calculate depth of new snow
        public static double CalcNewSnowDepth(double Tavg, double Prec, double SublimationFrac)
        {
            double NewSnow = CalcSnowFrac(Tavg) * Prec;
            double NewSnowDepth = NewSnow * (1 - SublimationFrac); // (mm) Account for sublimation here
            if (NewSnowDepth < 0 || NewSnowDepth > Prec)
                throw new Exception("Error, newSnowDepth = " + NewSnowDepth + " availablePrecipitation = " + Prec);
            return NewSnowDepth;
        }
    }
}
