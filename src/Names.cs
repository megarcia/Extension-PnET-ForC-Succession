// NOTE: File --> System.IO
//       Landis.Data --> Landis.Core

using System;
using System.Collections.Generic;
using System.IO;
using Landis.Core;

namespace Landis.Extension.Succession.PnETForC
{
    public static class Names
    {
        /// <summary>
        /// Data structure
        /// </summary>
        public static SortedDictionary<string, Parameter<string>> parameters = new SortedDictionary<string, Parameter<string>>(StringComparer.InvariantCultureIgnoreCase);  // from PnET

        /// <summary>
        /// Combined PnET-ForC parameters
        /// </summary>
        public const string ExtensionName = "PnET-ForC-Succession";  // combining PnET and ForC
        public const string Parallel = "Parallel";  // from PnET
        public const string StartYear = "StartYear";  // from PnET
        public const string Timestep = "Timestep";  // from both PnET and ForC

        /// <summary>
        /// Domain mapping parameters
        /// </summary>
        public const string MapCoordinates = "MapCoordinates";  // from PnET

        /// <summary>
        /// PnET parameters
        /// </summary>
        public const string PnETGenericParameters = "PnETGenericParameters";  // from PnET
        public const string PnETGenericDefaultParameters = "PnETGenericDefaultParameters";  // from PnET
        public const string EcoregionParameters = "EcoregionParameters";  // from PnET
        public const string PnETSpeciesParameters = "PnETSpeciesParameters";  // from PnET

        /// <summary>
        /// Weather/climate parameters
        /// </summary>
        public const string ClimateConfigFile = "ClimateConfigFile";  // from PnET
        public const string PARunits = "PARunits";  // from PnET
        public const string PrecipEventsWithReplacement = "PrecipEventsWithReplacement";  // from PnET

        /// <summary>
        /// Cohort calculation methods and parameters
        /// </summary>
        public const string CohortBinSize = "CohortBinSize";  // from PnET
        public const string CohortStacking = "CohortStacking";  // from PnET
        public const string LayerThreshRatio = "LayerThreshRatio";  // from PnET

        /// <summary>
        /// Canopy calculation methods and parameters
        /// </summary>
        public const string IMAX = "IMAX";  // from PnET
        public const string MaxCanopyLayers = "MaxCanopyLayers";  // from PnET
        public const string MinFolRatioFactor = "MinFolRatioFactor";  // from PnET
        public const string CanopySumScale = "CanopySumScale";  // from PnET

        /// <summary>
        /// Photosynthesis calculation methods and parameters 
        /// </summary>
        public const string DTemp = "DTemp";  // from PnET
        public const string AMaxBFCO2 = "AMaxBFCO2";  // from PnET

        /// <summary>
        /// Respiration calculation methods and parameters 
        /// </summary>
        public const string Wythers = "Wythers";  // from PnET

        /// <summary>
        /// Evapotranspiration calculation methods and parameters 
        /// </summary>
        public const string ETExtCoeff = "ETExtCoeff";  // from PnET
        public const string ReferenceETCropCoeff = "ReferenceETCropCoeff";  // from PnET

        /// <summary>
        /// Hydrology calculation methods and parameters
        /// </summary>
        public const string VanGenuchten = "VanGenuchten";  // from PnET
        public const string PressureHeadCalculationMethod = "PressureHeadCalculationMethod";  // from PnET
        public const string SaxtonAndRawls = "SaxtonAndRawlsParameters";  // from PnET
        public const string SoilIceDepth = "SoilIceDepth";  // from PnET
        public const string LeakageFrostDepth = "LeakageFrostDepth";  // from PnET

        /// <summary>
        /// Disturbance parameters
        /// </summary>
        public const string DisturbanceReductions = "DisturbanceReductions";  // from PnET

        /// <summary>
        /// Initiation parameters
        /// </summary>
        public const string InitialCommunities = "InitialCommunities";  // from PnET
        public const string InitialCommunitiesMap = "InitialCommunitiesMap";  // from PnET
        public const string InitialCommunitiesSpinup = "InitialCommunitiesSpinup";  // from PnET
        public const string SpinUpWaterStress = "SpinUpWaterStress";  // from PnET
        public const string LeafLitterMap = "LeafLitterMap";  // from PnET
        public const string WoodyDebrisMap = "WoodyDebrisMap";  // from PnET

        /// <summary>
        /// Reproduction parameters
        /// </summary>
        public const string SeedingAlgorithm = "SeedingAlgorithm";  // from both PnET and ForC
        public const string InvertPest = "InvertPest";  // from PnET

        /// <summary>
        /// Output parameters
        /// </summary>
        public const string PnETOutputSites = "PnETOutputSites";  // from PnET
        public const string PnEToutputSiteCoordinates = "PnEToutputSiteCoordinates";  // from PnET
        public const string PnEToutputSiteLocation = "PnEToutputSiteLocation";  // from PnET

        /// <summary>
        /// ForC parameters
        /// </summary>
        public const string ClimateTable = "ClimateTable";  // from ForC
        public const string DisturbFireTransferDOM = "DisturbFireTransferDOM";  // from ForC
        public const string DisturbOtherTransferDOM = "DisturbOtherTransferDOM";  // from ForC
        public const string DisturbFireTransferBiomass = "DisturbFireTransferBiomass";  // from ForC
        public const string DisturbOtherTransferBiomass = "DisturbOtherTransferBiomass";  // from ForC
        public const string CalibrateMode = "CalibrateMode";  // from ForC
        public const string ClimateFile = "ClimateFile";  // from ForC
        public const string ClimateFile2 = "ForCSClimateFile";  // from ForC
        public const string PnETGroupFile = "PnETfunctionalGroupFile";  // from ForC
        public const string ForCSOutput = "ForCSOutput";  // from ForC
        public const string ForCSMapControl = "ForCSMapControl";  // from ForC
        public const string SpinUp = "SpinUp";  // from ForC
        public const string AvailableLightBiomass = "AvailableLightBiomass";  // from ForC
        public const string LightEstablishmentTable = "LightEstablishmentTable";  // from ForC
        public const string SpeciesParameters = "SpeciesParameters";  // from ForC
        public const string FireReductionParameters = "FireReductionParameters";  // from ForC
        public const string DOMPools = "DOMPools";  // from ForC
        public const string EcoSppDOMParms = "EcoSppDOMParameters";  // from ForC
        public const string ForCSProportions = "ForCSProportions";  // from ForC
        public const string ANPPTimeSeries = "ANPPTimeSeries";  // from ForC
        public const string MaxBiomassTimeSeries = "MaxBiomassTimeSeries";  // from ForC
        public const string EstablishProbabilities = "EstablishProbabilities";  // from ForC
        public const string RootDynamics = "RootDynamics";  // from ForC
        public const string initSnagFile = "SnagFile";  // from ForC
        public const string SnagData = "SnagData";  // from ForC
        public const string DMFile = "DisturbanceMatrixFile";  // from ForC
        public const string DisturbTypeFire = "Fire";  // from ForC
        public const string DisturbTypeHarvest = "Harvest";  // from ForC

        /// <summary>
        /// PnET Ecoregion parameters
        /// </summary>
        public const string LeakageFrac = "LeakageFrac";  // from PnET, at ecoregion level
        public const string RunoffCapture = "RunoffCapture";  // from PnET, at ecoregion level
        public const string PrecLossFrac = "PrecLossFrac";  // from PnET, at ecoregion level
        public const string RootingDepth = "RootingDepth";  // from PnET, at ecoregion level
        public const string SoilType = "SoilType";  // from PnET, at ecoregion level
        public const string PrecIntConst = "PrecIntConst";  // from PnET, at ecoregion level
        public const string SnowSublimFrac = "SnowSublimFrac";  // from PnET, at ecoregion level
        public const string PrecipEvents = "PrecipEvents";  // from PnET, at ecoregion level
        public const string Latitude = "Latitude";  // from PnET, at ecoregion level
        public const string climateFileName = "climateFileName";  // from PnET, at ecoregion level
        public const string WinterSTD = "WinterSTD";  // from PnET, at ecoregion level
        public const string MossDepth = "MossDepth";  // from PnET, at ecoregion level
        public const string EvapDepth = "EvapDepth";  // from PnET, at ecoregion level
        public const string FrostFactor = "FrostFactor";  // from PnET, at ecoregion level

        /// <summary>
        /// PnET Species parameters
        /// </summary>
        public const string FolN_slope = "FolN_slope";  // from PnET, at species level
        public const string FolN_intercept = "FolN_intercept";  // from PnET, at species level
        public const string FolBiomassFrac_slope = "FolBiomassFrac_slope";  // from PnET, at species level
        public const string FolBiomassFrac_intercept = "FolBiomassFrac_intercept";  // from PnET, at species level
        public const string FOzone_slope = "FOzone_slope";  // from PnET, at species level
        public static readonly string[] MutuallyExclusiveCanopyTypes = new string[] { "dark", "light", "decid", "ground", "open" };  // from PnET, at species level
        public const string LeafOnMinT = "LeafOnMinT"; // Optional  // from PnET, at species level
        public const string RefoliationMinimumTrigger = "RefolMinimumTrigger";  // from PnET, at species level
        public const string MaxRefoliationFrac = "RefolMaximum";  // from PnET, at species level
        public const string RefoliationCost = "RefolCost";  // from PnET, at species level
        public const string NonRefoliationCost = "NonRefolCost";  // from PnET, at species level

        public static List<string> AllNames
        {
            get
            {
                List<string> Names = new List<string>();
                foreach (var name in typeof(Names).GetFields())
                {
                    string value = name.GetValue(name).ToString();
                    Names.Add(value);
                }
                return Names;
            }
        }

        public static void LoadParameters(SortedDictionary<string, Parameter<string>> modelParameters)
        {
            parameters = modelParameters;
        }

        public static bool TryGetParameter(string label, out Parameter<string> parameter)
        {
            parameter = null;
            if (label == null)
                return false;
            if (parameters.ContainsKey(label) == false)
                return false;
            else
            {
                parameter = parameters[label];
                return true;
            }
        }

        public static Dictionary<string, Parameter<string>> LoadTable(string label, List<string> RowLabels, List<string> Columnheaders, bool transposed = false)
        {
            string filename = GetParameter(label).Value;
            if (File.Exists(filename) == false)
                throw new Exception("File not found " + filename);
            ParameterTableParser parser = new ParameterTableParser(filename, label, RowLabels, Columnheaders, transposed);
            Dictionary<string, Parameter<string>> parameters = Landis.Data.Load<Dictionary<string, Parameter<string>>>(filename, parser);

            return parameters;
        }

        public static Parameter<string> GetParameter(string label)
        {
            if (parameters.ContainsKey(label) == false)
                throw new Exception("No value provided for parameter " + label);

            return parameters[label];
        }

        public static Parameter<string> GetParameter(string label, double min, double max)
        {
            if (parameters.ContainsKey(label) == false)
                throw new Exception("No value provided for parameter " + label);
            Parameter<string> p = parameters[label];
            foreach (KeyValuePair<string, string> value in p)
            {
                double f;
                if (double.TryParse(value.Value, out f) == false)
                    throw new Exception("Unable to parse value " + value.Value + " for parameter " + label + " unexpected format.");
                if (f > max || f < min)
                    throw new Exception("Parameter value " + value.Value + " for parameter " + label + " is out of range. [" + min + "," + max + "]");
            }
            return p;
        }

        public static Parameter<string> GetParameter(string label, float min, float max)
        {
            if (parameters.ContainsKey(label) == false)
                throw new Exception("No value provided for parameter " + label);
            Parameter<string> p = parameters[label];
            foreach (KeyValuePair<string, string> value in p)
            {
                float f;
                if (float.TryParse(value.Value, out f) == false)
                    throw new Exception("Unable to parse value " + value.Value + " for parameter " + label + " unexpected format.");
                if (f > max || f < min)
                    throw new Exception("Parameter value " + value.Value + " for parameter " + label + " is out of range. [" + min + "," + max + "]");
            }
            return p;
        }

        public static bool HasMultipleMatches(string lifeForm, ref string[] matches)
        {
            int matchCount = 0;
            foreach (string type in MutuallyExclusiveCanopyTypes)
            {
                if (!string.IsNullOrEmpty(lifeForm) && lifeForm.ToLower().Contains(type.ToLower()))
                {
                    matches[matchCount] = type;
                    matchCount += 1;
                }
                if (matchCount > 1)
                    return true;
            }
            return false;
        }
    }
}
