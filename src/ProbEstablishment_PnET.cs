// NOTE: ISpecies --> Landis.Core

using System;
using System.Collections.Generic;
using Landis.Core;
using Landis.Library.Parameters;

namespace Landis.Extension.Succession.PnETForC
{
    public class ProbEstablishment : IProbEstablishment
    {
        private List<IPnETSpecies> establishedSpecies;
        private Dictionary<IPnETSpecies, double> speciesProbEstablishment;
        private Dictionary<IPnETSpecies, double> speciesFWater;
        private Dictionary<IPnETSpecies, double> speciesFRad;
        private LocalOutput probEstablishmentSiteOutput;

        public Library.Parameters.Species.AuxParm<double> SpeciesProbEstablishment
        {
            get
            {
                Library.Parameters.Species.AuxParm<double> SpeciesProbEstablishment = new Library.Parameters.Species.AuxParm<double>(Globals.ModelCore.Species);
                foreach (ISpecies species in Globals.ModelCore.Species)
                {
                    IPnETSpecies pnetspecies = SpeciesParameters.PnETSpecies[species];
                    SpeciesProbEstablishment[species] = speciesProbEstablishment[pnetspecies];
                }
                return SpeciesProbEstablishment; // 0.0-1.0 index
            }
        }

        public ProbEstablishment(string SiteOutputName, string FileName)
        {
            Reset();
            if (SiteOutputName != null && FileName != null)
                probEstablishmentSiteOutput = new LocalOutput(SiteOutputName, "Establishment.csv", OutputHeader);
        }

        public double GetSpeciesFWater(IPnETSpecies species)
        {
            {
                return speciesFWater[species];
            }
        }

        public double GetSpeciesFRad(IPnETSpecies species)
        {
            {
                return speciesFRad[species];
            }
        }

        public string OutputHeader
        {
            get
            {
                return "Year,Species,ProbEstablishment,FWater_Avg,FRad_Avg,ActiveMonths,IsEstablished";
            }
        }

        public Dictionary<IPnETSpecies,double> CalcProbEstablishmentForMonth(IPnETEcoregionVars pnetvars, IPnETEcoregionData ecoregion, double PAR, IHydrology hydrology,double minHalfSat, double maxHalfSat, bool invertProbEstablishment, double fracRootAboveFrost)
        {
            Dictionary<IPnETSpecies, double> speciesProbEstablishment = new Dictionary<IPnETSpecies, double>();
            double rangeHalfSat = maxHalfSat - minHalfSat;
            foreach (IPnETSpecies species in SpeciesParameters.PnETSpecies.AllSpecies)
            {
                if (pnetvars.Tmin > species.PsnTmin && pnetvars.Tmax < species.PsnTmax && fracRootAboveFrost > 0)
                {
                    // Adjust HalfSat for CO2 effect
                    double halfSat_intercept = species.HalfSat - Constants.CO2RefConc * species.HalfSatFCO2;
                    double halfSat_adj = species.HalfSatFCO2 * pnetvars.CO2 + halfSat_intercept;
                    double fRad = Math.Min(1.0, Math.Pow(Photosynthesis.CalcFRad(PAR, halfSat_adj), 2) * (1.0 / Math.Pow(species.EstablishmentFRad, 2)));
                    double fRad_adj = fRad;
                    // Optional adjustment to invert ProbEstablishment based on relative halfSat
                    if (invertProbEstablishment && rangeHalfSat > 0)
                    {
                        double fRad_adj_intercept = (species.HalfSat - minHalfSat) / rangeHalfSat;
                        double fRad_adj_slope = (fRad_adj_intercept * 2) - 1;
                        fRad_adj = 1 - fRad_adj_intercept + fRad * fRad_adj_slope;
                    }
                    speciesFRad[species] = fRad_adj;
                    double soilWaterPressureHead = hydrology.PressureHeadTable.CalcSoilWaterContent(hydrology.SoilWaterContent, ecoregion.SoilType);
                    double fWater = Math.Min(1.0, Math.Pow(Photosynthesis.CalcFWater(species.H1, species.H2, species.H3, species.H4, soilWaterPressureHead), 2) * (1.0 / Math.Pow(species.EstablishmentFWater, 2)));
                    speciesFWater[species] = fWater;
                    double probEstablishment = Math.Min(1.0, fRad_adj * fWater);
                    speciesProbEstablishment[species] = probEstablishment;
                }                
            }
            return speciesProbEstablishment;
        }

        public bool IsEstablishedSpecies(IPnETSpecies species)
        {
            return establishedSpecies.Contains(species);
        }
       
        public void AddEstablishedSpecies(IPnETSpecies species)
        {
            establishedSpecies.Add(species);
        }
        
        public void RecordProbEstablishment(int year, IPnETSpecies species, double annualProbEstablishment, double annualFWater, double annualFRad, bool established, int monthCount)
        {
            if (established)
            {
                if (!IsEstablishedSpecies(species))
                    establishedSpecies.Add(species);
            }
            if (probEstablishmentSiteOutput != null)
            {
                if (monthCount == 0)
                    probEstablishmentSiteOutput.Add(year.ToString() + "," + species.Name + "," + annualProbEstablishment + "," + 0 + "," + 0 + "," + 0 + "," + IsEstablishedSpecies(species));
                else
                    probEstablishmentSiteOutput.Add(year.ToString() + "," + species.Name + "," + annualProbEstablishment + "," + annualFWater + "," + annualFRad + "," + monthCount + "," + IsEstablishedSpecies(species));
                probEstablishmentSiteOutput.Write();
            }
            // Record annualProbEstablishment to be accessed as speciesProbEstablishment
            speciesProbEstablishment[species] = annualProbEstablishment;
        }

        public void Reset()
        {
            speciesProbEstablishment = new Dictionary<IPnETSpecies, double>();
            speciesFWater = new Dictionary<IPnETSpecies, double>();
            speciesFRad = new Dictionary<IPnETSpecies, double>();
            establishedSpecies = new List<IPnETSpecies>();
            foreach (IPnETSpecies species in SpeciesParameters.PnETSpecies.AllSpecies)
            {
                speciesProbEstablishment.Add(species, 0.0);
                speciesFWater.Add(species, 0.0);
                speciesFRad.Add(species, 0.0);
            }
        }
    }
}
