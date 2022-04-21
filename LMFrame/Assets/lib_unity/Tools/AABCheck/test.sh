#!/bin/bash

fname=$1
fkeystore=$2

rm "$fname.apks"

java -jar bundletool.jar build-apks --bundle="$fname.aab" --output="$fname.apks" --ks=$fkeystore --ks-pass=pass:123123 --ks-key-alias=tilemaster --key-pass=pass:123123
