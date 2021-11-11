echo "rebooting cluster"
for host in $(cat config/hosts.txt); do ssh $host "sudo reboot"; done

echo "done"