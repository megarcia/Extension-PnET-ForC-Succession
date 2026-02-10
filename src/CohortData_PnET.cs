// NOTE: ISpecies --> Landis.Core

using System.Collections.Generic;
using System;
using Landis.Core;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// Data for an individual cohort that is not shared with other cohorts.
    /// </summary>
    public struct CohortData
    {
        /// <summary>
        /// The cohort
        /// </summary>
        public Cohort Cohort;

        /// <summary>
        /// The universal cohort data
        /// </summary>
        public Library.UniversalCohorts.CohortData UniversalData;

        /// <summary>
        /// Succession timestep used by Biomass cohorts (yrs)
        /// </summary>
        public byte SuccessionTimestep;

        /// <summary>
        /// The cohort's live aboveground biomass (g/m2).
        /// </summary>
        public double AGBiomass;

        /// <summary>
        /// The cohort's live total biomass (wood + root) (g/m2).
        /// </summary>
        public double TotalBiomass;

        /// <summary>
        /// Are trees phsyiologically active
        /// </summary>
        public bool IsLeafOn;

        /// <summary>
        /// Max biomass achived in the cohorts' life time. 
        /// This value remains high after the cohort has reached its 
        /// peak biomass. It is used to determine canopy layers where
        /// it prevents that a cohort could descent in the canopy when 
        /// it declines (g/m2)
        /// </summary>
        public double MaxBiomass;

        /// <summary>
        /// Foliage (g/m2)
        /// </summary>
        public double Fol;

        /// <summary>
        /// Maximum Foliage For The Year (g/m2)
        /// </summary>
        public double MaxFolYear;

        /// <summary>
        /// Non-Soluble Carbons
        /// </summary>
        public double NSC;

        /// <summary>
        /// Defoliation Fraction
        /// </summary>
        public double DefoliationFrac;

        /// <summary>
        /// Annual Wood Senescence (g/m2)
        /// </summary>
        public double LastWoodSenescence;

        /// <summary>
        /// Annual Foliage Senescence (g/m2)
        /// </summary>
        public double LastFolSenescence;

        /// <summary>
        /// Last Average FRad
        /// </summary>
        public double LastFRad;

        /// <summary>
        /// Last Growing Season FRad
        /// </summary>
        public List<double> LastSeasonFRad;

        /// <summary>
        /// Adjusted Fraction of Foliage
        /// </summary>
        public double adjFolBiomassFrac;

        /// <summary>
        /// Adjusted Half Sat
        /// </summary>
        public double AdjHalfSat;

        /// <summary>
        /// Adjusted Foliage Carbons
        /// </summary>
        public double adjFolN;

        /// <summary>
        /// Boolean whether cohort has been killed by cold temp relative to cold tolerance
        /// </summary>
        public int ColdKill;

        /// <summary>
        /// The Layer of the Cohort
        /// </summary>
        public byte Layer;

        /// <summary>
        /// Leaf area index per subcanopy layer (m/m)
        /// </summary>
        public double[] LAI;

        /// <summary>
        /// Leaf area index (m/m) maximum last year
        /// </summary>
        public double LastLAI;

        /// <summary>
        /// Aboveground Biomass last year
        /// </summary>
        public double LastAGBio;

        /// <summary>
        /// Gross photosynthesis (gC/mo)
        /// </summary>
        public double[] GrossPsn;

        /// <summary>
        /// Foliar respiration (gC/mo)
        /// </summary>
        public double[] FoliarRespiration;

        /// <summary>
        /// Net photosynthesis (gC/mo)
        /// </summary>
        public double[] NetPsn;

        /// <summary>
        /// Mainenance respiration (gC/mo)
        /// </summary>
        public double[] MaintenanceRespiration;

        /// <summary>
        /// Transpiration (mm/mo)
        /// </summary>
        public double[] Transpiration;

        /// <summary>
        /// PotentialTranspiration (mm/mo)
        /// </summary>
        public double[] PotentialTranspiration;

        /// <summary>
        /// Reduction factor for suboptimal radiation on growth
        /// </summary>
        public double[] FRad;

        /// <summary>
        /// Reduction factor for suboptimal or supra optimal water 
        /// </summary>
        public double[] FWater;

        /// <summary>
        /// Actual water used to calculate FWater
        /// </summary>
        public double[] SoilWaterContent;

        /// <summary>
        /// Actual pressurehead used to calculate FWater
        /// </summary>
        public double[] PressHead;

        /// <summary>
        /// Number of precip events allocated to sublayer
        /// </summary>
        public int[] NumPrecipEvents;

        /// <summary>
        /// Reduction factor for ozone 
        /// </summary>
        public double[] FOzone;

        /// <summary>
        /// Interception (mm/mo)
        /// </summary>
        public double[] Interception;

        /// <summary>
        /// Adjustment folN based on fRad
        /// </summary>
        public double[] AdjFolN;

        /// <summary>
        /// Adjustment folBiomassFrac based on fRad
        /// </summary>
        public double[] AdjFolBiomassFrac;

        /// <summary>
        /// Modifier of CiCa ratio based on fWater and Ozone
        /// </summary>
        public double[] CiModifier;

        /// <summary>
        /// Adjustment to Amax based on CO2
        /// </summary>
        public double[] DelAmax;

        /// <summary>
        /// Fraction of layer biomass attributed to cohort
        /// </summary>
        public double BiomassLayerFrac;

        /// <summary>
        /// Fraction of layer canopy (foliage) attributed to cohort
        /// </summary>
        public double CanopyLayerFrac;

        /// <summary>
        /// Fraction of layer canopy growing space available to cohort
        /// </summary>
        public double CanopyGrowingSpace;

        /// <summary>
        /// CohortData constructor #1
        /// </summary>
        /// <param name="cohort"></param>
        public CohortData(Cohort cohort)
        {
            SuccessionTimestep = cohort.SuccessionTimestep;
            AdjFolN = cohort.AdjFolN;
            adjFolN = cohort.adjFolN;
            AdjFolBiomassFrac = cohort.AdjFolBiomassFrac;
            adjFolBiomassFrac = cohort.adjFolBiomassFrac;
            AdjHalfSat = cohort.AdjHalfSat;
            UniversalData.Age = cohort.Age;
            AGBiomass = cohort.PnETSpecies.AGBiomassFrac * cohort.TotalBiomass + cohort.Fol;
            UniversalData.Biomass = (int)(AGBiomass * cohort.CanopyLayerFrac);
            TotalBiomass = cohort.TotalBiomass;
            MaxBiomass = cohort.MaxBiomass;
            CiModifier = cohort.CiModifier;
            ColdKill = cohort.ColdKill;
            DefoliationFrac = cohort.DefoliationFrac;
            DelAmax = cohort.DelAmax;
            Fol = cohort.Fol;
            MaxFolYear = cohort.MaxFolYear;
            FoliarRespiration = cohort.FoliarRespiration;
            FOzone = cohort.FOzone;
            FRad = cohort.FRad;
            FWater = cohort.FWater;
            GrossPsn = cohort.GrossPsn;
            Interception = cohort.Interception;
            LAI = cohort.LAI;
            LastLAI = cohort.LastLAI;
            LastFolSenescence = cohort.LastFolSenescence;
            LastFRad = cohort.LastFRad;
            LastSeasonFRad = cohort.LastSeasonFRad;
            LastWoodSenescence = cohort.LastWoodSenescence;
            LastAGBio = cohort.LastAGBio;
            Layer = cohort.Layer;
            IsLeafOn = cohort.IsLeafOn;
            MaintenanceRespiration = cohort.MaintenanceRespiration;
            NetPsn = cohort.NetPsn;
            NSC = cohort.NSC;
            PressHead = cohort.PressHead;
            NumPrecipEvents = cohort.NumPrecipEvents;
            Transpiration = cohort.Transpiration;
            PotentialTranspiration = cohort.PotentialTranspiration;
            SoilWaterContent = cohort.SoilWaterContent;
            BiomassLayerFrac = cohort.BiomassLayerFrac;
            CanopyLayerFrac = cohort.CanopyLayerFrac;
            Cohort = cohort;
            CanopyGrowingSpace = cohort.CanopyGrowingSpace;
            UniversalData.ANPP = cohort.ANPP;
        }

        /// <summary>
        /// CohortData constructor #2
        /// </summary>
        /// <param name="age"></param>
        /// <param name="successionTimestep"></param>
        /// <param name="totalBiomass"></param>
        /// <param name="totalANPP"></param>
        /// <param name="species"></param>
        /// <param name="cohortStacking"></param>
        public CohortData(ushort age, byte successionTimestep, double totalBiomass, double totalANPP, ISpecies species, bool cohortStacking)
        {
            SuccessionTimestep = successionTimestep;
            AdjFolN = new double[Globals.IMAX];
            adjFolN = 0.0; ;
            AdjFolBiomassFrac = new double[Globals.IMAX];
            adjFolBiomassFrac = 0.0;
            AdjHalfSat = 0.0;
            UniversalData.Age = age;
            IPnETSpecies pnetspecies = SpeciesParameters.PnETSpecies.AllSpecies[species.Index];
            Cohort = new Cohort(species, pnetspecies, 0, "", 1, cohortStacking, successionTimestep);
            AGBiomass = pnetspecies.AGBiomassFrac * totalBiomass;
            UniversalData.Biomass = (int)AGBiomass;
            TotalBiomass = totalBiomass;
            MaxBiomass = totalBiomass;
            CiModifier = new double[Globals.IMAX];
            ColdKill = int.MaxValue;
            DefoliationFrac = 0.0;
            DelAmax = new double[Globals.IMAX];
            Fol = 0.0;
            MaxFolYear = 0.0;
            FoliarRespiration = new double[Globals.IMAX];
            FOzone = new double[Globals.IMAX];
            FRad = new double[Globals.IMAX];
            FWater = new double[Globals.IMAX];
            GrossPsn = new double[Globals.IMAX];
            Interception = new double[Globals.IMAX];
            LAI = new double[Globals.IMAX];
            LastFolSenescence = 0.0;
            LastFRad = 0.0;
            LastSeasonFRad = new List<double>();
            LastWoodSenescence = 0.0;
            LastAGBio = AGBiomass;
            Layer = 0;
            IsLeafOn = false;
            MaintenanceRespiration = new double[Globals.IMAX];
            NetPsn = new double[Globals.IMAX];
            NSC = 0.0;
            PressHead = new double[Globals.IMAX];
            NumPrecipEvents = new int[Globals.IMAX];
            Transpiration = new double[Globals.IMAX];
            PotentialTranspiration = new double[Globals.IMAX];
            SoilWaterContent = new double[Globals.IMAX];
            BiomassLayerFrac = 1.0;
            double cohortIdealFol = pnetspecies.FolBiomassFrac * Math.Exp(-pnetspecies.LiveWoodBiomassFrac * MaxBiomass) * TotalBiomass;
            double cohortLAI = Canopy.CalcCohortLAI(pnetspecies, cohortIdealFol);
            LastLAI = cohortLAI;
            CanopyLayerFrac = LastLAI / pnetspecies.MaxLAI;
            if (cohortStacking)
                CanopyLayerFrac = 1.0;
            CanopyGrowingSpace = 1.0;
            UniversalData.ANPP = totalANPP;
        }
    }
}
