This version of Landis-II runs on Linux using a Docker container, for which there are instructions below.
I (MGarcia) modified the ForC-Succession Extension (now v4.1). Most of the mods involved splitting files 
with multiple classes and interfaces to their components, and some changes in variable and class names 
for clarity and to make them more explanatory.

These test files were provided in the Extension-ForC-Succession package on GitHub at https://github.com/
LANDIS-II-Foundation/Extension-ForCS-Succession/tree/2e5d4ee00821d8047e5cb962aac3712227154727/testing/v8%20Scenario.
I changed input file names and some of their contents as listed below.

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

File name changes in this test suite included:

scenario.txt --> ForC-Scenario.txt (see also below for internal changes)

==============================

The following file name changes have updated listings in ForC-Scenario.txt:

species.txt --> ForC-Species.txt
ecoregions.gis --> ForC-Ecoregions.gis
ecoregions.txt --> ForC-Ecoregions.txt
FORC-successionv4simpleroots.txt --> ForC-Succession_v4_simpleroots.txt (see also below for internal changes)

==============================

In ForC-Succession_v4_simpleroots.txt, note that 
    NOTE: the extension name "ForC Succession" is now "ForC-Succession"

The following file name changes have updated listings:

ForCSClimateInputv3.0 no CC.txt --> ForC-ClimateInput.txt
    NOTE: the extension name "ForC Succession" is now "ForC-Succession"
initial-communitiesv3.0generallyold.csv --> ForC-InitialCommunities.csv
    NOTE: the related but unused file initial-communitiesv3.0generallyold.txt --> ForC-InitialCommunities.txt
initial-communities.gis --> ForC-InitialCommunities.gis
ForCS_DM.txt --> ForC-DisturbanceMatrix.txt
    NOTE: the extension name "ForC Succession" is now "ForC-Succession"
EcoSppDOMParameters.csv --> ForC-DOMParameters.csv
ANPPTimeSeries.csv --> ForC-ANPPTimeSeries.csv
MaxBiomassTimeSeries.csv --> ForC-MaxBiomassTimeSeries.csv
EstablishProbabilities.csv --> ForC-ProbEstablishment.csv

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

My updates to the ForC-Succession Extension did not require any changes to input files.

Future renaming of variables in the ForC-Succession code may require such modifcations.

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

Clément Hardy introduced me/us to the Docker approach to compiling Landis-II for Linux. There is a 
Dockerfile included here for that purpose, trimmed to include only the ForC-Succession extension used
in this test suite. A separate Dockerfile that builds plenty of other Landis-II extensions is available
from me or Clément.

I work on a Mac, so I use the Terminal, which is Linux. I have Docker Desktop v4.47 installed and running.
I also have Clément's helpful tools in the "files_to_help_compilation" directory. In the present directory, 
to build the Docker container

<path>/TestSim$ docker build -t landis-ii_forc-succession_linux .

Once the Docker image is built successfully, you can see that it exists with

<path>/TestSim$ docker images

To start the image in a container

<path>/TestSim$ bash ForC-TestContainer.sh

This will run the container and put me in its top directory, so

root@[hexcode]:/# cd TestSim

Then, to run the test simulation,

root@[hexcode]:/# bash ForC-TestRun.sh

The model run will create a "Landis-log.txt" file and two new folders, "Metadata" and "output"
