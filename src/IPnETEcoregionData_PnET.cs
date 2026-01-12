// NOTE: IEcoregion --> Landis.Core

using Landis.Core;

namespace Landis.Extension.Succession.PnETForC
{
    public interface IPnETEcoregionData : IEcoregion
    {
        /// <summary>
        /// Ecoregion name
        /// </summary>
        new string Name { get; }  // also declared in IEcoregion : IEcoregionParameters

        /// <summary>
        /// Is active ecoregion
        /// </summary>
        new bool Active { get; }  // also declared in IEcoregion : IEcoregionParameters

        /// <summary>
        /// Fraction of water above field capacity that drains out of the soil rooting zone immediately after entering the soil (fast leakage)
        /// </summary>
        double LeakageFrac { get; }

        /// <summary>
        /// Depth of surface water (mm) that can be held on site instead of running off
        /// </summary>
        double RunoffCapture { get; }

        /// <summary>
        /// Fraction of incoming precipitation that does not enter the soil - surface runoff due to impermeability, slope, etc.
        /// </summary>
        double PrecLossFrac { get; }

        /// <summary>
        /// Ecoregion soil type descriptor 
        /// </summary>
        string SoilType { get; }

        /// <summary>
        /// Rate at which incoming precipitation is intercepted by foliage for each unit of LAI
        /// </summary>
        double PrecIntConst { get; }

        /// <summary>
        /// Depth of rooting zone in the soil (mm)
        /// </summary>
        double RootingDepth { get; }

        /// <summary>
        /// Volumetric soil water content (mm/m) at field capacity
        /// </summary>
        double FieldCapacity { get; set; }

        /// <summary>
        /// Volumetric soil water content (mm/m) at wilting point
        /// </summary>
        double WiltingPoint { get; set; }

        /// <summary>
        /// Volumetric soil water content (mm/m) at porosity
        /// </summary>
        double Porosity { get; set; }

        /// <summary>
        /// Fraction of snow pack that sublimates before melting
        /// </summary>
        double SnowSublimFrac { get; }

        double LeakageFrostDepth { get; }

        int PrecipEvents { get; }

        /// <summary>
        /// Ecoregion latitude
        /// </summary>
        double Latitude { get; }

        double WinterSTD { get; }

        double MossDepth { get; }

        /// <summary>
        /// Maximum soil depth susceptible to surface evaporation
        /// </summary>
        double EvapDepth { get; }

        /// <summary>
        /// Tuning parameter to adjust frost depth
        /// </summary>
        double FrostFactor { get; }

        IPnETEcoregionVars Variables { get; set; }
    }
}
