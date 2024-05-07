#!/bin/sh

#reruns test up to 4 times if failed
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

#start Android Emulator
targetWindowName=$(osascript -e 'tell app "Terminal" to do script "~/Library/Android/sdk/emulator/emulator -avd Pixel_XL_API_33"')

androidEmulator_window_id=$(echo ${targetWindowName} | sed 's/.*window id \([0-9]*\).*/\1/')
echo "Appium window ${androidEmulator_window_id}"

#boot IOS Simulator
osascript -e 'tell app "Terminal" to do script "/Applications/Xcode.app/Contents/Developer/Applications/Simulator.app/Contents/MacOS/Simulator -CurrentDeviceUDID 75EBD8C0-A809-4C7C-B45D-169469835DC4"'

sleep 30

#Test android
runTest android.runsettings ClickAcceptAllButtonTest
sleep 5
runTest android.runsettings ClickRejectAllButtonTest
sleep 5
runTest android.runsettings OpenPmLayersTest
sleep 5
runTest android.runsettings SaveAndExitTest
sleep 5
runTest android.runsettings ClearAllButtonTest

#Test ios
sleep 5
runTest ios.runsettings ClickAcceptAllButtonTest
sleep 5
runTest ios.runsettings ClickRejectAllButtonTest
sleep 5
runTest ios.runsettings OpenPmLayersTest
sleep 5
runTest ios.runsettings SaveAndExitTest
sleep 5
runTest ios.runsettings ClearAllButtonTest

osascript -e 'tell app "Terminal" to close window id '${androidEmulator_window_id}''