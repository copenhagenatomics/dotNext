echo "Building project"
set -e

HOSTS=config/hosts.txt

bash utils/publish_arm.sh
set +e
bash utils/ClearLog_remote.sh $HOSTS
bash utils/upload_app.sh $HOSTS
bash utils/upload_config.sh $HOSTS
