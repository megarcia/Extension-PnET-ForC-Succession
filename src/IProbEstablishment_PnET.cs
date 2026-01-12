using System.Collections.Generic;
using Landis.Library.Parameters;

namespace Landis.Extension.Succession.PnETForC
{
    public interface IProbEstablishment
    {
        Library.Parameters.Species.AuxParm<double> SpeciesProbEstablishment { get; }

        double GetSpeciesFWater(IPnETSpecies species);

        double GetSpeciesFRad(IPnETSpecies species);

        Dictionary<IPnETSpecies, double> CalcProbEstablishmentForMonth(IPnETEcoregionVars pnetvars, IPnETEcoregionData ecoregion, double PAR, IHydrology hydrology, double minHalfSat, double maxHalfSat, bool invertProbEstablishment, double fracRootAboveFrost);
         
        bool IsEstablishedSpecies(IPnETSpecies species);

        void AddEstablishedSpecies(IPnETSpecies species);

        void RecordProbEstablishment(int year, IPnETSpecies species, double annualProbEstablishment, double annualFWater, double annualFRad, bool established, int monthCount);

        void Reset();
    }
}
