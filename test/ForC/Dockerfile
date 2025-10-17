FROM ubuntu:22.04

################################################################
# PREPARATIONS
################################################################

# Set the DEBIAN_FRONTEND environment variable to noninteractive
# Avoid prompts when installing packages
ENV DEBIAN_FRONTEND=noninteractive

# PREPARING PACKAGES AND UPGRADING, AND INSTALLING DEPENDENCIES OF LANDIS-II
# apt-get clean and rm -rf /var/lib/apt/lists/* are used to clear the packages folder caches
# to avoid putting them in the docker layerin system, freeing space
RUN apt-get update -y && apt-get upgrade -y \
&& apt-get update && apt-get -y upgrade \
&& apt-get install -y wget vim pip nano git python3 python-is-python3 \
&& apt-get install -y libjpeg62 \
&& apt-get install -y libpng16-16 \
&& apt-get install -y gdal-bin \
&& apt-get install -y libgdal-dev \
# Installing libssl1 sometimes needed for Dotnet
&& wget http://archive.ubuntu.com/ubuntu/pool/main/o/openssl/libssl1.1_1.1.0g-2ubuntu4_amd64.deb \
&& dpkg -i libssl1.1_1.1.0g-2ubuntu4_amd64.deb \
&& apt-get clean && rm -rf /var/lib/apt/lists/*
ENV C_INCLUDE_PATH=/usr/include/gdal
ENV CPLUS_INCLUDE_PATH=/usr/include/gdal

# INSTALLING DOTNET SDK AND RUNTIME 8.0
RUN mkdir /bin/.dotnet/ && cd /bin/.dotnet/ \
&& wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
&& chmod +x ./dotnet-install.sh \
&& ./dotnet-install.sh --channel 8.0 -InstallDir /bin/.dotnet/  \
&& ./dotnet-install.sh --channel 8.0 --runtime aspnetcore -InstallDir /bin/.dotnet/ \
&& apt-get clean && rm -rf /var/lib/apt/lists/*
ENV DOTNET_ROOT=/bin/.dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

# PREPARING VARIABLES FOR DOWNLOADS
# WARNING : With a bad internet connection, git clone can sometimes fail to.
# These commands should help git on bad internet connections
RUN git config --global http.version HTTP/1.1 \
&& git config --global http.postBuffer 524288000 \
&& git config --global http.lowSpeedLimit 0 \
&& git config --global http.lowSpeedTime 999999

# PREPARING VARIABLE FOR MORE EASY COMMANDS 
ENV LANDIS_EXTENSIONS_TOOL="/bin/LANDIS_Linux/Core-Model-v8-LINUX/build/Release/Landis.Extensions.dll"
ENV LANDIS_CONSOLE="/bin/LANDIS_Linux/Core-Model-v8-LINUX/build/Release/Landis.Console.dll"
ENV LANDIS_FOLDER="/bin/LANDIS_Linux/Core-Model-v8-LINUX/"

################################################################
# COMPILING LANDIS-II
################################################################

#### COMPILING THE CORE 

RUN mkdir /bin/LANDIS_Linux \
&& cd /bin/LANDIS_Linux \
&& git clone https://github.com/LANDIS-II-Foundation/Core-Model-v8-LINUX.git \
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Tool-Console/src \
&& dotnet build -c Release 

# Downloading support libraries and installing them.
# We fix the commit that is used to avoid breaking the build of the Docker image
# because of updates to the repository
RUN cd /bin/LANDIS_Linux \
&& git clone https://github.com/LANDIS-II-Foundation/Support-Library-Dlls-v8.git \
&& cd /bin/LANDIS_Linux/Support-Library-Dlls-v8/ \
&& git checkout a2e944d788a1948782f4819308272f6693c93b81 \
&& mv /bin/LANDIS_Linux/Support-Library-Dlls-v8/* /bin/LANDIS_Linux/Core-Model-v8-LINUX/build/extensions \
&& rm -r /bin/LANDIS_Linux/Support-Library-Dlls-v8

#### COMPILING AND REGISTERING EXTENSIONS 
# INFO: The files necessary for the compilation are downloaded through the 
# script downloadSpecificGitCommitAndFolder.sh
# It is used to avoid downloading the entire repository as it's not needed.
#
# The script takes three arguments:
# $1 The URL of the repo
# $2 The commit hash for the commit from which we want to download files
# $3 The folder we want to download files from (excludes all of the others)
#
# The folder we want is almost always /src which contains the source code, 
# but sometimes we need some files in /deploy as well.
#
# We also always end up deleting the files downloaded for the build to avoid 
# saving them in docker layering system (saves space).
#
# For the .txt file necessary to register the extension in the extension.xml 
# file, I've decided to download them through their unique Github URL 
# (corresponding to the commit). This is again to avoid downloading a lot of 
# files for nothing.

COPY ./files_to_help_compilation/editing_csproj_LANDIS-II_files.py /bin/LANDIS_Linux/Core-Model-v8-LINUX
COPY ./files_to_help_compilation/downloadSpecificGitCommitAndFolder.sh /bin/LANDIS_Linux/Core-Model-v8-LINUX
RUN chmod +x /bin/LANDIS_Linux/Core-Model-v8-LINUX/downloadSpecificGitCommitAndFolder.sh

#### SUCCESSION EXTENSIONS

### Forest Carbon Succession (ForCS)
##  an updated v4.1 by MGarcia that needs debugging and testing
ENV FORC_SUCCESSION_DEVELOPER="megarcia"
ENV FORC_SUCCESSION_COMMIT="1dc128516f34d7f77268a0d508192a5174602605"
# Get Extension source code
RUN cd /bin/LANDIS_Linux/Core-Model-v8-LINUX \
&& ./downloadSpecificGitCommitAndFolder.sh https://github.com/$FORC_SUCCESSION_DEVELOPER/Extension-ForCS-Succession.git $FORC_SUCCESSION_COMMIT /src \ 
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Extension-ForCS-Succession/src \
&& python /bin/LANDIS_Linux/Core-Model-v8-LINUX/editing_csproj_LANDIS-II_files.py ./ForCS.csproj \
# Build the Extension
&& dotnet build -c Release \
# Register the extension properly
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/ \
&& ./downloadSpecificGitCommitAndFolder.sh https://github.com/$FORC_SUCCESSION_DEVELOPER/Extension-ForCS-Succession.git $FORC_SUCCESSION_COMMIT /deploy \
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Extension-ForCS-Succession \
&& wget https://raw.githubusercontent.com/$FORC_SUCCESSION_DEVELOPER/Extension-ForCS-Succession/$FORC_SUCCESSION_COMMIT/deploy/installer/ForC-Succession.txt \
&& dotnet $LANDIS_EXTENSIONS_TOOL add "ForC-Succession.txt" \
# Clean up Extension installation source code
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/ \
&& rm -r Extension-ForCS-Succession

##################################################################
# RECOMPILING SOME SUPPORT LIBRARIES TO SOLVE LIBRARY ERRORS
#
# Some library errors seem to come from the fact that the
# support libraries in the support library repository are built in
# an environment with references to libraries that are not yet available,
# or other issues. By re-compiling, we avoid these issues.
##################################################################

### Metadata Library
ENV METADATA_LIB_DEVELOPER="LANDIS-II-Foundation"
ENV METADATA_LIB_COMMIT="72b8caa14cdd6af81c6e1e4541c4c3e18bc63eca"
RUN cd /bin/LANDIS_Linux/Core-Model-v8-LINUX \
&& ./downloadSpecificGitCommitAndFolder.sh https://github.com/$METADATA_LIB_DEVELOPER/Library-Metadata.git $METADATA_LIB_COMMIT /src \ 
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Library-Metadata/src \
&& python /bin/LANDIS_Linux/Core-Model-v8-LINUX/editing_csproj_LANDIS-II_files.py ./Metadata.csproj \
&& dotnet build -c Release \
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/ \
&& rm -r Library-Metadata

### Universal Initial Community Library
ENV UNIV_INIT_COMMUNITY_DEVELOPER="LANDIS-II-Foundation"
ENV UNIV_INIT_COMMUNITY_COMMIT="5dc6dd299eef88ded1c88871470d58c26c1a4093"
RUN cd /bin/LANDIS_Linux/Core-Model-v8-LINUX \
&& rm /bin/LANDIS_Linux/Core-Model-v8-LINUX/build/extensions/Landis.Library.InitialCommunity.Universal.dll \
&& mkdir /bin/LANDIS_Linux/Core-Model-v8-LINUX/Library-Initial-Community/ \
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Library-Initial-Community/ \
&& git clone https://github.com/$UNIV_INIT_COMMUNITY_DEVELOPER/Library-Initial-Community.git \ 
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Library-Initial-Community/Library-Initial-Community/ \
&& git checkout $UNIV_INIT_COMMUNITY_COMMIT \
&& python /bin/LANDIS_Linux/Core-Model-v8-LINUX/editing_csproj_LANDIS-II_files.py ./initial-community.csproj \
&& dotnet build -c Release \
&& cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/ \
&& rm -r Library-Initial-Community

##################################################################
# RECOMPILING THE CORE TO REFERENCE LIBRARIES
#
# Recompiling the core as per 
# https://github.com/CU-ESIIL/ExtremeWildfire/blob/main/docker/landis2/Dockerfile
# This part seems absolutely essential. Basically, it seems like the core of 
# LANDIS-II v8 needs to have all libraries of all extensions clearly indicated as 
# dependencies so that it can find them when running. All of the DLLs indicated 
# here will be copied from /build/extensions to /build/Release during this rebuild 
# process. Simply copying the DLLs into /build/Release is not enough; you have to 
# indicate them here for the rebuild.
#
# WARNING : If you added a new extension in the previous sections, you have to 
# indicate its .dll here, or it will not work!
##################################################################

# To recompile the core properly, we need to add references to all of the libraries 
# that are used in the Console.csproj file of Tool-Console. Instead of doing it by 
# hand, I made a python script that will detect all of the libraries in /build/
# extensions, and add them in Console.csproj with the right format. Open the script 
# for more info.

COPY ./files_to_help_compilation/adding_xml_dll_references.py /bin/LANDIS_Linux/Core-Model-v8-LINUX/
RUN cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/ \
&& python adding_xml_dll_references.py /bin/LANDIS_Linux/Core-Model-v8-LINUX/build/extensions /bin/LANDIS_Linux/Core-Model-v8-LINUX/Tool-Console/src/Console.csproj
RUN cd /bin/LANDIS_Linux/Core-Model-v8-LINUX/Tool-Console/src \
&& dotnet build -c Release

##################################################################
# FINISHING
##################################################################

### Re-configure git for latest version of HTTP protocol
RUN git config --global --unset http.version
