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
		public ShellHelper shellHelper;

		[SetUp]
		public void Setup()
		{
			string testDir = NUnit.Framework.TestContext.CurrentContext.TestDirectory;
			var rootDir = testDir.Substring(0,testDir.IndexOf("UI-TESTS"))+"UI-TESTS";
			shellHelper = new ShellHelper(rootDir);
			shellHelper.StartAppium();
			shellHelper.StartAltTester();
			System.Threading.Thread.Sleep(10000);
			var desiredCaps = new AppiumOptions();
			desiredCaps.AddAdditionalCapability("platformName", TestContext.Parameters["platformName"]);
			desiredCaps.AddAdditionalCapability("deviceName", TestContext.Parameters["deviceName"]);
			desiredCaps.AddAdditionalCapability("appium:app", (string)rootDir+TestContext.Parameters["appium:app"]);
			desiredCaps.AddAdditionalCapability("appium:automationName", TestContext.Parameters["appium:automationName"]);
			desiredCaps.AddAdditionalCapability("appium:altUnityHost", "192.168.1.78"); //appium --use-plugins=altunity
			desiredCaps.AddAdditionalCapability("appium:altUnityPort", 13000);
			desiredCaps.AddAdditionalCapability("appium:sendKeyStrategy", "setValue");
			if (platformAndroid)
			{
				// desiredCaps.AddAdditionalCapability("appium:chromedriverAutodownload", true);
				desiredCaps.AddAdditionalCapability("appium:chromedriverExecutable", (string)rootDir+TestContext.Parameters["appium:chromedriverExecutable"]);
				driverAndroid = new AndroidDriver<AndroidElement>(appiumServerUri, desiredCaps, initTimeoutSec);

				AltReversePortForwarding.ReversePortForwardingAndroid();
			}
			if (platformIOS)
				driverIOS = new IOSDriver<IOSElement>(appiumServerUri, desiredCaps, initTimeoutSec);
			
        	altDriver = new AltDriver(host: "192.168.1.78", enableLogging: false);

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

			Console.WriteLine($"Current button for tap: firstLayerGDPR.pressAcceptAll");
			pages.firstLayerGDPR.pressAcceptAll();
			Console.WriteLine($"Current button for tap: firstLayerCCPA.pressAcceptAll");
			pages.firstLayerCCPA.pressAcceptAll();
			Console.WriteLine($"Current button for tap: firstLayerUSNAT.pressAcceptAll");
			pages.firstLayerUSNAT.pressAcceptAll();
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
			
    		Assert.That(data!="-", Is.True);	
		}

		[Test]
		public void ClickRejecttAllButtonTest()
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

			Console.WriteLine($"Current button for tap: firstLayerGDPR.pressRejectAll");
			pages.firstLayerGDPR.pressRejectAll();
			Console.WriteLine($"Current button for tap: firstLayerCCPA.pressRejectAll");
			pages.firstLayerCCPA.pressRejectAll();
			Console.WriteLine($"Current button for tap: firstLayerUSNAT.pressRejectAll");
			pages.firstLayerUSNAT.pressRejectAll();
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");

    		Assert.That(data!="-", Is.True);
		}

		[Test]
		public void OpenPmLayersTest()
		{
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}
			bool isOpen = false;

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();
			//pages.preFirstLayer.SetContex(firstLayerContext);

			Console.WriteLine($"Current button for tap: firstLayerGDPR.pressAcceptAll");
			pages.firstLayerGDPR.pressAcceptAll();
			Console.WriteLine($"Current button for tap: firstLayerCCPA.pressAcceptAll");
			pages.firstLayerCCPA.pressAcceptAll();
			Console.WriteLine($"Current button for tap: firstLayerUSNAT.pressAcceptAll");
			pages.firstLayerUSNAT.pressAcceptAll();

			Console.WriteLine($"Check for contex count: preFirstLayer.GetContexNum");
			Console.WriteLine($"Contex count: {pages.preFirstLayer.GetContexNum()}");

			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
			
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressGDPRPmLayer");
        	pages.nativeAppLayer.pressGDPRPmLayer();
			Console.WriteLine($"Check for webView open: PmLayerGDPR.webViewIsOpen");
			isOpen = pages.pmLayerGDPR.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Current button for tap: pmLayerGDPR.pressExit");
        	pages.pmLayerGDPR.pressExit();
			
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressCCPAPmLayer");
        	pages.nativeAppLayer.pressCCPAPmLayer();
			Console.WriteLine($"Check for webView open: pmLayerCCPA.webViewIsOpen");
			isOpen = pages.pmLayerCCPA.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Current button for tap: pmLayerCCPA.pressExit");
        	pages.pmLayerCCPA.pressExit();
			
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressUSNATPmLayer");
        	pages.nativeAppLayer.pressUSNATPmLayer();
			Console.WriteLine($"Check for webView open: pmLayerUSNAT.webViewIsOpen");
			isOpen = pages.pmLayerUSNAT.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Current button for tap: pmLayerUSNAT.pressExit");
        	pages.pmLayerUSNAT.pressExit();
			
    		Assert.That(data!="-", Is.True);	
		}

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        	altDriver.Stop();
			if (platformAndroid)
        		AltReversePortForwarding.RemoveReversePortForwardingAndroid();
			shellHelper.StopAltTester();
			shellHelper.StopAppium();
        }
    }
}
