// functional class Photosynthesis, from PnET
// --> functions CurvilinearPsnTempResponse
//               DTempResponse
//               CalcDVPD
//               CalcJH2O
//               CalcAmaxB_CO2
//               CalcCiModifier
//               CalcCiElev
//               CalcDelAmaxCi
//               CalcFRad
//               CalcPotentialGrossPsn
//               CalcFWater
//               CalcFOzone
//               CalcAdjHalfSat
//               CalcAdjFolN
//               CalcJCO2_JH2O

using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class Photosynthesis
    {
        /// <summary>
        /// Curvilinear response of photosynthesis to temperature
        /// </summary>
        /// <param name="Tday"></param>
        /// <param name="PsnTopt"></param>
        /// <param name="PsnTmin"></param>
        /// <param name="PsnTmax"></param>
        /// <returns></returns>
        public static double CurvilinearPsnTempResponse(double Tday,
                                                        double PsnTopt,
                                                        double PsnTmin,
                                                        double PsnTmax)
        {
            if (Tday < PsnTmin)
                return 0.0;
            else if (Tday > PsnTopt)
                return 1.0;
            else
                return (PsnTmax - Tday) * (Tday - PsnTmin) / Math.Pow((PsnTmax - PsnTmin) / 2.0, 2);
        }

        /// <summary>
        /// Alternate response of photosynthesis to temperature
        /// </summary>
        /// <param name="Tday"></param>
        /// <param name="PsnTopt"></param>
        /// <param name="PsnTmin"></param>
        /// <param name="PsnTmax"></param>
        /// <returns></returns>
        public static double DTempResponse(double Tday,
                                           double PsnTopt,
                                           double PsnTmin,
                                           double PsnTmax)
        {
            if (Tday < PsnTmin || Tday > PsnTmax)
                return 0.0;
            else
            {
                if (Tday <= PsnTopt)
                {
                    double PsnTmaxEst = PsnTopt + (PsnTopt - PsnTmin);
                    return Math.Max(0.0, (PsnTmaxEst - Tday) * (Tday - PsnTmin) / Math.Pow((PsnTmaxEst - PsnTmin) / 2.0, 2));
                }
                else
                {
                    double PsnTminEst = PsnTopt + (PsnTopt - PsnTmax);
                    return Math.Max(0.0, (PsnTmax - Tday) * (Tday - PsnTminEst) / Math.Pow((PsnTmax - PsnTminEst) / 2.0, 2));
                }
            }
        }

        /// <summary>
        /// Calculate DVPD, gradient of effect of vapor pressure deficit 
        /// on growth
        /// </summary>
        /// <param name="VPD"></param>
        /// <param name="DVPD1"></param>
        /// <param name="DVPD2"></param>
        /// <returns></returns>
        public static double CalcDVPD(double VPD,
                                      double DVPD1,
                                      double DVPD2)
        {
            double DVPD = Math.Max(0, 1.0 - DVPD1 * Math.Pow(VPD, DVPD2));
            return DVPD;
        }

        /// <summary>
        /// Calculate JH20
        /// </summary>
        /// <param name="Tmin"></param>
        /// <param name="VPD"></param>
        /// <returns></returns>
        public static double CalcJH2O(double Tmin,
                                      double VPD)
        {
            double JH2O = Constants.CalperJ * (VPD / (Constants.GasConst_JperkmolK * (Tmin + Constants.Tref_K)));
            return JH2O;
        }

        /// <summary>
        /// Modify AmaxB based on CO2 level using linear interpolation
        /// uses 2 known points: (350, AmaxB) and (550, AmaxB * AMaxBFCO2)
        /// </summary>
        /// <param name="CO2"></param>
        /// <param name="AmaxB"></param>
        /// <param name="AMaxBFCO2"></param>
        /// <returns></returns>
        public static double CalcAmaxB_CO2(double CO2,
                                           double AmaxB,
                                           double AMaxBFCO2)
        {
            // AmaxB_slope = [(AmaxB * AMaxBFCO2) - AmaxB] / [550 - 350]
            double AmaxB_slope = (AMaxBFCO2 - 1.0) * AmaxB / 200.0;
            // AmaxB_intercept = AmaxB - (AmaxB_slope * 350)
            double AmaxB_intercept = -1.0 * (((AMaxBFCO2 - 1.0) * 1.75) - 1.0) * AmaxB;
            double AmaxB_CO2 = AmaxB_slope * CO2 + AmaxB_intercept;
            return AmaxB_CO2;
        }

        /// <summary>
        /// Calculate CiModifier as a function of leaf O3 tolerance
        /// Regression coefs estimated from New 3 algorithm for Ozone drought.xlsx
        /// https://usfs.box.com/s/eksrr4d7fli8kr9r4knfr7byfy9r5z0i
        /// Uses data provided by Yasutomo Hoshika and Elena Paoletti
        /// </summary>
        /// <param name="CumulativeO3"></param>
        /// <param name="StomataO3Sensitivity"></param>
        /// <param name="FWaterOzone"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double CalcCiModifier(double CumulativeO3,
                                            string StomataO3Sens,
                                            double FWaterOzone)
        {
            double CiModifier = 1.0; // if no ozone, ciModifier defaults to 1
            if (CumulativeO3 > 0)
            {
                if (StomataO3Sens == "Sensitive" || StomataO3Sens == "Sens")
                    CiModifier = FWaterOzone + (-0.0176 * FWaterOzone + 0.0118) * CumulativeO3;
                else if (StomataO3Sens == "Intermediate" || StomataO3Sens == "Int")
                    CiModifier = FWaterOzone + (-0.0148 * FWaterOzone + 0.0062) * CumulativeO3;
                else if (StomataO3Sens == "Tolerant" || StomataO3Sens == "Tol")
                    CiModifier = FWaterOzone + (-0.021 * FWaterOzone + 0.0087) * CumulativeO3;
                else
                    throw new Exception("O3 data provided, but species StomataO3Sensitivity is not set to Sensitive, Intermediate, or Tolerant");
            }
            CiModifier = Math.Max(0.00001, Math.Min(CiModifier, 1.0));
            return CiModifier;
        }

        /// <summary>
        /// Calculate elevated leaf internal CO2 concentration
        /// </summary>
        /// <param name="Ci_Ca"></param>
        /// <param name="CiModifier"></param>
        /// <param name="CO2"></param>
        /// <returns></returns>
        public static double CalcCiElev(double Ci_Ca,
                                        double CiModifier,
                                        double CO2)
        {
            double modCi_Ca = Ci_Ca * CiModifier;
            double CiElev = CO2 * modCi_Ca;
            return CiElev;
        }

        /// <summary>
        /// Calculate vertical gradient of CO2 concentration in canopy
        /// based on Franks (2013, New Phytologist, 197:1077-1094) and
        /// modified by M. Kubiske
        /// </summary>
        /// <param name="CiElev"></param>
        /// <returns></returns>
        public static double CalcDelAmaxCi(double CiElev)
        {
            // the CO2 compensation point at which photorespiration balances 
            // exactly with photosynthesis.  Assumed to be 40 based on leaf 
            // Temp = 25ºC
            double Gamma = 40;
            double DelAmaxCi = (CiElev - Gamma) / (CiElev + 2 * Gamma) * (Constants.CO2RefConc + 2 * Gamma) / (Constants.CO2RefConc - Gamma);
            DelAmaxCi = Math.Max(DelAmaxCi, 0.0);
            return DelAmaxCi;
        }

        /// <summary>
        /// Radiative (light) effect on photosynthesis
        /// </summary>
        /// <param name="Radiation"></param>
        /// <param name="HalfSat"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double CalcFRad(double Rad,
                                      double HalfSat)
        {
            if (HalfSat <= 0)
                throw new Exception("HalfSat <= 0. Cannot calculate fRad.");
            double FRad = 1.0 - Math.Exp(-1.0 * Rad * Math.Log(2.0) / HalfSat);
            return FRad;
        }

        /// <summary>
        /// Calculate the potential gross photosynthesis (gC/m2 ground/mo)
        /// </summary>
        /// <param name="AmaxAdj"></param>
        /// <param name="BaseFolResp"></param>
        /// <param name="DaySpan"></param>
        /// <param name="DVPD"></param>
        /// <param name="DayLength"></param>
        /// <param name="PsnFTemp"></param>
        /// <param name="FRad"></param>
        /// <param name="FAge"></param>
        /// <param name="Fol"></param>
        /// <returns></returns>
        public static double CalcPotentialGrossPsn(double AmaxAdj,
                                                   double BaseFolResp,
                                                   double DaySpan,
                                                   double DVPD,
                                                   double DayLength,
                                                   double PsnFTemp,
                                                   double FRad,
                                                   double FAge,
                                                   double Fol)
        {
            double GrossAmax = AmaxAdj + BaseFolResp;
            // Reference gross Psn (lab conditions) in gC/g Fol/month
            double RefGrossPsn = DaySpan * (GrossAmax * DVPD * DayLength * Constants.MC) / Constants.billion;
            // Calculate gross psn from stress factors and reference gross psn (gC/g Fol/month)
            // Reduction factors include temperature (PsnFTemp), water (FWater), light (FRad), age (FAge)
            // Remove FWater from psn reduction because it is accounted for in WUE through ciModifier [mod2, mod3]
            double PotentialGrossPsn = 1.0 / Globals.IMAX * PsnFTemp * FRad * FAge * RefGrossPsn * Fol;
            return PotentialGrossPsn;
        }

        /// <summary>
        /// Soil water effect on photosynthesis
        /// </summary>
        /// <param name="H1"></param>
        /// <param name="H2"></param>
        /// <param name="H3"></param>
        /// <param name="H4"></param>
        /// <param name="pressureHead"></param>
        /// <returns></returns>
        public static double CalcFWater(double H1,
                                        double H2,
                                        double H3,
                                        double H4,
                                        double pressureHead)
        {
            double minThreshold = H1;
            if (H2 <= H1)
                minThreshold = H2;
            // Calculate water stress
            if (pressureHead <= H1)
                return 0.0;
            else if (pressureHead < minThreshold || pressureHead >= H4)
                return 0.0;
            else if (pressureHead > H3)
                return 1.0 - ((pressureHead - H3) / (H4 - H3));
            else if (pressureHead < H2)
                return 1.0 / (H2 - H1) * pressureHead - (H1 / (H2 - H1));
            else
                return 1.0;
        }

        /// <summary>
        /// O3 effect on photosynthesis
        /// </summary>
        /// <param name="O3"></param>
        /// <param name="Layer"></param>
        /// <param name="nLayers"></param>
        /// <param name="FolMass"></param>
        /// <param name="LastFOzone"></param>
        /// <param name="WVConductance"></param>
        /// <param name="FOzone_slope"></param>
        /// <returns></returns>
        public static double CalcFOzone(double O3,
                                        int Layer,
                                        int nLayers,
                                        double FolMass,
                                        double LastFOzone,
                                        double WVConductance,
                                        double FOzone_slope)
        {
            double DroughtO3Frac = 1.0; // Not using DroughtO3Frac from PnET code per M. Kubiske and A. Chappelka
            double kO3Eff = 0.0026 * FOzone_slope;  // Scaled by species using input parameters
            double O3Prof = 0.6163 + (0.00105 * FolMass);
            double RelLayer = (double)(Layer / (double)nLayers);
            double RelO3 = Math.Min(1.0, 1.0 - RelLayer * O3Prof * Math.Pow(RelLayer * O3Prof, 2));
            // Kubiske method (using water vapor conductance in place of conductance
            double FOzone = Math.Min(1.0, (LastFOzone * DroughtO3Frac) + (kO3Eff * WVConductance * O3 * RelO3));
            return FOzone;
        }

        /// <summary>
        /// Calculate adjustment for CO2 saturation level on photosynthesis
        /// </summary>
        /// <param name="CO2"></param>
        /// <param name="HalfSat"></param>
        /// <param name="HalfSatFCO2"></param>
        /// <returns></returns>
        public static double CalcAdjHalfSat(double CO2,
                                            double HalfSat,
                                            double HalfSatFCO2)
        {
            double halfSatIntercept = HalfSat - Constants.CO2RefConc * HalfSatFCO2;
            double AdjHalfSat = HalfSatFCO2 * CO2 + halfSatIntercept;
            return AdjHalfSat;
        }

        /// <summary>
        /// Calculate foliar N adjusted for canopy position
        /// </summary>
        /// <param name="FolN_shape"></param>
        /// <param name="FolN_intercept"></param>
        /// <param name="FolN"></param>
        /// <param name="FRad"></param>
        /// <returns></returns>
        /// via non-linear reduction in FolN with canopy depth via FRad
        public static double CalcAdjFolN(double FolN_shape,
                                         double FolN_intercept,
                                         double FolN,
                                         double FRad)
        {
            double adjFolN = FolN + ((FolN_intercept - FolN) * Math.Pow(FRad, FolN_shape));
            return adjFolN;
        }

        /// <summary>
        /// Calculate ratio JCO2/JH2O
        /// </summary>
        /// <param name="Tmin"></param>
        /// <param name="CO2"></param>
        /// <param name="CiElev"></param>
        /// <param name="JH2O"></param>
        /// <param name="CiModifier"></param>
        /// <returns></returns>
        public static double CalcJCO2_JH2O(double JH2O,
                                           double Tmin,
                                           double CO2,
                                           double CiElev,
                                           double CiModifier)
        {
            double V = Constants.GasConst_JperkmolK * (Tmin + Constants.Tref_K) / Constants.Pref_kPa;
            double JCO2 = 0.139 * ((CO2 - CiElev) / V) * 0.000001;
            double JCO2_JH2O = JCO2 / JH2O / CiModifier;
            return JCO2_JH2O;
        }
    }
}
