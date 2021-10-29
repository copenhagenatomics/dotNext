echo "Deleting /home/pi/testB/"
for host in $(cat hosts.txt); do ssh $host "sudo shutdown -h"; done

echo "done"