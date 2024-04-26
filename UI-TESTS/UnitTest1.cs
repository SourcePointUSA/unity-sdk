namespace UnityAppiumTests
{
    [TestFixture]
    public class WebViewTests
    {
        private readonly Uri appiumServerUri = new Uri("http://127.0.0.1:4723");
        private readonly TimeSpan initTimeoutSec = TimeSpan.FromSeconds(180);
		public TestContext TestContext { get; set; }
		public bool platformIOS {get => TestContext.Parameters["platformName"]=="iOS";}
		public bool platformAndroid {get => TestContext.Parameters["platformName"]=="Android";}
        private IOSDriver<IOSElement> driverIOS;
        private AndroidDriver<AndroidElement> driverAndroid;
		private IWebDriver driver { get { if(platformIOS) return driverIOS; else return driverAndroid;} }
    	private AltDriver altDriver;
		WebDriverWait webDriverWait;

		public Pages pages;

		[SetUp]
		public void Setup()
		{
			string testDir = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
			var rootDir = testDir.Substring(0,testDir.IndexOf("UI-TESTS"))+"UI-TESTS";
			var desiredCaps = new AppiumOptions();
			desiredCaps.AddAdditionalCapability("platformName", TestContext.Parameters["platformName"]);
			desiredCaps.AddAdditionalCapability("deviceName", TestContext.Parameters["deviceName"]);
			desiredCaps.AddAdditionalCapability("appium:app", (string)rootDir+TestContext.Parameters["appium:app"]);
			desiredCaps.AddAdditionalCapability("appium:automationName", TestContext.Parameters["appium:automationName"]);
			desiredCaps.AddAdditionalCapability("appium:altUnityHost", "192.168.1.78"); //appium --use-plugins=altunity
			// desiredCaps.AddAdditionalCapability("appium:altUnityHost", "127.0.0.1"); //appium --use-plugins=altunity
			desiredCaps.AddAdditionalCapability("appium:altUnityPort", 13000); //dotnet test -s android.runsettings
			desiredCaps.AddAdditionalCapability("appium:sendKeyStrategy", "setValue"); //grouped");
			if (platformAndroid)
			{
				// desiredCaps.AddAdditionalCapability("appium:chromedriverAutodownload", true);
				desiredCaps.AddAdditionalCapability("appium:chromedriverExecutable", (string)rootDir+TestContext.Parameters["appium:chromedriverExecutable"]);
				driverAndroid = new AndroidDriver<AndroidElement>(appiumServerUri, desiredCaps, initTimeoutSec);

				AltReversePortForwarding.ReversePortForwardingAndroid();
			}
			if (platformIOS)
				driverIOS = new IOSDriver<IOSElement>(appiumServerUri, desiredCaps, initTimeoutSec);
        
        	altDriver = new AltDriver();

			webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
			pages = new Pages(TestContext.Parameters["platformName"], webDriverWait, driverAndroid, driverIOS, altDriver);
		}

		[Test]
		public void ClickAcceptAllButtonTest()
		{
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();
			//pages.preFirstLayer.SetContex(firstLayerContext);

			// Ensure the Context is changed
			if (platformAndroid)
				Console.WriteLine($"Current context: {driverAndroid.Context}");
			else
				Console.WriteLine($"Current context: {driverIOS.Context}");

			pages.firstLayerGDPR.pressAcceptAll();
			pages.firstLayerCCPA.pressAcceptAll();
			pages.firstLayerUSNAT.pressAcceptAll();
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");

    		Assert.That(data!="-", Is.True);
		}

        [TearDown]
        public void Teardown()
        {
	        driver.Quit();
        	altDriver.Stop();
            if (platformAndroid)
        		AltReversePortForwarding.RemoveReversePortForwardingAndroid();
        }
    }
}
