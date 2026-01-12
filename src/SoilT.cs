using System;
using System.Collections.Generic;

namespace Landis.Extension.Succession.PnETForC
{
    public class SoilT
    {
        public static SortedList<double, double> CalcMonthlySoilTemps(SortedList<double, double> depthTempDict, IPnETEcoregionData Ecoregion, int daysOfWinter, double snowpack, IHydrology hydrology, double lastTempBelowSnow)
        {
            // Snow calculations, now handled in Snow class
            double densitySnow_kg_m3 = Snow.CalcDensity(daysOfWinter);
            double snowDepth = Snow.CalcDepth(densitySnow_kg_m3, snowpack);
            if (Ecoregion.Variables.Tavg >= 0)
            {
                double fracAbove0 = Ecoregion.Variables.Tmax / (Ecoregion.Variables.Tmax - Ecoregion.Variables.Tmin);
                snowDepth *= fracAbove0;
            }
            double snowThermalConductivity = Snow.CalcThermalConductivity(densitySnow_kg_m3);
            double snowThermalDamping = Snow.CalcThermalDamping(snowThermalConductivity);
            double snowDampingRatio = Snow.CalcDampingRatio(snowDepth, snowThermalDamping);
            // Soil thermal conductivity via De Vries model (convert to kJ/m.d.K)
            double ThermalConductivity_theta = CalcThermalConductivitySoil_Watts(hydrology.SoilWaterContent, Ecoregion.Porosity, Ecoregion.SoilType) / Constants.Convert_kJperday_to_Watts;
            double D = ThermalConductivity_theta / Hydrology_SaxtonRawls.GetCTheta(Ecoregion.SoilType);  //m2/day
            double Dmonth = D * Ecoregion.Variables.DaySpan; // m2/month
            double d = Math.Pow(Constants.omega / (2.0 * Dmonth), 0.5);
            double maxDepth = Ecoregion.RootingDepth + Ecoregion.LeakageFrostDepth;
            double testDepth = 0;
            double tempBelowSnow = Ecoregion.Variables.Tavg;
            if (snowDepth > 0)
                tempBelowSnow = lastTempBelowSnow + (Ecoregion.Variables.Tavg - lastTempBelowSnow) * snowDampingRatio;
            while (testDepth <= (maxDepth / 1000.0))
            {
                // adapted from Kang et al. (2000) and Liang et al. (2014)
                double DRz = Math.Exp(-1.0 * testDepth * d);
                double zTemp = depthTempDict[testDepth] + (tempBelowSnow - depthTempDict[testDepth]) * DRz;
                depthTempDict[testDepth] = zTemp;
                if (testDepth == 0.0)
                    testDepth = 0.10;
                else if (testDepth == 0.10)
                    testDepth = 0.25;
                else
                    testDepth += 0.25;
            }
            if (maxDepth < 100) // mm
                depthTempDict[0.1] = depthTempDict[0.0];
            return depthTempDict;
        }

        /// <summary>
        /// Calculate ga term in De Vries model of soil thermal conductivity
        /// </summary>
        /// <param name="WaterContent"></param>
        /// <param name="Porosity"></param>
        /// <returns></returns>
        public static double CalcAirShapeFactor(double WaterContent, double Porosity)
        {
            double ga = 0.035 + 0.298 * (WaterContent / Porosity);
            return ga;
        }

        /// <summary>
        /// Calculate thermal conductivity of De Vries "fluid"
        /// </summary>
        /// <param name="WaterContent"></param>
        /// <param name="ClayFrac"></param>
        /// <returns></returns>
        public static double CalcThermalConductivityFluid(double WaterContent, double ClayFrac)
        {
            double theta0 = 0.33 * ClayFrac + 0.078;
            double ratio = WaterContent / theta0;
            double q = 7.25 * ClayFrac + 2.52;
            double fw = 1.0 / (1.0 + Math.Pow(ratio, -q));
            return fw;
        }

        /// <summary>
        /// Calculate weights in De Vries model of soil thermal conductivity
        /// </summary>
        /// <param name="ga"></param>
        /// <param name="gc"></param>
        /// <param name="Numerator"></param>
        /// <param name="ThermalConductivityFluid"></param>
        /// <returns></returns>
        public static double CalcDeVriesWeight(double ga, double Numerator, double ThermalConductivityFluid)
        {
            double gc = 1.0 - 2.0 * ga;
            double term1 = 2.0 / 3.0 / (1.0 + ga * ((Numerator / ThermalConductivityFluid) - 1.0));
            double term2 = 1.0 / 3.0 / (1.0 + gc * ((Numerator / ThermalConductivityFluid) - 1.0));
            double weight = term1 + term2;
            return weight;
        }

        /// <summary>
        /// Calculate thermal conductivity of moist/wet soil via De Vries model
        /// (De Vries, 1963) summarized in (Campbell et al., 1994; Tong et al., 2016) 
        /// </summary>
        /// <param name="WaterContent"></param>
        /// <param name="Porosity"></param>
        /// <param name="SoilType"></param>
        /// <returns></returns>
        public static double CalcThermalConductivitySoil_Watts(double WaterContent, double Porosity, string SoilType)
        {
            double ga = CalcAirShapeFactor(WaterContent, Porosity);
            double ClayFrac = Hydrology_SaxtonRawls.GetClayFrac(SoilType);
            double ThermalConductivityFluid = CalcThermalConductivityFluid(WaterContent, ClayFrac);
            double Kair = CalcDeVriesWeight(ga, Constants.ThermalConductivityAir_Watts, ThermalConductivityFluid);
            double ThermalConductivitySoil_Watts = Hydrology_SaxtonRawls.GetThermalConductivitySoil(SoilType) * Constants.Convert_kJperday_to_Watts;
            double Ksoil = CalcDeVriesWeight(ga, ThermalConductivitySoil_Watts, ThermalConductivityFluid);
            double Kwater = CalcDeVriesWeight(ga, Constants.ThermalConductivityWater_Watts, ThermalConductivityFluid);
            double AirContent = Porosity - WaterContent;
            double numerator_air = Kair * AirContent * Constants.ThermalConductivityAir_Watts;
            double numerator_soil = Ksoil * (1.0 - Porosity) * ThermalConductivitySoil_Watts;
            double numerator_water = Kwater * WaterContent * Constants.ThermalConductivityWater_Watts;
            double numerator = numerator_air + numerator_soil + numerator_water;
            double denominator = Kair * AirContent + Ksoil * (1.0 - Porosity) + Kwater * WaterContent;
            double ThermalConductivitySoil = numerator / denominator;
            return ThermalConductivitySoil;
        }
    }
}
