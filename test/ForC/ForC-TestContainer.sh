#!/bin/bash

docker run --mount type=bind,src=$(pwd),dst=/TestSim -it --name=landis landis-ii_forc-succession_linux
