appium --use-plugins=altunity &>~/work/unity-sdk/unity-sdk/logs/appiumLog.txt &
cd ~ && AltTesterDesktop.app/Contents/MacOS/AltTesterDesktop -batchmode -nographics -port 13000 -license $1 -termsAndConditionsAccepted -logfile ~/work/unity-sdk/unity-sdk/logs/altTesterLog.txt &>~/work/unity-sdk/unity-sdk/logs/runAltTesterLog.txt &
pidAltTester=$!
sleep 15
test-rerun ~/work/unity-sdk/unity-sdk/UI-TESTS -s ~/work/unity-sdk/unity-sdk/UI-TESTS/ios.runsettings --filter Name=$2
exitCode=$?
kill -9 $pidAltTester
sleep 15
exit $exitCode