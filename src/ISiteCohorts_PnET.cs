// NOTE: ISpecies --> Landis.Core

using System.Collections.Generic;
using Landis.Core;
using Landis.Library.Parameters;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Succession.PnETForC 
{
    public interface ISiteCohorts : Library.UniversalCohorts.ISiteCohorts
    {
        double[] NetPsn { get; }
        double[] MaintResp { get; }
        double[] GrossPsn { get; }
        double[] FoliarRespiration { get; }
        double[] AverageAlbedo { get; }
        double[] ActiveLayerDepth { get; }
        double[] FrostDepth { get; }
        double[] MonthlyAvgSnowpack { get; }
        double[] MonthlyAvgWater { get; }
        double[] MonthlyAvgLAI { get; }
        double[] MonthlyEvap { get; }
        double[] MonthlyActualTrans { get; }
        double[] MonthlyLeakage { get; }
        double[] MonthlyInterception { get; }
        double[] MonthlyRunoff { get; }
        double[] MonthlyActualET { get; }
        double[] MonthlyPotentialEvap { get; }
        double[] MonthlyPotentialTrans { get; }
        double CanopyLAImax { get; }
        double SiteMossDepth { get; }
        int AverageAge { get; }
        Library.Parameters.Species.AuxParm<int> CohortCountPerSpecies { get; }
        Library.Parameters.Species.AuxParm<bool> SpeciesPresent { get; }
        IProbEstablishment ProbEstablishment { get; }
        Library.Parameters.Species.AuxParm<int> MaxFoliageYearPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> BiomassPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> AGBiomassPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> WoodBiomassPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> BGBiomassPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> FoliageBiomassPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> NSCPerSpecies { get; }
        Library.Parameters.Species.AuxParm<double> LAIPerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> WoodSenescencePerSpecies { get; }
        Library.Parameters.Species.AuxParm<int> FolSenescencePerSpecies { get; }
        Library.Parameters.Species.AuxParm<List<ushort>> CohortAges { get; }
        double BiomassSum { get; }
        double AGBiomassSum { get; }
        double WoodBiomassSum { get; }
        double WoodSenescenceSum { get; }
        double FolSenescenceSum { get; }
        int CohortCount { get; }
        double JulySubCanopyPAR { get; }
        double MaxSubCanopyPAR { get; }
        double LeafLitter { get; }
        double WoodyDebris { get; }
        int AgeMax { get; }
        double AvgSoilWaterContent { get; }
        double BGBiomassSum { get; }
        double FolBiomassSum { get; }
        double NSCSum { get; }
        double ActualETSum { get; } //mm
        double NetPsnSum { get; }
        double PotentialET { get; }

        List<ISpecies> SpeciesEstByPlanting { get; set; }
        List<ISpecies> SpeciesEstBySerotiny { get; set; }
        List<ISpecies> SpeciesEstByResprout { get; set; }
        List<ISpecies> SpeciesEstBySeeding { get; set; }
        List<int> CohortsDiedBySuccession { get; set; }
        List<int> CohortsDiedByCold { get; set; }
        List<int> CohortsDiedByHarvest { get; set; }
        List<int> CohortsDiedByFire { get; set; }
        List<int> CohortsDiedByWind { get; set; }
        List<int> CohortsDiedByOther { get; set; }
    }
}
