
namespace Landis.Extension.Succession.PnETForC
{
    public class PnETSpeciesVars
    {
        /// <summary>
        /// Unitless respiration adjustment based on temperature: for output only
        /// </summary>
        public double RespirationFTemp; 

        /// <summary>
        /// Scaling factor of respiration given day and night temperature and day and night length
        /// </summary>
        public double MaintenanceRespirationFTemp; 

        /// <summary>
        /// Respiration Q10 factor
        /// </summary>
        public double RespirationFQ10;  

        /// <summary>
        /// Base foliar respiration fraction (using Wythers when selected)
        /// </summary>
        public double BaseFoliarRespirationFrac; 

        /// <summary>
        /// Photosynthesis reduction factor due to temperature: for output only
        /// </summary>
        public double PsnFTemp; 

        /// <summary>
        /// Adjustment to Amax based on CO2: for output only
        /// </summary>
        public double DelAmax;  

        public double JH2O;

        public double AmaxB_CO2;

        /// <summary>
        /// Gradient of effect of vapor pressure deficit on growth
        /// </summary>
        public double DVPD; 
    }
}
