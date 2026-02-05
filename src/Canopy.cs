// functional class Canopy
// --> functions CalcLAISum
//               CalcLAI
//               CalcCohortLAI

using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class Canopy
    {
        public static double CalcLAISum(int index,
                                        double[] LAI)
        {
            double LAISum = 0;
            if (LAI != null)
            {
                for (int i = 0; i < index; i++)
                    LAISum += LAI[i];
            }
            return LAISum;
        }

        public static double CalcLAI(IPnETSpecies species,
                                     double fol,
                                     int index,
                                     double LAISum)
        {
            // Leaf area index for a subcanopy layer, a function of 
            // specific leaf weight SLWMAX and the depth of the canopy.
            // Canopy depth is expressed by the mass of foliage above 
            // this subcanopy layer (i.e., slwdel * index/imax * fol)
            // Total LAI is capped at 25; above that, additional 
            // sublayers get LAI of 0.01
            double LAIlayerMax = Math.Max(0.01, 25.0 - LAISum);
            double LAIlayer = 1.0 / Globals.IMAX * fol / (species.SLWmax - species.SLWDel * index * (1 / Globals.IMAX) * fol);
            if (fol > 0.0 && LAIlayer <= 0.0)
            {
                Globals.ModelCore.UI.WriteLine("\n Warning: LAI was calculated to be negative for " + species.Name + ". This could be caused by a low value for SLWmax.  LAI applied in this case is a max of 25 for each cohort.");
                LAIlayer = LAIlayerMax / (Globals.IMAX - index);
            }
            else
                LAIlayer = Math.Min(LAIlayerMax, LAIlayer);
            return LAIlayer;
        }

        public static double CalcCohortLAI(IPnETSpecies species,
                                           double fol)
        {
            double CohortLAI = 0;
            for (int i = 0; i < Globals.IMAX; i++)
                CohortLAI += CalcLAI(species, fol, i, CohortLAI);
            return CohortLAI;
        }
    }
}
