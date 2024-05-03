#!/bin/sh
runTest()
{
    local i=0
    local max=4
    while [ $i -lt $max ]
    do
        if dotnet test -s $1 --filter Name=$2; then
            break
        else
            true $(( i++ ))
        fi
    done
}

#targetWindowName=$(osascript -e 'tell app "Terminal" to do script "~/Library/Android/sdk/emulator/emulator -avd Pixel_XL_API_33"')
#androidEmulator_window_id=$(echo ${targetWindowName} | sed 's/.*window id \([0-9]*\).*/\1/')
#echo "Appium window ${androidEmulator_window_id}"

runTest android.runsettings ClickAcceptAllButtonTest
sleep 5
runTest android.runsettings ClickRejecttAllButtonTest
sleep 5
runTest android.runsettings OpenPmLayersTest
sleep 5
runTest android.runsettings SaveAndExitTest

#osascript -e 'tell app "Terminal" to close window id '${androidEmulator_window_id}''
