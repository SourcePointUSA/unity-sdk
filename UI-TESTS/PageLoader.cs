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
        public PmLayerGDPR pmLayerGDPR;
        public PmLayerCCPA pmLayerCCPA;
        public PmLayerUSNAT pmLayerUSNAT;
        public DriverHelper driverHelper;

        public Pages(string platformName, WebDriverWait webDriverWait, AndroidDriver<AndroidElement> driverAndroid, IOSDriver<IOSElement> driverIOS, AltDriver altDriver)
        {
            preFirstLayer = platformName == "Android" ? new PreFirstLayerAndroid(webDriverWait, driverAndroid) : new PreFirstLayerIOS(webDriverWait, driverIOS);
            firstLayerGDPR = platformName == "Android" ? new FirstLayerGDPRAndroid(webDriverWait) : new FirstLayerGDPRIOS(webDriverWait);
            firstLayerCCPA = platformName == "Android" ? new FirstLayerCCPAAndroid(webDriverWait) : new FirstLayerCCPAIOS(webDriverWait);
            firstLayerUSNAT = platformName == "Android" ? new FirstLayerUSNATAndroid(webDriverWait) : new FirstLayerUSNATIOS(webDriverWait);
            nativeAppLayer = new NativeAppLayer(altDriver);
            pmLayerGDPR = platformName == "Android" ? new PmLayerGDPRAndroid(webDriverWait) : new PmLayerGDPRIOS(webDriverWait);
            pmLayerCCPA = platformName == "Android" ? new PmLayerCCPAAndroid(webDriverWait) : new PmLayerCCPAIOS(webDriverWait);
            pmLayerUSNAT = platformName == "Android" ? new PmLayerUSNATAndroid(webDriverWait) : new PmLayerUSNATIOS(webDriverWait);

            driverHelper = new DriverHelper(platformName, driverAndroid, driverIOS, webDriverWait);
            firstLayerGDPR.driverHelper = driverHelper;
            firstLayerCCPA.driverHelper = driverHelper;
            firstLayerUSNAT.driverHelper = driverHelper;
            pmLayerGDPR.driverHelper = driverHelper;
            pmLayerCCPA.driverHelper = driverHelper;
            pmLayerUSNAT.driverHelper = driverHelper;
        }

        public void firstLayerGO(bool acceptGDPR, bool acceptCCPA, bool acceptUSNAT)
        {
            if (acceptGDPR)
            {
			    Console.WriteLine($"Current button for tap: firstLayerGDPR.pressAcceptAll");
			    firstLayerGDPR.pressAcceptAll();
            }
            else
            {
			    Console.WriteLine($"Current button for tap: firstLayerGDPR.pressRejectAll");
			    firstLayerGDPR.pressRejectAll();
            }

            if (acceptCCPA)
            {
			    Console.WriteLine($"Current button for tap: firstLayerCCPA.pressAcceptAll");
			    firstLayerCCPA.pressAcceptAll();
            }
            else
            {
			    Console.WriteLine($"Current button for tap: firstLayerCCPA.pressRejectAll");
			    firstLayerCCPA.pressRejectAll();
            }

            if (acceptUSNAT)
            {
			    Console.WriteLine($"Current button for tap: firstLayerUSNAT.pressAcceptAll");
			    firstLayerUSNAT.pressAcceptAll();
            }
            else
            {
			    Console.WriteLine($"Current button for tap: firstLayerUSNAT.pressRejectAll");
			    firstLayerUSNAT.pressRejectAll();
            }
        }
    }
}