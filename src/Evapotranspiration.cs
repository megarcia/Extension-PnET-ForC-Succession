// functional class Evapotranspiration, from PnET
// --> functions CalcPotentialEvaporation_umol
//               CalcPotentialGroundET_Radiation_umol
//               CalcReferenceET_Hamon
//               CalcPotentialGroundET_LAI_WATER
//               CalcPotentialGroundET_LAI_WEPP
//               CalcPotentialGroundET_LAI
//               CalcWVConductance
//               CalcWUE

using System;
using System.Data;

namespace Landis.Extension.Succession.PnETForC
{
    public class Evapotranspiration
    {
        /// <summary>
        /// PE calculations based on Stewart and Rouse 1976 and Cabrera et al. 2016
        /// NOTE: apparently unreferenced in PnET-Cohort library
        /// </summary>
        /// <param name="PAR">Daytime solar radiation (PAR) (micromol/m2.s)</param>
        /// <param name="Tair">Daytime air temperature (°C) [Tday]</param>
        /// <param name="DaySpan">Days in the month</param>
        /// <param name="DayLength">Length of daylight (s)</param>
        /// <returns></returns>
        public static double CalcPotentialEvaporation_umol(double PAR,
                                                           double Tair,
                                                           double DaySpan,
                                                           double DayLength)
        {
            // convert PAR (umol/m2.s) to total solar radiation (W/m2) (Reis and Ribeiro, 2019, eq. 39)  
            // convert Rs_W (W/m2) to Rs (MJ/m2.d) (Reis and Ribeiro, 2019, eq. 13)
            double Rs = PAR / 2.02 * Constants.SecondsPerDay / 1000000.0;
            // get slope of vapor pressure curve at Tair
            double VPSlope = Weather.CalcVaporPressureCurveSlope(Tair);
            // calculate potential evaporation (Stewart & Rouse, 1976, eq. 11)
            double PotentialEvaporation_MJ = VPSlope / (VPSlope + Constants.PsychrometricCoeff) * (1.624 + 0.9265 * Rs); // MJ/m2.day 
            // convert MJ/m2.day to mm/day (http://www.fao.org/3/x0490e/x0490e0i.htm)
            double PotentialEvaporation = PotentialEvaporation_MJ * 0.408;
            return PotentialEvaporation * DaySpan;  // mm/month 
        }

        /// <summary>
        /// PET calculations via Priestley-Taylor
        /// NOTE: apparently unreferenced in PnET-Cohort library
        /// </summary>
        /// <param name="AboveCanopyPAR">Daytime PAR (umol/m2.s) at top of canopy</param>
        /// <param name="SubCanopyPAR">Daytime PAR (umol/m2.s) at bottom of canopy</param>
        /// <param name="DayLength">Daytime length (s)</param>
        /// <param name="T">Average monthly temperature (C)</param>
        /// <param name="DaySpan">Days in the month</param>
        /// <returns></returns>
        public static double CalcPotentialGroundET_Radiation_umol(double AboveCanopyPAR,
                                                                  double SubCanopyPAR,
                                                                  double DayLength,
                                                                  double T,
                                                                  double DaySpan)
        {
            // convert daytime PAR (umol/m2*s) to total daily PAR (umol/m2*s)
            double Rs_daily = AboveCanopyPAR / Constants.SecondsPerDay / DayLength; 
            // convert daily PAR (umol/m2*s) to total solar radiation (W/m2)
            //     Reis and Ribeiro 2019 (Consants and Values)  
            double Rs_W = Rs_daily / 2.02; 
            // Back-calculate LAI from aboveCanopyPAR and subCanopyPAR
            double k = 0.3038;
            double LAI = Math.Log(SubCanopyPAR / AboveCanopyPAR) / (-1.0 * k);
            double AboveCanopyNetRad;
            if (LAI < 2.4)
                AboveCanopyNetRad = -26.8818 + 0.693066 * Rs_W;
            else
                AboveCanopyNetRad = -33.2467 + 0.741644 * Rs_W;
            double SubCanopyNetRad = AboveCanopyNetRad * Math.Exp(-1.0 * k * LAI);
            double alpha = 1.0;
            double VPSlope = Weather.CalcVaporPressureCurveSlope(T);
            // conversion W/m2 to MJ/m2.d
            double PotentialET_ground = alpha * (VPSlope / (VPSlope + Constants.PsychrometricCoeff)) / Constants.LatentHeatVaporWater * SubCanopyNetRad * Constants.SecondsPerDay / 1000000.0; // m/day
            return PotentialET_ground * 1000 * DaySpan; //mm/month
        }

        /// <summary>
        /// Reference ET calculations via Hamon
        /// NOTE: has interface entry
        /// </summary>
        /// <param name="T">Average monthly temperature (C)</param>
        /// <param name="DayLength">Daytime length (s)</param>
        /// <returns></returns>
        public static double CalcReferenceET_Hamon(double T,
                                                   double DayLength)
        {
            if (T < 0.0)
                return 0.0;
            double k = 1.2;   // proportionality coefficient
            double es = Weather.CalcVaporPressure(T);
            double N = DayLength / Constants.SecondsPerHour / 12.0;
            double ReferenceET = k * 0.165 * 216.7 * N * (10.0 * es / (T + 273.3)); // TODO: verify the 10x factor
            return ReferenceET; // mm/day
        }

        /// <summary>
        /// Potential ET given LAI via WATER (???)
        /// NOTE: apparently unreferenced in PnET-Cohort library
        /// </summary>
        /// <param name="LAI">Total canopy LAI</param>
        /// <param name="T">Average monthly temperature (C)</param>
        /// <param name="DayLength">Daytime length (s)</param>
        /// <param name="DaySpan">Days in the month</param>
        /// <returns></returns>
        public static double CalcPotentialGroundET_LAI_WATER(double LAI,
                                                             double T,
                                                             double DayLength,
                                                             double DaySpan)
        {
            double ReferenceET = CalcReferenceET_Hamon(T, DayLength); // mm/day
            double Egp = 0.8 * ReferenceET * Math.Exp(-0.695 * LAI); // mm/day
            return Egp * DaySpan; //mm/month
        }

        /// <summary>
        /// Potential ET given LAI via WEPP (???)
        /// NOTE: apparently unreferenced in PnET-Cohort library
        /// </summary>
        /// <param name="LAI">Total canopy LAI</param>
        /// <param name="T">Average monthly temperature (C)</param>
        /// <param name="DayLength">Daytime length (s)</param>
        /// <param name="DaySpan">Days in the month</param>
        /// <returns></returns>
        public static double CalcPotentialGroundET_LAI_WEPP(double LAI,
                                                            double T,
                                                            double DayLength,
                                                            double DaySpan)
        {
            double ReferenceET = CalcReferenceET_Hamon(T, DayLength); // mm/day
            double Egp = ReferenceET * Math.Exp(-0.4 * LAI); // mm/day
            return Egp * DaySpan; // mm/month
        }

        /// <summary>
        /// Potential ET given LAI and a given crop coefficient
        /// </summary>
        /// <param name="LAI">Total canopy LAI</param>
        /// <param name="T">Average monthly temperature (C)</param>
        /// <param name="dayLength">Daytime length (s)</param>
        /// <param name="daySpan">Days in the month</param>
        /// <param name="k">LAI extinction coefficient</param>
        /// <param name="cropCoeff">Crop coefficient (scalar adjustment)</param>
        /// <returns></returns>
        public static double CalcPotentialGroundET_LAI(double LAI,
                                                       double T,
                                                       double DayLength,
                                                       double DaySpan,
                                                       double k)
        {
            double CropCoeff = ((Parameter<double>)Names.GetParameter("ReferenceETCropCoeff")).Value;
            double ReferenceET = CalcReferenceET_Hamon(T, DayLength); // mm/day
            double Egp = CropCoeff * ReferenceET * Math.Exp(-k * LAI); // mm/day
            return Egp * DaySpan; // mm/month
        }

        /// <summary>
        /// Calculate water vapor conductance
        /// </summary>
        /// <param name="CO2"></param>
        /// <param name="Tavg"></param>
        /// <param name="CiElev"></param>
        /// <param name="netPsn"></param>
        /// <returns></returns>
        public static double CalcWVConductance(double CO2,
                                               double Tavg,
                                               double CiElev,
                                               double netPsn)
        {
            double Ca_Ci = CO2 - CiElev;
            double conductance_mol = netPsn / Ca_Ci * 1.6 * 1000;
            double conductance = conductance_mol / (444.5 - 1.3667 * Tavg) * 10.0;
            return conductance;
        }

        /// <summary>
        /// Calculate water use efficiency using photosynthesis,
        /// canopy layer fraction, and transpiration  
        /// </summary>
        /// <param name="grossPsn"></param>
        /// <param name="canopyLayerFrac"></param>
        /// <param name="transpiration"></param>
        /// <returns></returns>
        public static double CalcWUE(double grossPsn,
                                     double canopyLayerFrac,
                                     double transpiration)
        {
            double JCO2_JH2O = 0.0;
            if (transpiration > 0.0)
                JCO2_JH2O = 0.0015 * grossPsn * canopyLayerFrac / transpiration;
            double WUE = JCO2_JH2O * Constants.MCO2_MC;
            return WUE;
        }
    }
}
