
echo "Deleting /home/pi/testC/"
for host in $(cat $HOSTS); do ssh $host "rm /home/pi/testC/ -r"; done

echo "Deleting /home/pi/storage/"
for host in $(cat $HOSTS); do ssh $host "rm /home/pi/storage/ -r"; done

echo "done"
