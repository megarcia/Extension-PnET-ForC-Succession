using System.Collections.Generic;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// PnET's Cohort Interface
    /// </summary>
    public interface ICohort : Library.UniversalCohorts.ICohort
    {
        new CohortData Data { get; }  // also declared in UniversalCohorts.ICohort
        byte SuccessionTimestep { get; }
        int AGBiomass { get; }
        int TotalBiomass { get; }
        bool IsLeafOn { get; }
        double MaxBiomass { get; }
        double Fol { get; }
        double MaxFolYear { get; }
        double NSC { get; }
        double DefoliationFrac { get; }
        double LastWoodSenescence { get; }
        double LastFolSenescence { get; }
        double LastFRad { get; }
        List<double> LastSeasonFRad { get; }
        double adjFolBiomassFrac { get; }
        double AdjHalfSat { get; }
        double adjFolN { get; }
        int ColdKill { get; }
        byte Layer { get; }
        double[] LAI { get; }
        double LastLAI { get; }
        double LastAGBio { get; }
        double[] GrossPsn { get; }
        double[] FoliarRespiration { get; }
        double[] NetPsn { get; }
        double[] MaintenanceRespiration { get; }
        double[] Transpiration { get; }
        double[] PotentialTranspiration { get; }
        double[] FRad { get; }
        double[] FWater { get; }
        double[] SoilWaterContent { get; }
        double[] PressHead { get; }
        int[] NumPrecipEvents { get; }
        double[] FOzone { get; }
        double[] Interception { get; }
        double[] AdjFolN { get; }
        double[] AdjFolBiomassFrac { get; }
        double[] CiModifier { get; }
        double[] DelAmax { get; }
        double BiomassLayerFrac { get; }
        double CanopyLayerFrac { get; }
        double CanopyGrowingSpace { get; }
    }
}
