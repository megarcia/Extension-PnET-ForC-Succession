// interface IHydrology, from PnET

namespace Landis.Extension.Succession.PnETForC
{
    public interface IHydrology
    {
        /// <summary>
        /// volumetric soilWaterContent (mm/m)
        /// </summary>
        double SoilWaterContent { get; } 

        /// <summary>
        /// Depth at which soil is frozen (mm); Rooting zone soil
        /// below this depth is frozen
        /// </summary>
        double FrozenSoilDepth { get; } 

        /// <summary>
        /// volumetric soil water content (mm/m) of the frozen soil
        /// </summary>
        double FrozenSoilWaterContent { get; } 

        /// <summary>
        /// Get the pressure head (mmH2O) for current soil water
        /// content
        /// </summary>
        /// <param name="ecoregion"></param>
        /// <returns></returns>
        double GetPressureHead(IPnETEcoregionData ecoregion); 

        /// <summary>
        /// Add mm water to volumetric soil water content (mm/m)
        /// (considering activeSoilDepth - frozen soil cannot
        /// accept water)
        /// </summary>
        /// <param name="soilWaterContent"></param>
        /// <param name="activeSoilDepth"></param>
        /// <returns></returns>
        bool AddWater(double soilWaterContent, double activeSoilDepth); 

        /// <summary>
        /// Calculate soil surface evaporation
        /// </summary>
        /// <param name="ecoregion"></param>
        /// <param name="potentialET"></param>
        /// <returns></returns>
        double CalcEvaporation(IPnETEcoregionData ecoregion, double potentialET);

        /// <summary>
        /// Change depth of frozen soil
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        bool SetFrozenSoilDepth(double depth); 

        /// <summary>
        /// Change water content of frozen soil
        /// </summary>
        /// <param name="soilWaterContent"></param>
        /// <returns></returns>
        bool SetFrozenSoilWaterContent(double soilWaterContent);  

        /// <summary>
        /// Get the PressureHeadTable object
        /// </summary>
        Hydrology_SaxtonRawls PressureHeadTable { get; } 
    }
}
