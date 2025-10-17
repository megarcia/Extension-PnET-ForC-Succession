This version of Landis-II runs on Linux using a Docker container, for which there are instructions below.
I (MGarcia) modified the PnET-Cohort Library (now v3), the PnET-Succession Extension (now v6.1), and the 
Output-Biomass-PnET Extension. Most of the mods involved changes in variable and class names for clarity 
and to make them more explanatory. Some mods separated functions and methods to new classes for easier 
maintenance and to reduce significant renundancy in the code. There were many changes to file names. 

These test files were obtained by Clément Hardy (Klemet) from https://github.com/LANDIS-II-Foundation/
Extension-PnET-Succession/tree/13ffece64f0b0180490aaa96cf91bd212e9e2e50/test/One_Cell. He modified them 
slightly in some places for compatibility with Linux. I changed file names and some of their contents as
listed below.

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

File name changes in this test suite included:


scenario.txt --> PnET-Scenario.txt (see also below for internal changes)

==============================

The following file name changes have updated listings in PnET-Scenario.txt:

ecoregion.img --> PnET-Ecoregion.img
ecoregion.txt --> PnET-Ecoregion.txt
pnetsuccession.txt --> PnET-Succession.txt (see also below for internal changes)
species.txt --> PnET-Species.txt
biomass.outputPnET.txt --> PnET-BiomassOutput.txt

==============================

The following file name changes have updated listings in PnET-Succession.txt:

PnEToutputsites_onecell.txt --> PnET-OutputSites_OneCell.txt 
    NOTE: keyword PNEToutputsites --> PnETOutputSites
initial-communities.csv --> PnET-InitialCommunities.csv
initial-communities.img --> PnET-InitialCommunities.img
SpeciesParameters.txt --> PnET-SpeciesParameters.txt
EcoregionParameters.txt --> PnET-EcoregionParameters.txt (see also below for internal changes)
PnETGenericParameters.txt --> PnET-GenericParameters.txt
disturbance_reductions.txt --> PnET-DisturbanceReductions.txt

==============================

The following file name changes have updated listings in PnET-EcoregionParameters.txt:

climate.txt --> PnET-Climate.txt

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

My updates to the PnET-Cohort Library required the following changes in several input files:

==============================

In PnET-SpeciesParameters.txt, the following variable names:

FracBelowG --> BGBiomassFrac
FracFol --> FolBiomassFrac
FrActWd --> LiveWoodBiomassFrac
PsnTMax --> PsnTmax
PsnTMin --> PsnTmin
PsnTOpt --> PsnTopt
TOfol --> FolTurnoverRate

==============================

In PnET-Climate.txt, the following header variable names:

TMax --> Tmax
TMin --> Tmin

==============================

In PnET-DisturbanceReductions.txt, the following variable names:

DeadWoodReduction --> WoodDebrisReduction
LitterReduction --> LeafLitterReduction

==============================

I copied the model file 
/usr/bin/LANDIS_Linux/Core-Model-v8-LINUX/build/Release/Defaults/PnETGenericDefaultParameters.txt
for modification. In this new PnETGenericDefaultParameters.txt, the following variable names: 

AmaxFrac --> AmaxAmod
BFolResp --> BaseFoliarRespiration
CO2AMaxBEff --> AMaxBFCO2
CO2HalfSatEff --> HalfSatFCO2
ColdTol --> ColdTolerance
DNSC --> NSCFrac
EstMoist --> EstablishmentFWater
EstRad --> EstablishmentFRad
FolNShape --> FolN_slope
FracBelowG --> BGBiomassFrac
FracFol --> FolBiomassFrac
FracFolShape --> FolBiomassFrac_slope
FrActWd --> LiveWoodBiomassFrac
KWdLit --> WoodDebrisDecompRate
MaxFolN --> FolN_intercept
MaxFracFol --> FolBiomassFrac_intercept
MaxPest --> MaxProbEstablishment
O3GrowthSens --> FOzone_slope
O3StomataSens --> StomataO3Sensitivity
PsnAgeRed --> PhotosynthesisFAge
PsnTMax --> PsnTmax
RETCropCoeff --> ReferenceETCropCoeff
TOroot --> RootTurnoverRate
TOwood --> WoodTurnoverRate

The parameters are also now alphabetized (accounting for the new names). If you want to change any names,
the accepted parameters names are listed in the PnET-Cohort Library files IPnetSpecies.cs and Names.cs. 
This updated Default Parameter file will be copied back to the location where the model expects to find it
in the test run described below.

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Clément introduced me/us to the Docker approach to compiling Landis-II for Linux. There is a Dockerfile
included here for that purpose, trimmed to include only those components and extensions used in this
test suite. A separate Dockerfile that builds plenty of other Landis-II extensions is available from
me or Clément.

I work on a Mac, so I use the Terminal, which is Linux. I have Docker Desktop v4.47 installed and running.
I also have Clément's helpful tools in the "files_to_help_compilation" directory. In the present directory, 
to build the Docker container

<path>/OneCellTestSim$ docker build -t landis-ii_pnet-succession_linux .

Once the Docker image is built successfully, you can see that it exists with

<path>/OneCellTestSim$ docker images

To start the image in a container

<path>/OneCellTestSim$ bash PnET-TestContainer.sh

This will run the container and put me in its top directory, so

root@[hexcode]:/# cd OneCellTestSim

Before you run the test simulation, you need to copy the new version of PnETGenericDefaultParameters.txt
back to its original location. That command is included in the script, so

root@[hexcode]:/# bash PnET-TestRun.sh

The model run will create a "Landis-log.txt" file and two new folders, "Metadata" and "output"
