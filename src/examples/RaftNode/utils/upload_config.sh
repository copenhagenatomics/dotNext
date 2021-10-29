#!/bin/bash -x
set -x
parallel-scp -r -v -h $HOSTS TestConfiguration.json /home/pi/testC