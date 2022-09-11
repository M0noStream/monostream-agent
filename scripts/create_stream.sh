#!/bin/bash

STREAM_ID=$1

BASE_TEMPLATE_PATH=~/monostream/base.template
BASE_SERVICE_FILE=$BASE_TEMPLATE_PATH/base.monostream.service
PARAMS_TO_EXTRACT=('IS_TRANSACTED' 'INNER_DEPTH_QUEUE' 'SOURCE_TYPE_NAME')
i=2
LOCAL_FOLDER=~/monostream/streams/$STREAM_ID
CONFIG_FILE_PATH=$LOCAL_FOLDER/appsettings.json


# Create local folder
cp -r $BASE_TEMPLATE_PATH/publish $LOCAL_FOLDER

# Configure file
for param in "${PARAMS_TO_EXTRACT[@]}"
do
        echo "Extracting param \`$param\`"
        echo $param ${!i}
        sed -i -z "s/#{$param}#/${!i}/" $CONFIG_FILE_PATH
        i=$(($i+1))
done

# Copy service folder
mv $LOCAL_FOLDER /srv/monostream/$STREAM_ID

# Copy service file
sudo sed "s/#{STREAM_ID}#/$STREAM_ID/g" $BASE_SERVICE_FILE | sudo tee /etc/systemd/system/monostream-$STREAM_ID.service

# Create service
sudo systemctl daemon-reload
