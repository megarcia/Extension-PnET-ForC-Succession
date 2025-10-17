#!/bin/bash

cp PnETGenericDefaultParameters.txt /usr/bin/LANDIS_Linux/Core-Model-v8-LINUX/build/Release/Defaults/
dotnet $LANDIS_CONSOLE PnET-Scenario.txt
