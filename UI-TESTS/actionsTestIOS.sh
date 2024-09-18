appium --use-plugins=altunity &>logs/appiumLog.txt &
cd ~ && AltTesterDesktop.app/Contents/MacOS/AltTesterDesktop -batchmode -nographics -port 13000 -license $1 -termsAndConditionsAccepted -logfile ~/work/unity-sdk/unity-sdk/logs/altTesterLog.txt &>~/work/unity-sdk/unity-sdk/logs/runAltTesterLog.txt &
pidAltTester=$!
sleep 5
test-rerun ~/work/unity-sdk/unity-sdk/UI-TESTS -s ~/work/unity-sdk/unity-sdk/UI-TESTS/android.runsettings --filter Name=$2
kill -9 $pidAltTester
sleep 5