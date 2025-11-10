// Authors: Caren Dymond, Sarah Beukema

// NOTE: ActiveSite --> Landis.SpatialModeling
// NOTE: IEcoregion --> Landis.Core
// NOTE: ISpecies --> Landis.Core

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// Fine and coarse roots.
    /// </summary>
    public class Roots
    {
        public static double CoarseRoot = 0.0;
        public static double FineRoot = 0.0;
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
        /// New ways of doing the roots (Nov 2011):
        /// Calculate coarse and fine roots based on woody biomass.
        /// These are no longer straight percentages.
        /// </summary>
        public static double CalcRootBiomass(ActiveSite site, ISpecies species, double agBiomass)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            int i;
            for (i = 0; i < 4; i++)
            {
                if (SpeciesData.MinWoodyBio[species][ecoregion][i + 1] > -999)
                {
                    if (agBiomass >= SpeciesData.MinWoodyBio[species][ecoregion][i] &&
                        agBiomass < SpeciesData.MinWoodyBio[species][ecoregion][i + 1])
                        break;
                }
                else
                    break;
            }
            double totalRootBiomass = agBiomass * SpeciesData.Ratio[species][ecoregion][i];
            FineRoot = totalRootBiomass * SpeciesData.FracFine[species][ecoregion][i];
            CoarseRoot = totalRootBiomass - FineRoot;
            return totalRootBiomass;
        }

        public static void CalcRootTurnover(ActiveSite site, ISpecies species, double agBiomass)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            double totalRootBiomass = CalcRootBiomass(site, species, agBiomass);
            int i;
            for (i = 0; i < 4; i++)
            {
                if (SpeciesData.MinWoodyBio[species][ecoregion][i + 1] > -999)
                {
                    if (agBiomass >= SpeciesData.MinWoodyBio[species][ecoregion][i] &&
                        agBiomass < SpeciesData.MinWoodyBio[species][ecoregion][i + 1])
                        break;
                }
                else
                    break;
            }
            CoarseRootTurnover = CoarseRoot * SpeciesData.CoarseTurnover[species][ecoregion][i];
            FineRootTurnover = FineRoot * SpeciesData.FineTurnover[species][ecoregion][i];
        }
    }
}
