#!/bin/bash -x
set -x
HOSTS=$1
parallel-scp -r -v -h $HOSTS config/TestConfiguration_remote.json /home/pi/testC
parallel-scp -r -v -h $HOSTS config/TestConfiguration_local.json /home/pi/testC
parallel-scp -r -v -h $HOSTS run_remote.sh /home/pi/
parallel-scp -r -v -h $HOSTS run_remote_local.sh /home/pi/