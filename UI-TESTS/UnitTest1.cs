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
			desiredCaps.AddAdditionalCapability("appium:altUnityHost", TestContext.Parameters["altTesterIP"]);
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
			
        	altDriver = new AltDriver(host: TestContext.Parameters["altTesterIP"],enableLogging: false);

			webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
			pages = new Pages(TestContext.Parameters["platformName"], webDriverWait, driverAndroid, driverIOS, altDriver);
		}

		[Test]
		public void ClickAcceptAllButtonTest()
		{
			Console.WriteLine(">>>ClickAcceptAllButtonTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
			
    		Assert.That(data!="-", Is.True);	
		}

		[Test]
		public void ClickRejectAllButtonTest()
		{
			Console.WriteLine(">>>ClickRejecttAllButtonTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(false, false, false);
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");

    		Assert.That(data!="-", Is.True);
		}

		[Test]
		public void OpenPmLayersTest()
		{
			Console.WriteLine(">>>OpenPmLayersTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}
			bool isOpen = false;

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
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

		[Test]
		public void SaveAndExitTest()
		{
			Console.WriteLine(">>>SaveAndExitTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}
			bool isOpen = false;

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
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
			Console.WriteLine($"Try to get: pmLayerGDPR.getCheckedSwitchesNum"); 
         	int num = pages.pmLayerGDPR.getCheckedSwitchesNum(); 
   			Console.WriteLine($"CheckedSwitches: {num}");
			Console.WriteLine($"Try to click: pmLayerGDPR.clickOnSwitches(2)"); 
			pages.pmLayerGDPR.clickOnSwitches(2);
			Console.WriteLine($"Current button for tap: pmLayerGDPR.pressSaveAndExit");
        	pages.pmLayerGDPR.pressSaveAndExit();
						
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressGDPRPmLayer");
        	pages.nativeAppLayer.pressGDPRPmLayer();
			Console.WriteLine($"Check for webView open: PmLayerGDPR.webViewIsOpen");
			isOpen = pages.pmLayerGDPR.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Try to get: pmLayerGDPR.getCheckedSwitchesNum"); 
         	num = pages.pmLayerGDPR.getCheckedSwitchesNum(); 
   			Console.WriteLine($"CheckedSwitches: {num}");
			Console.WriteLine($"Current button for tap: pmLayerGDPR.pressExit");
        	pages.pmLayerGDPR.pressExit();

			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var dataNew = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {dataNew}");
			
    		Assert.That(num==2, Is.True);
    		Assert.That(data!=dataNew, Is.True);	
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
