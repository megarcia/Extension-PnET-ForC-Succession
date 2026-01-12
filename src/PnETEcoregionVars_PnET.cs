using System;
using System.Collections.Generic;

namespace Landis.Extension.Succession.PnETForC
{
    public class PnETEcoregionVars : IPnETEcoregionVars
    {
        private DateTime _date;
        private IObservedClimate obs_clim;
        private double _vpd;
        private double _dayspan;
        private double _tavg;
        private double _tday;
        private double _dayLength;

        public double VPD
        {
            get
            {
                return _vpd;
            }
        }

        public byte Month
        {
            get
            {
                return (byte)_date.Month;
            }
        }

        public double Tday
        {
            get
            {
                return _tday;
            }
        }

        public double Prec
        {
            get
            {
                return obs_clim.Prec;
            }
        }

        public double O3
        {
            get
            {
                return obs_clim.O3;
            }
        }

        public double CO2
        {
            get
            {
                return obs_clim.CO2;
            }
        }

        public double SPEI
        {
            get
            {
                return obs_clim.SPEI;
            }
        }

        public double PAR0
        {
            get
            {
                return obs_clim.PAR0;
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
        }

        /// <summary>
        /// Number of days in the month
        /// </summary>
        public double DaySpan
        {
            get
            {
                return _dayspan;
            }
        }

        /// <summary>
        /// Year
        /// </summary>
        public int Year
        {
            get
            {
                return _date.Year;
            }
        }

        /// <summary>
        /// Time (decimal year)
        /// </summary>
        public double Time
        {
            get
            {
                return _date.Year + 1.0 / 12.0 * (_date.Month - 1);
            }
        }

        public double Tavg
        {
            get
            {
                return _tavg;
            }
        }

        public double Tmin
        {
            get
            {
                return obs_clim.Tmin;
            }
        }

        public double Tmax
        {
            get
            {
                return obs_clim.Tmax;
            }
        }

        public double DayLength
        {
            get
            {
                return _dayLength;
            }
        }

        private Dictionary<string, PnETSpeciesVars> speciesVariables;

        public PnETSpeciesVars this[string species]
        {
            get
            {
                return speciesVariables[species];
            }
        }

        public PnETEcoregionVars(IObservedClimate climate_dataset, DateTime Date, bool Wythers, bool DTemp, List<IPnETSpecies> Species, double Latitude)
        {
            _date = Date;
            obs_clim = climate_dataset;
            speciesVariables = new Dictionary<string, PnETSpeciesVars>();
            _tavg = Weather.CalcTavg(climate_dataset.Tmin, climate_dataset.Tmax);
            _dayspan = Calendar.CalcDaySpan(Date.Month);
            double hr = Calendar.CalcDaylightHrs(Date.DayOfYear, Latitude); //hours of daylight
            _dayLength = Calendar.CalcDayLength(hr);
            double nightLength = Calendar.CalcNightLength(hr);
            _tday = Weather.CalcTday(Tavg, climate_dataset.Tmax);
            _vpd = Weather.CalcVPD(Tday, climate_dataset.Tmin);
            foreach (IPnETSpecies pnetspecies in Species)
            {
                PnETSpeciesVars pnetspeciesvars = GetSpeciesVariables(ref climate_dataset, Wythers, DTemp, DayLength, nightLength, pnetspecies);
                speciesVariables.Add(pnetspecies.Name, pnetspeciesvars);
            }
        }

        private PnETSpeciesVars GetSpeciesVariables(ref IObservedClimate climate_dataset, bool Wythers, bool DTemp, double dayLength, double nightLength, IPnETSpecies spc)
        {
            // Class that contains species specific PnET variables for a certain month
            PnETSpeciesVars pnetspeciesvars = new PnETSpeciesVars();
            pnetspeciesvars.DVPD = Photosynthesis.CalcDVPD(VPD, spc.DVPD1, spc.DVPD2);
            pnetspeciesvars.JH2O = Photosynthesis.CalcJH2O(climate_dataset.Tmin, VPD);
            pnetspeciesvars.AmaxB_CO2 = Photosynthesis.CalcAmaxB_CO2(climate_dataset.CO2, spc.AmaxB, spc.AMaxBFCO2);
            if (DTemp)
                pnetspeciesvars.PsnFTemp = Photosynthesis.DTempResponse(Tday, spc.PsnTopt, spc.PsnTmin, spc.PsnTmax);
            else
                pnetspeciesvars.PsnFTemp = Photosynthesis.CurvilinearPsnTempResponse(Tday, spc.PsnTopt, spc.PsnTmin, spc.PsnTmax); // Modified 051216(BRM)
            // Respiration gC/timestep (RespTempResponses[0] = day respiration factor)
            // Respiration acclimation subroutine from:
            //      Tjoelker, M.G., Oleksyn, J., Reich, P.B. 1999: Acclimation of 
            //      respiration to temperature and C02 in seedlings of boreal tree 
            //      species in relation to plant size and relative growth rate. 
            //      Global Change Biology. 49:679-691.
            // and 
            //      Tjoelker, M.G., Oleksyn, J., Reich, P.B. 2001. Modeling 
            //      respiration of vegetation: evidence for a general temperature-
            //      dependent Q10. Global Change Biology. 7:223-230.
            // This set of algorithms resets the veg parameter "BaseFoliarRespirationFrac"
            // from the static vegetation parameter, then recalculates 
            // BaseFoliarRespiration based on the adjusted BaseFoliarRespirationFrac
            //
            // Base parameter in Q10 temperature dependency calculation
            double Q10base;
            if (Wythers)
            {
                pnetspeciesvars.BaseFoliarRespirationFrac = Respiration.CalcBaseFolRespFrac_Wythers(Tavg);
                Q10base = Respiration.CalcQ10_Wythers(Tavg, spc.PsnTopt);
            }
            else
            {
                pnetspeciesvars.BaseFoliarRespirationFrac = spc.BaseFoliarRespiration;
                Q10base = spc.Q10;
            }
            // Respiration Q10 factor
            pnetspeciesvars.RespirationFQ10 = Respiration.CalcFQ10(Q10base, Tavg, spc.PsnTopt);
            // Respiration adjustment for temperature
            double RespFTemp = Respiration.CalcFTemp(Q10base, Tday, Tmin, spc.PsnTopt, dayLength, nightLength);
            pnetspeciesvars.RespirationFTemp = RespFTemp;
            // Scaling factor of respiration given day and night temperature and day and night length
            pnetspeciesvars.MaintenanceRespirationFTemp = spc.MaintResp * RespFTemp;
            return pnetspeciesvars;
        }
    }
}
