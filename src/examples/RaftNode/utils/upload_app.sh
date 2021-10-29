#!/bin/bash -x


set -x
parallel-scp -r -v -h $HOSTS bin/Debug/net6.0/linux-arm/publish/ /home/pi/testC