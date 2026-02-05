// Authors: Arjan de Bruijn

// NOTE: ActiveSite --> Landis.SpatialModeling
// NOTE: ExtensionType --> Landis.Core
// NOTE: ICommunity --> Landis.Library.InitialCommunities.Universal
// NOTE: ISpecies --> Landis.Core
// NOTE: Pool --> Landis.Library.UniversalCohorts

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landis.Core;
using Landis.Library.InitialCommunities.Universal;
using Landis.Library.Parameters;
using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.PnETForC
{
    public class SiteCohorts : Library.UniversalCohorts.SiteCohorts, ISiteCohorts
    {
        private double canopylaimax;
        private double avgSoilWaterContent;
        private double snowpack;
        private double[] CanopyLAI;
        private double subcanopypar;
        private double julysubcanopypar;
        private double maxsubcanopypar;
        private double fracRootAboveFrost;
        private double soilDiffusivity;
        private double leakageFrac;
        private double[] netpsn = null;
        private double[] grosspsn = null;
        private double[] foliarRespiration = null;
        private double[] maintresp = null;
        private double[] averageAlbedo = null;
        private double[] activeLayerDepth = null;
        private double[] frostDepth = null;
        private double[] monthCount = null;
        private double[] monthlySnowpack = null;
        private double[] monthlyWater = null;
        private double[] monthlyLAI = null;
        private double[] monthlyLAICumulative = null;
        private double[] monthlyEvap = null;
        private double[] monthlyActualTrans = null;
        private double[] monthlyInterception = null;
        private double[] monthlyLeakage = null;
        private double[] monthlyRunoff = null;
        private double[] monthlyActualET = null;
        private double[] monthlyPotentialEvap = null;
        private double[] monthlyPotentialTrans = null;
        private double transpiration;
        private double potentialTranspiration;
        private double HeterotrophicRespiration;
        private Hydrology hydrology = null;
        IProbEstablishment probEstablishment = null;
        public ActiveSite Site;
        public Dictionary<ISpecies, List<Cohort>> cohorts = null;
        public List<ISpecies> SpeciesEstablishedByPlanting = null;
        public List<ISpecies> SpeciesEstablishedBySerotiny = null;
        public List<ISpecies> SpeciesEstablishedByResprout = null;
        public List<ISpecies> SpeciesEstablishedBySeed = null;
        public List<int> CohortsKilledBySuccession = null;
        public List<int> CohortsKilledByCold = null;
        public List<int> CohortsKilledByHarvest = null;
        public List<int> CohortsKilledByFire = null;
        public List<int> CohortsKilledByWind = null;
        public List<int> CohortsKilledByOther = null;
        public List<ExtensionType> DisturbanceTypesReduced = null;
        public IPnETEcoregionData Ecoregion;
        public LocalOutput siteoutput;
        private double[] ActualET = new double[12]; // mm/mo
        private static IDictionary<uint, SiteCohorts> initialSites;
        private static byte MaxCanopyLayers;
        private static double LayerThreshRatio;
        private double interception;
        private double precLoss;
        private static byte Timestep;
        private static int CohortBinSize;
        private static bool PrecipEventsWithReplacement;
        private int nlayers;
        private static int MaxLayer;
        private static bool soilIceDepth;
        private static bool permafrost;
        private static bool invertProbEstablishment;
        public SortedList<double, double> depthTempDict = new SortedList<double, double>();  //for permafrost
        double lastTempBelowSnow = double.MaxValue;
        private static double maxHalfSat;
        private static double minHalfSat;
        private static bool CohortStacking;
        Dictionary<double, bool> ratioAbove10 = new Dictionary<double, bool>();
        private static double CanopySumScale;

        public List<ISpecies> SpeciesEstByPlanting
        {
            get
            {
                return SpeciesEstablishedByPlanting;
            }
            set
            {
                SpeciesEstablishedByPlanting = value;
            }
        }

        public List<ISpecies> SpeciesEstBySerotiny
        {
            get
            {
                return SpeciesEstablishedBySerotiny;
            }
            set
            {
                SpeciesEstablishedBySerotiny = value;
            }
        }

        public List<ISpecies> SpeciesEstByResprout
        {
            get
            {
                return SpeciesEstablishedByResprout;
            }
            set
            {
                SpeciesEstablishedByResprout = value;
            }
        }

        public List<ISpecies> SpeciesEstBySeeding
        {
            get
            {
                return SpeciesEstablishedBySeed;
            }
            set
            {
                SpeciesEstablishedBySeed = value;
            }
        }

        public List<int> CohortsDiedBySuccession
        {
            get
            {
                return CohortsKilledBySuccession;
            }
            set
            {
                CohortsKilledBySuccession = value;
            }
        }

        public List<int> CohortsDiedByCold
        {
            get
            {
                return CohortsKilledByCold;
            }
            set
            {
                CohortsKilledByCold = value;
            }
        }

        public List<int> CohortsDiedByHarvest
        {
            get
            {
                return CohortsKilledByHarvest;
            }
            set
            {
                CohortsKilledByHarvest = value;
            }
        }

        public List<int> CohortsDiedByFire
        {
            get
            {
                return CohortsKilledByFire;
            }
            set
            {
                CohortsKilledByFire = value;
            }
        }

        public List<int> CohortsDiedByWind
        {
            get
            {
                return CohortsKilledByWind;
            }
            set
            {
                CohortsKilledByWind = value;
            }
        }

        public List<int> CohortsDiedByOther
        {
            get
            {
                return CohortsKilledByOther;
            }
            set
            {
                CohortsKilledByOther = value;
            }
        }

        public double Transpiration
        {
            get
            {
                return transpiration;
            }
        }

        public double PotentialTranspiration
        {
            get
            {
                return potentialTranspiration;
            }
        }

        public double JulySubCanopyPAR
        {
            get
            {
                return julysubcanopypar;
            }
        }

        public double SubCanopyPAR
        {
            get
            {
                return subcanopypar;
            }
        }

        public IProbEstablishment ProbEstablishment
        {
            get
            {
                return probEstablishment;
            }
        }

        public double MaxSubCanopyPAR
        {
            get
            {
                return maxsubcanopypar;
            }
        }

        public double AvgSoilWaterContent
        {
            get
            {
                return avgSoilWaterContent;
            }
        }

        public double[] NetPsn
        {
            get
            {
                if (netpsn == null)
                {
                    double[] netpsn_array = new double[12];
                    for (int i = 0; i < netpsn_array.Length; i++)
                        netpsn_array[i] = 0;
                    return netpsn_array;
                }
                else
                    return netpsn.ToArray();
            }
        }

        public static bool InitialSitesContainsKey(uint key)
        {
            if (initialSites != null && initialSites.ContainsKey(key))
                return true;
            return false;
        }

        public static void Initialize()
        {
            initialSites = new Dictionary<uint, SiteCohorts>();
            Timestep = ((Parameter<byte>)Names.GetParameter(Names.Timestep)).Value;
            LayerThreshRatio = ((Parameter<double>)Names.GetParameter(Names.LayerThreshRatio, 0, double.MaxValue)).Value;
            MaxCanopyLayers = ((Parameter<byte>)Names.GetParameter(Names.MaxCanopyLayers, 0, 20)).Value;
            soilIceDepth = ((Parameter<bool>)Names.GetParameter(Names.SoilIceDepth)).Value;
            invertProbEstablishment = ((Parameter<bool>)Names.GetParameter(Names.InvertPest)).Value;
            CohortStacking = ((Parameter<bool>)Names.GetParameter(Names.CohortStacking)).Value;
            CanopySumScale = ((Parameter<double>)Names.GetParameter(Names.CanopySumScale, 0f, 1f)).Value;
            permafrost = false;
            Parameter<string> CohortBinSizeParm = null;
            if (Names.TryGetParameter(Names.CohortBinSize, out CohortBinSizeParm))
            {
                if (! Int32.TryParse(CohortBinSizeParm.Value, out CohortBinSize))
                    throw new Exception("CohortBinSize is not an integer value.");
            }
            else
                CohortBinSize = Timestep;
            string precipEventsWithReplacement = Names.GetParameter(Names.PrecipEventsWithReplacement).Value;
            PrecipEventsWithReplacement = true;
            if (precipEventsWithReplacement == "false" || precipEventsWithReplacement == "no")
                PrecipEventsWithReplacement = false;
            maxHalfSat = 0;
            minHalfSat = double.MaxValue;
            foreach (IPnETSpecies pnetspecies in SpeciesParameters.PnETSpecies.AllSpecies)
            {
                if (pnetspecies.HalfSat > maxHalfSat)
                    maxHalfSat = pnetspecies.HalfSat;
                if (pnetspecies.HalfSat < minHalfSat)
                    minHalfSat = pnetspecies.HalfSat;
            }
        }

        /// <summary>
        /// Constructor for initialization of SiteCohorts with no initialSite entry yet        
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="site"></param>
        /// <param name="initialCommunity"></param>
        /// <param name="usingClimateLibrary"></param>
        /// <param name="initialCommunitiesSpinup"></param>
        /// <param name="minFolRatioFactor"></param>
        /// <param name="SiteOutputName"></param>
        public SiteCohorts(DateTime StartDate, ActiveSite site, ICommunity initialCommunity, bool usingClimateLibrary, string initialCommunitiesSpinup, double minFolRatioFactor, string SiteOutputName = null)
        {
            Ecoregion = PnETEcoregionData.GetPnETEcoregion(Globals.ModelCore.Ecoregion[site]);
            Site = site;
            cohorts = new Dictionary<ISpecies, List<Cohort>>();
            SpeciesEstablishedByPlanting = new List<ISpecies>();
            SpeciesEstablishedBySerotiny = new List<ISpecies>();
            SpeciesEstablishedByResprout = new List<ISpecies>();
            SpeciesEstablishedBySeed = new List<ISpecies>();
            CohortsKilledBySuccession = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByCold = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByHarvest = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByFire = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByWind = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByOther = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            DisturbanceTypesReduced = new List<ExtensionType>();
            uint key = CalcKey((ushort)initialCommunity.MapCode, Globals.ModelCore.Ecoregion[site].MapCode);
            SiteVars.MonthlyPressureHead[site] = new double[0];
            SiteVars.MonthlySoilTemp[site] = new SortedList<double, double>[0];
            int tempMaxCanopyLayers = MaxCanopyLayers;
            if (CohortStacking)
                tempMaxCanopyLayers = initialCommunity.Cohorts.Count();
            lock (Globals.InitialSitesThreadLock)
            {
                if (initialSites.ContainsKey(key) == false)
                    initialSites.Add(key, this);
            }
            List<IPnETEcoregionVars> ecoregionInitializer = usingClimateLibrary ? PnETEcoregionData.GetClimateRegionData(Ecoregion, StartDate, StartDate.AddMonths(1)) : PnETEcoregionData.GetData(Ecoregion, StartDate, StartDate.AddMonths(1));
            hydrology = new Hydrology(Ecoregion.FieldCapacity);
            avgSoilWaterContent = hydrology.SoilWaterContent;
            subcanopypar = ecoregionInitializer[0].PAR0;
            maxsubcanopypar = subcanopypar;
            SiteVars.WoodyDebris[Site] = new Pool();
            SiteVars.LeafLitter[Site] = new Pool();
            SiteVars.FineFuels[Site] = SiteVars.LeafLitter[Site].Mass;
            List<double> cohortBiomassLayerFrac = new List<double>();
            List<double> cohortCanopyLayerFrac = new List<double>();
            if (SiteOutputName != null)
            {
                siteoutput = new LocalOutput(SiteOutputName, "Site.csv", Header(site));
                probEstablishment = new ProbEstablishment(SiteOutputName, "Establishment.csv");
            }
            else
                probEstablishment = new ProbEstablishment(null, null);
            bool biomassProvided = false;
            foreach (Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in initialCommunity.Cohorts)
            {
                foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                {
                    if (cohort.Data.Biomass > 0)  // 0 biomass indicates biomass value was not read in
                    {
                        biomassProvided = true;
                        break;
                    }
                }
            }
            if (biomassProvided && !(initialCommunitiesSpinup.ToLower() == "spinup"))
            {
                List<double> CohortBiomassList = new List<double>();
                List<double> CohortMaxBiomassList = new List<double>();
                if (initialCommunitiesSpinup.ToLower() == "nospinup")
                {
                    foreach (Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in initialCommunity.Cohorts)
                    {
                        foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                        {
                            // TODO: Add warning if biomass is 0
                            bool addCohort = AddNewCohort(new Cohort(SpeciesParameters.PnETSpecies[cohort.Species], cohort.Data.Age, cohort.Data.Biomass, SiteOutputName, (ushort)(StartDate.Year - cohort.Data.Age), CohortStacking, Timestep));
                            CohortBiomassList.Add(AllCohorts.Last().AGBiomass);
                            CohortMaxBiomassList.Add(AllCohorts.Last().MaxBiomass);
                        }
                    }
                }
                else
                {
                    if ((initialCommunitiesSpinup.ToLower() != "spinuplayers") && (initialCommunitiesSpinup.ToLower() != "spinuplayersrescale"))
                        Globals.ModelCore.UI.WriteLine("Warning:  InitialCommunitiesSpinup parameter is not 'Spinup', 'SpinupLayers','SpinupLayersRescale' or 'NoSpinup'.  Biomass is provided so using 'SpinupLayers' by default.");
                    SpinUp(StartDate, site, initialCommunity, usingClimateLibrary, SiteOutputName, false);
                    // species-age key to store maxbiomass values
                    Dictionary<ISpecies, Dictionary<int, double[]>> cohortDictionary = new Dictionary<ISpecies, Dictionary<int, double[]>>();
                    foreach (Cohort cohort in AllCohorts)
                    {
                        ISpecies species = cohort.Species;
                        int age = cohort.Age;
                        double lastSeasonAvgFRad = 0F;
                        if (cohort.LastSeasonFRad.Count() > 0)
                            lastSeasonAvgFRad = cohort.LastSeasonFRad.ToArray().Average();
                        if (cohortDictionary.ContainsKey(species))
                        {
                            if (cohortDictionary[species].ContainsKey(age))
                            {
                                // TODO: message duplicate species and age
                            }
                            else
                            {
                                double[] values = new double[] { (int)cohort.MaxBiomass, cohort.Biomass, lastSeasonAvgFRad };
                                cohortDictionary[species].Add(age, values);
                            }
                        }
                        else
                        {
                            Dictionary<int, double[]> ageDictionary = new Dictionary<int, double[]>();
                            double[] values = new double[] { (int)cohort.MaxBiomass, cohort.Biomass, lastSeasonAvgFRad };
                            ageDictionary.Add(age, values);
                            cohortDictionary.Add(species, ageDictionary);
                        }
                    }
                    ClearAllCohorts();
                    foreach (Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in initialCommunity.Cohorts)
                    {
                        foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                        {
                            // TODO: Add warning if biomass is 0
                            int age = cohort.Data.Age;
                            ISpecies species = cohort.Species;
                            double[] values = cohortDictionary[species][age];
                            int cohortMaxBiomass = (int)values[0];
                            double cohortSpinupBiomass = values[1];
                            double lastSeasonAvgFRad = values[2];
                            double inputMaxBiomass = Math.Max(cohortMaxBiomass, cohort.Data.Biomass);
                            if (initialCommunitiesSpinup.ToLower() == "spinuplayersrescale")
                                inputMaxBiomass = cohortMaxBiomass * (cohort.Data.Biomass / cohortSpinupBiomass);
                            double cohortCanopyGrowingSpace = 1f;
                            bool addCohort = AddNewCohort(new Cohort(SpeciesParameters.PnETSpecies[cohort.Species], cohort.Data.Age, cohort.Data.Biomass, (int)inputMaxBiomass, cohortCanopyGrowingSpace, SiteOutputName, (ushort)(StartDate.Year - cohort.Data.Age), CohortStacking, Timestep, lastSeasonAvgFRad));
                            CohortBiomassList.Add(AllCohorts.Last().AGBiomass);
                            CohortMaxBiomassList.Add(AllCohorts.Last().MaxBiomass);
                        }
                    }
                }
                bool runAgain = true;
                int attempts = 0;
                while (runAgain)
                {
                    attempts++;
                    bool badSpinup = false;
                    // Sort cohorts into layers                    
                    List<List<double>> cohortBins = GetBinsByCohort(CohortMaxBiomassList);
                    double[] CanopyLAISum = new double[tempMaxCanopyLayers];
                    double[] LayerBiomass = new double[tempMaxCanopyLayers];
                    List<double>[] LayerBiomassValues = new List<double>[tempMaxCanopyLayers];
                    double[] LayerFoliagePotential = new double[tempMaxCanopyLayers];
                    List<double>[] LayerFoliagePotentialValues = new List<double>[tempMaxCanopyLayers];
                    Dictionary<Cohort, double> canopyFracs = new Dictionary<Cohort, double>();
                    CanopyLAI = new double[tempMaxCanopyLayers];
                    List<double> NewCohortMaxBiomassList = new List<double>();
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layerIndex = 0;
                        foreach (List<double> layerBiomassList in cohortBins)
                        {
                            if (layerBiomassList.Contains(cohort.MaxBiomass))
                            {
                                cohort.Layer = (byte)layerIndex;
                                // if "ground" then ensure cohort.Layer = 0
                                if (cohort.PnETSpecies.Lifeform.ToLower().Contains("ground"))
                                    cohort.Layer = 0;
                                break;
                            }
                            layerIndex++;
                        }
                        int layer = cohort.Layer;
                        int layerCount = cohortBins[layer].Count();
                        double newWoodBiomass = CalcNewWoodBiomass(cohort.PnETSpecies.BGBiomassFrac, cohort.PnETSpecies.LiveWoodBiomassFrac, cohort.PnETSpecies.FolBiomassFrac, cohort.AGBiomass, layerCount);
                        double newTotalBiomass = newWoodBiomass / cohort.PnETSpecies.AGBiomassFrac;
                        cohort.CanopyLayerFrac = 1f / layerCount;
                        if (CohortStacking)
                            cohort.CanopyLayerFrac = 1.0f;
                        double cohortFol = cohort.adjFolBiomassFrac * cohort.FActiveBiom * cohort.TotalBiomass;
                        double cohortLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortFol);
                        cohortLAI = Math.Min(cohortLAI, cohort.PnETSpecies.MaxLAI);
                        cohort.LastLAI = cohortLAI;
                        cohort.CanopyGrowingSpace = Math.Min(cohort.CanopyGrowingSpace, 1.0f);
                        double cohortLAIRatio = Math.Min(cohortLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (CohortStacking)
                            cohortLAIRatio = 1.0f;
                        canopyFracs.Add(cohort, cohortLAIRatio);
                        cohort.NSC = cohort.PnETSpecies.NSCFrac * cohort.FActiveBiom * (cohort.TotalBiomass + cohort.Fol) * cohort.PnETSpecies.CFracBiomass;
                        cohort.Fol = cohortFol * (1 - cohort.PnETSpecies.FolTurnoverRate);
                        if (LayerFoliagePotentialValues[layer] == null)
                            LayerFoliagePotentialValues[layer] = new List<double>();
                        LayerFoliagePotentialValues[layer].Add(cohortLAIRatio);
                        LayerFoliagePotential[layer] += cohortLAIRatio;
                    }
                    // Adjust cohort biomass values so that site-values equal input biomass
                    double[] LayerFoliagePotentialAdj = new double[tempMaxCanopyLayers];
                    int index = 0;
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layer = cohort.Layer;
                        int layerCount = cohortBins[layer].Count();
                        double canopyLayerFrac = Math.Min(canopyFracs[cohort], cohort.CanopyGrowingSpace);
                        canopyLayerFrac = Math.Min(canopyFracs[cohort], 1f / layerCount);
                        if (LayerFoliagePotential[layer] > 1)
                        {
                            double canopyLayerFracAdj = canopyFracs[cohort] / LayerFoliagePotential[layer];
                            canopyLayerFrac = (canopyLayerFracAdj - canopyFracs[cohort]) * CanopySumScale + canopyFracs[cohort];
                            cohort.CanopyGrowingSpace = canopyLayerFrac;
                        }
                        else
                            cohort.CanopyGrowingSpace = 1f;
                        cohort.CanopyLayerFrac = Math.Min(canopyFracs[cohort], canopyLayerFrac);
                        if (CohortStacking)
                        {
                            canopyLayerFrac = 1.0f;
                            cohort.CanopyLayerFrac = 1.0f;
                            cohort.CanopyGrowingSpace = 1.0f;
                        }
                        double targetBiomass = (double)CohortBiomassList[index];
                        double newWoodBiomass = targetBiomass / cohort.CanopyLayerFrac;
                        double newTotalBiomass = newWoodBiomass / cohort.PnETSpecies.AGBiomassFrac;
                        cohort.ChangeBiomass((int)Math.Round((newTotalBiomass - cohort.TotalBiomass) / 2f));
                        double cohortFoliage = cohort.adjFolBiomassFrac * cohort.FActiveBiom * cohort.TotalBiomass;
                        double cohortLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortFoliage);
                        cohortLAI = Math.Min(cohortLAI, cohort.PnETSpecies.MaxLAI);
                        cohort.LastLAI = cohortLAI;
                        cohort.CanopyGrowingSpace = Math.Min(cohort.CanopyGrowingSpace, 1.0f);
                        cohort.CanopyLayerFrac = Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (CohortStacking)
                        {
                            canopyLayerFrac = 1.0f;
                            cohort.CanopyLayerFrac = 1.0f;
                            cohort.CanopyGrowingSpace = 1.0f;
                        }
                        double cohortFol = cohort.adjFolBiomassFrac * cohort.FActiveBiom * cohort.TotalBiomass;
                        cohort.Fol = cohortFol * (1 - cohort.PnETSpecies.FolTurnoverRate);
                        cohort.NSC = cohort.PnETSpecies.NSCFrac * cohort.FActiveBiom * (cohort.TotalBiomass + cohort.Fol) * cohort.PnETSpecies.CFracBiomass;
                        // Check cohort.Biomass
                        LayerFoliagePotentialAdj[layer] += cohort.CanopyLayerFrac;
                        CanopyLAISum[layer] += cohort.LAI.Sum() * cohort.CanopyLayerFrac;
                        LayerBiomass[layer] += cohort.CanopyLayerFrac * cohort.TotalBiomass;
                        index++;
                        NewCohortMaxBiomassList.Add(cohort.MaxBiomass);
                    }
                    // Re-sort layers
                    cohortBins = GetBinsByCohort(NewCohortMaxBiomassList);
                    double[] CanopyLayerSum = new double[tempMaxCanopyLayers];
                    List<double> FinalCohortMaxBiomassList = new List<double>();
                    // Assign new layers
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layerIndex = 0;
                        foreach (List<double> layerBiomassList in cohortBins)
                        {
                            if (layerBiomassList.Contains(cohort.MaxBiomass))
                            {
                                cohort.Layer = (byte)layerIndex;
                                // if "ground" then ensure cohort.Layer = 0
                                if (cohort.PnETSpecies.Lifeform.ToLower().Contains("ground"))
                                    cohort.Layer = 0;
                                break;
                            }
                            layerIndex++;
                        }
                    }
                    // Calculate new layer frac
                    double[] MainLayerCanopyFrac = new double[tempMaxCanopyLayers];
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layerIndex = cohort.Layer;
                        double LAISum = cohort.LAI.Sum();
                        if (cohort.IsLeafOn)
                        {
                            if (LAISum > cohort.LastLAI)
                                cohort.LastLAI = LAISum;
                        }
                        if (CohortStacking)
                            MainLayerCanopyFrac[layerIndex] += 1.0f;
                        else
                            MainLayerCanopyFrac[layerIndex] += Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                    }
                    int cohortIndex = 0;
                    double canopySumScale = CanopySumScale;
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layer = cohort.Layer;
                        int layerCount = cohortBins[layer].Count();
                        double targetBiomass = (double)CohortBiomassList[cohortIndex];
                        double canopyLayerFrac = Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (MainLayerCanopyFrac[layer] > 1)
                        {
                            double canopyLayerFracAdj = cohort.CanopyLayerFrac / MainLayerCanopyFrac[layer];
                            canopyLayerFrac = (canopyLayerFracAdj - cohort.CanopyLayerFrac) * canopySumScale + cohort.CanopyLayerFrac;
                            cohort.CanopyGrowingSpace = Math.Min(cohort.CanopyGrowingSpace, canopyLayerFrac);
                        }
                        else
                        {
                            cohort.CanopyGrowingSpace = 1f;
                        }
                        if (CohortStacking)
                        {
                            canopyLayerFrac = 1.0f;
                            cohort.CanopyLayerFrac = 1.0f;
                            cohort.CanopyGrowingSpace = 1.0f;
                        }
                        double newWoodBiomass = targetBiomass / canopyLayerFrac;
                        double newTotalBiomass = newWoodBiomass / cohort.PnETSpecies.AGBiomassFrac;
                        double changeAmount = newTotalBiomass - cohort.TotalBiomass;
                        double tempFActiveBiom = (double)Math.Exp(-cohort.PnETSpecies.LiveWoodBiomassFrac * newTotalBiomass);
                        double cohortTempFoliage = cohort.adjFolBiomassFrac * tempFActiveBiom * newTotalBiomass;
                        double cohortTempLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortTempFoliage);
                        cohortTempLAI = Math.Min(cohortTempLAI, cohort.PnETSpecies.MaxLAI);
                        double tempBiomass = newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac * Math.Min(cohortTempLAI / cohort.PnETSpecies.MaxLAI, canopyLayerFrac);
                        if (CohortStacking)
                            tempBiomass = newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac * 1.0f;
                        double diff = tempBiomass - targetBiomass;
                        double lastDiff = diff;
                        bool match = Math.Abs(tempBiomass - targetBiomass) < 2;
                        double multiplierRoot = 1f;
                        while (!match)
                        {
                            double multiplier = multiplierRoot;
                            if (Math.Abs(tempBiomass - targetBiomass) > 1000)
                                multiplier = multiplierRoot * 200f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 500)
                                multiplier = multiplierRoot * 100f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 100)
                                multiplier = multiplierRoot * 20f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 50)
                                multiplier = multiplierRoot * 10f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 10)
                                multiplier = multiplierRoot * 2f;
                            lastDiff = diff;
                            if (tempBiomass > targetBiomass)
                                newTotalBiomass = Math.Max(newTotalBiomass - multiplier, 1);
                            else
                                newTotalBiomass = Math.Max(newTotalBiomass + multiplier, 1);
                            changeAmount = newTotalBiomass - cohort.TotalBiomass;
                            tempFActiveBiom = (double)Math.Exp(-cohort.PnETSpecies.LiveWoodBiomassFrac * newTotalBiomass);
                            cohortTempFoliage = cohort.adjFolBiomassFrac * tempFActiveBiom * newTotalBiomass;
                            cohortTempLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortTempFoliage);
                            cohortTempLAI = Math.Min(cohortTempLAI, cohort.PnETSpecies.MaxLAI);
                            if (CohortStacking)
                                tempBiomass = newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac * 1.0f;
                            else
                                tempBiomass = newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac * Math.Min(cohortTempLAI / cohort.PnETSpecies.MaxLAI, canopyLayerFrac);
                            diff = tempBiomass - targetBiomass;
                            if (Math.Abs(diff) > Math.Abs(lastDiff))
                                break;
                            if ((attempts < 3) && ((tempBiomass <= 0) || double.IsNaN(tempBiomass)))
                            {
                                badSpinup = true;
                                break;
                            }
                            match = Math.Abs(tempBiomass - targetBiomass) < 2;
                        }
                        cohort.ChangeBiomass((int)Math.Round((newTotalBiomass - cohort.TotalBiomass) * 1f / 1f));
                        double cohortFoliage = cohort.adjFolBiomassFrac * cohort.FActiveBiom * cohort.TotalBiomass;
                        double cohortLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortFoliage);
                        cohortLAI = Math.Min(cohortLAI, cohort.PnETSpecies.MaxLAI);
                        cohort.LastLAI = cohortLAI;
                        cohort.CanopyLayerFrac = Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (CohortStacking)
                            cohort.CanopyLayerFrac = 1.0f;
                        CanopyLayerSum[layer] += cohort.CanopyLayerFrac;
                        cohort.Fol = cohortFoliage * (1 - cohort.PnETSpecies.FolTurnoverRate);
                        cohort.NSC = cohort.PnETSpecies.NSCFrac * cohort.FActiveBiom * (cohort.TotalBiomass + cohort.Fol) * cohort.PnETSpecies.CFracBiomass;
                        cohortIndex++;
                        FinalCohortMaxBiomassList.Add(cohort.MaxBiomass);
                    }
                    //Re-sort layers
                    cohortBins = GetBinsByCohort(FinalCohortMaxBiomassList);
                    // Assign new layers
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layerIndex = 0;
                        foreach (List<double> layerBiomassList in cohortBins)
                        {
                            if (layerBiomassList.Contains(cohort.MaxBiomass))
                            {
                                cohort.Layer = (byte)layerIndex;
                                // if "ground" then ensure cohort.Layer = 0
                                if (cohort.PnETSpecies.Lifeform.ToLower().Contains("ground"))
                                    cohort.Layer = 0;
                                break;
                            }
                            layerIndex++;
                        }
                    }
                    // Calculate new layer frac
                    MainLayerCanopyFrac = new double[tempMaxCanopyLayers];
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layerIndex = cohort.Layer;
                        double LAISum = cohort.LAI.Sum();
                        if (cohort.IsLeafOn)
                        {
                            if (LAISum > cohort.LastLAI)
                                cohort.LastLAI = LAISum;
                        }
                        if (CohortStacking)
                            MainLayerCanopyFrac[layerIndex] += 1.0f;
                        else
                            MainLayerCanopyFrac[layerIndex] += Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                    }
                    CanopyLayerSum = new double[tempMaxCanopyLayers];
                    cohortIndex = 0;
                    foreach (Cohort cohort in AllCohorts)
                    {
                        int layer = cohort.Layer;
                        int layerCount = cohortBins[layer].Count();
                        double targetBiomass = (double)CohortBiomassList[cohortIndex];
                        double canopyLayerFrac = Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (MainLayerCanopyFrac[layer] > 1)
                        {
                            double canopyLayerFracAdj = cohort.CanopyLayerFrac / MainLayerCanopyFrac[layer];
                            canopyLayerFrac = (canopyLayerFracAdj - cohort.CanopyLayerFrac) * canopySumScale + cohort.CanopyLayerFrac;
                            cohort.CanopyGrowingSpace = Math.Min(cohort.CanopyGrowingSpace, canopyLayerFrac);
                        }
                        else
                            cohort.CanopyGrowingSpace = 1f;
                        if (CohortStacking)
                        {
                            canopyLayerFrac = 1.0f;
                            cohort.CanopyLayerFrac = 1.0f;
                            cohort.CanopyGrowingSpace = 1.0f;
                        }
                        double newWoodBiomass = targetBiomass / canopyLayerFrac;
                        double newTotalBiomass = newWoodBiomass / cohort.PnETSpecies.AGBiomassFrac;
                        double changeAmount = newTotalBiomass - cohort.TotalBiomass;
                        double tempMaxBio = Math.Max(cohort.MaxBiomass, newTotalBiomass);
                        double tempFActiveBiom = (double)Math.Exp(-cohort.PnETSpecies.LiveWoodBiomassFrac * tempMaxBio);
                        double cohortTempFoliage = cohort.adjFolBiomassFrac * tempFActiveBiom * newTotalBiomass;
                        double cohortTempLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortTempFoliage);
                        cohortTempLAI = Math.Min(cohortTempLAI, cohort.PnETSpecies.MaxLAI);
                        double tempBiomass = (newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac + cohortTempFoliage) * Math.Min(cohortTempLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (CohortStacking)
                            tempBiomass = (newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac + cohortTempFoliage) * 1.0f;
                        if ((attempts < 3) && ((tempBiomass <= 0) || double.IsNaN(tempBiomass)))
                        {
                            badSpinup = true;
                            break;
                        }
                        double diff = tempBiomass - targetBiomass;
                        double lastDiff = diff;
                        bool match = Math.Abs(tempBiomass - targetBiomass) < 2;
                        int loopCount = 0;
                        while (!match)
                        {
                            double multiplier = 1f;
                            if (Math.Abs(tempBiomass - targetBiomass) > 1000)
                                multiplier = 200f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 500)
                                multiplier = 100f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 100)
                                multiplier = 20f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 50)
                                multiplier = 10f;
                            else if (Math.Abs(tempBiomass - targetBiomass) > 10)
                                multiplier = 2f;
                            if (tempBiomass > targetBiomass)
                                newTotalBiomass = Math.Max(newTotalBiomass - multiplier, 1);
                            else
                                newTotalBiomass = Math.Max(newTotalBiomass + multiplier, 1);
                            changeAmount = newTotalBiomass - cohort.TotalBiomass;
                            tempMaxBio = Math.Max(cohort.MaxBiomass, newTotalBiomass);
                            tempFActiveBiom = (double)Math.Exp(-cohort.PnETSpecies.LiveWoodBiomassFrac * tempMaxBio);
                            cohortTempFoliage = cohort.adjFolBiomassFrac * tempFActiveBiom * newTotalBiomass;
                            cohortTempLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohortTempFoliage);
                            cohortTempLAI = Math.Min(cohortTempLAI, cohort.PnETSpecies.MaxLAI);
                            tempBiomass = (newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac + cohortTempFoliage) * Math.Min(cohortTempLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                            if (CohortStacking)
                                tempBiomass = (newTotalBiomass * cohort.PnETSpecies.AGBiomassFrac + cohortTempFoliage) * 1.0f;
                            diff = tempBiomass - targetBiomass;
                            if (Math.Abs(diff) > Math.Abs(lastDiff))
                            {
                                if ((Math.Abs(diff) / targetBiomass > 0.10) && (attempts < 3))
                                    badSpinup = true;
                                break;
                            }
                            if ((attempts < 3) && ((tempBiomass <= 0) || double.IsNaN(tempBiomass)))
                            {
                                badSpinup = true;
                                break;
                            }
                            match = Math.Abs(tempBiomass - targetBiomass) < 2;
                            loopCount++;
                            if (loopCount > 1000)
                                break;
                        }
                        if (badSpinup)
                            break;
                        if (loopCount <= 1000)
                        {
                            double cohortFoliage = cohort.adjFolBiomassFrac * tempFActiveBiom * newTotalBiomass;
                            cohort.Fol = cohortFoliage;
                            cohort.ChangeBiomass((int)Math.Round((newTotalBiomass - cohort.TotalBiomass) * 1f / 1f));
                        }
                        else
                            cohort.Fol = cohort.adjFolBiomassFrac * cohort.FActiveBiom * cohort.TotalBiomass;
                        double cohortLAI = Canopy.CalcCohortLAI(cohort.PnETSpecies, cohort.Fol);
                        cohortLAI = Math.Min(cohortLAI, cohort.PnETSpecies.MaxLAI);
                        cohort.LastLAI = cohortLAI;
                        cohort.CanopyLayerFrac = Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                        if (CohortStacking)
                            cohort.CanopyLayerFrac = 1.0f;
                        CanopyLayerSum[layer] += cohort.CanopyLayerFrac;
                        cohort.Fol = cohort.Fol * (1 - cohort.PnETSpecies.FolTurnoverRate);
                        cohort.NSC = cohort.PnETSpecies.NSCFrac * cohort.FActiveBiom * (cohort.TotalBiomass + cohort.Fol) * cohort.PnETSpecies.CFracBiomass;
                        double fol_total_ratio = cohort.Fol / (cohort.Fol + cohort.Wood);
                        // Calculate minimum foliage/total biomass ratios from Jenkins (reduced by MinFolRatioFactor to be not so strict)
                        double ratioLimit = 0;
                        if (SpeciesParameters.PnETSpecies[cohort.Species].SLWDel == 0) //Conifer
                            ratioLimit = 0.057f * minFolRatioFactor;
                        else
                            ratioLimit = 0.019f * minFolRatioFactor;
                        if ((attempts < 3) && (fol_total_ratio < ratioLimit))
                        {
                            badSpinup = true;
                            break;
                        }
                        cohortIndex++;
                    }
                    if (badSpinup)
                    {
                        if ((initialCommunitiesSpinup.ToLower() == "spinuplayers") && (attempts < 2))
                        {
                            Globals.ModelCore.UI.WriteLine("");
                            Globals.ModelCore.UI.WriteLine("Warning: initial community " + initialCommunity.MapCode + " could not initialize properly using SpinupLayers.  Processing with SpinupLayersRescale option instead.");
                            ClearAllCohorts();
                            SpinUp(StartDate, site, initialCommunity, usingClimateLibrary, null, false);
                            // species-age key to store maxbiomass values and canopy growing space
                            Dictionary<ISpecies, Dictionary<int, double[]>> cohortDictionary = new Dictionary<ISpecies, Dictionary<int, double[]>>();
                            foreach (Cohort cohort in AllCohorts)
                            {
                                ISpecies spp = cohort.Species;
                                int age = cohort.Age;
                                double lastSeasonAvgFRad = cohort.LastSeasonFRad.ToArray().Average();
                                if (cohortDictionary.ContainsKey(spp))
                                {
                                    if (cohortDictionary[spp].ContainsKey(age))
                                    {
                                        // FIXME - message duplicate species and age
                                    }
                                    else
                                    {
                                        double[] values = new double[] { (int)cohort.MaxBiomass, cohort.Biomass, lastSeasonAvgFRad };
                                        cohortDictionary[spp].Add(age, values);
                                    }
                                }
                                else
                                {
                                    Dictionary<int, double[]> ageDictionary = new Dictionary<int, double[]>();
                                    double[] values = new double[] { (int)cohort.MaxBiomass, cohort.Biomass, lastSeasonAvgFRad };
                                    ageDictionary.Add(age, values);
                                    cohortDictionary.Add(spp, ageDictionary);
                                }
                            }
                            ClearAllCohorts();
                            CohortBiomassList = new List<double>();
                            CohortMaxBiomassList = new List<double>();
                            foreach (Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in initialCommunity.Cohorts)
                            {
                                foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                                {
                                    int age = cohort.Data.Age;
                                    ISpecies spp = cohort.Species;
                                    double[] values = cohortDictionary[spp][age];
                                    int cohortMaxBiomass = (int)values[0];
                                    double cohortSpinupBiomass = values[1];
                                    double lastSeasonAvgFRad = values[2];
                                    double inputMaxBiomass = Math.Max(cohortMaxBiomass, cohort.Data.Biomass);
                                    inputMaxBiomass = cohortMaxBiomass * (cohort.Data.Biomass / cohortSpinupBiomass);
                                    double cohortCanopyGrowingSpace = 1f;
                                    bool addCohort = AddNewCohort(new Cohort(SpeciesParameters.PnETSpecies[cohort.Species], cohort.Data.Age, cohort.Data.Biomass, (int)inputMaxBiomass, cohortCanopyGrowingSpace, SiteOutputName, (ushort)(StartDate.Year - cohort.Data.Age), CohortStacking, Timestep, lastSeasonAvgFRad));
                                    CohortBiomassList.Add(AllCohorts.Last().AGBiomass);
                                    CohortMaxBiomassList.Add(AllCohorts.Last().MaxBiomass);
                                    AllCohorts.Last().SetAvgFRad(lastSeasonAvgFRad);
                                }
                            }
                            badSpinup = false;
                        }
                        else if ((initialCommunitiesSpinup.ToLower() == "spinuplayersrescale") && (attempts < 2))
                        {
                            Globals.ModelCore.UI.WriteLine("");
                            Globals.ModelCore.UI.WriteLine("Warning: initial community " + initialCommunity.MapCode + " could not initialize properly using SpinupLayersRescale.  Processing with SpinupLayers option instead.");
                            ClearAllCohorts();
                            SpinUp(StartDate, site, initialCommunity, usingClimateLibrary, null, false);
                            // species-age key to store maxbiomass values, biomass, LastSeasonFRad
                            Dictionary<ISpecies, Dictionary<int, double[]>> cohortDictionary = new Dictionary<ISpecies, Dictionary<int, double[]>>();
                            foreach (Cohort cohort in AllCohorts)
                            {
                                ISpecies spp = cohort.Species;
                                int age = cohort.Age;
                                double lastSeasonAvgFRad = cohort.LastSeasonFRad.ToArray().Average();
                                if (cohortDictionary.ContainsKey(spp))
                                {
                                    if (cohortDictionary[spp].ContainsKey(age))
                                    {
                                        // FIXME - message duplicate species and age
                                    }
                                    else
                                    {
                                        double[] values = new double[] { (int)cohort.MaxBiomass, cohort.Biomass, lastSeasonAvgFRad };
                                        cohortDictionary[spp].Add(age, values);
                                    }
                                }
                                else
                                {
                                    Dictionary<int, double[]> ageDictionary = new Dictionary<int, double[]>();
                                    double[] values = new double[] { (int)cohort.MaxBiomass, cohort.Biomass, lastSeasonAvgFRad };
                                    ageDictionary.Add(age, values);
                                    cohortDictionary.Add(spp, ageDictionary);
                                }

                            }
                            ClearAllCohorts();
                            CohortBiomassList = new List<double>();
                            CohortMaxBiomassList = new List<double>();
                            foreach (Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in initialCommunity.Cohorts)
                            {
                                foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                                {
                                    int age = cohort.Data.Age;
                                    ISpecies spp = cohort.Species;
                                    double[] values = cohortDictionary[spp][age];
                                    int cohortMaxBiomass = (int)values[0];
                                    double cohortSpinupBiomass = values[1];
                                    double lastSeasonAvgFRad = values[2];
                                    double inputMaxBiomass = Math.Max(cohortMaxBiomass, cohort.Data.Biomass);
                                    double cohortCanopyGrowingSpace = 1f;
                                    bool addCohort = AddNewCohort(new Cohort(SpeciesParameters.PnETSpecies[cohort.Species], cohort.Data.Age, cohort.Data.Biomass, (int)inputMaxBiomass, cohortCanopyGrowingSpace, SiteOutputName, (ushort)(StartDate.Year - cohort.Data.Age), CohortStacking, Timestep, lastSeasonAvgFRad));
                                    CohortBiomassList.Add(AllCohorts.Last().AGBiomass);
                                    CohortMaxBiomassList.Add(AllCohorts.Last().MaxBiomass);
                                    AllCohorts.Last().SetAvgFRad(lastSeasonAvgFRad);
                                }
                            }
                            badSpinup = false;
                        }
                        else // NoSpinup or secondAttempt
                        {
                            Globals.ModelCore.UI.WriteLine("");
                            if (initialCommunitiesSpinup.ToLower() == "nospinup")
                                Globals.ModelCore.UI.WriteLine("Warning: initial community " + initialCommunity.MapCode + " could not initialize properly on first attempt using NoSpinup. Reprocessing.");
                            else
                                Globals.ModelCore.UI.WriteLine("Warning: initial community " + initialCommunity.MapCode + " could not initialize properly using SpinupLayers or SpinupLayersRescale.  Processing with NoSpinup option instead.");
                            ClearAllCohorts();
                            CohortBiomassList = new List<double>();
                            CohortMaxBiomassList = new List<double>();
                            foreach (Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in initialCommunity.Cohorts)
                            {
                                foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                                {
                                    // TODO: Add warning if biomass is 0
                                    bool addCohort = AddNewCohort(new Cohort(SpeciesParameters.PnETSpecies[cohort.Species], cohort.Data.Age, cohort.Data.Biomass, SiteOutputName, (ushort)(StartDate.Year - cohort.Data.Age), CohortStacking, Timestep));
                                    CohortBiomassList.Add(AllCohorts.Last().AGBiomass);
                                    CohortMaxBiomassList.Add(AllCohorts.Last().MaxBiomass);
                                }
                            }
                        }
                    }
                    else
                    {
                        canopylaimax = CanopyLAISum.Sum();
                        runAgain = false;
                    }
                }
            }
            else
                SpinUp(StartDate, site, initialCommunity, usingClimateLibrary, SiteOutputName);
        }

        /// <summary>
        /// Estimate new wood biomass from BGBiomassFrac, 
        /// LiveWoodBiomassFrac, FolBiomassFrac, total AGBiomass,
        /// and number of cohorts occupying a given canopy layer. 
        /// </summary>
        /// <param name="BGBiomassFrac"></param>
        /// <param name="LiveWoodBiomassFrac"></param>
        /// <param name="FolBiomassFrac"></param>
        /// <param name="AGBiomass"></param>
        /// <param name="LayerCount"></param>
        /// <returns></returns>
        public static double CalcNewWoodBiomass(double BGBiomassFrac, double LiveWoodBiomassFrac, double FolBiomassFrac, double AGBiomass, int LayerCount)
        {
            double estimate_slope = -8.236285f +
                                   27.768424f * BGBiomassFrac +
                                   191053.281571f * LiveWoodBiomassFrac +
                                   312.812679f * FolBiomassFrac +
                                   -594492.216284f * BGBiomassFrac * LiveWoodBiomassFrac +
                                   -941.447695f * BGBiomassFrac * FolBiomassFrac +
                                   -6490254.134756f * LiveWoodBiomassFrac * FolBiomassFrac +
                                   19879995.810771f * BGBiomassFrac * LiveWoodBiomassFrac * FolBiomassFrac;
            double estimate_intercept = 1735.179f +
                                       2994.393f * BGBiomassFrac +
                                       10167232.544f * LiveWoodBiomassFrac +
                                       53598.871f * FolBiomassFrac +
                                       -92028081.987f * BGBiomassFrac * LiveWoodBiomassFrac +
                                       -168141.498f * BGBiomassFrac * FolBiomassFrac +
                                       -1104139533.563f * LiveWoodBiomassFrac * FolBiomassFrac +
                                       3507005746.011f * BGBiomassFrac * LiveWoodBiomassFrac * FolBiomassFrac;
            // Inflate AGBiomass by # of cohorts in layer, assuming equal space among them
            double newWoodBiomass = estimate_intercept + estimate_slope * AGBiomass * LayerCount;
            return newWoodBiomass;
        }

        /// <summary>
        /// Constructor for SiteCohorts that have an initial site already set up
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="site"></param>
        /// <param name="initialCommunity"></param>
        /// <param name="SiteOutputName"></param>
        public SiteCohorts(DateTime StartDate, ActiveSite site, ICommunity initialCommunity, string SiteOutputName = null)
        {
            Ecoregion = PnETEcoregionData.GetPnETEcoregion(Globals.ModelCore.Ecoregion[site]);
            Site = site;
            cohorts = new Dictionary<ISpecies, List<Cohort>>();
            SpeciesEstablishedByPlanting = new List<ISpecies>();
            SpeciesEstablishedBySerotiny = new List<ISpecies>();
            SpeciesEstablishedByResprout = new List<ISpecies>();
            SpeciesEstablishedBySeed = new List<ISpecies>();
            CohortsKilledBySuccession = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByCold = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByHarvest = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByFire = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByWind = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            CohortsKilledByOther = new List<int>(new int[Globals.ModelCore.Species.Count()]);
            DisturbanceTypesReduced = new List<ExtensionType>();
            uint key = CalcKey((ushort)initialCommunity.MapCode, Globals.ModelCore.Ecoregion[site].MapCode);
            if (initialSites.ContainsKey(key))
            {
                if (SiteOutputName != null)
                {
                    this.siteoutput = new LocalOutput(SiteOutputName, "Site.csv", Header(site));
                    probEstablishment = new ProbEstablishment(SiteOutputName, "Establishment.csv");
                }
                else
                    probEstablishment = new ProbEstablishment(null, null);
                subcanopypar = initialSites[key].subcanopypar;
                maxsubcanopypar = initialSites[key].MaxSubCanopyPAR;
                avgSoilWaterContent = initialSites[key].AvgSoilWaterContent;
                hydrology = new Hydrology(initialSites[key].hydrology.SoilWaterContent);
                SiteVars.WoodyDebris[Site] = SiteVars.WoodyDebris[initialSites[key].Site].Clone();
                SiteVars.LeafLitter[Site] = SiteVars.LeafLitter[initialSites[key].Site].Clone();
                SiteVars.FineFuels[Site] = SiteVars.LeafLitter[Site].Mass;
                SiteVars.MonthlyPressureHead[site] = (double[])SiteVars.MonthlyPressureHead[initialSites[key].Site].Clone();
                this.canopylaimax = initialSites[key].CanopyLAImax;
                List<double> cohortBiomassLayerFrac = new List<double>();
                List<double> cohortCanopyLayerFrac = new List<double>();
                List<double> cohortCanopyGrowingSpace = new List<double>();
                List<double> cohortLastLAI = new List<double>();
                List<double> cohortLastWoodSenescence = new List<double>();
                List<double> cohortLastFolSenescence = new List<double>();
                List<double> cohortLastYearAvgFRad = new List<double>();
                foreach (ISpecies species in initialSites[key].cohorts.Keys)
                {
                    foreach (Cohort cohort in initialSites[key].cohorts[species])
                    {
                        bool addCohort = false;
                        if (SiteOutputName != null)
                            addCohort = AddNewCohort(new Cohort(cohort, (ushort)(StartDate.Year - cohort.Age), SiteOutputName));
                        else
                            addCohort = AddNewCohort(new Cohort(cohort));
                        double biomassLayerFrac = cohort.BiomassLayerFrac;
                        cohortBiomassLayerFrac.Add(biomassLayerFrac);
                        double canopyLayerFrac = cohort.CanopyLayerFrac;
                        cohortCanopyLayerFrac.Add(canopyLayerFrac);
                        double canopyGrowingSpace = cohort.CanopyGrowingSpace;
                        cohortCanopyGrowingSpace.Add(canopyGrowingSpace);
                        double lastLAI = cohort.LastLAI;
                        cohortLastLAI.Add(lastLAI);
                        double lastWoodSenes = cohort.LastWoodSenescence;
                        cohortLastWoodSenescence.Add(lastWoodSenes);
                        double lastFolSenes = cohort.LastFolSenescence;
                        cohortLastFolSenescence.Add(lastFolSenes);
                    }
                }
                int index = 0;
                foreach (Cohort cohort in AllCohorts)
                {
                    cohort.BiomassLayerFrac = cohortBiomassLayerFrac[index];
                    cohort.CanopyLayerFrac = cohortCanopyLayerFrac[index];
                    cohort.CanopyGrowingSpace = cohortCanopyGrowingSpace[index];
                    cohort.LastLAI = cohortLastLAI[index];
                    cohort.LastWoodSenescence = cohortLastWoodSenescence[index];
                    cohort.LastFolSenescence = cohortLastFolSenescence[index];
                    index++;
                }
                SiteVars.MonthlySoilTemp[site] = new SortedList<double, double>[SiteVars.MonthlyPressureHead[site].Count()];
                for (int m = 0; m < SiteVars.MonthlyPressureHead[site].Count(); m++)
                {
                    SiteVars.MonthlySoilTemp[site][m] = SiteVars.MonthlySoilTemp[initialSites[key].Site][m];
                }
                netpsn = initialSites[key].NetPsn;
                foliarRespiration = initialSites[key].FoliarRespiration;
                grosspsn = initialSites[key].GrossPsn;
                maintresp = initialSites[key].MaintResp;
                averageAlbedo = initialSites[key].AverageAlbedo;
                CanopyLAI = initialSites[key].CanopyLAI;
                transpiration = initialSites[key].Transpiration;
                potentialTranspiration = initialSites[key].PotentialTranspiration;
                // Calculate AdjFolFrac
                AllCohorts.ForEach(x => x.CalcAdjFolBiomassFrac());
            }
        }

        /// <summary>
        /// Spin up sites if no biomass is provided
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="site"></param>
        /// <param name="initialCommunity"></param>
        /// <param name="usingClimateLibrary"></param>
        /// <param name="SiteOutputName"></param>
        /// <param name="AllowMortality"></param>
        /// <exception cref="System.Exception"></exception>
        private void SpinUp(DateTime StartDate, ActiveSite site, ICommunity initialCommunity, bool usingClimateLibrary, string SiteOutputName = null, bool AllowMortality = true)
        {
            List<Library.UniversalCohorts.ICohort> sortedAgeCohorts = new List<Library.UniversalCohorts.ICohort>();
            foreach (var speciesCohorts in initialCommunity.Cohorts)
            {
                foreach (Library.UniversalCohorts.ICohort cohort in speciesCohorts)
                {
                    sortedAgeCohorts.Add(cohort);
                }
            }
            sortedAgeCohorts = new List<Library.UniversalCohorts.ICohort>(sortedAgeCohorts.OrderByDescending(o => o.Data.Age));
            if (sortedAgeCohorts.Count == 0)
                return;
            List<double> CohortMaxBiomassList = new List<double>();
            DateTime date = StartDate.AddYears(-(sortedAgeCohorts[0].Data.Age - 1));
            Library.Parameters.Ecoregions.AuxParm<List<PnETEcoregionVars>> mydata = new Library.Parameters.Ecoregions.AuxParm<List<PnETEcoregionVars>>(Globals.ModelCore.Ecoregions);
            while (date.CompareTo(StartDate) <= 0)
            {
                //  Add those cohorts that were born at the current year
                while (sortedAgeCohorts.Count() > 0 && StartDate.Year - date.Year == (sortedAgeCohorts[0].Data.Age - 1))
                {
                    Cohort cohort = new Cohort(sortedAgeCohorts[0].Species, SpeciesParameters.PnETSpecies[sortedAgeCohorts[0].Species], (ushort)date.Year, SiteOutputName,1, CohortStacking, Timestep);
                    if (CohortStacking)
                    {
                        cohort.CanopyLayerFrac = 1.0f;
                        cohort.CanopyGrowingSpace = 1.0f;
                    }
                    bool addCohort = AddNewCohort(cohort);
                    sortedAgeCohorts.Remove(sortedAgeCohorts[0]);
                }
                // Simulation time runs until the next cohort is added
                DateTime EndDate = (sortedAgeCohorts.Count == 0) ? StartDate : new DateTime((int)(StartDate.Year - (sortedAgeCohorts[0].Data.Age - 1)), 1, 15);
                if (date.CompareTo(StartDate) == 0)
                    break;
                var climate_vars = usingClimateLibrary ? PnETEcoregionData.GetClimateRegionData(Ecoregion, date, EndDate) : PnETEcoregionData.GetData(Ecoregion, date, EndDate);
                Grow(climate_vars, AllowMortality,SiteOutputName != null);
                date = EndDate;
            }
            if (sortedAgeCohorts.Count > 0)
                throw new Exception("Not all cohorts in the initial communities file were initialized.");
        }

        List<List<int>> GetRandomRange(List<List<int>> bins)
        {
            List<List<int>> random_range = new List<List<int>>();
            if (bins != null) for (int b = 0; b < bins.Count(); b++)
                {
                    random_range.Add(new List<int>());
                    List<int> copy_range = new List<int>(bins[b]);
                    while (copy_range.Count() > 0)
                    {
                        int k = Distributions.DiscreteUniformRandom(0, copy_range.Count()-1);
                        random_range[b].Add(copy_range[k]);
                        copy_range.RemoveAt(k);
                    }
                }

            return random_range;
        }

        public void SetActualET(double value, int Month)
        {
            ActualET[Month-1] = value;
        }

        public void SetPotentialET(double value)
        {
            PotentialET = value;
        }

        class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return y.CompareTo(x);
            }
        }

        public bool Grow(List<IPnETEcoregionVars> data, bool AllowMortality = true, bool outputCohortData = true)
        {
            bool success = true;
            double sumPressureHead = 0;
            int countPressureHead = 0;
            probEstablishment.Reset();
            canopylaimax = double.MinValue;
            int tempMaxCanopyLayers = MaxCanopyLayers;
            if (CohortStacking)
                tempMaxCanopyLayers = AllCohorts.Count();
            SortedDictionary<double, Cohort> SubCanopyCohorts = new SortedDictionary<double, Cohort>();
            List<double> CohortBiomassList = new List<double>();
            List<double> CohortMaxBiomassList = new List<double>();
            int SiteAGBiomass = AllCohorts.Sum(a => a.AGBiomass);
            MaxLayer = 0;
            for (int cohort = 0; cohort < AllCohorts.Count(); cohort++)
            {
                if (Globals.ModelCore.CurrentTime > 0)
                    AllCohorts[cohort].CalcDefoliationFrac(Site, SiteAGBiomass);
                CohortBiomassList.Add(AllCohorts[cohort].TotalBiomass);
                CohortMaxBiomassList.Add(AllCohorts[cohort].MaxBiomass);
            }
            ratioAbove10.Clear();
            List<List<double>> cohortBins = GetBinsByCohort(CohortMaxBiomassList);
            List<int> cohortAges = new List<int>();
            List<List<int>> rawBins = new List<List<int>>();
            int subLayerIndex = 0;
            for (int cohort = 0; cohort < AllCohorts.Count(); cohort++)
            {
                string lifeForm = AllCohorts[cohort].PnETSpecies.Lifeform.ToLower();
                int cohortLayer = 0;
                // Lifeform "ground" always restricted to layer 0
                if (!lifeForm.Contains("ground"))
                {
                    for (int j = 0; j < cohortBins.Count(); j++)
                    {
                        if (cohortBins[j].Contains(AllCohorts[cohort].MaxBiomass))
                            cohortLayer = j;
                    }
                    if (AllCohorts[cohort].Layer > MaxLayer)
                        MaxLayer = AllCohorts[cohort].Layer;
                }
                for (int i = 1; i <= Globals.IMAX; i++)
                {
                    SubCanopyCohorts.Add(subLayerIndex, AllCohorts[cohort]);
                    while (rawBins.Count() < (cohortLayer + 1))
                    {
                        List<int> subList = new List<int>();
                        rawBins.Add(subList);
                    }
                    rawBins[cohortLayer].Add(subLayerIndex);
                    subLayerIndex++;
                }
                if (!cohortAges.Contains(AllCohorts[cohort].Age))
                    cohortAges.Add(AllCohorts[cohort].Age);
            }
            List<List<int>> LayeredBins = new List<List<int>>();            
            LayeredBins = rawBins;
            nlayers = 0;
            foreach (List<int> layerList in LayeredBins)
            {
                if (layerList.Count > 0)
                    nlayers++;
            }
            MaxLayer = LayeredBins.Count - 1;
            List<List<int>> random_range = GetRandomRange(LayeredBins);
            foliarRespiration = new double[13];
            netpsn = new double[13];
            grosspsn = new double[13];
            maintresp = new double[13];
            averageAlbedo = new double[13];
            activeLayerDepth = new double[13];
            frostDepth = new double[13];
            monthCount = new double[13];
            monthlySnowpack = new double[13];
            monthlyWater = new double[13];
            monthlyLAI = new double[13];
            monthlyLAICumulative = new double[13];
            monthlyEvap = new double[13];
            monthlyActualTrans = new double[13];
            monthlyInterception = new double[13];
            monthlyLeakage = new double[13];
            monthlyRunoff = new double[13];
            monthlyActualET = new double[13];
            monthlyPotentialEvap = new double[13];
            monthlyPotentialTrans = new double[13];
            Dictionary<IPnETSpecies, double> cumulativeEstab = new Dictionary<IPnETSpecies, double>();
            Dictionary<IPnETSpecies, List<double>> annualFWater = new Dictionary<IPnETSpecies, List<double>>();
            Dictionary<IPnETSpecies, double> cumulativeFWater = new Dictionary<IPnETSpecies, double>();
            Dictionary<IPnETSpecies, List<double>> annualFRad = new Dictionary<IPnETSpecies, List<double>>();
            Dictionary<IPnETSpecies, double> cumulativeFRad = new Dictionary<IPnETSpecies, double>();
            Dictionary<IPnETSpecies, double> monthlyEstab = new Dictionary<IPnETSpecies, double>();
            Dictionary<IPnETSpecies, int> monthlyCount = new Dictionary<IPnETSpecies, int>();
            Dictionary<IPnETSpecies, int> coldKillMonth = new Dictionary<IPnETSpecies, int>(); // month in which cold kills each species
            foreach (IPnETSpecies pnetspecies in SpeciesParameters.PnETSpecies.AllSpecies)
            {
                cumulativeEstab[pnetspecies] = 1;
                annualFWater[pnetspecies] = new List<double>();
                cumulativeFWater[pnetspecies] = 0;
                annualFRad[pnetspecies] = new List<double>();
                cumulativeFRad[pnetspecies] = 0;
                monthlyCount[pnetspecies] = 0;
                coldKillMonth[pnetspecies] = int.MaxValue;
            }
            double[] lastOzoneEffect = new double[SubCanopyCohorts.Count()];
            for (int i = 0; i < lastOzoneEffect.Length; i++)
            {
                lastOzoneEffect[i] = 0;
            }
            double lastFracBelowFrost = hydrology.FrozenSoilDepth/Ecoregion.RootingDepth;
            int daysOfWinter = 0;
            if (Globals.ModelCore.CurrentTime > 0) // cold can only kill after spinup
            {
                // Loop through months & species to determine if cold temp would kill any species
                double extremeMinTemp = double.MaxValue;
                int extremeMonth = 0;
                for (int m = 0; m < data.Count(); m++)
                {
                    double minTemp = data[m].Tavg - (double)(3.0 * Ecoregion.WinterSTD);
                    if (minTemp < extremeMinTemp)
                    {
                        extremeMinTemp = minTemp;
                        extremeMonth = m;
                    }
                }
                SiteVars.ExtremeMinTemp[Site] = extremeMinTemp;
                foreach (IPnETSpecies pnetspecies in SpeciesParameters.PnETSpecies.AllSpecies)
                {
                    // Check if low temp kills species
                    if (extremeMinTemp < pnetspecies.ColdTolerance)
                        coldKillMonth[pnetspecies] = extremeMonth;
                }
            }
            //Clear pressurehead site values
            sumPressureHead = 0;
            countPressureHead = 0;
            SiteVars.MonthlyPressureHead[this.Site] = new double[data.Count()];
            SiteVars.MonthlySoilTemp[this.Site] = new SortedList<double, double>[data.Count()];
            for (int m = 0; m < data.Count(); m++)
            {
                Ecoregion.Variables = data[m];
                transpiration = 0;
                potentialTranspiration = 0;
                subcanopypar = data[m].PAR0;
                interception = 0;
                // Reset monthly variables that get reported as single year snapshots
                if (data[m].Month == 1)
                {
                    foliarRespiration = new double[13];
                    netpsn = new double[13];
                    grosspsn = new double[13];
                    maintresp = new double[13];
                    averageAlbedo = new double[13];
                    activeLayerDepth = new double[13];
                    frostDepth = new double[13];
                    // Reset annual SiteVars
                    SiteVars.AnnualPotentialEvaporation[Site] = 0;
                    SiteVars.ClimaticWaterDeficit[Site] = 0;
                    canopylaimax = double.MinValue;
                    monthlyLAI = new double[13];
                    // Reset max foliage and AdjFolBiomassFrac in each cohort
                    foreach (ISpecies spc in cohorts.Keys)
                    {
                        foreach (Cohort cohort in cohorts[spc])
                        {
                            cohort.ResetFoliageMax();
                            cohort.LastAGBio = cohort.AGBiomass;
                            cohort.CalcAdjFolBiomassFrac();
                            cohort.ClearFRad();
                        }
                    }
                }
                double ozoneD40 = 0;
                if (m > 0)
                    ozoneD40 = Math.Max(0, data[m].O3 - data[m - 1].O3);
                else
                    ozoneD40 = data[m].O3;
                double O3_D40_ppmh = ozoneD40 / 1000; // convert D40 units to ppm h
                // Melt snow
                double snowmelt = Snow.CalcMelt(snowpack, data[m].Tavg, data[m].DaySpan, Ecoregion.Name, Site.Location.ToString()); // mm
                // Add new snow
                double newSnowDepth = Snow.CalcNewSnowDepth(data[m].Tavg, data[m].Prec, Ecoregion.SnowSublimFrac);
                // Update snowpack depth
                snowpack += newSnowDepth - snowmelt;
                if (snowpack < 0)
                    throw new Exception("Error, snowpack = " + snowpack + "; ecoregion = " + Ecoregion.Name + "; site = " + Site.Location);
                // 
                fracRootAboveFrost = 1;
                leakageFrac = Ecoregion.LeakageFrac;
                double fracThawed = 0;
                // Soil temp calculations - need for permafrost and Root Rot
                // snow calculations - from "Soil thawing worksheet with snow.xlsx"
                if (data[m].Tavg <= 0)
                    daysOfWinter += (int)data[m].DaySpan;
                else if (snowpack > 0)
                    daysOfWinter += (int)data[m].DaySpan;
                else
                    daysOfWinter = 0;
                double densitySnow_kg_m3 = Snow.CalcDensity(daysOfWinter);
                double snowDepth = Snow.CalcDepth(densitySnow_kg_m3, snowpack);
                if (lastTempBelowSnow == double.MaxValue)
                {
                    double thermalConductivity_Snow = Snow.CalcThermalConductivity(densitySnow_kg_m3);
                    double thermalDamping_Snow = Snow.CalcThermalDamping(thermalConductivity_Snow);
                    double DRz_snow = Snow.CalcDampingRatio(snowDepth, thermalDamping_Snow);
                    // Damping ratio for moss - adapted from Kang et al. (2000) and Liang et al. (2014)
                    double thermalDamping_Moss = (double)Math.Sqrt(2.0F * Constants.ThermalDiffusivityMoss / Constants.omega);
                    double DRz_moss = (double)Math.Exp(-1.0F * SiteMossDepth * thermalDamping_Moss);
                    // Soil thermal conductivity via De Vries model (convert to kJ/m.d.K)
                    double ThermalConductivity_theta = SoilT.CalcThermalConductivitySoil_Watts(hydrology.SoilWaterContent, Ecoregion.Porosity, Ecoregion.SoilType) / Constants.Convert_kJperday_to_Watts;
                    // Soil thermal diffusivity
                    double D = ThermalConductivity_theta / Hydrology_SaxtonRawls.GetCTheta(Ecoregion.SoilType);  // m2/day
                    double Dmms = D * 1000000F / Constants.SecondsPerDay; // mm2/s
                    soilDiffusivity = Dmms;
                    double Dmonth = D * data[m].DaySpan; // m2/month
                    double ks = Dmonth * 1000000F / (data[m].DaySpan * Constants.SecondsPerDay); // mm2/s
                    double d = (double)Math.Sqrt(2 * Dmms / Constants.omega);
                    double maxDepth = Ecoregion.RootingDepth + Ecoregion.LeakageFrostDepth;
                    double lastBelowZeroDepth = 0;
                    double bottomFreezeDepth = maxDepth / 1000;
                    activeLayerDepth[data[m].Month - 1] = bottomFreezeDepth;
                    // When there was frozen soil at the end of summer, assume that the bottom of the ice lens is as deep as possible
                    frostDepth[data[m].Month - 1] = bottomFreezeDepth;
                    double testDepth = 0;
                    double zTemp = 0;
                    double tSum = 0;
                    double pSum = 0;
                    double tmax = double.MinValue;
                    double tmin = double.MaxValue;
                    int maxMonth = 0;
                    int minMonth = 0;
                    int mCount = 0;
                    if (m < 12)
                    {
                        mCount = Math.Min(12, data.Count());
                        foreach (int z in Enumerable.Range(0, mCount))
                        {
                            tSum += data[z].Tavg;
                            pSum += data[z].Prec;
                            if (data[z].Tavg > tmax)
                            {
                                tmax = data[z].Tavg;
                                maxMonth = data[z].Month;
                            }
                            if (data[z].Tavg < tmin)
                            {
                                tmin = data[z].Tavg;
                                minMonth = data[z].Month;
                            }
                        }
                    }
                    else
                    {
                        mCount = 12;
                        foreach (int z in Enumerable.Range(m - 11, 12))
                        {
                            tSum += data[z].Tavg;
                            pSum += data[z].Prec;
                            if (data[z].Tavg > tmax)
                            {
                                tmax = data[z].Tavg;
                                maxMonth = data[z].Month;
                            }
                            if (data[z].Tavg < tmin)
                            {
                                tmin = data[z].Tavg;
                                minMonth = data[z].Month;
                            }
                        }
                    }
                    double annualTavg = tSum / mCount;
                    double annualPcpAvg = pSum / mCount;
                    double tAmplitude = (tmax - tmin) / 2;
                    double tempBelowSnow = Ecoregion.Variables.Tavg;
                    if (snowDepth > 0)
                        tempBelowSnow = annualTavg + (Ecoregion.Variables.Tavg - annualTavg) * DRz_snow;
                    lastTempBelowSnow = tempBelowSnow;
                    // Regardless of frozen soil status, need to fill the tempDict with values
                    bool foundBottomIce = false;
                    // Calculate depth to bottom of ice lens with FrostDepth
                    while (testDepth <= bottomFreezeDepth)
                    {
                        // adapted from Kang et al. (2000) and Liang et al. (2014); added FrostFactor for calibration
                        double DRz = (double)Math.Exp(-1.0F * testDepth * d * Ecoregion.FrostFactor);
                        int lagMax = data[m].Month + (3 - maxMonth);
                        int lagMin = data[m].Month + (minMonth - 5);
                        if (minMonth >= 9)
                            lagMin = data[m].Month + (minMonth - 12 - 5);
                        double lagAvg = (lagMax + lagMin) / 2f;
                        zTemp = (double)(annualTavg + tAmplitude * DRz_snow * DRz_moss * DRz * Math.Sin(Constants.omega * lagAvg - testDepth / d));
                        depthTempDict[testDepth] = zTemp;
                        if (zTemp <= 0 && !permafrost)
                            lastBelowZeroDepth = testDepth;
                        if (zTemp > 0 && lastBelowZeroDepth > 0 && !foundBottomIce && !permafrost)
                        {
                            frostDepth[data[m].Month - 1] = lastBelowZeroDepth;
                            foundBottomIce = true;
                        }
                        if (zTemp <= 0)
                        {
                            if (testDepth < activeLayerDepth[data[m].Month - 1])
                                activeLayerDepth[data[m].Month - 1] = testDepth;
                        }
                        if (testDepth == 0f)
                            testDepth = 0.10f;
                        else if (testDepth == 0.10f)
                            testDepth = 0.25f;
                        else
                            testDepth += 0.25F;
                    }
                    // The ice lens is deeper than the max depth
                    if (zTemp <= 0 && !foundBottomIce && !permafrost)
                        frostDepth[data[m].Month - 1] = bottomFreezeDepth;
                }
                depthTempDict = SoilT.CalcMonthlySoilTemps(depthTempDict, Ecoregion, daysOfWinter, snowpack, hydrology, lastTempBelowSnow);
                SortedList<double, double> monthlyDepthTempDict = new SortedList<double, double>();
                // monthlyDepthTempDict.Add(0.1f, depthTempDict[0.1f]);
                lastTempBelowSnow = depthTempDict[0];
                if (soilIceDepth)
                {
                    // Calculate depth to bottom of ice lens with FrostDepth
                    double maxDepth = Ecoregion.RootingDepth + Ecoregion.LeakageFrostDepth;
                    double bottomFreezeDepth = maxDepth / 1000;
                    double lastBelowZeroDepth = 0;
                    double testDepth = 0;
                    bool foundBottomIce = false;
                    double zTemp = 0;
                    activeLayerDepth[data[m].Month - 1] = bottomFreezeDepth;
                    while (testDepth <= bottomFreezeDepth)
                    {
                        zTemp = depthTempDict[testDepth];
                        if (zTemp <= 0 && !permafrost)
                            lastBelowZeroDepth = testDepth;
                        if (zTemp > 0 && lastBelowZeroDepth > 0 && !foundBottomIce && !permafrost)
                        {
                            frostDepth[data[m].Month - 1] = lastBelowZeroDepth;
                            foundBottomIce = true;
                        }
                        if (zTemp <= 0)
                        {
                            if (testDepth < activeLayerDepth[data[m].Month - 1])
                                activeLayerDepth[data[m].Month - 1] = testDepth;
                        }
                        if (testDepth == 0f)
                            testDepth = 0.10f;
                        else if (testDepth == 0.10f)
                            testDepth = 0.25f;
                        else
                            testDepth += 0.25F;
                    }
                    // The ice lens is deeper than the max depth
                    if (zTemp <= 0 && !foundBottomIce && !permafrost)
                        frostDepth[data[m].Month - 1] = bottomFreezeDepth;
                    fracRootAboveFrost = Math.Min(1, activeLayerDepth[data[m].Month - 1] * 1000 / Ecoregion.RootingDepth);
                    double fracRootBelowFrost = 1 - fracRootAboveFrost;
                    fracThawed = Math.Max(0, fracRootAboveFrost - (1 - lastFracBelowFrost));
                    double fracNewFrozenSoil = Math.Max(0, fracRootBelowFrost - lastFracBelowFrost);
                    if (fracRootAboveFrost < 1) // If part of the rooting zone is frozen
                    {
                        if (fracNewFrozenSoil > 0)  // freezing
                        {
                            double newFrozenSoil = fracNewFrozenSoil * Ecoregion.RootingDepth;
                            bool successpct = hydrology.SetFrozenSoilWaterContent(((hydrology.FrozenSoilDepth * hydrology.FrozenSoilWaterContent) + (newFrozenSoil * hydrology.SoilWaterContent)) / (hydrology.FrozenSoilDepth + newFrozenSoil));
                            // Volume of rooting soil that is frozen
                            bool successdepth = hydrology.SetFrozenSoilDepth(Ecoregion.RootingDepth * fracRootBelowFrost);
                            // SoilWaterContent is a volumetric value (mm/m) so frozen water does not need to be removed, as the concentration stays the same
                        }
                    }
                    if (fracThawed > 0) // thawing
                    {
                        // Thawing soil water added to existing water - redistributes evenly in active soil
                        hydrology.ThawFrozenSoil(Ecoregion, lastFracBelowFrost, fracThawed, fracRootAboveFrost, fracRootBelowFrost, Site.Location.ToString());
                        // Volume of rooting soil that is frozen
                        bool successdepth = hydrology.SetFrozenSoilDepth(Ecoregion.RootingDepth * fracRootBelowFrost);  
                    }
                    double leakageFrostReduction = 1.0F;
                    if ((activeLayerDepth[data[m].Month - 1] * 1000) < (Ecoregion.RootingDepth + Ecoregion.LeakageFrostDepth))
                    {
                        if ((activeLayerDepth[data[m].Month - 1] * 1000) < Ecoregion.RootingDepth)
                            leakageFrostReduction = 0.0F;
                        else
                            leakageFrostReduction = (Math.Min(activeLayerDepth[data[m].Month - 1] * 1000, Ecoregion.LeakageFrostDepth) - Ecoregion.RootingDepth) / (Ecoregion.LeakageFrostDepth - Ecoregion.RootingDepth);
                    }
                    leakageFrac = Ecoregion.LeakageFrac * leakageFrostReduction;
                    lastFracBelowFrost = fracRootBelowFrost;
                }
                AllCohorts.ForEach(x => x.InitializeSubLayers());
                if (data[m].Prec < 0)
                    throw new Exception("Error, this.data[m].Prec = " + data[m].Prec + "; ecoregion = " + Ecoregion.Name + "; site = " + Site.Location);
                // Calculate above-canopy reference daily ET
                double ReferenceET = Evapotranspiration.CalcReferenceET_Hamon(data[m].Tavg, data[m].DayLength); //mm/day
                double newSnow = Snow.CalcSnowFrac(data[m].Tavg) * data[m].Prec;
                double newRain = data[m].Prec - newSnow;
                // Reduced by interception
                if (CanopyLAI == null)
                    CanopyLAI = new double[tempMaxCanopyLayers];
                interception = newRain * (double)(1 - Math.Exp(-1 * Ecoregion.PrecIntConst * CanopyLAI.Sum()));
                double surfaceRain = newRain - interception;
                // Reduced by PrecLossFrac
                precLoss = surfaceRain * Ecoregion.PrecLossFrac;
                double precin = surfaceRain - precLoss;
                if (precin < 0)
                    throw new Exception("Error, precin = " + precin + " newSnow = " + newSnow + "; ecoregion = " + Ecoregion.Name + "; site = " + Site.Location);
                // maximum number of precipitation events per month
                int numPrecipEvents = Ecoregion.PrecipEvents;
                // Divide precip into discrete events within the month
                double PrecInByEvent = precin / numPrecipEvents;
                if (PrecInByEvent < 0)
                    throw new Exception("Error, PrecInByEvent = " + PrecInByEvent + "; ecoregion = " + Ecoregion.Name + "; site = " + Site.Location);
                if (fracRootAboveFrost >= 1)
                {
                    bool successpct = hydrology.SetFrozenSoilWaterContent(0F);
                    bool successdepth = hydrology.SetFrozenSoilDepth(0F);
                }
                double MeltInWater = snowmelt;
                // Calculate ground PotentialET
                double groundPotentialET = Evapotranspiration.CalcPotentialGroundET_LAI(CanopyLAI.Sum(), data[m].Tavg, data[m].DayLength, data[m].DaySpan, ((Parameter<double>)Names.GetParameter("ETExtCoeff")).Value);
                double  groundPotentialETbyEvent = groundPotentialET / numPrecipEvents;  // divide evaporation into discreet events to match precip
                // Randomly choose which layers will receive the precip events
                // If # of layers < precipEvents, some layers will show up multiple times in number list.  This ensures the same number of precip events regardless of the number of cohorts
                List<int> randomNumbers = new List<int>();
                if (PrecipEventsWithReplacement)
                {
                    // Sublayer selection with replacement    
                    while (randomNumbers.Count < numPrecipEvents)
                    {
                        int rand = Distributions.DiscreteUniformRandom(1, SubCanopyCohorts.Count());
                        randomNumbers.Add(rand);
                    }
                }
                else
                {
                    // Sublayer selection without replacement
                    if (SubCanopyCohorts.Count() > 0)
                    {
                        while (randomNumbers.Count < numPrecipEvents)
                        {
                            List<int> subCanopyList = Enumerable.Range(1, SubCanopyCohorts.Count()).ToList();
                            while ((randomNumbers.Count < numPrecipEvents) && (subCanopyList.Count() > 0))
                            {
                                int rand = Distributions.DiscreteUniformRandom(0, subCanopyList.Count() - 1);
                                randomNumbers.Add(subCanopyList[rand]);
                                subCanopyList.RemoveAt(rand);
                            }
                        }
                    }
                }
                var groupList = randomNumbers.GroupBy(i => i);
                // Reset Hydrology values
                hydrology.Runoff = 0;
                hydrology.Leakage = 0;
                hydrology.Evaporation = 0;
                hydrology.PotentialEvaporation = groundPotentialET;
                hydrology.PotentialET = 0;
                double PotentialETcumulative = 0;
                double TransCumulative = 0;
                double InterceptCumulative = 0;
                double O3_ppmh = data[m].O3 / 1000; // convert AOT40 units to ppm h
                double lastO3 = 0;
                if (m > 0)
                    lastO3 = data[m - 1].O3 / 1000f;
                double O3_ppmh_month = Math.Max(0, O3_ppmh - lastO3);
                List<IPnETSpecies> species = SpeciesParameters.PnETSpecies.AllSpecies.ToList();
                Dictionary<string, double> DelAmax_spp = new Dictionary<string, double>();
                Dictionary<string, double> JCO2_spp = new Dictionary<string, double>();
                Dictionary<string, double> Amax_spp = new Dictionary<string, double>();
                Dictionary<string, double> PsnFTempRefNetPsn_spp = new Dictionary<string, double>();
                Dictionary<string, double> Ca_Ci_spp = new Dictionary<string, double>();
                double subCanopyPrecip = 0;
                double subCanopyPotentialET = 0;;
                double subCanopyMelt = 0;
                int subCanopyIndex = 0;
                int layerCount = 0;
                if (LayeredBins != null)
                    layerCount = LayeredBins.Count();
                double[] layerWtLAI = new double[layerCount];
                double[] layerSumBio = new double[layerCount];
                double[] layerSumCanopyFrac = new double[layerCount];
                if (LayeredBins != null && LayeredBins.Count() > 0)
                {
                    // main canopy layers
                    for (int b = LayeredBins.Count() - 1; b >= 0; b--)
                    {
                        // sublayers within main canopy b
                        foreach (int r in random_range[b])
                        {
                            Cohort cohort = SubCanopyCohorts.Values.ToArray()[r];
                            // A cohort cannot be reduced to a lower layer once it reaches a higher layer
                            cohort.Layer = (byte)b;
                        }
                    }
                    for (int b = LayeredBins.Count() - 1; b >= 0; b--)
                    {
                        // main canopy layers
                        double mainLayerPARweightedSum = 0;
                        double mainLayerLAIweightedSum = 0;
                        double mainLayerPAR = subcanopypar;
                        double mainLayerBioSum = 0;
                        double mainLayerCanopyFrac = 0;
                        // Estimate layer SumCanopyFrac
                        double sumCanopyFrac = 0;
                        foreach (int r in random_range[b])
                        {
                            // sublayers within main canopy b
                            Cohort cohort = SubCanopyCohorts.Values.ToArray()[r];
                            sumCanopyFrac += cohort.LastLAI / cohort.PnETSpecies.MaxLAI;
                        }
                        sumCanopyFrac /= Globals.IMAX;
                        foreach (int r in random_range[b])
                        {
                            // sublayers within main canopy b
                            subCanopyIndex++;
                            int precipCount = 0;
                            subCanopyPrecip = 0;
                            subCanopyPotentialET = 0;
                            subCanopyMelt = MeltInWater / SubCanopyCohorts.Count();
                            PotentialETcumulative = PotentialETcumulative + ReferenceET * data[m].DaySpan / SubCanopyCohorts.Count();
                            bool coldKillBoolean = false;
                            foreach (var g in groupList)
                            {
                                if (g.Key == subCanopyIndex)
                                {
                                    precipCount = g.Count();
                                    subCanopyPrecip = PrecInByEvent; 
                                    InterceptCumulative += interception / groupList.Count();
                                    if (snowpack == 0)
                                        subCanopyPotentialET = groundPotentialETbyEvent;
                                }
                            }
                            Cohort cohort = SubCanopyCohorts.Values.ToArray()[r];
                            IPnETSpecies pnetspecies = cohort.PnETSpecies;
                            if (coldKillMonth[pnetspecies] == m)
                                coldKillBoolean = true;
                            double FOzone = lastOzoneEffect[subCanopyIndex - 1];
                            double PotentialETnonfor = PotentialETcumulative - TransCumulative - InterceptCumulative - hydrology.Evaporation; // hydrology.Evaporation is cumulative
                            success = cohort.CalcPhotosynthesis(subCanopyPrecip, precipCount, leakageFrac, ref hydrology, mainLayerPAR,
                                ref subcanopypar, O3_ppmh, O3_ppmh_month, subCanopyIndex, SubCanopyCohorts.Count(), ref FOzone,
                                fracRootAboveFrost, snowpack, subCanopyMelt, coldKillBoolean, data[m], this, sumCanopyFrac, subCanopyPotentialET, AllowMortality);
                            if (!success)
                                throw new Exception("Error in CalcPhotosynthesis");
                            TransCumulative += cohort.Transpiration[cohort.index - 1];
                            lastOzoneEffect[subCanopyIndex - 1] = FOzone;
                            if (groundPotentialET > 0)
                            {
                                // If more than one precip event assigned to layer, repeat evaporation for all events prior to respiration
                                for (int p = 1; p <= precipCount; p++)
                                    hydrology.CalcSoilEvaporation(Ecoregion, snowpack, fracRootAboveFrost, PotentialETnonfor, Site.Location.ToString());
                            }
                        } // end sublayer loop in canopy b
                        int cCount = AllCohorts.Count();
                        foreach (Cohort cohort in AllCohorts)
                        {
                            if (cohort.Layer == b)
                            {
                                double LAISum = cohort.LAI.Sum();
                                if (cohort.IsLeafOn)
                                {
                                    if (LAISum > cohort.LastLAI)
                                        cohort.LastLAI = LAISum;
                                }
                                double PARFracUnderCohort = (double)Math.Exp(-cohort.PnETSpecies.K * LAISum);
                                if (CohortStacking)
                                    mainLayerPARweightedSum += PARFracUnderCohort * 1.0f;
                                else
                                    mainLayerPARweightedSum += PARFracUnderCohort * Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                                mainLayerLAIweightedSum += LAISum * Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                                mainLayerBioSum += cohort.AGBiomass;
                                cohort.ANPP = (int)(cohort.AGBiomass - cohort.LastAGBio);
                                if(CohortStacking)
                                    mainLayerCanopyFrac += 1.0f;
                                else
                                    mainLayerCanopyFrac += Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                            }
                        }
                        layerSumBio[b] = mainLayerBioSum;
                        layerSumCanopyFrac[b] = mainLayerCanopyFrac;
                        if (layerSumCanopyFrac[b] > 1)
                            mainLayerPARweightedSum = 0;
                        List<double> Frac_list = new List<double>();
                        List<double> prop_List = new List<double>();
                        List<int> index_List = new List<int>();
                        int index = 0;
                        foreach (Cohort cohort in AllCohorts)
                        {
                            if (cohort.Layer == b)
                            {
                                index++;
                                index_List.Add(index);
                                cohort.BiomassLayerFrac = cohort.AGBiomass / layerSumBio[b];
                                cohort.CanopyLayerFrac = Math.Min(cohort.LastLAI / cohort.PnETSpecies.MaxLAI, cohort.CanopyGrowingSpace);
                                if (layerSumCanopyFrac[b] > 1)
                                {
                                    if (cohort.growMonth == 1)
                                    {
                                        double canopyLayerFracAdj = cohort.CanopyLayerFrac / layerSumCanopyFrac[b];
                                        cohort.CanopyLayerFrac = (canopyLayerFracAdj - cohort.CanopyLayerFrac) * CanopySumScale + cohort.CanopyLayerFrac;
                                        cohort.CanopyGrowingSpace = Math.Min(cohort.CanopyGrowingSpace, cohort.CanopyLayerFrac);
                                    }
                                    double LAISum = cohort.LAI.Sum();
                                    double PARFracUnderCohort = (double)Math.Exp(-cohort.PnETSpecies.K * LAISum);
                                    Frac_list.Add(PARFracUnderCohort);
                                    if (CohortStacking)
                                        mainLayerPARweightedSum += PARFracUnderCohort * 1.0f;
                                    else
                                        mainLayerPARweightedSum += PARFracUnderCohort * cohort.CanopyLayerFrac;
                                }
                                if (CohortStacking)
                                {
                                    cohort.CanopyLayerFrac = 1.0f;
                                    cohort.CanopyGrowingSpace = 1.0f;                                    
                                }
                                prop_List.Add(cohort.CanopyLayerFrac);
                                cohort.ANPP = (int)(cohort.ANPP * cohort.CanopyLayerFrac);
                            }
                        }
                        if (mainLayerBioSum > 0)
                        {
                            if (Frac_list.Count() > 0)
                            {                                
                                double cumulativeFracProp = 1;
                                for (int i = 0; i < Frac_list.Count(); i++)
                                {
                                    double prop = prop_List[i];
                                    double frac = Frac_list[i];
                                    cumulativeFracProp = cumulativeFracProp * (double)Math.Pow(frac, prop);
                                }
                                subcanopypar = mainLayerPAR * cumulativeFracProp;
                            }
                            else
                                subcanopypar = mainLayerPAR * (mainLayerPARweightedSum + (1 - mainLayerCanopyFrac));
                            layerWtLAI[b] = mainLayerLAIweightedSum;
                        }
                        else
                            subcanopypar = mainLayerPAR;
                    }
                    // end main canopy layer loop
                    hydrology.PotentialET += PotentialETcumulative;
                }
                else // No cohorts are present
                {
                    if (MeltInWater > 0)
                    {
                        // Instantaneous runoff due to snowmelt (excess of soilPorosity)
                        hydrology.CalcRunoff(Ecoregion, MeltInWater, fracRootAboveFrost, Site.Location.ToString());
                    }
                    if (precin > 0)
                    {
                        for (int p = 0; p < numPrecipEvents; p++)
                        {
                            // Instantaneous runoff due to rain (excess of soilPorosity)
                            hydrology.CalcRunoff(Ecoregion, precin, fracRootAboveFrost, Site.Location.ToString());
                        }
                    }
                    // Evaporation
                    PotentialETcumulative += ReferenceET * data[m].DaySpan / numPrecipEvents;
                    hydrology.PotentialET += PotentialETcumulative;
                    double PotentialETnonfor = groundPotentialET / numPrecipEvents;
                    hydrology.CalcSoilEvaporation(Ecoregion, snowpack, fracRootAboveFrost, PotentialETnonfor, Site.Location.ToString());
                    // Infiltration (let captured surface water soak into soil)
                    hydrology.CalcInfiltration(Ecoregion, fracRootAboveFrost, Site.Location.ToString());
                    // Fast Leakage
                    hydrology.CalcLeakage(Ecoregion, leakageFrac, fracRootAboveFrost, Site.Location.ToString());
                }
                SiteVars.AnnualPotentialEvaporation[Site] = hydrology.PotentialEvaporation;
                int cohortCount = AllCohorts.Count();
                CanopyLAI = new double[tempMaxCanopyLayers];
                double[] CanopyLAISum = new double[tempMaxCanopyLayers];
                double[] CanopyLAICount = new double[tempMaxCanopyLayers];
                double[] CanopyAlbedo = new double[tempMaxCanopyLayers];
                double[] LayerLAI = new double[tempMaxCanopyLayers];
                double[] CanopyFracSum = new double[tempMaxCanopyLayers];
                CumulativeLeafAreas leafAreas = new CumulativeLeafAreas();
                monthCount[data[m].Month - 1]++;
                monthlySnowpack[data[m].Month - 1] += snowpack;
                monthlyWater[data[m].Month - 1] += hydrology.SoilWaterContent;
                monthlyEvap[data[m].Month - 1] += hydrology.Evaporation;
                monthlyInterception[data[m].Month - 1] += InterceptCumulative;
                monthlyLeakage[data[m].Month - 1] += hydrology.Leakage;
                monthlyRunoff[data[m].Month - 1] += hydrology.Runoff;
                monthlyPotentialEvap[data[m].Month - 1] += hydrology.PotentialEvaporation;
                foreach (Cohort cohort in AllCohorts)
                {
                    foliarRespiration[data[m].Month - 1] += cohort.FoliarRespiration.Sum() * cohort.CanopyLayerFrac;
                    netpsn[data[m].Month - 1] += cohort.NetPsn.Sum() * cohort.CanopyLayerFrac;
                    grosspsn[data[m].Month - 1] += cohort.GrossPsn.Sum() * cohort.CanopyLayerFrac;
                    maintresp[data[m].Month - 1] += cohort.MaintenanceRespiration.Sum() * cohort.CanopyLayerFrac;
                    transpiration += cohort.Transpiration.Sum(); // Transpiration already scaled to CanopyLayerFrac
                    potentialTranspiration += cohort.PotentialTranspiration.Sum(); // Transpiration already scaled to CanopyLayerFrac
                    CalcCumulativeLeafArea(ref leafAreas, cohort);                    
                    int layer = cohort.Layer;
                    if (layer < CanopyLAISum.Length)
                    {
                        CanopyLAISum[layer] += cohort.LAI.Sum() * (cohort.PnETSpecies.AGBiomassFrac * cohort.TotalBiomass + cohort.Fol);
                        CanopyLAICount[layer] += cohort.PnETSpecies.AGBiomassFrac * cohort.TotalBiomass + cohort.Fol;
                    }
                    else
                        Globals.ModelCore.UI.WriteLine("DEBUG: Cohort count = " + AllCohorts.Count() + "; CanopyLAISum count = " + CanopyLAISum.Count());
                    CanopyAlbedo[layer] += CalcAlbedoWithSnow(cohort, cohort.Albedo, snowDepth) * cohort.CanopyLayerFrac;
                    LayerLAI[layer] += cohort.SumLAI * cohort.CanopyLayerFrac;
                    monthlyLAI[data[m].Month - 1] += cohort.LAI.Sum() * cohort.CanopyLayerFrac;
                    monthlyLAICumulative[data[m].Month - 1] += cohort.LAI.Sum() * cohort.CanopyLayerFrac;
                    CanopyFracSum[layer] += cohort.CanopyLayerFrac;
                }
                monthlyActualTrans[data[m].Month - 1] += transpiration;
                monthlyPotentialTrans[data[m].Month - 1] += potentialTranspiration;
                monthlyActualET[data[m].Month - 1] = monthlyActualTrans[data[m].Month - 1] + monthlyEvap[data[m].Month - 1] + monthlyInterception[data[m].Month - 1];
                double groundAlbedo = 0.20F;
                if (snowDepth > 0)
                {
                    // TODO: move this calculation to Snow class
                    double snowMultiplier = snowDepth >= Constants.SnowReflectanceThreshold ? 1 : snowDepth / Constants.SnowReflectanceThreshold;
                    groundAlbedo = (double)(groundAlbedo + (groundAlbedo * (2.125 * snowMultiplier)));
                }
                for (int layer = 0; layer < tempMaxCanopyLayers; layer++)
                {
                    if (CanopyFracSum[layer] < 1.0)
                    {
                        double fracGround = 1.0f - CanopyFracSum[layer];
                        CanopyAlbedo[layer] += fracGround * groundAlbedo;
                    }
                    else if (CanopyFracSum[layer] > 1.0)
                        CanopyAlbedo[layer] = CanopyAlbedo[layer] / CanopyFracSum[layer];
                }
                if (AllCohorts.Count == 0)
                    averageAlbedo[data[m].Month - 1] = groundAlbedo;
                else
                {
                    if (LayerLAI.Max() == 0)
                    {
                        var index = Array.FindLastIndex(CanopyAlbedo, value => value != groundAlbedo);
                        // If a value not equal to zero was found
                        if (index != -1)
                            averageAlbedo[data[m].Month - 1] = CanopyAlbedo[index];
                        else
                            averageAlbedo[data[m].Month - 1] = groundAlbedo;
                    }
                    else if (LayerLAI.Max() < 1)
                    {
                        var index = Array.FindLastIndex(LayerLAI, value => value != 0);
                        // If a value not equal to zero was found
                        if (index != -1)
                            averageAlbedo[data[m].Month - 1] = CanopyAlbedo[index];
                        else
                            averageAlbedo[data[m].Month - 1] = groundAlbedo;
                    }
                    else
                    {
                        for (int layer = tempMaxCanopyLayers - 1; layer >= 0; layer--)
                        {
                            if (LayerLAI[layer] > 1)
                            {
                                averageAlbedo[data[m].Month - 1] = CanopyAlbedo[layer];
                                break;
                            }
                        }
                    }
                }
                for (int layer = 0; layer < tempMaxCanopyLayers; layer++)
                {
                    if (layer < layerWtLAI.Length)
                        CanopyLAI[layer] = layerWtLAI[layer];
                    else
                        CanopyLAI[layer] = 0;
                }
                canopylaimax = Math.Max(canopylaimax, LayerLAI.Sum());
                if (data[m].Tavg > 0)
                {
                    double monthlyPressureHead = hydrology.GetPressureHead(Ecoregion);
                    sumPressureHead += monthlyPressureHead;
                    countPressureHead += 1;
                    SiteVars.MonthlyPressureHead[Site][m] = monthlyPressureHead;
                    SiteVars.MonthlySoilTemp[Site][m] = monthlyDepthTempDict;
                }
                else
                {
                    SiteVars.MonthlyPressureHead[Site][m] = -9999;
                    SiteVars.MonthlySoilTemp[Site][m] = null;
                }
                // Calculate establishment probability
                if (Globals.ModelCore.CurrentTime > 0)
                {
                    probEstablishment.CalcProbEstablishmentForMonth(data[m], Ecoregion, subcanopypar, hydrology, minHalfSat, maxHalfSat, invertProbEstablishment, fracRootAboveFrost);
                    foreach (IPnETSpecies pnetspecies in SpeciesParameters.PnETSpecies.AllSpecies)
                    {
                        if (annualFWater.ContainsKey(pnetspecies))
                        {
                            if (data[m].Tmin > pnetspecies.PsnTmin && data[m].Tmax < pnetspecies.PsnTmax && fracRootAboveFrost > 0) // Active growing season
                            {
                                // Store monthly values for later averaging
                                annualFWater[pnetspecies].Add(probEstablishment.GetSpeciesFWater(pnetspecies));
                                annualFRad[pnetspecies].Add(probEstablishment.GetSpeciesFRad(pnetspecies));
                            }
                        }
                    }
                }
                double ActualET = hydrology.Evaporation + TransCumulative + InterceptCumulative;
                this.SetActualET(ActualET, data[m].Month);
                this.SetPotentialET(PotentialETcumulative);
                SiteVars.ClimaticWaterDeficit[Site] += PotentialETcumulative - ActualET;
                // Infiltration (add surface water to soil)
                if ((hydrology.SurfaceWater > 0) & (hydrology.SoilWaterContent < Ecoregion.Porosity))
                    hydrology.CalcInfiltration(Ecoregion, fracRootAboveFrost, Site.Location.ToString());
                if (siteoutput != null && outputCohortData)
                {
                    AddSiteOutput(data[m]);
                    AllCohorts.ForEach(a => a.UpdateCohortData(data[m]));
                }
                if (data[m].Tavg > 0)
                {
                    sumPressureHead += hydrology.PressureHeadTable.CalcSoilWaterPressureHead(hydrology.SoilWaterContent,Ecoregion.SoilType);
                    countPressureHead += 1;
                }
                if (data[m].Month == 7)
                    julysubcanopypar = subcanopypar;
                // Store growing season FRad values                
                AllCohorts.ForEach(x => x.StoreFRad());
                // Reset all cohort values
                AllCohorts.ForEach(x => x.NullSubLayers());
                //  Processes that happen only once per year
                if (data[m].Month == (int)Calendar.Months.December)
                {
                    //  Decompose litter
                    HeterotrophicRespiration = (ushort)(SiteVars.LeafLitter[Site].Decompose() + SiteVars.WoodyDebris[Site].Decompose());
                    // Calculate AdjFolFrac
                    AllCohorts.ForEach(x => x.CalcAdjFolBiomassFrac());
                    // Filter monthly pest values
                    // This assumes up to 3 months of growing season are relevant for establishment
                    // When > 3 months of growing season, exlcude 1st month, assuming trees focus on foliage growth in first month
                    // When > 4 months, ignore the 4th month and beyond as not primarily relevant for establishment
                    // When < 3 months, include all months
                    foreach (IPnETSpecies pnetspecies in SpeciesParameters.PnETSpecies.AllSpecies)
                    {
                        if (annualFWater[pnetspecies].Count > 3)
                        {
                            cumulativeFWater[pnetspecies] = cumulativeFWater[pnetspecies] + annualFWater[pnetspecies][1] + annualFWater[pnetspecies][2] + annualFWater[pnetspecies][3];
                            cumulativeFRad[pnetspecies] = cumulativeFRad[pnetspecies] + annualFRad[pnetspecies][1] + annualFRad[pnetspecies][2] + annualFRad[pnetspecies][3];
                            monthlyCount[pnetspecies] = monthlyCount[pnetspecies] + 3;
                        }
                        else if (annualFWater[pnetspecies].Count > 2)
                        {
                            cumulativeFWater[pnetspecies] = cumulativeFWater[pnetspecies] + annualFWater[pnetspecies][0] + annualFWater[pnetspecies][1] + annualFWater[pnetspecies][2];
                            cumulativeFRad[pnetspecies] = cumulativeFRad[pnetspecies] + annualFRad[pnetspecies][0] + annualFRad[pnetspecies][1] + annualFRad[pnetspecies][2];
                            monthlyCount[pnetspecies] = monthlyCount[pnetspecies] + 3;
                        }
                        else if (annualFWater[pnetspecies].Count > 1)
                        {
                            cumulativeFWater[pnetspecies] = cumulativeFWater[pnetspecies] + annualFWater[pnetspecies][0] + annualFWater[pnetspecies][1];
                            cumulativeFRad[pnetspecies] = cumulativeFRad[pnetspecies] + annualFRad[pnetspecies][0] + annualFRad[pnetspecies][1];
                            monthlyCount[pnetspecies] = monthlyCount[pnetspecies] + 2;
                        }
                        else if (annualFWater[pnetspecies].Count == 1)
                        {
                            cumulativeFWater[pnetspecies] = cumulativeFWater[pnetspecies] + annualFWater[pnetspecies][0];
                            cumulativeFRad[pnetspecies] = cumulativeFRad[pnetspecies] + annualFRad[pnetspecies][0];
                            monthlyCount[pnetspecies] = monthlyCount[pnetspecies] + 1;
                        }
                        //Reset annual lists for next year
                        annualFWater[pnetspecies].Clear();
                        annualFRad[pnetspecies].Clear();
                    }
                }
                avgSoilWaterContent += hydrology.SoilWaterContent;
            }
            // Above is monthly loop                           
            // Below runs once per timestep
            avgSoilWaterContent /= data.Count(); // convert to average value
            if (Globals.ModelCore.CurrentTime > 0)
            {
                foreach (IPnETSpecies pnetspecies in SpeciesParameters.PnETSpecies.AllSpecies)
                {
                    bool estab = false;
                    double pest = 0;
                    if (monthlyCount[pnetspecies] > 0)
                    {
                        // Transform cumulative probability of no successful establishments to probability of at least one successful establishment
                        cumulativeFWater[pnetspecies] = cumulativeFWater[pnetspecies] / monthlyCount[pnetspecies];
                        cumulativeFRad[pnetspecies] = cumulativeFRad[pnetspecies] / monthlyCount[pnetspecies];
                        // Calculate Pest from average FWater, FRad and modified by MaxProbEstablishment
                        pest = cumulativeFWater[pnetspecies] * cumulativeFRad[pnetspecies] * pnetspecies.MaxProbEstablishment;
                    }                    
                    if (!pnetspecies.PreventEstablishment)
                    {
                        if (pest > (double)Distributions.ContinuousUniformRandom())
                        {
                            probEstablishment.AddEstablishedSpecies(pnetspecies);
                            estab = true;
                        }
                    }
                    ProbEstablishment.RecordProbEstablishment(Globals.ModelCore.CurrentTime, pnetspecies, pest, cumulativeFWater[pnetspecies], cumulativeFRad[pnetspecies], estab, monthlyCount[pnetspecies]);
                }
            }
            if (siteoutput != null && outputCohortData)
            {
                siteoutput.Write();
                AllCohorts.ForEach(cohort => { cohort.WriteCohortData(); });
            }
            double avgPH = sumPressureHead / countPressureHead;
            SiteVars.PressureHead[Site] = avgPH;
            if((Globals.ModelCore.CurrentTime > 0) || AllowMortality)
                RemoveMarkedCohorts();
            return success;
        }

        // Finds the maximum value from an array of doubles
        private double Max(double[] values)
        {
            double maximum = double.MinValue;
            for(int i = 0; i < values.Length; i++)
            {
                if (values[i] > maximum)
                    maximum = values[i];
            }
            return maximum;
        }

        private bool isSummer(byte month)
        {
            switch (month)
            {
                case 5:
                    return true;
                case 6:
                    return true;
                case 7:
                    return true;
                case 8:
                    return true;
                default:
                    return false;
            }
        }

        private bool isWinter(byte month)
        {
            switch (month)
            {
                case 1:
                    return true;
                case 2:
                    return true;
                case 3:
                    return true;
                case 12:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Albedo calculation by adding snow consideration
        /// </summary>
        /// <param name="cohort"></param>
        /// <param name="albedo"></param>
        /// <param name="snowDepth"></param>
        /// <returns></returns>
        private double CalcAlbedoWithSnow(Cohort cohort, double albedo, double snowDepth)
        {
            // Inactive sites become large negative values on the map and are not considered in the averages
            if (!PnETEcoregionData.GetPnETEcoregion(Globals.ModelCore.Ecoregion[this.Site]).Active)
                return -1;
            double finalAlbedo = 0;
            double snowMultiplier = snowDepth >= Constants.SnowReflectanceThreshold ? 1 : snowDepth / Constants.SnowReflectanceThreshold;
            if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && (cohort.PnETSpecies.Lifeform.ToLower().Contains("ground")
                        || cohort.PnETSpecies.Lifeform.ToLower().Contains("open")
                        || cohort.SumLAI == 0))
                finalAlbedo = (double)(albedo + (albedo * (2.75 * snowMultiplier)));
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && cohort.PnETSpecies.Lifeform.ToLower().Contains("dark"))
                finalAlbedo = (double)(albedo + (albedo * (0.8 * snowMultiplier)));
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && cohort.PnETSpecies.Lifeform.ToLower().Contains("light"))
                finalAlbedo = (double)(albedo + (albedo * (0.75 * snowMultiplier)));
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && cohort.PnETSpecies.Lifeform.ToLower().Contains("decid"))
                finalAlbedo = (double)(albedo + (albedo * (0.35 * snowMultiplier)));
            return finalAlbedo;
        }

        private void CalcCumulativeLeafArea(ref CumulativeLeafAreas leafAreas, Cohort cohort)
        {
            if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && cohort.PnETSpecies.Lifeform.ToLower().Contains("dark"))
                leafAreas.DarkConifer += cohort.SumLAI;
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && cohort.PnETSpecies.Lifeform.ToLower().Contains("light"))
                leafAreas.LightConifer += cohort.SumLAI;
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && cohort.PnETSpecies.Lifeform.ToLower().Contains("decid"))
                leafAreas.Deciduous += cohort.SumLAI;
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && (cohort.PnETSpecies.Lifeform.ToLower().Contains("ground")
                        || cohort.PnETSpecies.Lifeform.ToLower().Contains("open")))
                leafAreas.GrassMossOpen += cohort.SumLAI;
            else if ((!string.IsNullOrEmpty(cohort.PnETSpecies.Lifeform))
                    && (cohort.PnETSpecies.Lifeform.ToLower().Contains("tree")
                        || cohort.PnETSpecies.Lifeform.ToLower().Contains("shrub")))
                leafAreas.Deciduous += cohort.SumLAI;
        }
        
        public double[] MaintResp
        {
            get
            {
                if (maintresp == null)
                {
                    double[] maintresp_array = new double[12];
                    for (int i = 0; i < maintresp_array.Length; i++)
                    {
                        maintresp_array[i] = 0;
                    }
                    return maintresp_array;
                }
                else
                    return maintresp.Select(r => (double)r).ToArray();
            }
        }

        public double[] FoliarRespiration
        {
            get
            {
                if (foliarRespiration == null)
                {
                    double[] foliarRespiration_array = new double[12];
                    for (int i = 0; i < foliarRespiration_array.Length; i++)
                    {
                        foliarRespiration_array[i] = 0;
                    }
                    return foliarRespiration_array;
                }
                else
                    return foliarRespiration.Select(psn => (double)psn).ToArray();
            }
        }

        public double[] GrossPsn
        {
            get
            {
                if (grosspsn == null)
                {
                    double[] grosspsn_array = new double[12];
                    for (int i = 0; i < grosspsn_array.Length; i++)
                    {
                        grosspsn_array[i] = 0;
                    }
                    return grosspsn_array;
                }
                else
                    return grosspsn.Select(psn => (double)psn).ToArray();
            }
        }

        public double[] MonthlyAvgSnowpack
        {
            get
            {
                if (monthlySnowpack == null)
                {
                    double[] snowpack_array = new double[12];
                    for (int i = 0; i < snowpack_array.Length; i++)
                    {
                        snowpack_array[i] = 0;
                    }
                    return snowpack_array;
                }
                else
                {
                    double[] snowSum = monthlySnowpack.Select(snowpack => (double)snowpack).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] snowpack_array = new double[12];
                    for (int i = 0; i < snowpack_array.Length; i++)
                    {
                        snowpack_array[i] = snowSum[i] / monthSum[i];
                    }
                    return snowpack_array;
                }
            }
        }

        public double[] MonthlyAvgWater
        {
            get
            {
                if (monthlyWater == null)
                {
                    double[] soilWaterContent_array = new double[12];
                    for (int i = 0; i < soilWaterContent_array.Length; i++)
                    {
                        soilWaterContent_array[i] = 0;
                    }
                    return soilWaterContent_array;
                }
                else
                {
                    double[] soilWaterContentSum = monthlyWater.Select(water => (double)water).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] soilWaterContent_array = new double[12];
                    for (int i = 0; i < soilWaterContent_array.Length; i++)
                    {
                        soilWaterContent_array[i] = soilWaterContentSum[i] / monthSum[i];
                    }
                    return soilWaterContent_array;
                }
            }
        }
    
        public double[] MonthlyAvgLAI
        {
            get
            {
                if (monthlyLAICumulative == null)
                {
                    double[] lai_array = new double[12];
                    for (int i = 0; i < lai_array.Length; i++)
                    {
                        lai_array[i] = 0;
                    }
                    return lai_array;
                }
                else
                {
                    double[] laiSum = monthlyLAICumulative.Select(lai => (double)lai).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] lai_array = new double[12];
                    for (int i = 0; i < lai_array.Length; i++)
                    {
                        lai_array[i] = laiSum[i] / monthSum[i];
                    }
                    return lai_array;
                }
            }
        }
    
        public double[] MonthlyEvap
        {
            get
            {
                if (monthlyEvap == null)
                {
                    double[] evap_array = new double[12];
                    for (int i = 0; i < evap_array.Length; i++)
                    {
                        evap_array[i] = 0;
                    }
                    return evap_array;
                }
                else
                {
                    double[] evapSum = monthlyEvap.Select(evap => (double)evap).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] evap_array = new double[12];
                    for (int i = 0; i < evap_array.Length; i++)
                    {
                        evap_array[i] = evapSum[i] / monthSum[i];
                    }
                    return evap_array;
                }
            }
        }
    
        public double[] MonthlyInterception
        {
            get
            {
                if (monthlyInterception == null)
                {
                    double[] interception_array = new double[12];
                    for (int i = 0; i < interception_array.Length; i++)
                    {
                        interception_array[i] = 0;
                    }
                    return interception_array;
                }
                else
                {
                    double[] interceptionSum = monthlyInterception.Select(interception => (double)interception).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] interception_array = new double[12];
                    for (int i = 0; i < interception_array.Length; i++)
                    {
                        interception_array[i] = interceptionSum[i] / monthSum[i];
                    }
                    return interception_array;
                }
            }
        }
        public double[] MonthlyActualTrans
        {
            get
            {
                if (monthlyActualTrans == null)
                {
                    double[] actualTrans_array = new double[12];
                    for (int i = 0; i < actualTrans_array.Length; i++)
                    {
                        actualTrans_array[i] = 0;
                    }
                    return actualTrans_array;
                }
                else
                {
                    double[] actualTransSum = monthlyActualTrans.Select(actualTrans => (double)actualTrans).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] actualTrans_array = new double[12];
                    for (int i = 0; i < actualTrans_array.Length; i++)
                    {
                        actualTrans_array[i] = actualTransSum[i] / monthSum[i];
                    }
                    return actualTrans_array;
                }
            }
        }
    
        public double[] MonthlyLeakage
        {
            get
            {
                if (monthlyLeakage == null)
                {
                    double[] leakage_array = new double[12];
                    for (int i = 0; i < leakage_array.Length; i++)
                    {
                        leakage_array[i] = 0;
                    }
                    return leakage_array;
                }
                else
                {
                    double[] leakageSum = monthlyLeakage.Select(leakage => (double)leakage).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] leakage_array = new double[12];
                    for (int i = 0; i < leakage_array.Length; i++)
                    {
                        leakage_array[i] = leakageSum[i] / monthSum[i];
                    }
                    return leakage_array;
                }
            }
        }
    
        public double[] MonthlyRunoff
        {
            get
            {
                if (monthlyRunoff == null)
                {
                    double[] runoff_array = new double[12];
                    for (int i = 0; i < runoff_array.Length; i++)
                    {
                        runoff_array[i] = 0;
                    }
                    return runoff_array;
                }
                else
                {
                    double[] runoffSum = monthlyRunoff.Select(runoff => (double)runoff).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] runoff_array = new double[12];
                    for (int i = 0; i < runoff_array.Length; i++)
                    {
                        runoff_array[i] = runoffSum[i] / monthSum[i];
                    }
                    return runoff_array;
                }
            }
        }
    
        public double[] MonthlyActualET
        {
            get
            {
                if (monthlyActualET == null)
                {
                    double[] actualET_array = new double[12];
                    for (int i = 0; i < actualET_array.Length; i++)
                    {
                        actualET_array[i] = 0;
                    }
                    return actualET_array;
                }
                else
                {
                    double[] actualETSum = monthlyActualET.Select(actualET => (double)actualET).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] actualET_array = new double[12];
                    for (int i = 0; i < actualET_array.Length; i++)
                    {
                        actualET_array[i] = actualETSum[i] / monthSum[i];
                    }
                    return actualET_array;
                }
            }
        }
    
        public double[] MonthlyPotentialEvap
        {
            get
            {
                if (monthlyPotentialEvap == null)
                {
                    double[] potentialEvap_array = new double[12];
                    for (int i = 0; i < potentialEvap_array.Length; i++)
                    {
                        potentialEvap_array[i] = 0;
                    }
                    return potentialEvap_array;
                }
                else
                {
                    double[] potentialEvap_Sum = monthlyPotentialEvap.Select(potentialEvap => (double)potentialEvap).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] potentialEvap_array = new double[12];
                    for (int i = 0; i < potentialEvap_array.Length; i++)
                    {
                        potentialEvap_array[i] = potentialEvap_Sum[i] / monthSum[i];
                    }
                    return potentialEvap_array;
                }
            }
        }

        public double[] MonthlyPotentialTrans
        {
            get
            {
                if (monthlyPotentialTrans == null)
                {
                    double[] potentialTrans_array = new double[12];
                    for (int i = 0; i < potentialTrans_array.Length; i++)
                    {
                        potentialTrans_array[i] = 0;
                    }
                    return potentialTrans_array;
                }
                else
                {
                    double[] potentialTransSum = monthlyPotentialTrans.Select(potentialTrans => (double)potentialTrans).ToArray();
                    double[] monthSum = monthCount.Select(months => (double)months).ToArray();
                    double[] potentialTrans_array = new double[12];
                    for (int i = 0; i < potentialTrans_array.Length; i++)
                    {
                        potentialTrans_array[i] = potentialTransSum[i] / monthSum[i];
                    }
                    return potentialTrans_array;
                }
            }
        }

        public double[] AverageAlbedo
        {
            get
            {
                if (averageAlbedo == null)
                {
                    double[] averageAlbedo_array = new double[12];
                    for (int i = 0; i < averageAlbedo_array.Length; i++)
                    {
                        averageAlbedo_array[i] = 0.20f;
                    }
                    return averageAlbedo_array;
                }
                else
                    return averageAlbedo.Select(r => (double)r).ToArray();
            }
        }

        public double[] ActiveLayerDepth
        {
            get
            {
                if (activeLayerDepth == null)
                {
                    double[] activeLayerDepth_array = new double[12];
                    for (int i = 0; i < activeLayerDepth_array.Length; i++)
                    {
                        activeLayerDepth_array[i] = 0;
                    }
                    return activeLayerDepth_array;
                }
                else
                    return activeLayerDepth.Select(r => (double)r).ToArray();
            }
        }

        public double[] FrostDepth
        {
            get
            {
                if (frostDepth == null)
                {
                    double[] frostDepth_array = new double[12];
                    for (int i = 0; i < frostDepth_array.Length; i++)
                    {
                        frostDepth_array[i] = 0;
                    }
                    return frostDepth_array;
                }
                else
                    return frostDepth.Select(r => (double)r).ToArray();
            }
        }

        public double NetPsnSum
        {
            get
            {
                if (netpsn == null)
                {
                    double[] netpsn_array = new double[12];
                    for (int i = 0; i < netpsn_array.Length; i++)
                    {
                        netpsn_array[i] = 0;
                    }
                    return netpsn_array.Sum();
                }
                else
                    return netpsn.Select(psn => (double)psn).ToArray().Sum();
            }
        }

        public double CanopyLAImax
        {
            get
            {
                return canopylaimax;
            }
        }

        public double[] MonthlyLAI
        {
            get
            {
                return monthlyLAI;
            }
        }

        public double SiteMossDepth
        {
            get
            {
                double mossDepth = Ecoregion.MossDepth; //m
                foreach (ISpecies species in cohorts.Keys)
                {
                    foreach (Cohort cohort in cohorts[species])
                        mossDepth += cohort.MossDepth * cohort.CanopyLayerFrac;
                }
                return mossDepth;
            }
        }

        public double WoodyDebris 
        {
            get
            {
                return SiteVars.WoodyDebris[Site].Mass;
            }
        }

        public double LeafLitter 
        {
            get
            {
                return SiteVars.LeafLitter[Site].Mass;
            }
        }
       
        public  Library.Parameters.Species.AuxParm<bool> SpeciesPresent
        {
            get
            {
                Library.Parameters.Species.AuxParm<bool> SpeciesPresent = new Library.Parameters.Species.AuxParm<bool>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    SpeciesPresent[species] = true;
                return SpeciesPresent;
            }
        }

        public Library.Parameters.Species.AuxParm<int> BiomassPerSpecies 
        { 
            get
            {
                Library.Parameters.Species.AuxParm<int> BiomassPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    BiomassPerSpecies[species] = cohorts[species].Sum(o => (int)(o.TotalBiomass * o.CanopyLayerFrac));
                return BiomassPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> AGBiomassPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> AGBiomassPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    AGBiomassPerSpecies[species] = cohorts[species].Sum(o => (int)(o.AGBiomass * o.CanopyLayerFrac));
                return AGBiomassPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> WoodBiomassPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> WoodBiomassPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    WoodBiomassPerSpecies[species] = cohorts[species].Sum(o => (int)(o.Wood * o.CanopyLayerFrac));
                return WoodBiomassPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> BGBiomassPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> BGBiomassPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    BGBiomassPerSpecies[species] = cohorts[species].Sum(o => (int)(o.Root * o.CanopyLayerFrac));
                return BGBiomassPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> FoliageBiomassPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> FoliageBiomassPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    FoliageBiomassPerSpecies[species] = cohorts[species].Sum(o => (int)(o.Fol * o.CanopyLayerFrac));
                return FoliageBiomassPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> MaxFoliageYearPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> MaxFoliageYearPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                {
                    // Edited according to Brian Miranda's advice (https://github.com/LANDIS-II-Foundation/Extension-Output-Biomass-PnET/issues/11#issuecomment-2400646970_
                    // to correct how the variable is computed, to make it similar to FolBiomassSum.
                    MaxFoliageYearPerSpecies[species] = cohorts[species].Sum(o => (int)(o.MaxFolYear * o.CanopyLayerFrac));
                }
                return MaxFoliageYearPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> NSCPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> NSCPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    NSCPerSpecies[species] = cohorts[species].Sum(o => (int)(o.NSC * o.CanopyLayerFrac));
                return NSCPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<double> LAIPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<double> LAIPerSpecies = new Library.Parameters.Species.AuxParm<double>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    LAIPerSpecies[species] = cohorts[species].Sum(o => o.LastLAI * o.CanopyLayerFrac);
                return LAIPerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> WoodSenescencePerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> WoodSenescencePerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    WoodSenescencePerSpecies[species] = cohorts[species].Sum(o => (int)(o.LastWoodSenescence * o.CanopyLayerFrac));
                return WoodSenescencePerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> FolSenescencePerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> FolSenescencePerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    FolSenescencePerSpecies[species] = cohorts[species].Sum(o => (int)(o.LastFolSenescence * o.CanopyLayerFrac));
                return FolSenescencePerSpecies;
            }
        }

        public Library.Parameters.Species.AuxParm<int> CohortCountPerSpecies
        {
            get
            {
                Library.Parameters.Species.AuxParm<int> CohortCountPerSpecies = new Library.Parameters.Species.AuxParm<int>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    CohortCountPerSpecies[species] = cohorts[species].Count();
                return CohortCountPerSpecies;
            }
        }


        public Library.Parameters.Species.AuxParm<List<ushort>> CohortAges
        {
            get
            {
                Library.Parameters.Species.AuxParm<List<ushort>> CohortAges = new Library.Parameters.Species.AuxParm<List<ushort>>(Globals.ModelCore.Species);
                foreach (ISpecies species in cohorts.Keys)
                    CohortAges[species] = new List<ushort>(cohorts[species].Select(o => o.Age));
                return CohortAges;
            }
        }

        public double BiomassSum
        {
            get
            {
                return AllCohorts.Sum(o => o.TotalBiomass * o.CanopyLayerFrac);
            }
        }

        public double AGBiomassSum
        {
            get
            {
                return AllCohorts.Sum(o => o.AGBiomass * o.CanopyLayerFrac);
            }
        }

        public double WoodBiomassSum
        {
            get
            {
                return AllCohorts.Sum(o => o.Wood * o.CanopyLayerFrac);
            }
        }

        public double WoodSenescenceSum
        {
            get
            {
                return AllCohorts.Sum(o => o.LastWoodSenescence * o.CanopyLayerFrac);
            }
        }

        public double FolSenescenceSum
        {
            get
            {
                return AllCohorts.Sum(o => o.LastFolSenescence * o.CanopyLayerFrac);
            }
        }

        public double BGBiomassSum
        {
            get
            {
                return AllCohorts.Sum(o => o.Root * o.CanopyLayerFrac);
            }
        }


        public double FolBiomassSum
        {
            get
            {
                return AllCohorts.Sum(o => o.Fol * o.CanopyLayerFrac);
            }
        }

        public double NSCSum
        {
            get
            {
                return AllCohorts.Sum(o => o.NSC * o.CanopyLayerFrac);
            }
        }

        public double PotentialET
        {
            get;
            set;
        }

        public int CohortCount
        {
            get
            {
                return cohorts.Values.Sum(o => o.Count());
            }
        }
        
        public int AverageAge 
        {
            get
            {
                return (int) cohorts.Values.Average(o => o.Average(x=>x.Age));
            }
        }

        public double ActualETSum
        {
            get
            {
                return ActualET.Sum();
            }
        }

        class SubCanopyComparer : IComparer<int[]>
        {
            // Compare second int (cumulative cohort biomass)
            public int Compare(int[] x, int[] y)
            {
                return (x[0] > y[0]) ? 1 : -1;
            }
        }

        private SortedDictionary<int[], Cohort> GetSubcanopyLayers()
        {
            SortedDictionary<int[], Cohort> subcanopylayers = new SortedDictionary<int[], Cohort>(new SubCanopyComparer());
            foreach (Cohort cohort in AllCohorts)
            {
                for (int i = 0; i < Globals.IMAX; i++)
                {
                    int[] subcanopylayer = new int[] { (ushort)((i + 1) / (double)Globals.IMAX * cohort.MaxBiomass) };
                    subcanopylayers.Add(subcanopylayer, cohort);
                }
            }
            return subcanopylayers;
        }

        private static int[] GetNextBinPositions(int[] index_in, int numcohorts)
        {            
            for (int index = index_in.Length - 1; index >= 0; index--)
            {
                int maxvalue = numcohorts - index_in.Length + index - 1;
                if (index_in[index] < maxvalue)
                {
                    index_in[index]++;
                    for (int i = index+1; i < index_in.Length; i++)
                    {
                        index_in[i] = index_in[i - 1] + 1;
                    }
                    return index_in;
                }
            }
            return null;
        }
      
        private int[] GetFirstBinPositions(int nlayers, int ncohorts)
        {
            int[] Bin = new int[nlayers - 1];
            for (int ly = 0; ly < Bin.Length; ly++)
            {
                Bin[ly] = ly+1;
            }
            return Bin;
        }

        public static List<T> Shuffle<T>(List<T> array)
        {
            int n = array.Count;
            while (n > 1)
            {
                int k = Distributions.DiscreteUniformRandom(0, n);
                n--;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
            return array;
        }

        uint CalcLayerMaxDev(List<double> f)
        {
            return (uint)Math.Max(Math.Abs(f.Max() - f.Average()), Math.Abs(f.Min() - f.Average()));
        }

        int[] MinMaxCohortNr(int[] Bin, int i, int Count)
        {
            int min = (i > 0) ? Bin[i - 1] : 0;
            int max = (i < Bin.Count()) ? Bin[i] : Count - 1;
            return new int[] { min, max };
        }

        private List<double> layerThreshRatio = new List<double>();

        private List<List<double>> GetBinsByCohort(List<double> CohortBiomassList)
        {
            if (CohortBiomassList.Count() == 0)
                return null;
            nlayers = 1;
            layerThreshRatio.Clear();
            double diffFrac = LayerThreshRatio;
            // sort by ascending biomass
            CohortBiomassList.Sort();
            // reverse to sort by descending biomass
            CohortBiomassList.Reverse();
            int tempMaxCanopyLayers = MaxCanopyLayers;
            if (CohortStacking)
            {
                tempMaxCanopyLayers = CohortBiomassList.Count();
                diffFrac = 1;
            }
            List<List<double>> CohortBins = new List<List<double>>();
            int topLayerIndex = 0;
            CohortBins.Add(new List<double>());
            CohortBins[0].Add(CohortBiomassList[0]);
            foreach (double cohortBio in CohortBiomassList)
            {
                double smallestThisLayer = CohortBins[0][0];
                double ratio = cohortBio / smallestThisLayer;
                if (ratio < diffFrac)
                {
                    if (topLayerIndex < (tempMaxCanopyLayers - 1))
                    {
                        topLayerIndex++;
                        nlayers++;
                        CohortBins.Add(new List<double>());
                        foreach (int i in Enumerable.Range(1, topLayerIndex).Reverse())
                        {
                            CohortBins[i] = new List<double>(CohortBins[i - 1]);
                        }
                        CohortBins[0].Clear();
                    }
                }
                // Add a negligable value [-1e-10; + 1e-10] to ratio in order to prevent duplicate keys
                double k = 1e-10 * 2.0 * (Distributions.ContinuousUniformRandom() - 0.5);
                layerThreshRatio.Add(ratio + k);
                if (!CohortBins[0].Contains(cohortBio))
                    CohortBins[0].Add(cohortBio);
            }
            bool tooManyLayers = false;
            if (CohortBins.Count() > tempMaxCanopyLayers)
                tooManyLayers = true;
            if (tooManyLayers)
            {
                List<double> sortedRatios = layerThreshRatio.ToList();
                sortedRatios.Sort();
                List<double> smallestRatios = new List<double>();
                for (int r = 0; r < (tempMaxCanopyLayers - 1); r++)
                {
                    smallestRatios.Add(sortedRatios[r]);
                }
                CohortBins.Clear();
                topLayerIndex = tempMaxCanopyLayers - 1;
                nlayers = 1;
                for (int r = 0; r < tempMaxCanopyLayers; r++)
                {
                    CohortBins.Add(new List<double>());
                }
                CohortBins[topLayerIndex].Add(CohortBiomassList[0]);
                int cohortInd = 0;
                foreach (double cohortRatio in layerThreshRatio)
                {
                    if (smallestRatios.Contains(cohortRatio))
                    {
                        topLayerIndex--;
                        nlayers++;
                    }
                    if (!CohortBins[topLayerIndex].Contains(CohortBiomassList[cohortInd]))
                        CohortBins[topLayerIndex].Add(CohortBiomassList[cohortInd]);
                    cohortInd++;
                }
            }
            return CohortBins;
        }

        public static uint CalcKey(uint a, ushort b)
        {
            uint value = (uint)((a << 16) | b);
            return value;
        }

        public List<Cohort> AllCohorts
        {
            get
            {
                List<Cohort> all = new List<Cohort>();
                foreach (ISpecies species in cohorts.Keys)
                    all.AddRange(cohorts[species]);
                return all;
            }
        }

        public void ClearAllCohorts()
        {
            cohorts.Clear();
        }

        public override int ReduceOrKillCohorts(Library.UniversalCohorts.IDisturbance disturbance)
        {
            List<int> reduction = new List<int>();
            List<Cohort> ToRemove = new List<Cohort>();
            foreach (List<Cohort> species_cohort in cohorts.Values)
            {
                SpeciesCohorts species_cohorts = GetSpeciesCohort(cohorts[species_cohort[0].Species]);
                for (int c = 0; c < species_cohort.Count(); c++)
                {
                    ICohort cohort = species_cohort[c];
                    // Disturbances return reduction in aboveground biomass
                    int _reduction = disturbance.ReduceOrKillMarkedCohort(cohort);
                    reduction.Add(_reduction);
                    if (reduction[reduction.Count() - 1] >= species_cohort[c].Biomass)  //Compare to aboveground biomass at site scale
                        ToRemove.Add(species_cohort[c]); // Edited by BRM - 090115
                    else
                    {
                        double reductionFrac = (double)reduction[reduction.Count() - 1] / (double)species_cohort[c].Biomass;  //Fraction of aboveground biomass at site scale
                        species_cohort[c].ReduceBiomass(this, reductionFrac, disturbance.Type);  // Reduction applies to all biomass
                    }
                }
            }
            foreach (Cohort cohort in ToRemove)
            {
                RemoveCohort(cohort, disturbance.Type);
            }
            return reduction.Sum();
        }

        public int AgeMax 
        {
            get
            {
                return (cohorts.Values.Count() > 0) ? cohorts.Values.Max(o => o.Max(x => x.Age)) : -1;
            }
        }

        Library.UniversalCohorts.ISpeciesCohorts Library.UniversalCohorts.IISiteCohorts<Library.UniversalCohorts.ISpeciesCohorts>.this[ISpecies species]
        {
            get
            {
                if (cohorts.ContainsKey(species))
                    return (Library.UniversalCohorts.ISpeciesCohorts)GetSpeciesCohort(cohorts[species]);
                return null;
            }
        }

        public new Library.UniversalCohorts.ISpeciesCohorts this[ISpecies species]  // also declared in UniversalCohorts.SiteCohorts
        {
            get
            {
                if (cohorts.ContainsKey(species))
                    return GetSpeciesCohort(cohorts[species]);
                return null;
            }
        }

        public override void RemoveMarkedCohorts(Library.UniversalCohorts.ICohortDisturbance disturbance)
        {
            base.RemoveMarkedCohorts(disturbance);
            ReduceOrKillCohorts(disturbance);
        }

        public override void RemoveMarkedCohorts(Library.UniversalCohorts.ISpeciesCohortsDisturbance disturbance)
        {
            //  Go through list of species cohorts from back to front so that
            //  a removal does not mess up the loop.
            base.RemoveMarkedCohorts(disturbance);
            int totalReduction = 0;
            List<Cohort> ToRemove = new List<Cohort>();
            Library.UniversalCohorts.SpeciesCohortBoolArray isSpeciesCohortDamaged = new Library.UniversalCohorts.SpeciesCohortBoolArray();
            foreach (ISpecies species in cohorts.Keys)
            {
                SpeciesCohorts speciescohort = GetSpeciesCohort(cohorts[species]);               
                isSpeciesCohortDamaged.SetAllFalse(speciescohort.Count);
                disturbance.MarkCohortsForDeath((Library.UniversalCohorts.ISpeciesCohorts)speciescohort, isSpeciesCohortDamaged);
                for (int c = 0; c < isSpeciesCohortDamaged.Count; c++)
                {
                    if (isSpeciesCohortDamaged[c])
                    {
                        totalReduction += (int) speciescohort[c].Data.UniversalData.Biomass;
                        ToRemove.Add(cohorts[species][c]);
                    }
                }
            }
            foreach (Cohort cohort in ToRemove)
            {
                Library.UniversalCohorts.Cohort.KilledByAgeOnlyDisturbance(disturbance, cohort, disturbance.CurrentSite, disturbance.Type);
                RemoveCohort(cohort, disturbance.Type);
            }
        }

        private void RemoveMarkedCohorts()
        {
            for (int c = cohorts.Values.Count - 1; c >= 0; c--)
            {
                List<Cohort> species_cohort = cohorts.Values.ElementAt(c);
                for (int cc = species_cohort.Count - 1; cc >= 0; cc--)
                {
                    if (species_cohort[cc].IsAlive == false)
                    {
                        bool coldKill = species_cohort[cc].ColdKill < int.MaxValue;
                        if (coldKill)
                            RemoveCohort(species_cohort[cc], new ExtensionType(Names.ExtensionName + ":Cold"));
                        else
                            RemoveCohort(species_cohort[cc], new ExtensionType(Names.ExtensionName));
                    }
                }
            }
        }

        public void RemoveCohort(Cohort cohort, ExtensionType disturbanceType)
        {
            if(disturbanceType.Name == Names.ExtensionName)
                CohortsKilledBySuccession[cohort.Species.Index] += 1;
            else if(disturbanceType.Name == (Names.ExtensionName+":Cold"))
                CohortsKilledByCold[cohort.Species.Index] += 1;
            else if(disturbanceType.Name == "disturbance:harvest")
                CohortsKilledByHarvest[cohort.Species.Index] += 1;
            else if(disturbanceType.Name == "disturbance:fire")
                CohortsKilledByFire[cohort.Species.Index] += 1;
            else if (disturbanceType.Name == "disturbance:wind")
                CohortsKilledByWind[cohort.Species.Index] += 1;
            else
                CohortsKilledByOther[cohort.Species.Index] += 1;
            if (disturbanceType.Name != Names.ExtensionName)
                Cohort.RaiseDeathEvent(this, cohort, Site, disturbanceType);
            cohorts[cohort.Species].Remove(cohort);
            if (cohorts[cohort.Species].Count == 0)
                cohorts.Remove(cohort.Species);
            if (!DisturbanceTypesReduced.Contains(disturbanceType))
            {
                Disturbance.ReduceDeadPools(this, disturbanceType); // Reduce dead pools before adding through Disturbance
                DisturbanceTypesReduced.Add(disturbanceType);
            }
            Disturbance.AllocateDeadPools(this, cohort, disturbanceType, 1.0);  // Disturbance fraction is 1.0 for complete removals
        }

        public new bool IsMaturePresent(ISpecies species)  // also declared in UniversalCohorts.SiteCohorts
        {
            bool speciesPresent = cohorts.ContainsKey(species);
            bool IsMaturePresent = (speciesPresent && (cohorts[species].Max(o => o.Age) >= species.Maturity)) ? true : false;
            return IsMaturePresent;
        }

        public bool AddNewCohort(Cohort newCohort)
        {
            bool addCohort = false;
            if (cohorts.ContainsKey(newCohort.Species))
            {
                // This should deliver only one KeyValuePair
                KeyValuePair<ISpecies, List<Cohort>> i = new List<KeyValuePair<ISpecies, List<Cohort>>>(cohorts.Where(o => o.Key == newCohort.Species))[0];
                List<Cohort> Cohorts = new List<Cohort>(i.Value.Where(o => o.Age < CohortBinSize));
                if (Cohorts.Count() > 1)
                {
                    foreach(Cohort Cohort in Cohorts.Skip(1))
                        newCohort.Accumulate(Cohort);
                }                
                if (Cohorts.Count() > 0)
                {
                    Cohorts[0].Accumulate(newCohort);
                    return addCohort;
                }
                else
                {
                    cohorts[newCohort.Species].Add(newCohort);
                    addCohort = true;
                    return addCohort;
                }
            }
            cohorts.Add(newCohort.Species, new List<Cohort>(new Cohort[] { newCohort }));
            addCohort = true;
            return addCohort;
        }

        SpeciesCohorts GetSpeciesCohort(List<Cohort> cohorts)
        {
            SpeciesCohorts species = new SpeciesCohorts(cohorts[0]);
            for (int cohort = 1; cohort < cohorts.Count; cohort++)
                species.AddNewCohort(cohorts[cohort]);
            return species;
        }

        public void AddWoodyDebris(double NewWoodyDebris, double WoodyDebrisDecompRate)
        {
            lock (Globals.WoodyDebrisThreadLock)
            {
                SiteVars.WoodyDebris[Site].AddMass(NewWoodyDebris, WoodyDebrisDecompRate);
            }
        }

        public void RemoveWoodyDebris(double percentReduction)
        {
            lock (Globals.WoodyDebrisThreadLock)
            {
                SiteVars.WoodyDebris[Site].ReduceMass(percentReduction);
            }
        }

        public void AddLeafLitter(double NewLeafLitter, double FolLignin)
        {
            lock (Globals.LeafLitterThreadLock)
            {
                // Calculate decomposition rate for species litter cohort based on AET and species-based lignin content
                // based on Meentemeyer, 1978, Ecology, 59, 465-472.
                // (this information copied from ForestFloor.cs in Extension-Biomass-Succession)
                double LeafLitterDecompRate = Math.Max(0.3, -0.5365 + (0.00241 * ActualET.Sum()) - (-0.01586 + (0.000056 * ActualET.Sum())) * FolLignin * 100);
                SiteVars.LeafLitter[Site].AddMass(NewLeafLitter, LeafLitterDecompRate);
            }
        }

        public void RemoveLeafLitter(double percentReduction)
        {
            lock (Globals.LeafLitterThreadLock)
            {
                SiteVars.LeafLitter[Site].ReduceMass(percentReduction);
            }
        }
        
        string Header(Landis.SpatialModeling.ActiveSite site)
        {            
            string s = OutputHeaders.Time +  "," +
                       OutputHeaders.Year + "," +
                       OutputHeaders.Month + "," +
                       OutputHeaders.Ecoregion + "," + 
                       OutputHeaders.SoilType +"," +
                       OutputHeaders.nCohorts + "," +
                       OutputHeaders.MaxLayerRatio + "," +
                       OutputHeaders.Layers + "," +
                       OutputHeaders.SumCanopyFrac + "," +
                       OutputHeaders.PAR0 + "," +
                       OutputHeaders.Tmin + "," +
                       OutputHeaders.Tavg + "," +
                       OutputHeaders.Tday + "," +
                       OutputHeaders.Tmax + "," +
                       OutputHeaders.Precip + "," +
                       OutputHeaders.CO2 + "," +
                       OutputHeaders.O3 + "," +
                       OutputHeaders.Runoff + "," + 
                       OutputHeaders.Leakage + "," + 
                       OutputHeaders.PotentialET + "," +
                       OutputHeaders.PotentialEvaporation + "," +
                       OutputHeaders.Evaporation + "," +
                       OutputHeaders.PotentialTranspiration + "," +
                       OutputHeaders.Transpiration + "," + 
                       OutputHeaders.Interception + "," +
                       OutputHeaders.SurfaceRunoff + "," +
                       OutputHeaders.SoilWaterContent + "," +
                       OutputHeaders.PressureHead + "," + 
                       OutputHeaders.SurfaceWater + "," +
                       OutputHeaders.AvailableWater + "," +
                       OutputHeaders.Snowpack + "," +
                       OutputHeaders.LAI + "," + 
                       OutputHeaders.VPD + "," + 
                       OutputHeaders.GrossPsn + "," + 
                       OutputHeaders.NetPsn + "," +
                       OutputHeaders.MaintResp + "," +
                       OutputHeaders.Wood + "," + 
                       OutputHeaders.Root + "," + 
                       OutputHeaders.Fol + "," + 
                       OutputHeaders.NSC + "," + 
                       OutputHeaders.HeteroResp + "," +
                       OutputHeaders.LeafLitter + "," + 
                       OutputHeaders.WoodyDebris + "," +
                       OutputHeaders.WoodSenescence + "," + 
                       OutputHeaders.FolSenescence + "," +
                       OutputHeaders.SubCanopyPAR + ","+
                       OutputHeaders.SoilDiffusivity + "," +
                       OutputHeaders.ActiveLayerDepth+","+
                       OutputHeaders.LeakageFrac + "," +
                       OutputHeaders.AverageAlbedo + "," +
                       OutputHeaders.FrostDepth + "," +
                       OutputHeaders.SPEI;
            return s;
        }

        private void AddSiteOutput(IPnETEcoregionVars monthdata)
        {
            double maxLayerRatio = 0;
            if (layerThreshRatio.Count() > 0)
                maxLayerRatio = layerThreshRatio.Max();
            string s = monthdata.Time + "," +
                       monthdata.Year + "," +
                       monthdata.Month + "," +
                       Ecoregion.Name + "," +
                       Ecoregion.SoilType + "," +
                       cohorts.Values.Sum(o => o.Count) + "," +
                       maxLayerRatio + "," +
                       nlayers + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.CanopyLayerFrac)) + "," +
                       monthdata.PAR0 + "," +
                       monthdata.Tmin + "," +
                       monthdata.Tavg + "," +
                       monthdata.Tday + "," +
                       monthdata.Tmax + "," +
                       monthdata.Prec + "," +
                       monthdata.CO2 + "," +
                       monthdata.O3 + "," +
                       hydrology.Runoff + "," +
                       hydrology.Leakage + "," +
                       hydrology.PotentialET + "," +
                       hydrology.PotentialEvaporation + "," +
                       hydrology.Evaporation + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.PotentialTranspiration.Sum())) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.Transpiration.Sum())) + "," +
                       interception + "," +
                       precLoss + "," +
                       hydrology.SoilWaterContent + "," +
                       hydrology.PressureHeadTable.CalcSoilWaterPressureHead(hydrology.SoilWaterContent,Ecoregion.SoilType)+ "," +
                       hydrology.SurfaceWater + "," +
                       ((hydrology.SoilWaterContent - Ecoregion.WiltingPoint) * Ecoregion.RootingDepth * fracRootAboveFrost + hydrology.SurfaceWater) + "," +  // mm of available water
                       snowpack + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.LAI.Sum() * x.CanopyLayerFrac)) + "," +
                       monthdata.VPD + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.GrossPsn.Sum() * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.NetPsn.Sum() * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.MaintenanceRespiration.Sum() * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.Wood * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.Root * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.Fol * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.NSC * x.CanopyLayerFrac)) + "," +
                       HeterotrophicRespiration + "," +
                       SiteVars.LeafLitter[Site].Mass + "," +
                       SiteVars.WoodyDebris[Site].Mass + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.LastWoodSenescence * x.CanopyLayerFrac)) + "," +
                       cohorts.Values.Sum(o => o.Sum(x => x.LastFolSenescence * x.CanopyLayerFrac)) + "," +
                       subcanopypar + "," +
                       soilDiffusivity + "," +
                       activeLayerDepth[monthdata.Month - 1] * 1000 + "," +
                       leakageFrac + "," +
                       averageAlbedo[monthdata.Month - 1] + "," +
                       frostDepth[monthdata.Month - 1] * 1000 + ","+
                       monthdata.SPEI;
            this.siteoutput.Add(s);
        }
 
        public override IEnumerator<Library.UniversalCohorts.ISpeciesCohorts> GetEnumerator()
        {
            foreach (ISpecies species in cohorts.Keys)
            {
                yield return this[species];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Library.UniversalCohorts.ISpeciesCohorts> IEnumerable<Library.UniversalCohorts.ISpeciesCohorts>.GetEnumerator()
        {
            foreach (ISpecies species in cohorts.Keys)
            {
                Library.UniversalCohorts.ISpeciesCohorts isp = this[species];
                yield return isp;
            }
             
        }

        public struct CumulativeLeafAreas
        {
            public double DarkConifer;
            public double LightConifer;
            public double Deciduous;
            public double GrassMossOpen;

            public double Total
            {
                get
                {
                    return DarkConifer + LightConifer + Deciduous + GrassMossOpen;
                }
            }

            public double DarkConiferFrac
            {
                get
                {
                    return Total == 0 ? 0 : DarkConifer / Total;
                }
            }

            public double LightConiferFrac
            {
                get
                {
                    return Total == 0 ? 0 : LightConifer / Total;
                }
            }

            public double DeciduousFrac
            {
                get
                {
                    return Total == 0 ? 0 : Deciduous / Total;
                }
            }

            public double GrassMossOpenFrac
            {
                get
                {
                    return Total == 0 ? 0 : GrassMossOpen / Total;
                }
            }

            public void Reset()
            {
                DarkConifer = 0;
                LightConifer = 0;
                Deciduous = 0;
                GrassMossOpen = 0;
            }
        }

        public IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
        {
            return from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & (1 << i)) != 0
                       select list[i];
        }
    }
}