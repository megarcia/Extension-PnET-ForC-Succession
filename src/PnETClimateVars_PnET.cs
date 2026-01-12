/// <summary>
/// John McNabb: This is a copy of PnETEcoregionVars substituting 
/// MonthlyClimateRecord _monthlyClimateRecord for 
/// IObservedClimate obs_clim
/// </summary>

using System;
using System.Collections.Generic;

namespace Landis.Extension.Succession.PnETForC
{
    public class PnETClimateVars : IPnETEcoregionVars
    {
        private MonthlyClimateRecord _monthlyClimateRecord;
        private DateTime _date;
        private double _vpd;
        private double _dayspan;
        private double _tavg;
        private double _tday;
        private double _dayLength;
        private Dictionary<string, PnETSpeciesVars> speciesVariables;

        public PnETClimateVars(MonthlyClimateRecord monthlyClimateRecord, DateTime date, bool wythers, bool dTemp, List<IPnETSpecies> Species, double latitude)
        {
            _monthlyClimateRecord = monthlyClimateRecord;
            _date = date;
            speciesVariables = new Dictionary<string, PnETSpeciesVars>();
            _tavg = Weather.CalcTavg(monthlyClimateRecord.Tmin, monthlyClimateRecord.Tmax);
            _dayspan = Calendar.CalcDaySpan(date.Month);
            double hr = Calendar.CalcDaylightHrs(date.DayOfYear, latitude);
            _dayLength = Calendar.CalcDayLength(hr);
            double nightLength = Calendar.CalcNightLength(hr);
            _tday = Weather.CalcTavg(_tavg, monthlyClimateRecord.Tmax);
            _vpd = Weather.CalcVPD(Tday, monthlyClimateRecord.Tmin);
            foreach (IPnETSpecies pnetspecies in Species)
            {
                PnETSpeciesVars pnetspeciesvars = GetSpeciesVariables(monthlyClimateRecord, wythers, dTemp, _dayLength, nightLength, pnetspecies);
                speciesVariables.Add(pnetspecies.Name, pnetspeciesvars);
            }
        }

        public double VPD => _vpd;
        public byte Month => (byte)_date.Month;
        public double Tday => _tday;
        public double Prec => (double)_monthlyClimateRecord.Prec;
        public double O3 => (double)_monthlyClimateRecord.O3;
        public double CO2 => (double)_monthlyClimateRecord.CO2;
        public double PAR0 => (double)_monthlyClimateRecord.PAR0;
        public DateTime Date => _date;
        public double DaySpan => _dayspan;
        public double Time => _date.Year + 1.0 / 12.0 * (_date.Month - 1);
        public int Year => _date.Year;
        public double Tavg => _tavg;
        public double Tmin => (double)_monthlyClimateRecord.Tmin;
        public double Tmax => (double)_monthlyClimateRecord.Tmax;
        public double DayLength => _dayLength;
        public double SPEI => (double)_monthlyClimateRecord.SPEI;
        public PnETSpeciesVars this[string species] => speciesVariables[species];

        private PnETSpeciesVars GetSpeciesVariables(MonthlyClimateRecord monthlyClimateRecord, bool wythers, bool dTemp, double dayLength, double nightLength, IPnETSpecies spc)
        {
            // Class that contains species specific PnET variables for a certain month
            PnETSpeciesVars pnetspeciesvars = new PnETSpeciesVars();
            pnetspeciesvars.DVPD = Photosynthesis.CalcDVPD(VPD, spc.DVPD1, spc.DVPD2);
            pnetspeciesvars.JH2O = Photosynthesis.CalcJH2O(monthlyClimateRecord.Tmin, VPD);
            pnetspeciesvars.AmaxB_CO2 = Photosynthesis.CalcAmaxB_CO2(monthlyClimateRecord.CO2, spc.AmaxB, spc.AMaxBFCO2);
            if (dTemp)
                pnetspeciesvars.PsnFTemp = Photosynthesis.DTempResponse(Tday, spc.PsnTopt, spc.PsnTmin, spc.PsnTmax);
            else
                pnetspeciesvars.PsnFTemp = Photosynthesis.CurvilinearPsnTempResponse(Tday, spc.PsnTopt, spc.PsnTmin, spc.PsnTmax); // Modified 051216(BRM)
            // Respiration gC/timestep (RespTempResponses[0] = day respiration factor)
            // Respiration acclimation subroutine From: Tjoelker, M.G., Oleksyn, J., Reich, P.B. 1999.
            // Acclimation of respiration to temperature and C02 in seedlings of boreal tree species
            // in relation to plant size and relative growth rate. Global Change Biology. 49:679-691,
            // and Tjoelker, M.G., Oleksyn, J., Reich, P.B. 2001. Modeling respiration of vegetation:
            // evidence for a general temperature-dependent Q10. Global Change Biology. 7:223-230.
            // This set of algorithms resets the veg parameter "BaseFoliarRespirationFrac" from
            // the static vegetation parameter, then recalculates BaseFoliarRespiration based on the adjusted
            // BaseFoliarRespirationFrac
            //
            // Base parameter in Q10 temperature dependency calculation
            double Q10base;
            if (wythers)
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
