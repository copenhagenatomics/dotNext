echo "Building project"
set -e
bash publish_arm.sh
set +e
bash ClearLog_remote.sh
bash upload_app.sh    
bash upload_config.sh
