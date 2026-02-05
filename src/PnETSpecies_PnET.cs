// NOTE: ISpecies --> Landis.Core
// NOTE: PostFireRegeneration --> Landis.Core

using System;
using System.Collections.Generic;
using System.Linq;
using Landis.Core;
using Landis.Library.Parameters;

namespace Landis.Extension.Succession.PnETForC
{
    /// <summary>
    /// The information for a tree species (its index and parameters).
    /// </summary>
    public class PnETSpecies : IPnETSpecies
    {
        static List<Tuple<ISpecies, IPnETSpecies>> SpeciesCombinations;
        private double _halfSatFCO2;
        private double _cfracbiomass;
        private double _woodydebrisdecayrate;
        private double _nscfrac;
        private double _bgbiomassfrac;
        private double _agbiomassfrac;
        private double _folbiomassfrac;
        private double _liveWoodBiomassFrac;
        private double _photosynthesisfage;
        private double _h1;
        private double _h2;
        private double _h3;
        private double _h4;
        private double _slwdel;
        private double _slwmax;
        private double _folturnoverrate;
        private double _rootturnoverrate;
        private double _halfsat;
        private double _initialnsc;
        private double _k;
        private double _woodturnoverrate;
        private double _establishmentfrad;
        private double _establishmentfwater;
        private double _maxProbEstablishment;
        private double _follignin;
        private bool _preventestablishment;
        private double _psntopt;
        private double _q10;
        private double _psntmin;
        private double _psntmax;
        private double _dvpd1;
        private double _foln;
        private double _dvpd2;
        private double _amaxa;
        private double _amaxb;
        private double _amaxamod;
        private double _aMaxBFCO2;
        private double _maintresp;
        private double _baseFoliarRespiration;
        private string _ozoneSens;
        private double _coldTolerance;
        private int _initBiomass;
        private string name;
        private int index;        
        private int maxSproutAge;
        private int minSproutAge;
        private PostFireRegeneration postfireregeneration;
        private int maxSeedDist;
        private int effectiveSeedDist;
        private double vegReprodProb;
        private byte fireTolerance;
        private byte shadeTolerance;
        int maturity;
        int longevity;
        private double _folN_slope;
        private double _folN_intercept;
        private double _folBiomassFrac_slope;
        private double _folBiomassFrac_intercept;
        private double _o3Coeff;
        private double _leafOnMinT;
        private double _NSCreserve;
        private string _lifeform;
        private double _refoliationMinimumTrigger;
        private double _maxRefoliationFrac;
        private double _refoliationCost;
        private double _nonRefoliationCost;
        private double _maxLAI;
        private double _mossScalar;
        private static Library.Parameters.Species.AuxParm<double> halfSatFCO2;
        private static Library.Parameters.Species.AuxParm<double> nscfrac;
        private static Library.Parameters.Species.AuxParm<double> cfracbiomass;
        private static Library.Parameters.Species.AuxParm<double> woodydebrisdecayrate;
        private static Library.Parameters.Species.AuxParm<double> bgbiomassfrac;
        private static Library.Parameters.Species.AuxParm<double> folbiomassfrac;
        private static Library.Parameters.Species.AuxParm<double> liveWoodBiomassFrac;
        private static Library.Parameters.Species.AuxParm<double> photosynthesisfage;
        private static Library.Parameters.Species.AuxParm<double> h1;
        private static Library.Parameters.Species.AuxParm<double> h2;
        private static Library.Parameters.Species.AuxParm<double> h3;
        private static Library.Parameters.Species.AuxParm<double> h4;
        private static Library.Parameters.Species.AuxParm<double> slwdel;
        private static Library.Parameters.Species.AuxParm<double> slwmax;    
        private static Library.Parameters.Species.AuxParm<double> folturnoverrate;
        private static Library.Parameters.Species.AuxParm<double> halfsat;
        private static Library.Parameters.Species.AuxParm<double> rootturnoverrate;
        private static Library.Parameters.Species.AuxParm<double> initialnsc;
        private static Library.Parameters.Species.AuxParm<double> k;
        private static Library.Parameters.Species.AuxParm<double> woodturnoverrate;
        private static Library.Parameters.Species.AuxParm<double> establishmentfrad;
        private static Library.Parameters.Species.AuxParm<double> establishmentfwater;
        private static Library.Parameters.Species.AuxParm<double> maxProbEstablishment;
        private static Library.Parameters.Species.AuxParm<double> follignin;
        private static Library.Parameters.Species.AuxParm<bool> preventestablishment;
        private static Library.Parameters.Species.AuxParm<double> psntopt;
        private static Library.Parameters.Species.AuxParm<double> q10;
        private static Library.Parameters.Species.AuxParm<double> psntmin;
        private static Library.Parameters.Species.AuxParm<double> psntmax;
        private static Library.Parameters.Species.AuxParm<double> dvpd1;
        private static Library.Parameters.Species.AuxParm<double> dvpd2;
        private static Library.Parameters.Species.AuxParm<double> foln;
        private static Library.Parameters.Species.AuxParm<double> amaxa;
        private static Library.Parameters.Species.AuxParm<double> amaxb;
        private static Library.Parameters.Species.AuxParm<double> amaxamod;
        private static Library.Parameters.Species.AuxParm<double> aMaxBFCO2;
        private static Library.Parameters.Species.AuxParm<double> maintresp;
        private static Library.Parameters.Species.AuxParm<double> baseFoliarRespiration;
        private static Library.Parameters.Species.AuxParm<double> coldTolerance;
        private static Library.Parameters.Species.AuxParm<string> ozoneSens;
        private static Library.Parameters.Species.AuxParm<double> folN_slope;
        private static Library.Parameters.Species.AuxParm<double> folN_intercept;
        private static Library.Parameters.Species.AuxParm<double> folBiomassFrac_slope;
        private static Library.Parameters.Species.AuxParm<double> folBiomassFrac_intercept;
        private static Library.Parameters.Species.AuxParm<double> o3Coeff;
        private static Library.Parameters.Species.AuxParm<double> leafOnMinT;
        private static Library.Parameters.Species.AuxParm<double> NSCreserve;
        private static Library.Parameters.Species.AuxParm<string> lifeform;
        private static Library.Parameters.Species.AuxParm<double> refoliationMinimumTrigger;
        private static Library.Parameters.Species.AuxParm<double> maxRefoliationFrac;
        private static Library.Parameters.Species.AuxParm<double> refoliationCost;
        private static Library.Parameters.Species.AuxParm<double> nonRefoliationCost;
        private static Library.Parameters.Species.AuxParm<double> maxlai;
        private static Library.Parameters.Species.AuxParm<double> mossScalar;
        private static Dictionary<ISpecies,double> maxLAI;
        private static Dictionary<ISpecies, string> lifeForm;

        public PnETSpecies()
        {
            halfSatFCO2 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("HalfSatFCO2");
            nscfrac = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("NSCFrac");
            cfracbiomass = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("CFracBiomass");
            woodydebrisdecayrate = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("woodydebrisdecayrate");
            bgbiomassfrac = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("bgbiomassfrac");
            /// agbiomassfrac = (Library.Parameters.Species.AuxParm<double>)(1.0F - BGBiomassFrac);
            folbiomassfrac = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("folbiomassfrac");
            liveWoodBiomassFrac = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("liveWoodBiomassFrac");
            photosynthesisfage = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("photosynthesisfage");
            h1 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("h1");
            h2 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("h2");
            h3 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("h3");
            h4 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("h4");
            slwdel = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("slwdel");
            slwmax = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("slwmax");
            folturnoverrate = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("folturnoverrate");
            halfsat = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("halfsat");
            rootturnoverrate = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("rootturnoverrate");
            initialnsc = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("initialnsc"); ;
            k = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("k"); ;
            woodturnoverrate = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("woodturnoverrate"); ;
            establishmentfrad = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("establishmentfrad"); ;
            establishmentfwater = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("establishmentfwater");
            maxProbEstablishment = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("MaxProbEstablishment");
            follignin = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("follignin");
            preventestablishment = (Library.Parameters.Species.AuxParm<bool>)(Parameter<bool>)Names.GetParameter("preventestablishment");
            psntopt = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("psntopt");
            q10 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("q10");
            psntmin = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("psntmin");
            psntmax = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("psntmax");
            dvpd1 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("dvpd1");
            dvpd2 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("dvpd2");
            foln = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("foln");
            amaxa = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("amaxa");
            amaxb = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("amaxb");
            amaxamod = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("AmaxAmod");
            aMaxBFCO2 = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("AMaxBFCO2");
            maintresp = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("maintresp");
            baseFoliarRespiration = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("baseFoliarRespiration");
            ozoneSens = (Library.Parameters.Species.AuxParm<string>)Names.GetParameter("StomataO3Sensitivity");
            folN_slope = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("FolN_slope");
            folN_intercept = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("FolN_intercept"); //Optional
            // If FolN_intercept is not provided, then set to foln
            if (folN_intercept[this] == -9999F)
                folN_intercept = foln;
            folBiomassFrac_slope = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("FolBiomassFrac_slope");
            folBiomassFrac_intercept = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("FolBiomassFrac_intercept"); //Optional
            // If FolBiomassFrac_intercept is not provided, then set to folbiomassfrac
            if (folBiomassFrac_intercept[this] == -9999F)
                folBiomassFrac_intercept = folbiomassfrac;
            o3Coeff = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("FOzone_slope");
            coldTolerance = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("ColdTolerance");
            leafOnMinT = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("LeafOnMinT"); //Optional
            // If LeafOnMinT is not provided, then set to PsnMinT
            if (leafOnMinT[this] == -9999F)
                leafOnMinT = psntmin;
            NSCreserve = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("NSCReserve");
            refoliationMinimumTrigger = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("RefolMinimumTrigger");
            maxRefoliationFrac = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("RefolMaximum");
            refoliationCost = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("RefolCost");
            nonRefoliationCost = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("NonRefolCost");
            maxlai = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("MaxLAI"); //Optional
            mossScalar = (Library.Parameters.Species.AuxParm<double>)(Parameter<double>)Names.GetParameter("MossScalar"); //Optional
            maxLAI = new Dictionary<ISpecies, double>();
            foreach (ISpecies species in Globals.ModelCore.Species)
            {
                if (maxlai[species] == -9999F)
                {
                    // Calculate MaxLAI
                    double peakBiomass = 1f / liveWoodBiomassFrac[species];
                    double peakFoliage = peakBiomass * folbiomassfrac[species] * (double)Math.Exp(-1f * liveWoodBiomassFrac[species] * peakBiomass);
                    double tempLAI = 0;
                    for (int i = 0; i < Globals.IMAX; i++)
                        tempLAI += (double)Math.Max(0.01, peakFoliage / Globals.IMAX / (slwmax[species] - (slwdel[species] * i * (peakFoliage / Globals.IMAX))));
                    maxLAI.Add(species, tempLAI);
                }
                else
                    maxLAI.Add(species, maxlai[species]);
            }
            lifeform = (Library.Parameters.Species.AuxParm<string>)(Parameter<string>)Names.GetParameter("Lifeform");
            lifeForm = new Dictionary<ISpecies, string>();
            foreach (ISpecies species in Globals.ModelCore.Species)
            {
                if (lifeform != null && lifeform[species] != null && !string.IsNullOrEmpty(lifeform[species]))
                {
                    string[] matches = new string[2];
                    if (Names.HasMultipleMatches(lifeform[species], ref matches))
                        throw new Exception("LifeForm parameter " + lifeForm + " contains mutually exclusive terms: " + matches[0] + " and " + matches[1] + ".");
                    lifeForm.Add(species, lifeform[species]);
                }
                else
                    lifeForm.Add(species, "tree");
            }
            SpeciesCombinations = new List<Tuple<ISpecies, IPnETSpecies>>();
            foreach (ISpecies species in Globals.ModelCore.Species)
            {
                PnETSpecies pnetspecies = new PnETSpecies(species);
                SpeciesCombinations.Add(new Tuple<ISpecies, IPnETSpecies>(species, pnetspecies));
            }
        }

        PnETSpecies(PostFireRegeneration postFireRegeneration,
                    double nscfrac, double cfracbiomass, double woodydebrisdecayrate,
                    double bgbiomassfrac, double folbiomassfrac, double liveWoodBiomassFrac,
                    double photosynthesisfage, double h1, double h2, double h3,
                    double h4, double slwdel, double slwmax, double folturnoverrate,
                    double rootturnoverrate, double halfsat, double initialnsc,
                    double k, double woodturnoverrate, double establishmentfrad, double establishmentfwater,
                    double maxprobestablishment, double follignin, bool preventestablishment,
                    double psntopt, double q10, double psntmin, double psntmax,
                    double dvpd1, double dvpd2, double foln, double amaxa,
                    double amaxb, double amaxamod, double aMaxBFCO2,
                    double maintresp, double baseFoliarRespiration, double coldTolerance,
                    string ozoneSens, int Index, string name,
                    int maxSproutAge, int minSproutAge, int maxSeedDist,
                    int effectiveSeedDist, double vegReprodProb,
                    byte fireTolerance, byte shadeTolerance, int maturity,
                    int longevity, double folN_slope, double folN_intercept,
                    double folBiomassFrac_slope, double folBiomassFrac_intercept, double o3Coeff,
                    double leafOnMinT, double NSCreserve, string lifeForm,
                    double refoliationMinimumTrigger, double maxRefoliationFrac,
                    double refoliationCost, double nonRefoliationCost,
                    double maxLAI)
        {
            double initBiomass = initialnsc / (nscfrac * cfracbiomass);
            _bgbiomassfrac = bgbiomassfrac;
            _agbiomassfrac = 1F - bgbiomassfrac;
            _initBiomass = (int)(initBiomass * (1F - (bgbiomassfrac * rootturnoverrate) - (_agbiomassfrac * woodturnoverrate)));
            _nscfrac = nscfrac;
            _cfracbiomass = cfracbiomass;
            _woodydebrisdecayrate = woodydebrisdecayrate;
            _folbiomassfrac = folbiomassfrac;
            _liveWoodBiomassFrac = liveWoodBiomassFrac;
            _photosynthesisfage = photosynthesisfage;
            _h1 = h1;
            _h2 = h2;
            _h3 = h3;
            _h4 = h4;
            _slwdel = slwdel;
            _slwmax = slwmax;
            _folturnoverrate = folturnoverrate;
            _rootturnoverrate = rootturnoverrate;
            _halfsat = halfsat;
            _initialnsc = initialnsc;
            _k = k;
            _woodturnoverrate = woodturnoverrate;
            _establishmentfrad = establishmentfrad;
            _establishmentfwater = establishmentfwater;
            _maxProbEstablishment = maxprobestablishment;
            _follignin = follignin;
            _preventestablishment = preventestablishment;
            _psntopt = psntopt;
            _q10 = q10;
            _psntmin = psntmin;
            _psntmax = psntmax;
            _dvpd1 = dvpd1;
            _foln = foln;
            _dvpd2 = dvpd2;
            _amaxa = amaxa;
            _amaxb = amaxb;
            _amaxamod = amaxamod;
            _aMaxBFCO2 = aMaxBFCO2;
            _maintresp = maintresp;
            _baseFoliarRespiration = baseFoliarRespiration;
            _coldTolerance = coldTolerance;
            _ozoneSens = ozoneSens;
            _folN_slope = folN_slope;
            _folN_intercept = folN_intercept;
            _folBiomassFrac_slope = folBiomassFrac_slope;
            _folBiomassFrac_intercept = folBiomassFrac_intercept;
            _o3Coeff = o3Coeff;
            _leafOnMinT = leafOnMinT;
            _NSCreserve = NSCreserve;
            _lifeform = lifeForm;
            _refoliationMinimumTrigger = refoliationMinimumTrigger;
            _maxRefoliationFrac = maxRefoliationFrac;
            _refoliationCost = refoliationCost;
            _nonRefoliationCost = nonRefoliationCost;
            _maxLAI = maxLAI;
            index = Index;
            postfireregeneration = postFireRegeneration;
            this.name = name;
            this.maxSproutAge = maxSproutAge;
            this.minSproutAge = minSproutAge;
            this.maxSeedDist = maxSeedDist;
            this.effectiveSeedDist = effectiveSeedDist;
            this.vegReprodProb = vegReprodProb;
            this.fireTolerance = fireTolerance;
            this.shadeTolerance = shadeTolerance;
            this.maturity = maturity;
            this.longevity = longevity;
        }

        private PnETSpecies(ISpecies species)
        {
            double initBiomass = initialnsc[species] / (nscfrac[species] * cfracbiomass[species]);
            _bgbiomassfrac = bgbiomassfrac[species];
            _agbiomassfrac = 1F - bgbiomassfrac[species];
            _initBiomass = (int)(initBiomass * (1F - (bgbiomassfrac[species] * rootturnoverrate[species]) - (_agbiomassfrac * woodturnoverrate[species])));
            _nscfrac = nscfrac[species];
            _cfracbiomass = cfracbiomass[species];
            _woodydebrisdecayrate = woodydebrisdecayrate[species];
            _folbiomassfrac = folbiomassfrac[species];
            _liveWoodBiomassFrac = liveWoodBiomassFrac[species];
            _photosynthesisfage = photosynthesisfage[species];
            _h1 = h1[species];
            _h2 = h2[species];
            _h3 = h3[species];
            _h4 = h4[species];
            _slwdel = slwdel[species];
            _slwmax = slwmax[species];
            _folturnoverrate = folturnoverrate[species];
            _rootturnoverrate = rootturnoverrate[species];
            _halfsat = halfsat[species];
            _initialnsc = initialnsc[species];
            _k = k[species];
            _woodturnoverrate = woodturnoverrate[species];
            _establishmentfrad = establishmentfrad[species];
            _establishmentfwater = establishmentfwater[species];
            _maxProbEstablishment = maxProbEstablishment[species];
            _follignin = follignin[species];
            _preventestablishment = preventestablishment[species];
            _psntopt = psntopt[species];
            _q10 = q10[species];
            _psntmin = psntmin[species];
            _psntmax = psntmax[species];
            _dvpd1 = dvpd1[species];
            _foln = foln[species];
            _dvpd2 = dvpd2[species];
            _amaxa = amaxa[species];
            _amaxb = amaxb[species];
            _amaxamod = amaxamod[species];
            _aMaxBFCO2 = aMaxBFCO2[species];
            _maintresp = maintresp[species];
            _baseFoliarRespiration = baseFoliarRespiration[species];
            _coldTolerance = coldTolerance[species];
            _halfSatFCO2 = halfSatFCO2[species];
            _ozoneSens = ozoneSens[species];
            _NSCreserve = NSCreserve[species];
            _lifeform = lifeForm[species];
            _refoliationMinimumTrigger = refoliationMinimumTrigger[species];
            _maxRefoliationFrac = maxRefoliationFrac[species];
            _refoliationCost = refoliationCost[species];
            _nonRefoliationCost = nonRefoliationCost[species];
            _maxLAI = maxLAI[species];
            _mossScalar = mossScalar[species];
            _folN_slope = folN_slope[species];
            _folN_intercept = folN_intercept[species];
            _folBiomassFrac_slope = folBiomassFrac_slope[species];
            _folBiomassFrac_intercept = folBiomassFrac_intercept[species];
            _o3Coeff = o3Coeff[species];
            _leafOnMinT = leafOnMinT[species];
            index = species.Index;
            name = species.Name;
            maxSproutAge = species.MaxSproutAge;
            minSproutAge = species.MinSproutAge;
            postfireregeneration = species.PostFireRegeneration;
            maxSeedDist = species.MaxSeedDist;
            effectiveSeedDist = species.EffectiveSeedDist;
            vegReprodProb = species.VegReprodProb;
            maturity = species.Maturity;
            longevity = species.Longevity;
        }

        public List<IPnETSpecies> AllSpecies
        {
            get
            {
                return SpeciesCombinations.Select(combination => combination.Item2).ToList();
            }
        }

        public IPnETSpecies this[ISpecies species]
        {
            get
            {
                return SpeciesCombinations.Where(spc => spc.Item1 == species).First().Item2;
            }
        }

        public ISpecies this[IPnETSpecies species]
        {
            get
            {
                return SpeciesCombinations.Where(spc => spc.Item2 == species).First().Item1;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public double BaseFoliarRespiration
        {
            get
            {
                return _baseFoliarRespiration;
            }
        }

        public double ColdTolerance
        {
            get
            {
                return _coldTolerance;
            }
        }

        public double AmaxA
        {
            get
            {
                return _amaxa;
            }
        }

        public double AmaxB
        {
            get
            {
                return _amaxb;
            }
        }

        public double AmaxAmod
        {
            get
            {
                return _amaxamod;
            }
        }

        public double AMaxBFCO2
        {
            get
            {
                return _aMaxBFCO2;
            }
        }

        public double MaintResp
        {
            get
            {
                return _maintresp;
            }
        }

        public double PsnTmin
        {
            get
            {
                return _psntmin;
            }
        }

        public double PsnTmax
        {
            get
            {
                if (_psntmax == -9999)
                    return _psntopt + (_psntopt - _psntmin);
                else
                    return _psntmax;
            }
        }

        public double DVPD1
        {
            get
            {
                return _dvpd1;
            }
        }

        public double FolN
        {
            get
            {
                return _foln;
            }
        }

        public double DVPD2
        {
            get
            {
                return _dvpd2;
            }

        }

        public double PsnTopt
        {
            get
            {
                return _psntopt;
            }
        }

        public double Q10
        {
            get
            {
                return _q10;
            }
        }

        public double EstablishmentFRad
        {
            get
            {
                return _establishmentfrad;
            }
        }

        public bool PreventEstablishment
        {
            get
            {
                return _preventestablishment;
            }
        }

        public double FolLignin
        {
            get
            {
                return _follignin;
            }
        }

        public double EstablishmentFWater
        {
            get
            {
                return _establishmentfwater;
            }
        }

        public double MaxProbEstablishment
        {
            get
            {
                return _maxProbEstablishment;
            }
        }

        public double WoodTurnoverRate
        {
            get
            {
                return _woodturnoverrate;
            }
        }

        public double K
        {
            get
            {
                return _k;
            }
        }

        public double InitialNSC
        {
            get
            {
                return _initialnsc;
            }
        }

        public double HalfSat
        {
            get
            {
                return _halfsat;
            }
        }

        public double RootTurnoverRate
        {
            get
            {
                return _rootturnoverrate;
            }
        }

        public double FolTurnoverRate
        {
            get
            {
                return _folturnoverrate;
            }
        }

        public double SLWDel
        {
            get
            {
                return _slwdel;
            }
        }

        public double SLWmax
        {
            get
            {
                return _slwmax;
            }
        }

        public double H4
        {
            get
            {
                return _h4;
            }
        }

        public double H3
        {
            get
            {
                return _h3;
            }
        }

        public double H2
        {
            get
            {
                return _h2;
            }
        }

        public double H1
        {
            get
            {
                return _h1;
            }
        }

        public double PhotosynthesisFAge
        {
            get
            {
                return _photosynthesisfage;
            }
        }

        public double WoodyDebrisDecayRate
        {
            get
            {
                return _woodydebrisdecayrate;
            }
        }

        public double LiveWoodBiomassFrac
        {
            get
            {
                return _liveWoodBiomassFrac;
            }
        }

        public double FolBiomassFrac
        {
            get
            {
                return _folbiomassfrac;
            }
        }

        public double BGBiomassFrac
        {
            get
            {
                return _bgbiomassfrac;
            }
        }

        public double AGBiomassFrac
        {
            get
            {
                return _agbiomassfrac;
            }
        }

        public double NSCFrac
        {
            get
            {
                return _nscfrac;
            }
        }

        public int InitBiomass
        {
            get
            {
                return _initBiomass;
            }
        }

        public double CFracBiomass
        {
            get
            {
                return _cfracbiomass;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
        public int MaxSproutAge
        {
            get
            {
                return maxSproutAge;
            }
        }

        public int MinSproutAge
        {
            get
            {
                return minSproutAge;
            }
        }

        public double HalfSatFCO2
        {
            get
            {
                return _halfSatFCO2;
            }
        }

        public PostFireRegeneration PostFireRegeneration
        {
            get
            {
                return postfireregeneration;
            }
        }

        public int MaxSeedDist
        {
            get
            {
                return maxSeedDist;
            }
        }

        public int EffectiveSeedDist
        {
            get
            {
                return effectiveSeedDist;
            }
        }

        public double VegReprodProb
        {
            get
            {
                return vegReprodProb;
            }
        }

        public byte FireTolerance
        {
            get
            {
                return fireTolerance;
            }
        }

        public byte ShadeTolerance
        {
            get
            {
                return shadeTolerance;
            }
        }

        public int Maturity
        {
            get
            {
                return maturity;
            }
        }

        public int Longevity
        {
            get
            {
                return longevity;
            }
        }

        public string StomataO3Sensitivity
        {
            get
            {
                return _ozoneSens;
            }
        }

        public double FolN_slope
        {
            get
            {
                return _folN_slope;
            }
        }

        public double FolN_intercept
        {
            get
            {
                return _folN_intercept;
            }
        }

        public double FolBiomassFrac_slope
        {
            get
            {
                return _folBiomassFrac_slope;
            }
        }

        public double FolBiomassFrac_intercept
        {
            get
            {
                return _folBiomassFrac_intercept;
            }
        }

        public double FOzone_slope
        {
            get
            {
                return _o3Coeff;
            }
        }

        public double LeafOnMinT
        {
            get
            {
                return _leafOnMinT;
            }
        }

        public double NSCReserve
        {
            get
            {
                return _NSCreserve;
            }
        }

        public string Lifeform
        {
            get
            {
                return _lifeform;
            }
        }

        public double RefoliationMinimumTrigger
        {
            get
            {
                return _refoliationMinimumTrigger;
            }
        }

        public double MaxRefoliationFrac
        {
            get
            {
                return _maxRefoliationFrac;
            }
        }

        public double RefoliationCost
        {
            get
            {
                return _refoliationCost;
            }
        }

        public double NonRefoliationCost
        {
            get
            {
                return _nonRefoliationCost;
            }
        }

        public double MaxLAI
        {
            get
            {
                return _maxLAI;
            }
        }

        public double MossScalar
        {
            get
            {
                // If mossScalar not provided, set to zero
                if (mossScalar[this] == -9999)
                    return 0;
                return _mossScalar;
            }
        }

        public static List<string> ParameterNames
        {
            get
            {
                Type type = typeof(PnETSpecies); // Get type pointer
                List<string> names = type.GetProperties().Select(x => x.Name).ToList(); // Obtain all fields
                return names;
            }
        }

        public string FullName
        {
            get;
            set;
        }
    }
}
