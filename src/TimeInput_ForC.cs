namespace Landis.Extension.Succession.PnETForC
{
    public class TimeInput : ITimeInput
    {
        int m_nYear = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeInput()
        {
        }

        public TimeInput(int nYear)
        {
            Year = nYear;
        }

        public int Year
        {
            get
            {
                return m_nYear;
            }
            set
            {
                m_nYear = value;
            }
        }
    }
}
