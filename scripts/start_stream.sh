#!/bin/bash

STREAM_ID=$1

echo Starting \`$STREAM_ID\`

sudo systemctl start $STREAM_ID

echo \`$STREAM_ID\` started successfully
