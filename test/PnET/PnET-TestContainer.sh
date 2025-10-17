#!/bin/bash

docker run --mount type=bind,src=$(pwd),dst=/OneCellTestSim -it --name=landis landis-ii_pnet-succession_linux
