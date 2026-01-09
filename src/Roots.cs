// functional class Roots, from ForC
// --> functions AddCoarseRootLitter (no references)
//               AddFineRootLitter (no references)
//               CalcCoarseRoot (referenced only in AddCoarseRootLitter)
//               CalcFineRoot (referenced only in AddFineRootLitter)
//               CalcRootBiomass (currently used)
//               CalcRootTurnover (currently used)
//
// Original ForC authors: Caren Dymond, Sarah Beukema
// Additional analysis and modifications by Matthew Garcia
//
// NOTE: ActiveSite --> Landis.SpatialModeling
// NOTE: IEcoregion --> Landis.Core
// NOTE: ISpecies --> Landis.Core

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// Calculations of fine and coarse root biomass and turnover.
    /// </summary>
    public class Roots
    {
        public static double CoarseRootBiomass = 0.0;
        public static double FineRootBiomass = 0.0;
        public static double CoarseRootTurnover = 0.0;
        public static double FineRootTurnover = 0.0;

        /// <summary>
        /// Kills coarse roots and add the biomass directly to the SOC pool.
        /// </summary>
        public static void AddCoarseRootLitter(double agWoodBiomass,
                                               ISpecies species,
                                               ActiveSite site)
        {

            double coarseRootBiomass = CalcCoarseRoot(agWoodBiomass); // Ratio above to below
            if (coarseRootBiomass > 0)
                SiteVars.SoilOrganicMatterC[site] += coarseRootBiomass * 0.47;  // = convert to g C / m2
        }

        /// <summary>
        /// Kills fine roots and add the biomass directly to the SOC pool.
        /// </summary>
        public static void AddFineRootLitter(double agFoliarBiomass,
                                             ISpecies species,
                                             ActiveSite site)
        {
            double fineRootBiomass = CalcFineRoot(agFoliarBiomass);
            if (fineRootBiomass > 0)
                SiteVars.SoilOrganicMatterC[site] += fineRootBiomass * 0.47;  // = convert to g C / m2
        }

        /// <summary>
        /// Calculate coarse and fine roots based on total aboveground biomass.
        /// Niklas & Enquist 2002: 25% of total stocks
        /// </summary>
        public static double CalcCoarseRoot(double agBiomass)
        {
            return agBiomass * 0.24;
        }

        public static double CalcFineRoot(double agBiomass)
        {
            return agBiomass * 0.06;
        }

        /// <summary>
        /// Calculate coarse and fine root biomass based on AGBiomass,
        /// not static fractions of AGBiomass (as in Niklas and Enquist,
        /// 2002).
        /// </summary>
        public static double CalcRootBiomass(ActiveSite site, ISpecies species, double agBiomass)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            // NOTE: the loop control variable "i" refers to the points in the
            // parameter's growth curve, as specified by the user.
            int i;
            for (i = 0; i < 4; i++)
            {
                if (SpeciesData.MinWoodyBiomass[species][ecoregion][i + 1] > -999)
                {
                    if (agBiomass >= SpeciesData.MinWoodyBiomass[species][ecoregion][i] &&
                        agBiomass < SpeciesData.MinWoodyBiomass[species][ecoregion][i + 1])
                        break;
                }
                else
                    break;
            }
            double totalRootBiomass = agBiomass * SpeciesData.BGtoAGBiomassRatio[species][ecoregion][i];
            FineRootBiomass = totalRootBiomass * SpeciesData.FracFineRoots[species][ecoregion][i];
            CoarseRootBiomass = totalRootBiomass - FineRootBiomass;
            return totalRootBiomass;
        }

        public static void CalcRootTurnover(ActiveSite site, ISpecies species, double agBiomass)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            double totalRootBiomass = CalcRootBiomass(site, species, agBiomass);
            // NOTE: the loop control variable "i" refers to the points in the
            // parameter's growth curve, as specified by the user.
            int i;
            for (i = 0; i < 4; i++)
            {
                if (SpeciesData.MinWoodyBiomass[species][ecoregion][i + 1] > -999)
                {
                    if (agBiomass >= SpeciesData.MinWoodyBiomass[species][ecoregion][i] &&
                        agBiomass < SpeciesData.MinWoodyBiomass[species][ecoregion][i + 1])
                        break;
                }
                else
                    break;
            }
            CoarseRootTurnover = CoarseRootBiomass * SpeciesData.CoarseRootTurnoverRate[species][ecoregion][i];
            FineRootTurnover = FineRootBiomass * SpeciesData.FineRootTurnoverRate[species][ecoregion][i];
        }
    }
}
