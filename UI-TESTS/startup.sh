#!/bin/sh

#targetWindowName=$(osascript -e 'tell app "Terminal" to do script "~/Library/Android/sdk/emulator/emulator -avd Pixel_XL_API_33"')
#androidEmulator_window_id=$(echo ${targetWindowName} | sed 's/.*window id \([0-9]*\).*/\1/')
#echo "Appium window ${androidEmulator_window_id}"

dotnet test -s android.runsettings --filter Name=ClickAcceptAllButtonTest
sleep 5
dotnet test -s android.runsettings --filter Name=ClickRejecttAllButtonTest
sleep 5
dotnet test -s android.runsettings --filter Name=OpenPmLayersTest

#osascript -e 'tell app "Terminal" to close window id '${androidEmulator_window_id}''
