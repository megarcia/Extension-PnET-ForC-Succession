// NOTE: IEcoregion --> Landis.Core

using System;
using System.Collections.Generic;
using System.Linq;
using Landis.Core;
using Landis.Library.Parameters;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// The information for a tree species (its index and parameters).
    /// </summary>
    public class PnETEcoregionData : IPnETEcoregionData
    {
        private IEcoregion ecoregion;
        private double _precintconst;
        private double _preclossfrac;
        private double _rootingdepth;
        private string _soiltype;
        private double _leakagefrac;
        private double _runoffcapture;
        private double _fieldcap;
        private double _wiltpnt;
        private double _soilPorosity;
        private double _snowsublimfrac;
        private double _latitude;
        private int _precipEvents;
        private double _leakageFrostDepth;
        private double _winterSTD;
        private double _mossDepth;
        IPnETEcoregionVars _variables;
        private double _evapDepth;
        private double _frostFactor;
        private static bool wythers;
        private static bool dtemp;
        private static double etExtCoeff;
        private static double retCropCoeff;
        private static Dictionary<IPnETEcoregionData, Dictionary<DateTime, IPnETEcoregionVars>> all_values = new Dictionary<IPnETEcoregionData, Dictionary<DateTime, IPnETEcoregionVars>>();
        private static Dictionary<IEcoregion, IPnETEcoregionData> AllEcoregions;
        private static Library.Parameters.Ecoregions.AuxParm<string> soiltype;
        private static Library.Parameters.Ecoregions.AuxParm<double> rootingdepth;
        private static Library.Parameters.Ecoregions.AuxParm<double> precintconst;
        private static Library.Parameters.Ecoregions.AuxParm<double> preclossfrac;
        private static Library.Parameters.Ecoregions.AuxParm<double> leakagefrac;
        private static Library.Parameters.Ecoregions.AuxParm<double> runoffcapture;
        private static Library.Parameters.Ecoregions.AuxParm<double> snowsublimfrac;
        private static Library.Parameters.Ecoregions.AuxParm<string> climateFileName;
        private static Library.Parameters.Ecoregions.AuxParm<double> latitude;
        private static Library.Parameters.Ecoregions.AuxParm<int> precipEvents;
        private static Library.Parameters.Ecoregions.AuxParm<double> leakageFrostDepth;
        private static Library.Parameters.Ecoregions.AuxParm<double> winterSTD;
        private static Library.Parameters.Ecoregions.AuxParm<double> mossDepth;
        private static Library.Parameters.Ecoregions.AuxParm<double> evapDepth;
        private static Library.Parameters.Ecoregions.AuxParm<double> frostFactor;

        public static List<IPnETEcoregionData> Ecoregions
        {
            get 
            {
                return AllEcoregions.Values.ToList();
            }
        }

        /// <summary>
        /// Returns the PnET Ecoregion for a given Landis Core Ecoregion
        /// </summary>
        /// <param name="landisCoreEcoregion"></param>
        public static IPnETEcoregionData GetPnETEcoregion(IEcoregion ecoregion)
        {
            return AllEcoregions[ecoregion];
        }

        public IPnETEcoregionVars Variables
        {
            get
            {
                return _variables;
            }
            set
            {
                _variables = value;
            }

        }

        public double FieldCapacity
        {
            get
            {
                return _fieldcap;
            }
            set
            {
                _fieldcap = value;
            }
        }

        public double WiltingPoint
        {
            get
            {
                return _wiltpnt;
            }
            set
            {
                _wiltpnt = value;
            }
        }

        public double Porosity
        {
            get
            {
                return _soilPorosity;
            }
            set
            {
                _soilPorosity = value;
            }
        }

        public double LeakageFrac
        {
            get
            {
                return _leakagefrac;
            }
        }

        public double RunoffCapture
        {
            get
            {
                return _runoffcapture;
            }
        }

        public double PrecIntConst
        {
            get
            {
                return _precintconst;
            }
        }

        public double RootingDepth
        {
            get
            {
                return _rootingdepth;
            }
        }

        public string SoilType
        {
            get
            {
                return _soiltype;
            }
        }

        public double PrecLossFrac
        {
            get
            {
                return _preclossfrac;
            }
        }

        public string Description
        {
            get
            {
                return ecoregion.Description;
            }
        }

        public bool Active
        {
            get
            {
                return ecoregion.Active;
            }
        }

        public ushort MapCode
        {
            get
            {
                return ecoregion.MapCode;
            }
        }

        public int Index
        {
            get
            {
                return ecoregion.Index;
            }
        }

        public string Name
        {
            get
            {
                return ecoregion.Name;
            }
        }

        public double SnowSublimFrac
        {
            get
            {
                return _snowsublimfrac;
            }
        }

        public double Latitude
        {
            get
            {
                return _latitude;
            }
        }

        public int PrecipEvents
        {
            get
            {
                return _precipEvents;
            }
        }

        public double LeakageFrostDepth
        {
            get
            {
                return _leakageFrostDepth;
            }
        }

        public double WinterSTD
        {
            get
            {
                return _winterSTD;
            }
        }

        public double MossDepth
        {
            get
            {
                return _mossDepth;
            }
        }

        /// <summary>
        /// Maximum soil depth susceptible to surface evaporation
        /// </summary>
        public double EvapDepth
        {
            get
            {
                return _evapDepth;
            }
        }

        public double FrostFactor
        {
            get
            {
                return _frostFactor; ;
            }
        }

        public static List<string> ParameterNames
        {
            get
            {
                Type type = typeof(PnETEcoregionData); // Get type pointer
                List<string> names = type.GetProperties().Select(x => x.Name).ToList(); // Obtain all fields
                names.Add("ClimateFileName");
                return names;
            }
        }

        public static List<IPnETEcoregionVars> GetClimateRegionData(IPnETEcoregionData ecoregion, DateTime start, DateTime end)
        {
            // Monthly simulation data untill but not including end
            List<IPnETEcoregionVars> data = new List<IPnETEcoregionVars>();
            // Date: the last date in the collection of running data
            DateTime date = new DateTime(start.Ticks);
            var oldYear = -1;
            // Ensure only one thread at a time accesses this shared object
            lock (Globals.EcoregionDataThreadLock)
            {
                while (end.Ticks > date.Ticks)
                {
                    if (!all_values[ecoregion].ContainsKey(date))
                    {
                        if (date.Year != oldYear)
                        {
                            if (Globals.IsFutureClimate(date))
                            {
                                ClimateRegionData.AnnualClimate[ecoregion] = 
                                Climate.Climate.FutureEcoregionYearClimate[ecoregion.Index][Globals.ConvertYearToFutureClimateYear(date)];
                            }
                            else
                            {
                                ClimateRegionData.AnnualClimate[ecoregion] = 
                                Climate.Climate.SpinupEcoregionYearClimate[ecoregion.Index][Globals.ConvertYearToSpinUpClimateYear(date)];
                            }
                            oldYear = date.Year;
                        }
                        var monthlyData = new MonthlyClimateRecord(ecoregion, date);
                        List<IPnETSpecies> species = SpeciesParameters.PnETSpecies.AllSpecies.ToList();
                        IPnETEcoregionVars ecoregion_variables = new PnETClimateVars(monthlyData, date, wythers, dtemp, species, ecoregion.Latitude);
                        all_values[ecoregion].Add(date, ecoregion_variables);
                    }
                    data.Add(all_values[ecoregion][date]);
                    date = date.AddMonths(1);
                }
            }
            return data;
        }

        public static List<IPnETEcoregionVars> GetData(IPnETEcoregionData ecoregion, DateTime start, DateTime end)
        {
            // Monthly simulation data untill but not including end
            List<IPnETEcoregionVars> data = new List<IPnETEcoregionVars>();
            // Date: the last date in the collection of running data
            DateTime date = new DateTime(start.Ticks);
            // Ensure only one thread at a time accesses this shared object
            lock (Globals.EcoregionDataThreadLock)
            {
                while (end.Ticks > date.Ticks)
                {
                    if (all_values[ecoregion].ContainsKey(date) == false)
                    {
                        IObservedClimate observedClimate = ObservedClimate.GetData(ecoregion, date);
                        List<IPnETSpecies> species = SpeciesParameters.PnETSpecies.AllSpecies.ToList();
                        IPnETEcoregionVars ecoregion_variables = new PnETEcoregionVars(observedClimate, date, wythers, dtemp, species, ecoregion.Latitude);
                        try
                        {
                            all_values[ecoregion].Add(date, ecoregion_variables);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                    data.Add(all_values[ecoregion][date]);
                    date = date.AddMonths(1);
                }
            }
            return data;
        }

        public static void Initialize()
        {
            soiltype = (Library.Parameters.Ecoregions.AuxParm<string>)Names.GetParameter("SoilType");
            rootingdepth = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("RootingDepth", 0, 1000);
            precintconst = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("PrecIntConst", 0, 1);
            preclossfrac = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("PrecLossFrac", 0, 1);
            snowsublimfrac = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("SnowSublimFrac", 0, 1);
            latitude = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("Latitude", 0, 90);
            leakageFrostDepth = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("LeakageFrostDepth", 0, 999999);
            precipEvents = (Library.Parameters.Ecoregions.AuxParm<int>)(Parameter<int>)Names.GetParameter("PrecipEvents", 1, 100);
            winterSTD = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("WinterSTD", 0, 100);
            mossDepth = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("MossDepth", 0, 1000);
            evapDepth = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("EvapDepth", 0, 9999999);
            wythers = ((Parameter<bool>)Names.GetParameter("Wythers")).Value;
            dtemp = ((Parameter<bool>)Names.GetParameter("DTemp")).Value;
            etExtCoeff = ((Parameter<double>)Names.GetParameter("ETExtCoeff")).Value;
            retCropCoeff = ((Parameter<double>)Names.GetParameter("ReferenceETCropCoeff")).Value;
            leakagefrac = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("LeakageFrac", 0, 1);
            runoffcapture = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter(Names.RunoffCapture, 0, 999999);
            frostFactor = (Library.Parameters.Ecoregions.AuxParm<double>)(Parameter<double>)Names.GetParameter("FrostFactor", 0, 999999);
            AllEcoregions = new Dictionary<IEcoregion, IPnETEcoregionData>();
            foreach (IEcoregion ecoregion in Globals.ModelCore.Ecoregions)
            {
                AllEcoregions.Add(ecoregion, new PnETEcoregionData(ecoregion));
            }
            all_values = new Dictionary<IPnETEcoregionData, Dictionary<DateTime, IPnETEcoregionVars>>();
            foreach (IPnETEcoregionData ecoregion in AllEcoregions.Values)
            {
                all_values[ecoregion] = new Dictionary<DateTime, IPnETEcoregionVars>();
            }
        }

        public PnETEcoregionData(IEcoregion ecoregion)
        {
            this.ecoregion = ecoregion;
            _rootingdepth = rootingdepth[ecoregion];
            _soiltype = soiltype[ecoregion];
            _precintconst = precintconst[ecoregion];
            _preclossfrac = preclossfrac[ecoregion];
            _leakagefrac = leakagefrac[ecoregion];
            _runoffcapture = runoffcapture[ecoregion];
            _snowsublimfrac = snowsublimfrac[ecoregion];
            _latitude = latitude[ecoregion];
            _precipEvents = precipEvents[ecoregion];
            _leakageFrostDepth = leakageFrostDepth[ecoregion];
            _winterSTD = winterSTD[ecoregion];
            _mossDepth = mossDepth[ecoregion];
            _evapDepth = evapDepth[ecoregion];
            _frostFactor = frostFactor[ecoregion];
        }

        public static bool TryGetParameter(string label, out Parameter<string> parameter)
        {
            parameter = null;
            if (label == null)
                return false;
            if (Names.parameters.ContainsKey(label) == false)
                return false;
            else
            {
                parameter = Names.parameters[label];
                return true;
            }
        }

        public static Parameter<string> GetParameter(string label)
        {
            if (Names.parameters.ContainsKey(label) == false)
                throw new Exception("No value provided for parameter " + label);

            return Names.parameters[label];
        }

        public static Parameter<string> GetParameter(string label, double min, double max)
        {
            if (Names.parameters.ContainsKey(label) == false)
                throw new Exception("No value provided for parameter " + label);
            Parameter<string> p = Names.parameters[label];
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
    }
}
 