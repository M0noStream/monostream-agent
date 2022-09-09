#!/bin/bash

STREAM_ID=$1

echo Stopping \`$STREAM_ID\`

sudo systemctl stop $STREAM_ID

echo Stopped \`$STREAM_ID\` successfully
