using System.Collections.ObjectModel;
namespace UnityAppiumTests
{
    public class Pages
    {
        public PreFirstLayer preFirstLayer;
        public FirstLayerGDPR firstLayerGDPR;
        public FirstLayerCCPA firstLayerCCPA;
        public FirstLayerUSNAT firstLayerUSNAT;
        public NativeAppLayer nativeAppLayer;

        public Pages(string platformName, WebDriverWait webDriverWait, AndroidDriver<AndroidElement> driverAndroid, IOSDriver<IOSElement> driverIOS, AltDriver altDriver)
        {
            preFirstLayer = platformName == "Android" ? new PreFirstLayerAndroid(webDriverWait, driverAndroid) : new PreFirstLayerIOS(webDriverWait, driverIOS);
            firstLayerGDPR = platformName == "Android" ? new FirstLayerGDPRAndroid(webDriverWait) : new FirstLayerGDPRIOS(webDriverWait);
            firstLayerCCPA = platformName == "Android" ? new FirstLayerCCPAAndroid(webDriverWait) : new FirstLayerCCPAIOS(webDriverWait);
            firstLayerUSNAT = platformName == "Android" ? new FirstLayerUSNATAndroid(webDriverWait) : new FirstLayerUSNATIOS(webDriverWait);
            nativeAppLayer = new NativeAppLayer(altDriver);
        }
    }
}