set -x
cd testC
./RaftNode tcp $[3261+$1] node$1 TestConfiguration_local.json