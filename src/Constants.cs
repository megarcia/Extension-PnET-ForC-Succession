// merged PnET and ForC Constants

using System;

namespace Landis.Extension.Succession.PnETForC 
{
    public class Constants
    {
        /// <summary>
        /// gravity in m/s^2
        /// </summary>
        public const double gravity = 9.80665;

        /// <summary>
        /// Molecular weight of C in g/mol
        /// </summary>
        public const double MC = 12.0;

        /// <summary>
        /// Molecular weight of CO2 in g/mol
        /// </summary>
        public const double MCO2 = 44.0;

        /// <summary>
        /// Molecular weight of CO2 relative to C 
        /// </summary>
        public const double MCO2_MC = MCO2 / MC;

        /// <summary>
        /// Reference atmospheric concentration of CO2 in ppm
        /// </summary>
        public const double CO2RefConc = 350.0;

        /// <summary>
        /// Seconds per hour
        /// </summary>
        public const int SecondsPerHour = 60 * 60;

        /// <summary>
        /// Seconds per day
        /// </summary>
        public const int SecondsPerDay = SecondsPerHour * 24;

        /// <summary>
        /// Billion
        /// </summary>
        public const int billion = 1000000000;

        // Joules <--> calorie conversion
        public const double Jpercal = 4.184;
        public const double CalperJ = 1.0 / Jpercal;

        /// <summary>
        /// Universal gas constant in J/kmol.K
        /// </summary>
        public const double GasConst_JperkmolK = 8314.47;

        /// <summary>
        /// Reference temperature in K
        /// </summary>
        public const double Tref_K = 273.15;

        /// <summary>
        /// Reference pressure in kPa
        /// </summary>
        public const double Pref_kPa = 101.3;

        /// <summary>
        /// latent heat of vaporization for water in MJ/m3
        /// </summary>
        public const double LatentHeatVaporWater = 2453.0;

        /// <summary>
        /// heat capacity of solid portion of soil in kJ/m3.K 
        /// (Farouki, 1986, in vanLier and Durigon, 2013)
        /// </summary>
        public const double HeatCapacitySoil = 1942.0;

        /// <summary>
        /// heat capacity of water in kJ/m3.K
        /// (vanLier and Durigon, 2013)
        /// </summary>
        public const double HeatCapacityWater = 4186.0;

        /// <summary>
        /// heat capacity of snow in J/kg.K 
        /// (https://www.engineeringtoolbox.com/specific-heat-capacity-d_391.html)
        /// </summary>
        public const double HeatCapacitySnow_Jperkg = 2090.0;

        /// <summary>
        /// heat capacity of moss in kJ/m3.K 
        /// (Sazonova and Romanovsky, 2003)
        /// </summary>
        public const double HeatCapacityMoss = 2500.0;

        /// <summary>
        /// thermal conductivity of air in kJ/m.d.K 
        /// (vanLier and Durigon, 2013)
        /// </summary>
        public const double ThermalConductivityAir_kJperday = 2.25;

        /// <summary>
        /// thermal conductivity of air in W/m.K 
        /// (CLM5 documentation, Table 2.7)
        /// </summary>
        public const double ThermalConductivityAir_Watts = 0.023;

        /// <summary>
        /// thermal conductivity of water in kJ/m.d.K
        /// (vanLier and Durigon, 2013)
        /// </summary>
        public const double ThermalConductivityWater_kJperday = 51.51;

        /// <summary>
        /// thermal conductivity of water in W/m.K 
        /// (CLM5 documentation, Table 2.7)
        /// </summary>
        public const double ThermalConductivityWater_Watts = 0.57;

        /// <summary>
        /// thermal conductivity of ice in W/m.K 
        /// (CLM5 documentation, Table 2.7)
        /// </summary>
        public const double ThermalConductivityIce_Watts = 2.29;

        /// <summary>
        /// thermal conductivity of clay soil in kJ/m.d.K
        /// (Michot et al., 2008 in vanLier and Durigon, 2013)
        /// </summary>
        public const double ThermalConductivityClay = 80.0;

        /// <summary>
        /// thermal conductivity of sandstone in kJ/m.d.K 
        /// (Gemant, 1950, in vanLier and Durigon, 2013)
        /// </summary>
        public const double ThermalConductivitySandstone = 360.0;

        /// <summary>
        /// thermal conductivity of moss in kJ/m.d.K
        /// converted from 0.2 W/m.K (Sazonova and Romanovsky, 2003)
        /// </summary>
        public const double ThermalConductivityMoss = 432.0;

        /// <summary>
        /// unexplained coefficient in vanLier and Durigon (2013)
        /// (via Farouki, 1986)
        /// </summary>
        public const double gs = 0.125;

        /// <summary>
        /// angular velocity of Earth in radians/month
        /// </summary>
        public const double omega = Math.PI * 2.0 / 12.0;

        /// <summary>
        /// length of temp record in months
        /// </summary>
        public const double tau = 12.0;

        /// <summary>
        /// intercept of function for bulk density of snow in kg/m3
        /// </summary>
        public const double DensitySnow_intercept = 165.0;

        /// <summary>
        /// slope of function for bulk density of snow in kg/m3
        /// </summary>
        public const double DensitySnow_slope = 1.3;

        /// <summary>
        /// Density of water in kg/m3
        /// </summary>
        public const double DensityWater = 1000.0;

        /// <summary>
        /// minimum depth of snow (m) that counts as 
        /// full snow cover for albedo calculations
        /// </summary>
        public const double SnowReflectanceThreshold = 0.1;

        /// <summary>
        /// thermal diffusivity of moss
        /// </summary>
        public const double ThermalDiffusivityMoss = ThermalConductivityMoss / HeatCapacityMoss;

        /// <summary>
        /// Psychrometric coefficient in kPa/K
        /// (Cabrera et al., 2016, Table 1)
        /// </summary>
        public const double PsychrometricCoeff = 0.062;

        /// <summary>
        /// field capacity pore water (negative) pressure in kPa
        /// </summary>
        public const double FieldCapacity_kPa = -33.0;

        /// <summary>
        /// field capacity pore water (negative) pressure in mmH2O
        /// </summary>
        public const double FieldCapacity_mmH2O = FieldCapacity_kPa / gravity;

        /// <summary>
        /// wilting point pore water (negative) pressure in kPa
        /// </summary>
        public const double WiltingPoint_kPa = -1500.0;

        /// <summary>
        /// wilting point pore water (negative) pressure in mmH2O
        /// </summary>
        public const double WiltingPoint_mmH2O = WiltingPoint_kPa / gravity;

        /// <summary>
        /// convert kJ/m.d.K to W/m.K
        /// </summary>
        public const double Convert_kJperday_to_Watts = 0.2777777778 / 24.0;

        /// <summary>
        /// ForC Constants
        /// Shouldn't some of these be input variables instead? 
        /// </summary>
        public const int FIREINTENSITYCOUNT = 5;
        public const int NUMSNAGS = 1000;
        public const int NUMBIOMASSPOOLS = 6;  // BiomassPoolTypes.FINEROOT + 1, The total number of biomass components.
        public const double BIOMASS_TO_CMASS = 0.5;
        public const int NUMDOMPOOLS = 10; // DOMPoolTypes.SPARECPOOL + 1;
        public const int NUMSNAGPOOLS = 2; // Snags.SnagType.BRANCHSNAG + 1 i.e., stem and branches snag pool
        public const double FINEROOTSABOVERATIO = 0.5;
        public const double COARSEROOTABOVERATIO = 0.5;
        public const int NUMDISTURBANCES = 9;  // note, if adding more disturbances, then increase this
        public const double DECAYREFTEMP = 10.0;  // reference soil temperature for decay calculation; originally decayRefTemp from DOMDecay.CalcDecayFTemp
        public const int NUMSLOWPOOLS = 2;  // number of above- and below-ground slow soil carbon pool; originally numberABSlowPool from SoilC.DoSoilDynamics
        public const int AGSLOWPOOLIDX = 0;  // above-ground slow carbon pool; originally aboveSlowPool from SoilC.DoSoilDynamics
        public const int BGSLOWPOOLIDX = 1;  // below-ground slow soil carbon pool; originally belowSlowPool from SoilC.DoSoilDynamics
    }
}
