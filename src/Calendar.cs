// functional class Calendar, from PnET
// --> enum Months
// --> functions CalcDaySpan
//               CalcNightLength
//               CalcDayLength
//               CalcDaylightHrs

using System;

namespace Landis.Extension.Succession.PnETForC
{
    public class Calendar
    {
        public enum Months : int
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public static int CalcDaySpan(int Month)
        {
            switch (Month)
            {
                case 1: return 31;
                case 2: return 28;
                case 3: return 31;
                case 4: return 30;
                case 5: return 31;
                case 6: return 30;
                case 7: return 31;
                case 8: return 31;
                case 9: return 30;
                case 10: return 31;
                case 11: return 30;
                case 12: return 31;
                default: throw new Exception("Month " + Month + " is not an integer between 1-12. Error assigning DaySpan");
            };
        }

        /// <summary>
        /// Nightlength in seconds
        /// </summary>
        /// <param name="Hrs"></param>
        /// <returns></returns>
        public static double CalcNightLength(double Hrs)
        {
            return 60 * 60 * (24 - Hrs);
        }

        /// <summary>
        /// DayLength in seconds
        /// </summary>
        /// <param name="Hrs"></param>
        /// <returns></returns>
        public static double CalcDayLength(double Hrs)
        {
            return 60 * 60 * Hrs;
        }

        /// <summary>
        /// Calculate hours of daylight
        /// </summary>
        /// <param name="DOY"></param>
        /// <param name="Latitude"></param>
        /// <returns></returns>
        public static double CalcDaylightHrs(int DOY, double Latitude)
        {
            double TA;
            double AC;
            double LatRad;
            double r;
            double z;
            double decl;
            double z2;
            double h;
            LatRad = Latitude * (2.0 * Math.PI) / 360.0;
            r = 1.0 - (0.0167 * Math.Cos(0.017203 * (DOY - 3.0)));
            z = 0.39785 * Math.Sin(4.868961 + 0.017203 * DOY + 0.033446 * Math.Sin(6.224111 + 0.017202 * DOY));
            if (Math.Abs(z) < 0.7)
                decl = Math.Atan(z / Math.Sqrt(1.0 - z * z));
            else
                decl = Math.PI / 2.0 - Math.Atan(Math.Sqrt(1.0 - z * z) / z);
            if (Math.Abs(LatRad) >= Math.PI / 2.0)
            {
                if (Latitude < 0)
                    LatRad = -1.0 * (Math.PI / 2.0 - 0.01);
                else
                    LatRad = 1.0 * (Math.PI / 2.0 - 0.01);
            }
            z2 = -Math.Tan(decl) * Math.Tan(LatRad);
            if (z2 >= 1.0)
                h = 0.0;
            else if (z2 <= -1.0)
                h = Math.PI;
            else
            {
                TA = Math.Abs(z2);
                if (TA < 0.7)
                    AC = (Math.PI / 2.0) - Math.Atan(TA / Math.Sqrt(1.0 - TA * TA));
                else
                    AC = Math.Atan(Math.Sqrt(1.0 - TA * TA) / TA);
                if (z2 < 0)
                    h = Math.PI - AC;
                else
                    h = AC;
            }
            return 2.0 * h * 24.0 / (2.0 * Math.PI);
        }
    }
}
