// Authors: Caren Dymond, Sarah Beukema

// NOTE: IEcoregion --> Landis.Core
// NOTE: ISpecies --> Landis.Core

using System.IO;
using Landis.Core;
using Landis.Library.Parameters;

namespace Landis.Extension.Succession.PnETForC
{
    public class SpeciesData
    {
        public static Library.Parameters.Species.AuxParm<int> FuncType;
        public static Library.Parameters.Species.AuxParm<double> LeafLongevity;
        public static Library.Parameters.Species.AuxParm<bool> Epicormic;
        public static Library.Parameters.Species.AuxParm<double> MortCurveShape;
        public static Library.Parameters.Species.AuxParm<int> MerchStemsMinAge;
        public static Library.Parameters.Species.AuxParm<double> MerchCurveParmA;
        public static Library.Parameters.Species.AuxParm<double> MerchCurveParmB;
        public static Library.Parameters.Species.AuxParm<double> FracNonMerch;
        public static Library.Parameters.Species.AuxParm<double> GrowthCurveShapeParm;
        public static Library.Parameters.Species.AuxParm<byte> FireTolerance;
        public static Library.Parameters.Species.AuxParm<byte> ShadeTolerance;

        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double>> ProbEstablishment;
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double>> ANPP_MAX_Spp;
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<int>> B_MAX_Spp;

        //  Establishment probability modifier for each species in each ecoregion (from biomass succession)
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double>> ProbEstablishModifier;

        // Root parameters (why are these all arrays instead of single values? MG 20251113)
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double[]>> MinWoodyBiomass;
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double[]>> BGtoAGBiomassRatio;
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double[]>> FracFineRoots;
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double[]>> FineRootTurnoverRate;
        public static Library.Parameters.Species.AuxParm<Library.Parameters.Ecoregions.AuxParm<double[]>> CoarseRootTurnoverRate;
        private static IInputParams m_iParams;
        private static bool bWroteMsg1;
        

        //---------------------------------------------------------------------
        public static void Initialize(IInputParams parameters)
        {
            FuncType = parameters.SppFunctionalType;
            LeafLongevity = parameters.LeafLongevity;
            Epicormic = parameters.Epicormic;
            MortCurveShape = parameters.MortCurveShape;
            MerchStemsMinAge = parameters.MerchStemsMinAge;
            MerchCurveParmA = parameters.MerchCurveParmA;
            MerchCurveParmB = parameters.MerchCurveParmB;
            FracNonMerch = parameters.FracNonMerch;
            GrowthCurveShapeParm = parameters.GrowthCurveShapeParm;
            FireTolerance = parameters.FireTolerance;
            ShadeTolerance = parameters.ShadeTolerance;

            // Roots
            // NOTE: the 5-element arrays here are lists of points in a 
            // user-specified growth curve, and "i" is the loop counter
            // to progress through those points during variable assignment.
            MinWoodyBiomass = Util.CreateSpeciesEcoregionArrayParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions, 5);
            BGtoAGBiomassRatio = Util.CreateSpeciesEcoregionArrayParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions, 5);
            FracFineRoots = Util.CreateSpeciesEcoregionArrayParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions, 5);
            FineRootTurnoverRate = Util.CreateSpeciesEcoregionArrayParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions, 5);
            CoarseRootTurnoverRate = Util.CreateSpeciesEcoregionArrayParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions, 5);
            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (EcoregionData.ActiveSiteCount[ecoregion] == 0)
                    continue;
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MinWoodyBiomass[species][ecoregion][i] = parameters.MinWoodyBiomass[ecoregion][species][i];
                        BGtoAGBiomassRatio[species][ecoregion][i] = parameters.BGtoAGBiomassRatio[ecoregion][species][i];
                        FracFineRoots[species][ecoregion][i] = parameters.FracFineRoots[ecoregion][species][i];
                        FineRootTurnoverRate[species][ecoregion][i] = parameters.FineRootTurnoverRate[ecoregion][species][i];
                        CoarseRootTurnoverRate[species][ecoregion][i] = parameters.CoarseRootTurnoverRate[ecoregion][species][i];
                    }
                }
            }
            m_iParams = parameters;
            bWroteMsg1 = false;
            // The initial ANPP and max biomass:
            GenerateNewANPPandMaxBiomass(parameters.Timestep, 0);
        }

        public static void GenerateNewANPPandMaxBiomass(int years, int spinupyear)
        {
            ANPP_MAX_Spp = Util.CreateSpeciesEcoregionParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
            B_MAX_Spp = Util.CreateSpeciesEcoregionParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
            ProbEstablishment = Util.CreateSpeciesEcoregionParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
            ProbEstablishModifier = Util.CreateSpeciesEcoregionParm<double>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
            // double MeanAnnualTemperature = 0.0;
            int usetime = PlugIn.ModelCore.CurrentTime;
            if (spinupyear < 0)
                usetime = spinupyear;
            for (int y = 0; y < years; ++y) 
            {
                foreach(IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions) 
                {
                    if (EcoregionData.ActiveSiteCount[ecoregion] == 0)
                        continue;
                     /* CODE RELATED TO THE USE OF ONE OF THE BIGGER LANDIS CLIMATE LIBRARIES
                    AnnualClimate_Monthly ecoClimate = EcoregionData.AnnualWeather[ecoregion]; //Climate Library v2
                    //AnnualClimate ecoClimate = EcoregionData.AnnualClimateArray[ecoregion][y];//Climate Library on GitHub
                    if(ecoClimate == null)
                        throw new System.ApplicationException("Error: CLIMATE NULL.");
                    if (usetime >= 0)
                        MeanAnnualTemperature = (double)ecoClimate.CalculateMeanAnnualTemp(usetime); //Climate Library v2
                        //MeanAnnualTemperature = (double)ecoClimate.MeanAnnualTemp(usetime);  //Climate Library on GitHub
                    else
                        MeanAnnualTemperature = (double)ecoClimate.CalculateMeanAnnualTemp(0); //Climate Library v2
                        //MeanAnnualTemperature = (double)ecoClimate.MeanAnnualTemp(0);  //Climate Library on GitHub
                    */
                    foreach(ISpecies species in PlugIn.ModelCore.Species)
                    {
                        IANPP anpp;
                        ProbEstablishModifier[species][ecoregion] = 1.0;
                        if (m_iParams.ANPPTimeCollection[ecoregion][species].TryGetValue(usetime + y, out anpp))
                        {
                            PlugIn.ModelCore.NormalDistribution.Mu = anpp.GramsPerMetre2Year;
                            PlugIn.ModelCore.NormalDistribution.Sigma = anpp.StdDev;
                            double sppANPP = PlugIn.ModelCore.NormalDistribution.NextDouble();
                            sppANPP = PlugIn.ModelCore.NormalDistribution.NextDouble();
                            ANPP_MAX_Spp[species][ecoregion] += sppANPP;
                        }
                        else if (usetime < 0)
                        {
                            // user didn't enter anything for all the spin-up time, and we must use the year 0 time.
                            if (m_iParams.ANPPTimeCollection[ecoregion][species].TryGetValue(0, out anpp))
                            {
                                if (!bWroteMsg1)
                                {
                                    PlugIn.ModelCore.UI.WriteLine("ANPP values were not entered for the earliest spin-up years. Year 0 values will be used.");
                                    bWroteMsg1 = true;
                                }
                                PlugIn.ModelCore.NormalDistribution.Mu = anpp.GramsPerMetre2Year;
                                PlugIn.ModelCore.NormalDistribution.Sigma = anpp.StdDev;
                                double sppANPP = PlugIn.ModelCore.NormalDistribution.NextDouble();
                                sppANPP = PlugIn.ModelCore.NormalDistribution.NextDouble();
                                ANPP_MAX_Spp[species][ecoregion] += sppANPP;
                            }
                        }
                        IMaxBiomass maxbio;
                        if (m_iParams.MaxBiomassTimeCollection[ecoregion][species].TryGetValue(usetime + y, out maxbio))
                            B_MAX_Spp[species][ecoregion] = (int)maxbio.MaxBio;
                        else if (usetime < 0)
                        {
                            // user didn't enter anything for all the spin-up time, and we must use the year 0 time.                            if (!bWroteMsg2)
                            PlugIn.ModelCore.UI.WriteLine("MaxBiomass values were not entered for the earliest spin-up years. Year 0 values will be used.");
                            if (m_iParams.MaxBiomassTimeCollection[ecoregion][species].TryGetValue(0, out maxbio))
                                B_MAX_Spp[species][ecoregion] = (int)maxbio.MaxBio;
                        }
                        // Establishmenet is only used in projection, not spin-up. 
                        // Don't check for values when usetime < 0
                        IProbEstablishment probEstablishment;
                        if (usetime >= 0)
                        {
                            if (m_iParams.ProbEstablishmentTimeCollection[ecoregion][species].TryGetValue(usetime + y, out probEstablishment))
                                ProbEstablishment[species][ecoregion] = (double)probEstablishment.Establishment;
                        }
                    }
                }
            }            
            EcoregionData.UpdateB_MAX();
            return;           
        }
    }
}
