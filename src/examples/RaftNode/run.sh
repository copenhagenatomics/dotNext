
set -x
./bin/Debug/net6.0/RaftNode tcp $[3261+$1] node$1 config/TestConfiguration_remote.json