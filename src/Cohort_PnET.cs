// authors: 

// NOTE: ActiveSite --> Landis.SpatialModeling
// NOTE: DeathEvent --> Library.UniversalCohorts.DeathEventHandler
// NOTE: DeathEventArgs --> Library.UniversalCohorts.DeathEventArgs
// NOTE: ExtensionType --> Landis.Core
// NOTE: ISpecies --> Landis.Core
// NOTE: Percentage --> Landis.Utilities

// uses dominance to allocate psn and subtract transpiration 
// from soil water, average cohort vars over layer

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Landis.Core;
using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;
using Landis.Utilities;

namespace Landis.Extension.Succession.PnETForC
{
    public class Cohort : Library.UniversalCohorts.Cohort, ICohort
    { 
        public delegate void SubtractTranspiration(double transpiration, IPnETSpecies Species);
        public ushort index;
        private ISpecies species;
        private IPnETSpecies PnETspecies;
        private CohortData data;
        private bool firstYear;
        private LocalOutput cohortoutput;

        /// <summary>
        /// Age (y)
        /// </summary>
        public new ushort Age  // also declared in UniversalCohorts.Cohort
        {
            get
            {
                return data.UniversalData.Age;
            }
        }

        /// <summary>
        /// Succession timestep used by biomass cohorts (yrs)
        /// </summary>
        public byte SuccessionTimestep
        {
            get
            {
                return data.SuccessionTimestep;
            }
        }

        /// <summary>
        /// Non soluble carbons
        /// </summary>
        public double NSC
        {
            get
            {
                return data.NSC;
            }
            set
            {
                data.NSC = value;
            }
        }

        /// <summary>
        /// The cohort's data
        /// </summary>
        public new CohortData Data  // also declared in UniversalCohorts.Cohort
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Maximum Foliage Value For Current Year
        /// </summary>
        public double MaxFolYear
        {
            get
            {
                return data.MaxFolYear;
            }
            set
            {
                data.MaxFolYear = value;
            }
        }

        /// <summary>
        /// Measure of cohort's diffuse reflection of solar radiation 
        /// out of total solar radiation, without snow reflectance
        /// </summary>
        public double Albedo
        {
            get
            {
                double albedo = 0;
                bool lifeform = !string.IsNullOrEmpty(PnETSpecies.Lifeform);
                bool ground = PnETSpecies.Lifeform.ToLower().Contains("ground");
                bool open = PnETSpecies.Lifeform.ToLower().Contains("open");
                bool dark = PnETSpecies.Lifeform.ToLower().Contains("dark");
                bool light = PnETSpecies.Lifeform.ToLower().Contains("light");
                bool deciduous = PnETSpecies.Lifeform.ToLower().Contains("decid");
                if (lifeform && (ground || open || SumLAI == 0))
                    albedo = 0.20F;
                else if (lifeform && dark)
                    albedo = (double)((-0.067 * Math.Log(SumLAI < 0.7 ? 0.7 : SumLAI)) + 0.2095);
                else if (lifeform && light)
                    albedo = (double)((-0.054 * Math.Log(SumLAI < 0.7 ? 0.7 : SumLAI)) + 0.2082);
                else if (lifeform && deciduous)
                    albedo = (double)((-0.0073 * SumLAI) + 0.231);
                // Do not allow albedo to be negative
                return albedo > 0 ? albedo : 0;
            }
        }

        /// <summary>
        /// Foliage (g/m2)
        /// </summary>
        public double Fol
        {
            get
            {
                return data.Fol;
            }
            set
            {
                data.Fol = value;
            }
        }

        /// <summary>
        /// Aboveground Biomass (g/m2) scaled to the site
        /// </summary>
        public new int Biomass  // also declared in UniversalCohorts.Cohort
        {
            get
            {
                return (int)(data.AGBiomass * data.CanopyLayerFrac);
            }
        }

        /// <summary>
        /// Species Moss Depth (m)
        /// </summary>
        public double MossDepth
        {
            get
            {
                return data.UniversalData.Biomass * PnETspecies.MossScalar;
            }
        }

        /// <summary>
        /// Aboveground Biomass (g/m2)
        /// </summary>
        public int AGBiomass
        {
            get
            {
                return (int)(Math.Round(PnETspecies.AGBiomassFrac * data.TotalBiomass) + data.Fol);
            }
        }

        /// <summary>
        /// Total Biomass (root + wood) (g/m2)
        /// </summary>
        public int TotalBiomass
        {
            get
            {
                return (int)Math.Round(data.TotalBiomass);
            }
        }

        /// <summary>
        /// Wood (g/m2)
        /// </summary>
        public uint Wood
        {
            get
            {
                return (uint)Math.Round(PnETspecies.AGBiomassFrac * data.TotalBiomass);
            }
        }

        /// <summary>
        /// Root (g/m2)
        /// </summary>
        public uint Root
        {
            get
            {
                return (uint)Math.Round(PnETspecies.BGBiomassFrac * data.TotalBiomass);
            }
        }

        /// <summary>
        /// Max biomass achived in the cohorts' life time. 
        /// This value remains high after the cohort has reached its 
        /// peak biomass. It is used to determine canopy layers where
        /// it prevents that a cohort could descent in the canopy when 
        /// it declines (g/m2)
        /// </summary>
        public double MaxBiomass
        {
            get
            {
                return data.MaxBiomass;
            }
        }

        /// <summary>
        /// Boolean whether cohort has been killed by cold temp relative to cold tolerance
        /// </summary>
        public int ColdKill
        {
            get
            {
                return data.ColdKill;
            }
        }

        /// <summary>
        /// Add dead wood to last senescence
        /// </summary>
        /// <param name="senescence"></param>
        public void AccumulateWoodSenescence(int senescence)
        {
            data.LastWoodSenescence += senescence;
        }

        /// <summary>
        /// Add dead foliage to last senescence
        /// </summary>
        /// <param name="senescence"></param>
        public void AccumulateFolSenescence(int senescence)
        {
            data.LastFolSenescence += senescence;
        }

        /// <summary>
        /// Growth reduction factor for age
        /// </summary>
        double FAge
        {
            get
            {
                return Math.Max(0, 1.0 - Math.Pow(Age / (double)PnETspecies.Longevity, PnETspecies.PhotosynthesisFAge));
            }
        }

        /// <summary>
        /// NSC fraction: measure for resources
        /// </summary>
        public double NSCfrac
        {
            get
            {
                return NSC / (FActiveBiom * (data.TotalBiomass + Fol) * PnETSpecies.CFracBiomass);
            }
        }

        /// <summary>
        /// Species with PnET parameter additions
        /// </summary>
        public IPnETSpecies PnETSpecies
        {
            get
            {
                return PnETspecies;
            }
        }

        /// <summary>
        /// LANDIS species (without PnET parameter additions)
        /// </summary>
        public new ISpecies Species  // also declared in UniversalCohorts.Cohort
        {
            get
            {
                return (ISpecies)Globals.ModelCore.Species[species.Index];
            }
        }

        /// <summary>
        /// Defoliation fraction - BRM
        /// </summary>
        public double DefoliationFrac
        {
            get
            {
                return data.DefoliationFrac;
            }
            private set
            {
                data.DefoliationFrac = value;
            }
        }

        /// <summary>
        /// Annual Wood Senescence (g/m2)
        /// </summary>
        public double LastWoodSenescence
        {
            get
            {
                return data.LastWoodSenescence;
            }
            set
            {
                data.LastWoodSenescence = value;
            }
        }

        /// <summary>
        /// Annual Foliage Senescence (g/m2)
        /// </summary>
        public double LastFolSenescence
        {
            get
            {
                return data.LastFolSenescence;
            }
            set
            {
                data.LastFolSenescence = value;
            }
        }

        /// <summary>
        /// Last average FRad
        /// </summary>
        public double LastFRad
        {
            get
            {
                return data.LastFRad;
            }
        }

        public double adjFolN
        {
            get
            {
                return data.adjFolN;
            }
        }

        public double[] AdjFolN
        {
            get
            {
                return data.AdjFolN;
            }
        }

        public double adjFolBiomassFrac
        {
            get
            {
                return data.adjFolBiomassFrac;
            }
        }

        public double[] AdjFolBiomassFrac
        {
            get
            {
                return data.AdjFolBiomassFrac;
            }
        }

        public double AdjHalfSat
        {
            get
            {
                return data.AdjHalfSat;
            }
        }

        public double[] CiModifier
        {
            get
            {
                return data.CiModifier;
            }
        }

        public double[] DelAmax
        {
            get
            {
                return data.DelAmax;
            }
        }

        public double[] FoliarRespiration
        {
            get
            {
                return data.FoliarRespiration;
            }
        }

        public double[] FOzone
        {
            get
            {
                return data.FOzone;
            }
        }

        public double[] FRad
        {
            get
            {
                return data.FRad;
            }
        }

        public double[] FWater
        {
            get
            {
                return data.FWater;
            }
        }

        public double[] GrossPsn
        {
            get
            {
                return data.GrossPsn;
            }
        }

        public double[] Interception
        {
            get
            {
                return data.Interception;
            }
        }

        public double[] LAI
        {
            get
            {
                return data.LAI;
            }
        }        

        public double LastLAI
        {
            get
            {
                return data.LastLAI;
            }
            set
            {
                data.LastLAI = value;
            }
        }

        public double LastAGBio
        {
            get
            {
                return data.LastAGBio;
            }
            set
            {
                data.LastAGBio = value;
            }
        }

        public List<double> LastSeasonFRad
        {
            get
            {
                return data.LastSeasonFRad;
            }
        }

        public byte Layer
        {
            get
            {
                return data.Layer;
            }
            set
            {
                data.Layer = value;
            }
        }

        public bool IsLeafOn
        {
            get
            {
                return data.IsLeafOn;
            }
        }

        public double[] MaintenanceRespiration
        {
            get
            {
                return data.MaintenanceRespiration;
            }
        }

        public double[] NetPsn
        {
            get
            {
                return data.NetPsn;
            }
        }

        public double[] PressHead
        {
            get
            {
                return data.PressHead;
            }
        }

        public double[] Transpiration
        {
            get
            {
                return data.Transpiration;
            }
        }

        public double[] PotentialTranspiration
        {
            get
            {
                return data.PotentialTranspiration;
            }
        }

        public double[] SoilWaterContent
        {
            get
            {
                return data.SoilWaterContent;
            }
        }

        public int[] NumPrecipEvents
        {
            get
            {
                return data.NumPrecipEvents;
            }
        }

        public double FActiveBiom
        {
            get
            {
                return Math.Exp(-PnETspecies.LiveWoodBiomassFrac * data.MaxBiomass);
            }
        }

        /// <summary>
        /// Determine if cohort is alive. It is assumed that a cohort is dead when 
        /// NSC decline below 1% of biomass
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return NSCfrac > 0.01;
            }
        }

        public double SumLAI
        {
            get
            {
                if (data.LAI == null)
                    return 0;
                return data.LAI.Sum();
            }
        }

        public double BiomassLayerFrac
        {
            get
            {
                return data.BiomassLayerFrac;
            }
            set
            {
                data.BiomassLayerFrac = value;
            }
        }

        public double CanopyLayerFrac
        {
            get
            {
                return data.CanopyLayerFrac;
            }
            set
            {
                data.CanopyLayerFrac = value;
            }
        }

        public double CanopyGrowingSpace
        {
            get
            {
                return data.CanopyGrowingSpace;
            }
            set
            {
                data.CanopyGrowingSpace = value;
            }
        }

        public new double ANPP  // also declared in UniversalCohorts.Cohort
        {
            get
            {
                return data.UniversalData.ANPP;
            }
            set
            {
                data.UniversalData.ANPP = value;
            }
        }

        /// <summary>
        /// List of DisturbanceTypes that have had ReduceDeadPools applied
        /// </summary>
        public List<ExtensionType> ReducedTypes = null;

        /// <summary>
        /// Index of growing season month
        /// </summary>
        public int growMonth = -1;

        /// <summary>
        /// Initialize subcanopy layers
        /// </summary>
        public void InitializeSubLayers()
        {
            index = 0;
            data.LAI = new double[Globals.IMAX];
            data.GrossPsn = new double[Globals.IMAX];
            data.FoliarRespiration = new double[Globals.IMAX];
            data.NetPsn = new double[Globals.IMAX];
            data.Transpiration = new double[Globals.IMAX];
            data.PotentialTranspiration = new double[Globals.IMAX];
            data.FRad = new double[Globals.IMAX];
            data.FWater = new double[Globals.IMAX];
            data.SoilWaterContent = new double[Globals.IMAX];
            data.PressHead = new double[Globals.IMAX];
            data.NumPrecipEvents = new int[Globals.IMAX];
            data.FOzone = new double[Globals.IMAX];
            data.MaintenanceRespiration = new double[Globals.IMAX];
            data.Interception = new double[Globals.IMAX];
            data.AdjFolN = new double[Globals.IMAX];
            data.AdjFolBiomassFrac = new double[Globals.IMAX];
            data.CiModifier = new double[Globals.IMAX];
            data.DelAmax = new double[Globals.IMAX];
        }

        /// <summary>
        /// Reset values for subcanopy layers
        /// </summary>
        public void NullSubLayers()
        {
            data.LAI = null;
            data.GrossPsn = null;
            data.FoliarRespiration = null;
            data.NetPsn = null;
            data.Transpiration = null;
            data.PotentialTranspiration = null;
            data.FRad = null;
            data.FWater = null;
            data.PressHead = null;
            data.NumPrecipEvents = null;
            data.SoilWaterContent = null;
            data.FOzone = null;
            data.MaintenanceRespiration = null;
            data.Interception = null;
            data.AdjFolN = null;
            data.AdjFolBiomassFrac = null;
            data.CiModifier = null;
            data.DelAmax = null;
        }

        public void StoreFRad()
        {
            // Filter for growing season months only
            if (data.IsLeafOn)
            {
                data.LastFRad = data.FRad.Average();
                data.LastSeasonFRad.Add(LastFRad);
            }
        }

        public void SetAvgFRad(double lastAvgFRad)
        {
            data.LastSeasonFRad.Add(lastAvgFRad);
        }

        public void ClearFRad()
        {
            data.LastSeasonFRad = new List<double>();
        }

        public void CalcAdjFolBiomassFrac()
        {
            if (data.LastSeasonFRad.Count() > 0)
            {
                double lastSeasonAvgFRad = data.LastSeasonFRad.ToArray().Average();
                double folBiomassFrac_slope = PnETspecies.FolBiomassFrac_slope;
                double folBiomassFrac_int = PnETspecies.FolBiomassFrac_intercept;
                //slope is shape parm; folBiomassFrac is minFolBiomassFrac; int is folBiomassFrac_intercept. EJG-7-24-18
                data.adjFolBiomassFrac = PnETspecies.FolBiomassFrac + ((folBiomassFrac_int - PnETspecies.FolBiomassFrac) * (double)Math.Pow(lastSeasonAvgFRad, folBiomassFrac_slope)); 
                firstYear = false;
            }
            else
                data.adjFolBiomassFrac = PnETspecies.FolBiomassFrac;
        }

        /// <summary>
        /// Get totals across several cohorts
        /// </summary>
        /// <param name="cohort"></param>
        public void Accumulate(Cohort cohort)
        {
            data.TotalBiomass += cohort.TotalBiomass;
            data.MaxBiomass = Math.Max(MaxBiomass, data.TotalBiomass);
            data.Fol += cohort.Fol;
            data.MaxFolYear = Math.Max(MaxFolYear, data.Fol);
            data.AGBiomass = cohort.PnETSpecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.UniversalData.ANPP += cohort.ANPP;
        }

        /// <summary>
        /// Increment the cohort age by one year.
        /// </summary>
        public new void IncrementAge()  // also declared in UniversalCohorts.Cohort
        {
            data.UniversalData.Age += 1;
        }

        /// <summary>
        /// Calculate the cohort biomass change.
        /// </summary>
        public int CalcBiomassChange()
        {
            int dBiomass = (int)data.AGBiomass - (int)data.LastAGBio;
            return dBiomass;
        }

        /// <summary>
        /// Change the cohort biomass.
        /// </summary>
        public new void ChangeBiomass(int dBiomass)  // also declared in UniversalCohorts.Cohort
        {
            double newTotalBiomass = data.TotalBiomass + dBiomass;
            data.TotalBiomass = Math.Max(0.0, newTotalBiomass);
            data.AGBiomass = PnETSpecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.MaxBiomass = Math.Max(data.MaxBiomass, data.TotalBiomass);
        }

        /// <summary>
        /// Change the cohort ANPP.
        /// </summary>
        public new void ChangeANPP(double dANPP)  // also declared in UniversalCohorts.Cohort
        {
            data.UniversalData.ANPP = data.UniversalData.ANPP + dANPP;
        }

        /// <summary>
        /// Constructor #1
        /// </summary>
        /// <param name="species"></param>
        /// <param name="PnETspecies"></param>
        /// <param name="establishmentYear"></param>
        /// <param name="SiteName"></param>
        /// <param name="fracBiomass"></param>
        /// <param name="cohortStacking"></param>
        /// <param name="successionTimestep"></param>
        public Cohort(ISpecies species, IPnETSpecies PnETspecies, ushort establishmentYear, string SiteName, double fracBiomass, bool cohortStacking, byte successionTimestep)
        {
            this.species = species;
            this.PnETspecies = PnETspecies;
            data.SuccessionTimestep = successionTimestep;
            data.UniversalData.Age = 1;
            data.ColdKill = int.MaxValue;
            data.NSC = (ushort)PnETspecies.InitialNSC;
            // Initialize biomass assuming fixed concentration of NSC, convert gC to gDW
            data.TotalBiomass = (uint)Math.Max(1.0, NSC / (PnETspecies.NSCFrac * PnETspecies.CFracBiomass) * fracBiomass);
            data.AGBiomass = PnETspecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.MaxBiomass = data.TotalBiomass;
            double cohortIdealFol = PnETspecies.FolBiomassFrac * FActiveBiom * data.TotalBiomass;
            double cohortLAI = Canopy.CalcCohortLAI(PnETSpecies, cohortIdealFol);
            data.LastLAI = cohortLAI;
            data.LastAGBio = data.AGBiomass;
            data.CanopyLayerFrac = data.LastLAI / PnETspecies.MaxLAI;
            if (cohortStacking)
                data.CanopyLayerFrac = 1.0;
            data.CanopyGrowingSpace = 1.0;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.UniversalData.ANPP = data.UniversalData.Biomass;
            // Then overwrite them if needed for output
            if (SiteName != null)
                InitializeOutput(SiteName, establishmentYear);
            data.LastSeasonFRad = new List<double>();
            firstYear = true;
        }

        /// <summary>
        /// Cohort constructor #2
        /// </summary>
        /// <param name="species"></param>
        /// <param name="cohortData"></param>
        public Cohort(ISpecies species, CohortData cohortData)
        {
            this.species = species;
            PnETspecies = SpeciesParameters.PnETSpecies.AllSpecies[species.Index];
            data = cohortData;
        }

        /// <summary>
        /// Cohort constructor #3 (cloning an existing cohort)
        /// </summary>
        /// <param name="cohort"></param>
        public Cohort(Cohort cohort)
        {
            species = cohort.Species;
            PnETspecies = cohort.PnETspecies;
            data.SuccessionTimestep = cohort.SuccessionTimestep;
            data.UniversalData.Age = cohort.Age;
            data.NSC = cohort.NSC;
            data.TotalBiomass = cohort.TotalBiomass;
            data.AGBiomass = cohort.PnETSpecies.AGBiomassFrac * cohort.TotalBiomass + cohort.Fol;
            data.UniversalData.Biomass = (int)(data.AGBiomass * cohort.CanopyLayerFrac);
            data.MaxBiomass = cohort.MaxBiomass;
            data.Fol = cohort.Fol;
            data.MaxFolYear = cohort.MaxFolYear;
            data.LastSeasonFRad = cohort.data.LastSeasonFRad;
            data.ColdKill = int.MaxValue;
            data.UniversalData.ANPP = cohort.ANPP;
        }

        /// <summary>
        /// Cohort constructor #4
        /// </summary>
        /// <param name="cohort"></param>
        /// <param name="establishmentYear"></param>
        /// <param name="SiteName"></param>
        public Cohort(Cohort cohort, ushort establishmentYear, string SiteName)
        {
            species = cohort.Species;
            PnETspecies = cohort.PnETspecies;
            data.SuccessionTimestep = cohort.SuccessionTimestep;
            data.UniversalData.Age = cohort.Age;
            data.NSC = cohort.NSC;
            data.TotalBiomass = cohort.TotalBiomass;
            data.AGBiomass = cohort.PnETSpecies.AGBiomassFrac * cohort.TotalBiomass + cohort.Fol;
            data.UniversalData.Biomass = (int)(data.AGBiomass * cohort.CanopyLayerFrac);
            data.MaxBiomass = cohort.MaxBiomass;
            data.Fol = cohort.Fol;
            data.MaxFolYear = cohort.MaxFolYear;
            data.LastSeasonFRad = cohort.data.LastSeasonFRad;
            data.ColdKill = int.MaxValue;
            data.UniversalData.ANPP = cohort.ANPP;
            if (SiteName != null)
                InitializeOutput(SiteName, establishmentYear);
        }

        /// <summary>
        /// Cohort constructor #5
        /// </summary>
        /// <param name="PnETspecies"></param>
        /// <param name="age"></param>
        /// <param name="woodBiomass"></param>
        /// <param name="SiteName"></param>
        /// <param name="establishmentYear"></param>
        /// <param name="cohortStacking"></param>
        /// <param name="successionTimestep"></param>
        public Cohort(IPnETSpecies PnETspecies, ushort age, int woodBiomass, string SiteName, ushort establishmentYear, bool cohortStacking, byte successionTimestep)
        {
            InitializeSubLayers();
            species = (ISpecies)PnETspecies;
            this.PnETspecies = PnETspecies;
            data.SuccessionTimestep = successionTimestep;
            data.UniversalData.Age = age;
            // incoming biomass is aboveground wood, calculate total biomass
            double biomass = woodBiomass / PnETspecies.AGBiomassFrac;
            data.TotalBiomass = biomass;
            data.MaxBiomass = biomass;
            data.LastSeasonFRad = new List<double>();
            data.adjFolBiomassFrac = PnETspecies.FolBiomassFrac_intercept;
            data.ColdKill = int.MaxValue;
            double cohortIdealFol = PnETspecies.FolBiomassFrac_intercept * FActiveBiom * data.TotalBiomass;
            double cohortLAI = 0;
            for (int i = 0; i < Globals.IMAX; i++)
            {
                double LAISum = Canopy.CalcLAISum(i, LAI);
                double subLayerLAI = Canopy.CalcLAI(PnETSpecies, cohortIdealFol, i, LAISum);
                cohortLAI += subLayerLAI;
                if (IsLeafOn)
                    LAI[index] = subLayerLAI;
            }
            if (IsLeafOn)
            {
                data.Fol = cohortIdealFol;
                data.MaxFolYear = cohortIdealFol;
            }
            data.LastLAI = cohortLAI;
            data.CanopyLayerFrac = data.LastLAI / PnETspecies.MaxLAI;
            if (cohortStacking)
                data.CanopyLayerFrac = 1.0f;
            data.CanopyGrowingSpace = 1.0f;
            data.AGBiomass = this.PnETspecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.LastAGBio = data.AGBiomass;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.NSC = this.PnETspecies.NSCFrac * FActiveBiom * (data.TotalBiomass + data.Fol) * PnETspecies.CFracBiomass;
            if (SiteName != null)
                InitializeOutput(SiteName, establishmentYear);
        }

        /// <summary>
        /// Cohort constructor #6
        /// </summary>
        /// <param name="PnETspecies"></param>
        /// <param name="age"></param>
        /// <param name="woodBiomass"></param>
        /// <param name="maxBiomass"></param>
        /// <param name="canopyGrowingSpace"></param>
        /// <param name="SiteName"></param>
        /// <param name="establishmentYear"></param>
        /// <param name="cohortStacking"></param>
        /// <param name="successionTimestep"></param>
        /// <param name="lastSeasonAvgFRad"></param>
        public Cohort(IPnETSpecies PnETspecies, ushort age, int woodBiomass, int maxBiomass, double canopyGrowingSpace, string SiteName, ushort establishmentYear, bool cohortStacking, byte successionTimestep, double lastSeasonAvgFRad)
        {
            InitializeSubLayers();
            species = (ISpecies)PnETspecies;
            this.PnETspecies = PnETspecies;
            data.SuccessionTimestep = successionTimestep;
            data.UniversalData.Age = age;
            // incoming biomass is aboveground wood, calculate total biomass
            double biomass = woodBiomass / PnETspecies.AGBiomassFrac;
            data.TotalBiomass = biomass;
            data.MaxBiomass = Math.Max(biomass, maxBiomass);
            data.LastSeasonFRad = new List<double>();
            data.LastSeasonFRad.Add(lastSeasonAvgFRad);
            CalcAdjFolBiomassFrac();
            data.ColdKill = int.MaxValue;
            double cohortIdealFol = adjFolBiomassFrac * FActiveBiom * data.TotalBiomass;
            double cohortLAI = 0;
            for (int i = 0; i < Globals.IMAX; i++)
            {
                double LAISum = Canopy.CalcLAISum(i, LAI);
                double subLayerLAI = Canopy.CalcLAI(PnETSpecies, cohortIdealFol, i, LAISum);
                cohortLAI += subLayerLAI;
                if (IsLeafOn)
                    LAI[index] = subLayerLAI;
            }
            if (IsLeafOn)
            {
                data.Fol = cohortIdealFol;
                data.MaxFolYear = cohortIdealFol;
            }
            data.LastLAI = cohortLAI;
            data.CanopyLayerFrac = data.LastLAI / PnETspecies.MaxLAI;
            if (cohortStacking)
                data.CanopyLayerFrac = 1.0f;
            data.CanopyGrowingSpace = 1.0f;
            data.AGBiomass = this.PnETspecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.LastAGBio = data.AGBiomass;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.NSC = this.PnETspecies.NSCFrac * FActiveBiom * (data.TotalBiomass + data.Fol) * PnETspecies.CFracBiomass;
            if (SiteName != null)
                InitializeOutput(SiteName, establishmentYear);
        }

        public void CalcDefoliationFrac(ActiveSite site, int SiteAGBiomass)
        {
            lock (Globals.DistributionThreadLock)
            {
                data.DefoliationFrac = (double)Library.UniversalCohorts.CohortDefoliation.Compute(site, this, 0, SiteAGBiomass);
            }
        }

        /// <summary>
        /// Photosynthesis by canopy layer
        /// </summary>
        /// <param name="PrecInByCanopyLayer"></param>
        /// <param name="precipCount"></param>
        /// <param name="leakageFrac"></param>
        /// <param name="hydrology"></param>
        /// <param name="mainLayerPAR"></param>
        /// <param name="SubCanopyPAR"></param>
        /// <param name="o3_cum"></param>
        /// <param name="o3_month"></param>
        /// <param name="subCanopyIndex"></param>
        /// <param name="layerCount"></param>
        /// <param name="fOzone"></param>
        /// <param name="frostFreeFrac"></param>
        /// <param name="MeltInByCanopyLayer"></param>
        /// <param name="coldKillBoolean"></param>
        /// <param name="variables"></param>
        /// <param name="siteCohort"></param>
        /// <param name="sumCanopyFrac"></param>
        /// <param name="groundPotentialETbyEvent"></param>
        /// <param name="allowMortality"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CalcPhotosynthesis(double PrecInByCanopyLayer, int precipCount, double leakageFrac, ref Hydrology hydrology, double mainLayerPAR, ref double SubCanopyPAR, double o3_cum, double o3_month, int subCanopyIndex, int layerCount, ref double fOzone, double frostFreeFrac, double snowpack, double MeltInByCanopyLayer, bool coldKillBoolean, IPnETEcoregionVars variables, SiteCohorts siteCohort, double sumCanopyFrac, double groundPotentialETbyEvent, bool allowMortality = true)
        {
            bool success = true;
            double lastFOzone = fOzone;
            fOzone = 0;
            // Leaf area index for the subcanopy layer by index. Function of specific leaf weight SLWMAX and the depth of the canopy
            // Depth of the canopy is expressed by the mass of foliage above this subcanopy layer (i.e. slwdel * index/imax *fol)
            double LAISum = Canopy.CalcLAISum(index, LAI);
            data.LAI[index] = Canopy.CalcLAI(PnETspecies, data.Fol, index, LAISum);
            if (MeltInByCanopyLayer > 0)
            {
                // Instantaneous runoff due to snowmelt (excess of soilPorosity)
                hydrology.CalcRunoff(siteCohort.Ecoregion, MeltInByCanopyLayer, frostFreeFrac, siteCohort.Site.Location.ToString());
                // Fast Leakage
                hydrology.CalcLeakage(siteCohort.Ecoregion, leakageFrac, frostFreeFrac, siteCohort.Site.Location.ToString());
            }
            if (PrecInByCanopyLayer > 0)
            {
                // If more than one precip event assigned to layer, repeat precip, runoff, leakage for all events prior to respiration
                for (int p = 1; p <= precipCount; p++)
                {
                    // Instantaneous runoff due to rain (excess of soilPorosity)
                    hydrology.CalcRunoff(siteCohort.Ecoregion, PrecInByCanopyLayer, frostFreeFrac, siteCohort.Site.Location.ToString());
                }
            }
            // Evaporation
            hydrology.CalcSoilEvaporation(siteCohort.Ecoregion, snowpack, frostFreeFrac, groundPotentialETbyEvent, siteCohort.Site.Location.ToString());
            // Infiltration (let captured surface water soak into soil)
            hydrology.CalcInfiltration(siteCohort.Ecoregion, frostFreeFrac, siteCohort.Site.Location.ToString());
            // Fast Leakage
            hydrology.CalcLeakage(siteCohort.Ecoregion, leakageFrac, frostFreeFrac, siteCohort.Site.Location.ToString());
            // Maintenance respiration depends on biomass,  non soluble carbon and temperature
            data.MaintenanceRespiration[index] = 1 / (double)Globals.IMAX * (double)Math.Min(NSC, variables[Species.Name].MaintenanceRespirationFTemp * (data.TotalBiomass * PnETspecies.CFracBiomass));//gC //IMAXinverse
            // Subtract mainenance respiration (gC/mo)
            data.NSC -= MaintenanceRespiration[index];
            if (data.NSC < 0)
                data.NSC = 0f;
            // Wood decomposition: do once per year to reduce unnescessary computation time so with the last subcanopy layer 
            if (index == Globals.IMAX - 1)
            {
                // In the last month
                if (variables.Month == (int)Calendar.Months.December)
                {
                    if (allowMortality)
                    {
                        // Check if nscfrac is below threshold to determine if cohort is alive
                        // if cohort is dead, nsc goes to zero and becomes functionally dead even though not removed until end of timestep
                        if (!IsAlive)
                            data.NSC = 0.0F;
                        else if (Globals.ModelCore.CurrentTime > 0 && TotalBiomass < (uint)PnETspecies.InitBiomass)  // Check if biomass < Initial Biomass -> cohort dies
                        {
                            data.NSC = 0.0F;
                            data.IsLeafOn = false;
                            data.NSC = 0.0F;
                            double folSenescence = FolSenescence();
                            data.LastFolSenescence = folSenescence;
                            siteCohort.AddLeafLitter(folSenescence * data.CanopyLayerFrac, PnETSpecies.FolLignin); // Using Canopy fractioning
                        }
                    }
                    double woodSenescence = WoodSenescence();
                    data.LastWoodSenescence = woodSenescence;
                    siteCohort.AddWoodyDebris(woodSenescence * data.CanopyLayerFrac, PnETspecies.WoodyDebrisDecompRate); // Using Canopy fractioning
                    // Release of NSC, will be added to biomass components next year
                    // Assumed that NSC will have a minimum concentration, excess is allocated to biomass
                    double NSCallocation = Math.Max(NSC - (PnETspecies.NSCFrac * FActiveBiom * data.TotalBiomass * PnETspecies.CFracBiomass), 0);
                    data.TotalBiomass += NSCallocation / PnETspecies.CFracBiomass;  // convert gC to gDW
                    data.AGBiomass = PnETspecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
                    data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
                    data.MaxBiomass = Math.Max(MaxBiomass, data.TotalBiomass);
                    data.NSC -= NSCallocation;
                    if (data.NSC < 0)
                        data.NSC = 0f;
                    data.UniversalData.Age++;
                }
            }
            // Phenology: do once per cohort per month, using the first sublayer 
            if (index == 0)
            {
                if (coldKillBoolean)
                {
                    data.ColdKill = (int)Math.Floor(variables.Tavg - (3.0 * siteCohort.Ecoregion.WinterSTD));
                    data.IsLeafOn = false;
                    data.NSC = 0.0F;
                    double folSenescence = FolSenescence();
                    data.LastFolSenescence = folSenescence;
                    siteCohort.AddLeafLitter(folSenescence * data.CanopyLayerFrac, PnETSpecies.FolLignin); // Using Canopy fractioning
                }
                else
                {
                    // When LeafOn becomes false for the first time in a year
                    if (variables.Tmin <= PnETSpecies.LeafOnMinT)
                    {
                        if (data.IsLeafOn == true)
                        {
                            data.IsLeafOn = false;
                            double folSenescence = FolSenescence();
                            data.LastFolSenescence = folSenescence;
                            siteCohort.AddLeafLitter(folSenescence * data.CanopyLayerFrac, PnETSpecies.FolLignin); // Using Canopy fractioning
                        }
                        growMonth = -1;
                    }
                    else
                    {
                        if (frostFreeFrac > 0)
                        {
                            // LeafOn becomes true for the first time in a year
                            if (data.IsLeafOn == false)
                                growMonth = 1;
                            else
                                growMonth += 1;
                            data.IsLeafOn = true;
                        }
                    }
                }
                if (data.IsLeafOn)
                {
                    // Apply defoliation only in the second growing season month
                    if (growMonth == 2)
                    {
                        Fol = Disturbance.ReduceFoliage(Fol, data.DefoliationFrac);
                        data.MaxFolYear = Math.Max(data.MaxFolYear, Fol);
                    }
                    else
                    {
                        if (firstYear)
                            data.adjFolBiomassFrac = PnETspecies.FolBiomassFrac_intercept;
                        // Foliage linearly increases with active biomass
                        double IdealFol = adjFolBiomassFrac * FActiveBiom * data.TotalBiomass; // Using adjusted FolBiomassFrac
                        double NSClimit = data.NSC;
                        if (mainLayerPAR < variables.PAR0) // indicates below the top layer
                        {
                            // lower canopy layers can retain a reserve of NSC (NSCReserve) which limits NSC available for refoliation - default is no reserve (NSCReserve = 0)
                            NSClimit = data.NSC - (PnETspecies.NSCReserve * (FActiveBiom * (data.TotalBiomass + data.Fol) * PnETspecies.CFracBiomass));
                        }
                        double FolCost = 0;
                        double FolTentative = 0;
                        if (growMonth < 2)  // Growing season months before defoliation outbreaks - can add foliage in first growing season month
                        {
                            if (IdealFol > data.Fol)
                            {
                                // Foliage allocation depends on availability of NSC (allows deficit at this time so no min nsc)
                                // carbon fraction of biomass to convert C to DW
                                FolCost = Math.Max(0, Math.Min(NSClimit, PnETspecies.CFracBiomass * (IdealFol - Fol))); // gC/mo
                                // Add foliage allocation to foliage
                                FolTentative = FolCost / PnETspecies.CFracBiomass;// gDW
                            }
                            data.LastLAI = 0;
                        }
                        else if (growMonth == 3) // Refoliation can occur in the 3rd growing season month
                        {
                            if (data.DefoliationFrac > 0)  // Only defoliated cohorts can add refoliate
                            {
                                if (data.DefoliationFrac > PnETspecies.RefoliationMinimumTrigger)  // Refoliation threshold is variable
                                {
                                    // Foliage allocation depends on availability of NSC (allows deficit at this time so no min nsc)
                                    // carbon fraction of biomass to convert C to DW
                                    double Folalloc = Math.Max(0f, Math.Min(NSClimit, PnETspecies.CFracBiomass * ((PnETspecies.MaxRefoliationFrac * IdealFol) - Fol)));  // variable refoliation
                                    FolCost = Math.Max(0f, Math.Min(NSClimit, PnETspecies.CFracBiomass * (PnETspecies.RefoliationCost * IdealFol - Fol)));  // cost of refol is the cost of getting to variable propotion of IdealFol
                                    FolTentative = Folalloc / PnETspecies.CFracBiomass;// gDW
                                }
                                else // No attempted refoliation but carbon loss after defoliation
                                {
                                    // Foliage allocation depends on availability of NSC (allows deficit at this time so no min nsc)
                                    // carbon fraction of biomass to convert C to DW
                                    FolCost = Math.Max(0f, Math.Min(NSClimit, PnETspecies.CFracBiomass * (PnETspecies.NonRefoliationCost * IdealFol))); // gC/mo variable fraction of IdealFol to take out NSC 
                                }
                            }
                            // Non-defoliated trees do not add to their foliage
                        }
                        if (FolTentative > 0.01)
                        {
                            // Leaf area index for the subcanopy layer by index. Function of specific leaf weight SLWMAX and the depth of the canopy
                            double tentativeLAI = Canopy.CalcCohortLAI(PnETSpecies, Fol + FolTentative);
                            double tentativeCanopyFrac = tentativeLAI / PnETspecies.MaxLAI;
                            if (sumCanopyFrac > 1)
                                tentativeCanopyFrac /= sumCanopyFrac;
                            // Downgrade foliage added if canopy is expanding 
                            double actualFol = FolTentative;
                            // Add Foliage
                            data.Fol += actualFol;
                            data.MaxFolYear = Math.Max(data.MaxFolYear, data.Fol);
                        }
                        // Subtract from NSC
                        data.NSC -= FolCost;
                        if (data.NSC < 0)
                            data.NSC = 0f;
                    }
                }
            }
            // Leaf area index for the subcanopy layer by index. Function of specific leaf weight SLWMAX and the depth of the canopy
            LAISum = Canopy.CalcLAISum(index, LAI);
            data.LAI[index] = Canopy.CalcLAI(PnETspecies, Fol, index, LAISum);
            // Adjust HalfSat for CO2 effect
            data.AdjHalfSat = Photosynthesis.CalcAdjHalfSat(variables.CO2, PnETspecies.HalfSat, PnETspecies.HalfSatFCO2);
            // Reduction factor for radiation on photosynthesis
            double LayerPAR = (double)(mainLayerPAR * Math.Exp(-PnETspecies.K * (LAI.Sum() - LAI[index])));
            FRad[index] = Photosynthesis.CalcFRad(LayerPAR, AdjHalfSat);
            // Get pressure head given ecoregion and soil water content (latter in hydrology)
            double PressureHead = hydrology.PressureHeadTable.CalcSoilWaterPressureHead(hydrology.SoilWaterContent, siteCohort.Ecoregion.SoilType);
            // Reduction water for sub or supra optimal soil water content
            double FWaterOzone = 1.0f;  // fWater for ozone functions; ignores H1 and H2 parameters because only impacts when drought-stressed
            SoilWaterContent[index] = hydrology.SoilWaterContent;
            PressHead[index] = PressureHead;
            NumPrecipEvents[index] = precipCount;
            if (Globals.ModelCore.CurrentTime > 0)
            {
                FWater[index] = Photosynthesis.CalcFWater(PnETspecies.H1, PnETspecies.H2, PnETspecies.H3, PnETspecies.H4, PressureHead);
                FWaterOzone = Photosynthesis.CalcFWater(-1.0, -1.0, PnETspecies.H3, PnETspecies.H4, PressureHead); // ignores H1 and H2 parameters because only impacts when drought-stressed
            }
            else // Spinup
            {
                if (Names.GetParameter(Names.SpinUpWaterStress).Value == "true"
                    || Names.GetParameter(Names.SpinUpWaterStress).Value == "yes")
                {
                    FWater[index] = Photosynthesis.CalcFWater(PnETspecies.H1, PnETspecies.H2, PnETspecies.H3, PnETspecies.H4, PressureHead);
                    FWaterOzone = Photosynthesis.CalcFWater(-1.0, -1.0, PnETspecies.H3, PnETspecies.H4, PressureHead); // ignores H1 and H2 parameters because only impacts when drought-stressed
                }
                else // Ignore H1 and H2 parameters during spinup
                {
                    FWater[index] = Photosynthesis.CalcFWater(-1.0, -1.0, PnETspecies.H3, PnETspecies.H4, PressureHead);
                    FWaterOzone = FWater[index];
                }
            }
            if (frostFreeFrac <= 0)
            {
                FWater[index] = 0;
                FWaterOzone = 0;
            }
            data.adjFolN = Photosynthesis.CalcAdjFolN(PnETspecies.FolN_slope, PnETspecies.FolN_intercept, PnETspecies.FolN, FRad[index]);
            AdjFolN[index] = adjFolN;  // Stored for output
            AdjFolBiomassFrac[index] = adjFolBiomassFrac; // Stored for output
            double ciModifier = Photosynthesis.CalcCiModifier(o3_cum, PnETspecies.StomataO3Sensitivity, FWaterOzone);
            CiModifier[index] = ciModifier;  // Stored for output
            // If trees are physiologically active
            if (IsLeafOn)
            {
                // CO2 ratio internal to the leaf versus external
                double cicaRatio = (-0.075 * adjFolN) + 0.875;
                // Elevated leaf internal CO2 concentration
                double ciElev = Photosynthesis.CalcCiElev(cicaRatio, ciModifier, variables.CO2);
                // modified Franks method (2013, New Phytologist, 197:1077-1094)
                double delamaxCi = Photosynthesis.CalcDelAmaxCi(ciElev);
                DelAmax[index] = delamaxCi;
                // M. Kubiske method for wue calculation: Improved methods for calculating WUE and Transpiration in PnET.
                double JCO2_JH2O = Photosynthesis.CalcJCO2_JH2O(variables[species.Name].JH2O, variables.Tmin, variables.CO2, ciElev, ciModifier);
                double wue = JCO2_JH2O * Constants.MCO2_MC;
                // Calculate potential gross photosynthesis
                double Amax = (double)(delamaxCi * (PnETspecies.AmaxA + variables[species.Name].AmaxB_CO2 * adjFolN)); // nmole CO2/g Fol/s
                double BaseFoliarRespiration = variables[species.Name].BaseFoliarRespirationFrac * Amax; // nmole CO2/g Fol/s
                double AmaxAdj = Amax * PnETspecies.AmaxAmod;  // Amax adjustment as applied in PnET
                double GrossPsnPotential = Photosynthesis.CalcPotentialGrossPsn(AmaxAdj, BaseFoliarRespiration, variables.DaySpan, variables[species.Name].DVPD, variables.DayLength, variables[species.Name].PsnFTemp, FRad[index], FAge, Fol);
                // M. Kubiske equation for transpiration: Improved methods for calculating WUE and Transpiration in PnET.
                // JH2O has been modified by CiModifier to reduce water use efficiency
                // Scale transpiration to fraction of site occupied (CanopyLayerFrac)
                // Corrected conversion factor                
                PotentialTranspiration[index] = (double)(0.0015 * (GrossPsnPotential / JCO2_JH2O)) * CanopyLayerFrac; //mm
                // It is possible for transpiration to calculate to exceed available water
                // In this case, we cap transpiration at available water, and back-calculate GrossPsn and NetPsn to downgrade those as well
                // Volumetric soil water content (mm/m) at species wilting point (h4) 
                // Convert kPA to mH2o (/gravity)
                double wiltPtWater = (double)hydrology.PressureHeadTable.CalcSoilWaterContent(PnETspecies.H4 * Constants.gravity, siteCohort.Ecoregion.SoilType);
                double availableWater = (hydrology.SoilWaterContent - wiltPtWater) * siteCohort.Ecoregion.RootingDepth * frostFreeFrac;
                if (PotentialTranspiration[index] > availableWater)
                {
                    Transpiration[index] = Math.Max(availableWater, 0.0); // mm
                    if (CanopyLayerFrac > 0.0)
                        GrossPsn[index] = Transpiration[index] / 0.0015 * JCO2_JH2O / CanopyLayerFrac;
                    else
                        GrossPsn[index] = 0.0;
                    if (PotentialTranspiration[index] > 0.0)
                        FWater[index] = Transpiration[index] / PotentialTranspiration[index];
                    else
                        FWater[index] = 0.0;
                }
                else
                {
                    GrossPsn[index] = GrossPsnPotential * FWater[index];  // gC/m2 ground/mo
                    Transpiration[index] = PotentialTranspiration[index] * FWater[index]; //mm
                }
                // Subtract transpiration from soil water content
                hydrology.SubtractTranspiration(siteCohort.Ecoregion, Transpiration[index], frostFreeFrac, siteCohort.Site.Location.ToString());
                // Infiltration (let captured surface water soak into soil)
                hydrology.CalcInfiltration(siteCohort.Ecoregion, frostFreeFrac, siteCohort.Site.Location.ToString());
                // Net foliage respiration depends on reference psn (BaseFoliarRespiration)
                // Substitute 24 hours in place of DayLength because foliar respiration does occur at night.  BaseFoliarRespiration and RespirationFQ10 use Tavg temps reflecting both day and night temperatures.
                double RefFoliarRespiration = BaseFoliarRespiration * variables[species.Name].RespirationFQ10 * variables.DaySpan * Constants.SecondsPerDay * Constants.MC / Constants.billion; // gC/g Fol/month
                // Actual foliage respiration (growth respiration) 
                FoliarRespiration[index] = RefFoliarRespiration * Fol / Globals.IMAX; // gC/m2 ground/mo
                // NetPsn depends on gross psn and foliage respiration
                double nonOzoneNetPsn = GrossPsn[index] - FoliarRespiration[index];
                // Convert Psn gC/m2 ground/mo to umolCO2/m2 fol/s
                double netPsn_ground = nonOzoneNetPsn * 1000000.0 * (1.0 / 12.0) * (1.0 / (variables.DayLength * variables.DaySpan));
                double netPsn_leaf_s = 0.0;
                if (netPsn_ground > 0.0 && LAI[index] > 0.0)
                {
                    netPsn_leaf_s = netPsn_ground * (1.0 / LAI[index]);
                    if (double.IsInfinity(netPsn_leaf_s))
                        netPsn_leaf_s = 0.0;
                }
                // Reduction factor for ozone on photosynthesis
                if (o3_month > 0.0)
                {
                    double wvConductance = Evapotranspiration.CalcWVConductance(variables.CO2, variables.Tavg, ciElev, netPsn_leaf_s);
                    double o3Coeff = PnETspecies.FOzone_slope;
                    fOzone = Photosynthesis.CalcFOzone(o3_month, subCanopyIndex, layerCount, Fol, lastFOzone, wvConductance, o3Coeff);
                }
                //Apply reduction factor for Ozone
                FOzone[index] = 1.0 - fOzone;
                NetPsn[index] = nonOzoneNetPsn * FOzone[index];
                // Add net psn to non soluble carbons
                data.NSC += NetPsn[index]; // gC
                if (data.NSC < 0)
                    data.NSC = 0.0;
            }
            else
            {
                // Reset subcanopy layer values
                NetPsn[index] = 0.0;
                FoliarRespiration[index] = 0.0;
                GrossPsn[index] = 0.0;
                Transpiration[index] = 0.0;
                PotentialTranspiration[index] = 0.0;
                FOzone[index] = 1.0;
            }
            index++;
            return success;
        }

        public int CalcNonWoodBiomass(ActiveSite site)
        {
            return (int)Fol;
        }

        public static Percentage CalcNonWoodPercentage(Cohort cohort, ActiveSite site)
        {
            return new Percentage(cohort.Fol / (cohort.Wood + cohort.Fol));
        }

        public void InitializeOutput(LocalOutput localOutput)
        {
            cohortoutput = new LocalOutput(localOutput);
        }

        public void InitializeOutput(string SiteName)
        {
            cohortoutput = new LocalOutput(SiteName, "Cohort_" + Species.Name + ".csv", OutputHeader);
        }

        public void InitializeOutput(string SiteName, ushort EstablishmentYear)
        {
            cohortoutput = new LocalOutput(SiteName, "Cohort_" + Species.Name + "_" + EstablishmentYear + ".csv", OutputHeader);
        }

        /// <summary>
        /// Raises a Cohort.DeathEvent
        /// </summary>
        public static void Died(object sender, ICohort cohort, ActiveSite site, ExtensionType disturbanceType)
        {
            if (DeathEvent != null)
                DeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }

        /// <summary>
        /// Raises a Cohort.DeathEvent if partial mortality.
        /// </summary>
        public static void PartialMortality(object sender, ICohort cohort, ActiveSite site, ExtensionType disturbanceType, double reduction)
        {
            if (PartialDeathEvent != null)
                PartialDeathEvent(sender, new PartialDeathEventArgs(cohort, site, disturbanceType, reduction));
        }

        /// <summary>
        /// Occurs when a cohort is killed by an age-only disturbance.
        /// </summary>
        public static new event DeathEventHandler<DeathEventArgs> AgeOnlyDeathEvent;  // also declared in UniversalCohorts.Cohort

        /// <summary>
        /// Occurs when a cohort dies either due to senescence or biomass
        /// disturbances.
        /// </summary>
        public static event DeathEventHandler<DeathEventArgs> DeathEvent;

        public static event PartialDeathEventHandler<PartialDeathEventArgs> PartialDeathEvent;

        /// <summary>
        /// Raises a Cohort.AgeOnlyDeathEvent
        /// </summary>
        public static void KilledByAgeOnlyDisturbance(object sender, ICohort cohort, ActiveSite site, ExtensionType disturbanceType)
        {
            if (AgeOnlyDeathEvent != null)
                AgeOnlyDeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }

        public void UpdateCohortData(IPnETEcoregionVars monthdata)
        {
            double netPsnSum = NetPsn.Sum();
            double grossPsnSum = GrossPsn.Sum();
            double transpirationSum = Transpiration.Sum();
            double WUE = Evapotranspiration.CalcWUE(grossPsnSum, CanopyLayerFrac, transpirationSum);
            // determine the limiting factor 
            double fWaterAvg = FWater.Average();
            double PressHeadAvg = PressHead.Average();
            double fRadAvg = FRad.Average();
            double fOzoneAvg = FOzone.Average();
            double fTemp = monthdata[Species.Name].PsnFTemp;
            string limitingFactor = "NA";
            if (ColdKill < int.MaxValue)
                limitingFactor = "ColdTolerance (" + ColdKill.ToString() + ")";
            else
            {
                List<double> factorList = new List<double>(new double[] { fWaterAvg, fRadAvg, fOzoneAvg, FAge, fTemp });
                double minFactor = factorList.Min();
                if (minFactor == fTemp)
                    limitingFactor = "fTemp";
                else if (minFactor == FAge)
                    limitingFactor = "fAge";
                else if (minFactor == fWaterAvg)
                {
                    if (PressHeadAvg > PnETSpecies.H3)
                        limitingFactor = "Too_dry";
                    else if (PressHeadAvg < PnETSpecies.H2)
                        limitingFactor = "Too_wet";
                    else
                        limitingFactor = "fWater";
                }
                else if (minFactor == fRadAvg)
                    limitingFactor = "fRad";
                else if (minFactor == fOzoneAvg)
                    limitingFactor = "fOzone";
            }
            // Cohort output file
            string s = Math.Round(monthdata.Time, 2) + "," +
                       monthdata.Year + "," +
                       monthdata.Month + "," +
                       Age + "," +
                       Layer + "," +
                       CanopyLayerFrac + "," +
                       CanopyGrowingSpace + "," +
                       SumLAI + "," +
                       SumLAI * CanopyLayerFrac + "," +
                       GrossPsn.Sum() + "," +
                       FoliarRespiration.Sum() + "," +
                       MaintenanceRespiration.Sum() + "," +
                       netPsnSum + "," + // Sum over canopy layers
                       transpirationSum + "," +
                       WUE.ToString() + "," +
                       Fol + "," +
                       Root + "," +
                       Wood + "," +
                       Fol * CanopyLayerFrac + "," +
                       Root * CanopyLayerFrac + "," +
                       Wood * CanopyLayerFrac + "," +
                       NSC + "," +
                       NSCfrac + "," +
                       fWaterAvg + "," +
                       SoilWaterContent.Average() + "," +
                       PressHead.Average() + "," +
                       fRadAvg + "," +
                       fOzoneAvg + "," +
                       DelAmax.Average() + "," +
                       monthdata[Species.Name].PsnFTemp + "," +
                       monthdata[Species.Name].RespirationFTemp + "," +
                       FAge + "," +
                       IsLeafOn + "," +
                       FActiveBiom + "," +
                       AdjFolN.Average() + "," +
                       AdjFolBiomassFrac.Average() + "," +
                       CiModifier.Average() + "," +
                       AdjHalfSat + "," +
                       limitingFactor + ",";
            cohortoutput.Add(s);
        }

        public string OutputHeader
        {
            get
            {
                // Cohort output file header
                string hdr = OutputHeaders.Time + "," +
                             OutputHeaders.Year + "," +
                             OutputHeaders.Month + "," +
                             OutputHeaders.Age + "," +
                             OutputHeaders.Layer + "," +
                             OutputHeaders.CanopyLayerFrac + "," +
                             OutputHeaders.CanopyGrowingSpace + "," +
                             OutputHeaders.LAI + "," +
                             OutputHeaders.LAISite + "," +
                             OutputHeaders.GrossPsn + "," +
                             OutputHeaders.FoliarRespiration + "," +
                             OutputHeaders.MaintResp + "," +
                             OutputHeaders.NetPsn + "," +
                             OutputHeaders.Transpiration + "," +
                             OutputHeaders.WUE + "," +
                             OutputHeaders.Fol + "," +
                             OutputHeaders.Root + "," +
                             OutputHeaders.Wood + "," +
                             OutputHeaders.FolSite + "," +
                             OutputHeaders.RootSite + "," +
                             OutputHeaders.WoodSite + "," +
                             OutputHeaders.NSC + "," +
                             OutputHeaders.NSCfrac + "," +
                             OutputHeaders.FWater + "," +
                             OutputHeaders.SoilWaterContent + "," +
                             OutputHeaders.PressureHead + "," +
                             OutputHeaders.FRad + "," +
                             OutputHeaders.FOzone + "," +
                             OutputHeaders.DelAMax + "," +
                             OutputHeaders.PhotosynthesisFTemp + "," +
                             OutputHeaders.RespirationFTemp + "," +
                             OutputHeaders.FAge + "," +
                             OutputHeaders.LeafOn + "," +
                             OutputHeaders.FActiveBiom + "," +
                             OutputHeaders.AdjFolN + "," +
                             OutputHeaders.AdjFolBiomassFrac + "," +
                             OutputHeaders.CiModifier + "," +
                             OutputHeaders.AdjHalfSat + "," +
                             OutputHeaders.LimitingFactor + ",";
                return hdr;
            }
        }

        public void WriteCohortData()
        {
            cohortoutput.Write();         
        }

        public double FolSenescence()
        {
            // If it is fall 
            double LeafLitter = PnETspecies.FolTurnoverRate * Fol;
            // If cohort is dead, then all foliage is lost
            if (NSCfrac <= 0.01)
                LeafLitter = Fol;
            Fol -= LeafLitter;
            return LeafLitter;
        }

        public double WoodSenescence()
        {
            double senescence = (Root * PnETspecies.RootTurnoverRate) + Wood * PnETspecies.WoodTurnoverRate;
            data.TotalBiomass -= senescence;
            data.AGBiomass = PnETspecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.MaxBiomass = Math.Max(data.MaxBiomass, data.TotalBiomass);
            return senescence;
        }

        public void ResetFoliageMax()
        {
            data.MaxFolYear = 0;
        }

        public void ReduceBiomass(object sitecohorts, double reductionFrac, ExtensionType disturbanceType)
        {
            if (!((SiteCohorts)sitecohorts).DisturbanceTypesReduced.Contains(disturbanceType))
            {
                Disturbance.ReduceDeadPools(sitecohorts, disturbanceType);  // Reduce dead pools before adding through Disturbance
                ((SiteCohorts)sitecohorts).DisturbanceTypesReduced.Add(disturbanceType);
            }
            Disturbance.AllocateDeadPools(sitecohorts, this, disturbanceType, reductionFrac);
            data.TotalBiomass *= 1.0 - reductionFrac;
            data.AGBiomass = PnETspecies.AGBiomassFrac * data.TotalBiomass + data.Fol;
            data.UniversalData.Biomass = (int)(data.AGBiomass * data.CanopyLayerFrac);
            data.MaxBiomass = Math.Max(data.MaxBiomass, data.TotalBiomass);
            Fol *= 1.0 - reductionFrac;
            data.MaxFolYear = Math.Max(data.MaxFolYear, Fol);
        }

        /// <summary>
        /// Raises a Cohort.AgeOnlyDeathEvent.
        /// </summary>
        public static void RaiseDeathEvent(object sender, Cohort cohort, ActiveSite site, ExtensionType disturbanceType)
        {
            if (DeathEvent != null)
                DeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }

        public new void ChangeParameters(ExpandoObject additionalParams)  // also declared in UniversalCohorts.Cohort
        {
            return;
        }
    }
}
