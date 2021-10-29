#!/bin/bash -x
set -x

parallel-scp -r -v -h hosts.txt TestConfiguration.json /home/pi/testC