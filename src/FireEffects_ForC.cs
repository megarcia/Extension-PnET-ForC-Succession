// Authors: Caren Dymond, Sarah Beukema

// NOTE: ICohort --> Landis.Library.UniversalCohorts

using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Succession.PnETForC
{
    public class FireEffects
    {
        /// <summary>
        /// Crown scorching is when a cohort loses its foliage to fire  
        /// but is not killed. If cohort species is capable of epicormic 
        /// sprouting, returns the likelihood of sprouting after the fire 
        /// based on fire severity and remaining species longevity. 
        /// </summary>
        /// <param name="cohort"></param>
        /// <param name="fireSeverity"></param>
        /// <returns></returns>
        public static double CrownScorching(ICohort cohort, byte fireSeverity)
        {
            int diff = (int)fireSeverity - SpeciesData.FireTolerance[cohort.Species];
            double ageFrac = 1.0 - ((double)cohort.Data.Age / (double)cohort.Species.Longevity);
            if (SpeciesData.Epicormic[cohort.Species])
            {
                if (diff < 0)  // fire severity < fire tolerance
                    return 0.5 * ageFrac;
                if (diff == 0)  // fire severity = fire tolerance
                    return 0.75 * ageFrac;
                if (diff > 0)  // fire severity > fire tolerance
                    return 1.0 * ageFrac;
            }
            return 0.0;
        }
    }
}
