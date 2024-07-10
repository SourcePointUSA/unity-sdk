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

#Make sure adb server working
adb kill-server
adb start-server
sleep 5

#Start Android Emulator
targetWindowName=$(osascript -e 'tell app "Terminal" to do script "~/Library/Android/sdk/emulator/emulator -avd Pixel_XL_API_33"')
androidEmulator_window_id=$(echo ${targetWindowName} | sed 's/.*window id \([0-9]*\).*/\1/')
echo "Android window ${androidEmulator_window_id}"

#Boot IOS Simulator
targetWindowName=$(osascript -e 'tell app "Terminal" to do script "/Applications/Xcode.app/Contents/Developer/Applications/Simulator.app/Contents/MacOS/Simulator -CurrentDeviceUDID 75EBD8C0-A809-4C7C-B45D-169469835DC4"')
iosEmulator_window_id=$(echo ${targetWindowName} | sed 's/.*window id \([0-9]*\).*/\1/')
echo "IOS window ${iosEmulator_window_id}"

#Wait for emulators
sleep 60

#Test android
runTest android.runsettings ClickAcceptAllButtonTest
sleep 5
runTest android.runsettings ClickRejectAllButtonTest
sleep 5
runTest android.runsettings OpenPmLayersTest
sleep 5
runTest android.runsettings SaveAndExitGDPRTest
sleep 5
runTest android.runsettings SaveAndExitCCPATest
sleep 5
runTest android.runsettings SaveAndExitUSNATTest
sleep 5
runTest android.runsettings ClearAllButtonTest
sleep 5
runTest android.runsettings AuthIdTest

#Kill android emulator
osascript -e 'tell app "Terminal" to close window id '${androidEmulator_window_id}''
sleep 5

#Test ios
runTest ios.runsettings ClickAcceptAllButtonTest
sleep 5
runTest ios.runsettings ClickRejecttAllButtonTest
sleep 5
runTest ios.runsettings OpenPmLayersTest
sleep 5
runTest ios.runsettings SaveAndExitGDPRTest
sleep 5
runTest ios.runsettings SaveAndExitCCPATest
sleep 5
runTest ios.runsettings SaveAndExitUSNATTest
sleep 5
runTest ios.runsettings ClearAllButtonTest
sleep 5
runTest ios.runsettings AuthIdTest

#Kill ios emulator
osascript -e 'tell app "Terminal" to close window id '${iosEmulator_window_id}''