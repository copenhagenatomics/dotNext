#!/bin/bash -x
set -x

parallel-scp -r -v -h hosts.txt bin/Debug/net5.0/linux-arm/publish/ /home/pi/testC