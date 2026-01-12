
namespace Landis.Extension.Succession.PnETForC
{
    public interface IHydrology
    {
        /// <summary>
        /// volumetric soilWaterContent (mm/m)
        /// </summary>
        double SoilWaterContent { get; } 

        /// <summary>
        /// Get the pressurehead (mmH2O) for the current soil water content
        /// </summary>
        /// <param name="ecoregion"></param>
        /// <returns></returns>
        double GetPressureHead(IPnETEcoregionData ecoregion); 

        /// <summary>
        /// Add mm water to volumetric soil water content (mm/m) (considering activeSoilDepth - frozen soil cannot accept water)
        /// </summary>
        /// <param name="soilWaterContent"></param>
        /// <param name="activeSoilDepth"></param>
        /// <returns></returns>
        bool AddWater(double soilWaterContent, double activeSoilDepth); 

        double CalcEvaporation(IPnETEcoregionData Ecoregion, double potentialET);

        /// <summary>
        /// volumetric soil water content (mm/m) of the frozen soil
        /// </summary>
        double FrozenSoilWaterContent { get; } 

        /// <summary>
        /// Depth at which soil is frozen (mm); Rooting zone soil below this depth is frozen
        /// </summary>
        double FrozenSoilDepth { get; } 

        /// <summary>
        /// Change FrozenSoilWaterContent
        /// </summary>
        /// <param name="soilWaterContent"></param>
        /// <returns></returns>
        bool SetFrozenSoilWaterContent(double soilWaterContent);  

        /// <summary>
        /// Change FrozenSoilDepth
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        bool SetFrozenSoilDepth(double depth); 

        /// <summary>
        /// Get the PressureHeadTable object
        /// </summary>
        Hydrology_SaxtonRawls PressureHeadTable { get; } 
    }
}
