// NOTE: ISpecies --> Landis.Core

using Landis.Core;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// The information for a tree species (its index and parameters).
    /// </summary>
    public interface IPnETSpecies : ISpecies
    {
        /// <summary>
        /// Species name 
        /// </summary>
        new string Name { get; }  // also declared in ISpecies : ISpeciesParameters

        /// <summary>
        /// Carbon fraction in biomass 
        /// </summary>
        double CFracBiomass { get; }

        /// <summary>
        /// Fraction of non-soluble carbon to active biomass
        /// </summary>
        double NSCFrac { get; }

        /// <summary>
        /// Fraction of species biomass that is below ground
        /// </summary>
        double BGBiomassFrac { get; }

        /// <summary>
        /// Fraction of species biomass that is above ground
        /// </summary>
        double AGBiomassFrac { get; }

        /// <summary>
        /// Fraction foliage biomass of active biomass
        /// </summary>
        double FolBiomassFrac { get; }

        /// <summary>
        /// Fraction active wood biomass of total biomass 
        /// </summary>
        double LiveWoodBiomassFrac { get; }

        /// <summary>
        /// Water stress parameter for excess water: pressure head below which growth halts
        /// </summary>
        double H1 { get; }

        /// <summary>
        /// Water stress parameter for excess water: pressure head below which growth declines
        /// </summary>
        double H2 { get; }

        /// <summary>
        /// Water stress parameter for water shortage: pressure head above which growth declines
        /// </summary>
        double H3 { get; }

        /// <summary>
        /// Water stress parameter for water shortage: pressure head above growth halts (= wilting point)
        /// </summary>
        double H4 { get; }

        /// <summary>
        /// Initial NSC for new cohort
        /// </summary>
        double InitialNSC { get; }

        /// <summary>
        /// Half saturation value for radiation (W/m2)
        /// </summary>
        double HalfSat { get; }

        /// <summary>
        /// Radiation extinction rate through the canopy (LAI-1)
        /// </summary>
        double K { get; }

        /// <summary>
        /// Decomposition constant of wood litter (yr-1)
        /// </summary>
        double WoodyDebrisDecayRate { get; }

        /// <summary>
        /// Species longevity (yr)
        /// </summary>
        new int Longevity { get; }  // also declared in ISpecies : ISpeciesParameters

        /// <summary>
        /// Growth reduction parameter with age
        /// </summary>
        double PhotosynthesisFAge { get; }

        /// <summary>
        /// Reduction of specific leaf weight throughout the canopy (g/m2/g)
        /// </summary>
        double SLWDel { get; }

        /// <summary>
        /// Max specific leaf weight (g/m2)
        /// </summary>
        double SLWmax { get; }

        /// <summary>
        /// Foliage turnover (g/g/y)
        /// </summary>
        double FolTurnoverRate { get; }

        /// <summary>
        /// Root turnover (g/g/y)
        /// </summary>
        double RootTurnoverRate { get; }

        /// <summary>
        /// Wood turnover (g/g/y)
        /// </summary>
        double WoodTurnoverRate { get; }

        /// <summary>
        /// Establishment factor related to light - fRad value that equates to optimal light for establishment
        /// </summary>
        double EstablishmentFRad { get; }

        /// <summary>
        /// Establishment factor related to moisture - fWater value that equates to optimal water for establishment
        /// </summary>
        double EstablishmentFWater { get; }

        /// <summary>
        /// Mamximum total probability of establishment under optimal conditions
        /// </summary>
        double MaxProbEstablishment { get; }

        /// <summary>
        /// Lignin concentration in foliage
        /// </summary>
        double FolLignin { get; }

        /// <summary>
        /// Prevent establishment 
        /// </summary>
        bool PreventEstablishment { get; }

        /// <summary>
        /// Optimal temperature for photosynthesis
        /// </summary>
        double PsnTopt { get; }

        /// <summary>
        /// Temperature response factor for respiration
        /// </summary>
        double Q10 { get; }

        /// <summary>
        /// Base foliar respiration (g respired / g photosynthesis)
        /// </summary>
        double BaseFoliarRespiration { get; }

        /// <summary>
        /// Minimum temperature for photosynthesis
        /// </summary>
        double PsnTmin { get; }

        /// <summary>
        /// Maximum temperature for photosynthesis
        /// </summary>
        double PsnTmax { get; }

        /// <summary>
        /// Foliar ratio of nitrogen to carbon  (gN/gC)
        /// </summary>
        double FolN { get; }

        /// <summary>
        /// Vapor pressure deficit response parameter 
        /// </summary>
        double DVPD1 { get; }

        /// <summary>
        /// Vapor pressure deficit response parameter 
        /// </summary>
        double DVPD2 { get; }

        /// <summary>
        /// Reference photosynthesis (g)
        /// </summary>
        double AmaxA { get; }

        /// <summary>
        /// Response parameter for photosynthesis to N
        /// </summary>
        double AmaxB { get; }

        /// <summary>
        /// Modifier of AmaxA due to averaging non-linear Amax data
        /// </summary>
        double AmaxAmod { get; }

        /// <summary>
        /// Reference maintenance respiration 
        /// </summary>
        double MaintResp { get; }

        /// <summary>
        /// Effect of CO2 on AMaxB (change in AMaxB with increase of 200 ppm CO2)
        /// </summary>
        double AMaxBFCO2 { get; }

        /// <summary>
        /// Effect of CO2 on HalfSat (change in HalfSat with increase of 1 ppm CO2 [slope])
        /// </summary>
        double HalfSatFCO2 { get; }

        /// <summary>
        /// Stomatal ozone sensitivity class (Sensitive, Intermediate, Tolerant)
        /// </summary>
        string StomataO3Sensitivity { get; }

        /// <summary>
        /// Slope for linear FolN relationship
        /// </summary>
        double FolN_slope { get; }

        /// <summary>
        /// Intercept for linear FolN relationship
        /// </summary>
        double FolN_intercept { get; }

        /// <summary>
        /// Slope for linear FolBiomassFrac relationship
        /// </summary>
        double FolBiomassFrac_slope { get; }

        /// <summary>
        /// Intercept for linear FolBiomassFrac relationship
        /// </summary>
        double FolBiomassFrac_intercept { get; }

        /// <summary>
        /// Slope coefficient for FOzone
        /// </summary>
        double FOzone_slope { get; }

        /// <summary>
        /// Cold tolerance
        /// </summary>
        double ColdTolerance { get; }

        /// <summary>
        /// Mininum Temp for leaf-on (optional)
        /// If not provided, LeafOnMinT = PsnTmin
        /// </summary>
        double LeafOnMinT { get; }

        /// <summary>
        /// Initial Biomass
        /// </summary>
        int InitBiomass { get; }

        /// <summary>
        /// Lower canopy NSC reserve 
        /// </summary>
        double NSCReserve { get; }

        /// <summary>
        /// Lifeform
        /// </summary>
        string Lifeform { get; }

        /// <summary>
        /// Minimum defoliation amount that triggers refoliation
        /// </summary>
        double RefoliationMinimumTrigger { get; }

        /// <summary>
        /// Maximum amount of refoliation
        /// </summary>
        double MaxRefoliationFrac { get; }

        /// <summary>
        /// Cost of refoliation
        /// </summary>
        double RefoliationCost { get; }

        /// <summary>
        /// Cost to NSC without refoliation
        /// </summary>
        double NonRefoliationCost { get; }

        /// <summary>
        /// Maximum LAI
        /// </summary>
        double MaxLAI { get; }

        /// <summary>
        /// Scalar value for calculating species moss depth
        /// </summary>
        double MossScalar { get; }
    }
}
