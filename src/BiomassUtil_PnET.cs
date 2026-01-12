using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class BiomassUtil
    {
        private double[] biomassData;
        private int biomassNum;
        private double biomassThreshold;

        public double BiomassThreshold
        {
            get
            {
                return biomassThreshold;
            }
            set
            {
                biomassThreshold = value;
            }
        }

        public BiomassUtil()
        {
        }

        public double GetBiomassData(int i, int j)
        {
            if (i > biomassNum || j < 1 || j > 2)
                throw new Exception("index error at GetBiomass");
            return biomassData[(i - 1) * 2 + j - 1];
        }

        public void SetBiomassData(int i, int j, double value)
        {
            if (i > biomassNum || j < 1 || j > 2)
                throw new Exception("index error at SetBiomass");
            biomassData[(i - 1) * 2 + j - 1] = value;
        }

        public void SetBiomassNum(int num)
        {
            biomassNum = num;
            biomassData = null;
            biomassData = new double[num * 2];
        }
    }
}
