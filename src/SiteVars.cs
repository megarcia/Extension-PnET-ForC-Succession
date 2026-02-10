// Original Authors: Robert M. Scheller, James B. Domingo
// Merged PnET-ForC version by Matthew Garcia, 10 Feb 2026

// NOTE: ActiveSite --> Landis.SpatialModeling
// NOTE: Debug --> System.Diagnostics
// NOTE: ISiteVar --> Landis.SpatialModeling
// NOTE: Pool --> Landis.Library.UniversalCohorts
// NOTE: Process --> System.Diagnostics
// NOTE: SiteCohorts --> Landis.Library.UniversalCohorts

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// Site (grid-cell) biomass pools and related variables.
    /// </summary>
    public static class SiteVars
    {
        // Fundamentals
        public static ISiteVar<SiteCohorts> SiteCohorts;  // PnET + ForC
        public static ISiteVar<Library.UniversalCohorts.SiteCohorts> UniversalCohorts;  // PnET
        private static ISiteVar<int> lastSuccessionTime;  // ForC
        private static ISiteVar<int> totalBiomass;  // ForC
        public static ISiteVar<double> capacityReduction;  // ForC
        public static ISiteVar<double> AbovegroundNPPcarbon;  // ForC

        // Mortality
        private static ISiteVar<int> previousYearMortality;  // ForC
        private static ISiteVar<int> currentYearMortality;  // ForC

        // Weather-related variables
        public static ISiteVar<double> ExtremeMinTemp;  // PnET
        public static ISiteVar<double> AnnualPotentialEvaporation;  // PnET
        public static ISiteVar<double> ClimaticWaterDeficit;  // PnET

        // Debris/litter variables:
        public static ISiteVar<Pool> WoodyDebris;  // PnET + ForC
        public static ISiteVar<Pool> LeafLitter;  // PnET + ForC
        public static ISiteVar<double> WoodyDebrisDecayRate;  // ForC
        public static ISiteVar<double> LeafLitterDecayRate;  // ForC
        public static ISiteVar<double> FineFuels;  // PnET

        // Soil variables 
        public static ISiteVar<double> PressureHead;  // PnET
        public static ISiteVar<double[]> MonthlyPressureHead;  // PnET
        public static ISiteVar<SortedList<double, double>[]> MonthlySoilTemp;  // PnET
        public static ISiteVar<double> FieldCapacity;  // PnET
        public static ISiteVar<double> SoilOrganicMatterC;  // ForC
        public static ISiteVar<SoilC> soilC;  // ForC

        // Disturbance variables
        public static ISiteVar<double> SmolderConsumption;  // PnET
        public static ISiteVar<double> FlamingConsumption;  // PnET
        public static ISiteVar<byte> fireSeverity;  // ForC
        public static ISiteVar<string> HarvestPrescriptionName;  // ForC

        // Site-level values for map output
        public static ISiteVar<double> NPP;  // ForC
        public static ISiteVar<double> RH;  // ForC
        public static ISiteVar<double> NEP;  // ForC
        public static ISiteVar<double> NBP;  // ForC
        public static ISiteVar<double> TotBiomassC;  // ForC
        public static ISiteVar<double> ToFPSC;  // ForC

        /// <summary>
        /// Biomass cohorts.
        /// </summary>
        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return SiteCohorts;
            }
            set
            {
                SiteCohorts = value;
            }
        }

        /// <summary>
        /// Time of last Succession simulation (apparently unused).
        /// </summary>
        public static ISiteVar<int> LastSuccessionTime
        {
            get
            {
                return lastSuccessionTime;
            }
        }

        /// <summary>
        /// Fire severity.
        /// </summary>
        public static ISiteVar<byte> FireSeverity
        {
            get
            {
                return fireSeverity;
            }
            set
            {
                fireSeverity = value;
            }

        }

        /// <summary>
        /// Reduction fraction for maximum site biomass.
        /// </summary>
        public static ISiteVar<double> CapacityReduction
        {
            get
            {
                return capacityReduction;
            }
            set
            {
                capacityReduction = value;
            }
        }

        /// <summary>
        /// Site total biomass.
        /// </summary>
        public static ISiteVar<int> TotalBiomass
        {
            get
            {
                return totalBiomass;
            }
        }

        /// <summary>
        /// Previous year site mortality.
        /// </summary>
        public static ISiteVar<int> PreviousYearMortality
        {
            get
            {
                return previousYearMortality;
            }
        }

        /// <summary>
        /// Current year site mortality.
        /// </summary>
        public static ISiteVar<int> CurrentYearMortality
        {
            get
            {
                return currentYearMortality;
            }
        }

        /// <summary>
        /// Module Initialization.
        /// </summary>
        public static void Initialize(IInputParams iParams, IInputDisturbanceMatrixParams iDMParams)
        {
            Debug.Assert(iParams != null);

            // Fundamentals
            UniversalCohorts = PlugIn.ModelCore.Landscape.NewSiteVar<Library.UniversalCohorts.SiteCohorts>();  // PnET
            SiteCohorts = PlugIn.ModelCore.Landscape.NewSiteVar<SiteCohorts>();  // ForC
            lastSuccessionTime = PlugIn.ModelCore.Landscape.NewSiteVar<int>();  // ForC
            totalBiomass = PlugIn.ModelCore.Landscape.NewSiteVar<int>();  // ForC
            AbovegroundNPPcarbon = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC

            // Mortality
            previousYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();  // ForC
            currentYearMortality = PlugIn.ModelCore.Landscape.NewSiteVar<int>();  // ForC

            // Weather
            ExtremeMinTemp = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // PnET
            AnnualPotentialEvaporation = PlugIn.ModelCore.Landscape.NewSiteVar<Double>();  // PnET
            ClimaticWaterDeficit = PlugIn.ModelCore.Landscape.NewSiteVar<Double>();  // PnET

            // Debris/litter
            WoodyDebris = PlugIn.ModelCore.Landscape.NewSiteVar<Pool>();  // PnET + ForC
            LeafLitter = PlugIn.ModelCore.Landscape.NewSiteVar<Pool>();  // PnET + ForC
            FineFuels = PlugIn.ModelCore.Landscape.NewSiteVar<Double>();  // PnET
            WoodyDebrisDecayRate = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            LeafLitterDecayRate = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC

            // Soil
            PressureHead = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // PnET
            MonthlyPressureHead = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();  // PnET
            MonthlySoilTemp = PlugIn.ModelCore.Landscape.NewSiteVar<SortedList<double, double>[]>();  // PnET
            FieldCapacity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // PnET
            soilC = PlugIn.ModelCore.Landscape.NewSiteVar<SoilC>();  // ForC
            SoilOrganicMatterC = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC

            // Disturbance
            SmolderConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<Double>();  // PnET
            FlamingConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<Double>();  // PnET
            fireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");  // ForC
            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");  // ForC

            // Outputs
            NPP = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            RH = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            NEP = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            NBP = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            TotBiomassC = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            ToFPSC = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // ForC
            
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                // site cohorts are initialized by the PlugIn.InitializeSite method
                soilC[site] = new SoilC(iParams, site, iDMParams);  // ForC
                WoodyDebris[site] = new Pool();  // ForC
                LeafLitter[site] = new Pool();  // ForC
            }

            // Succession variables
            PlugIn.ModelCore.RegisterSiteVar(siteCohorts, "Succession.UniversalCohorts");  // ForC
            PlugIn.ModelCore.RegisterSiteVar(WoodyDebris, "Succession.WoodyDebris");  // PnET + ForC
            PlugIn.ModelCore.RegisterSiteVar(LeafLitter, "Succession.Litter");  // PnET + ForC
            PlugIn.ModelCore.RegisterSiteVar(FineFuels, "Succession.FineFuels"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(PressureHead, "Succession.PressureHead"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(MonthlyPressureHead, "Succession.MonthlyPressureHead"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(MonthlySoilTemp, "Succession.MonthlySoilTemp"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(FieldCapacity, "Succession.SoilFieldCapacity"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(ExtremeMinTemp, "Succession.ExtremeMinTemp"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(AnnualPotentialEvaporation, "Succession.PET"); // PnET // FIXME ???
            PlugIn.ModelCore.RegisterSiteVar(ClimaticWaterDeficit, "Succession.CWD"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(SmolderConsumption, "Succession.SmolderConsumption"); // PnET
            PlugIn.ModelCore.RegisterSiteVar(FlamingConsumption, "Succession.FlamingConsumption"); // PnET
        }

        // Reset certain site values
        public static void ResetAnnualValues(Site site)
        {
            AbovegroundNPPcarbon[site] = 0.0;
            TotalBiomass[site] = 0;
            TotalBiomass[site] = Library.UniversalCohorts.Cohorts.ComputeNonYoungBiomass(SiteVars.Cohorts[site]);
            PreviousYearMortality[site] = CurrentYearMortality[site];
            CurrentYearMortality[site] = 0;
        }
    }
}
