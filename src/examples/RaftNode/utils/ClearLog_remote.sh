HOSTS=$1
echo "Deleting /home/pi/testC/"
for host in $(cat $HOSTS); do ssh $host "rm /home/pi/testC/ -r"; done

echo "Deleting /home/pi/run_remote.sh"
for host in $(cat $HOSTS); do ssh $host "rm /home/pi/run_remote.sh -r"; done

echo "done"
