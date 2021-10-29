echo "shuting down cluster"
for host in $(cat config/hosts.txt); do ssh $host "sudo shutdown -h"; done

echo "done"