namespace UnityAppiumTests
{
    [TestFixture]
    public class WebViewTests
    {
        private readonly Uri appiumServerUri = new Uri("http://127.0.0.1:4723");
        private readonly TimeSpan initTimeoutSec = TimeSpan.FromSeconds(600);
		public TestContext TestContext { get; set; }
		public bool platformIOS {get => TestContext.Parameters["platformName"]=="iOS";}
		public bool platformAndroid {get => TestContext.Parameters["platformName"]=="Android";}
        private IOSDriver driverIOS;
        private AndroidDriver driverAndroid;
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
			//shellHelper.StartAppium();
			//shellHelper.StartAltTester();
			System.Threading.Thread.Sleep(10000);
			var desiredCaps = new AppiumOptions();
			desiredCaps.DeviceName = TestContext.Parameters["deviceName"];
			desiredCaps.App = (string)rootDir+TestContext.Parameters["appium:app"];
			desiredCaps.AutomationName = TestContext.Parameters["appium:automationName"];
			desiredCaps.AddAdditionalAppiumOption("platformName", TestContext.Parameters["platformName"]);
			desiredCaps.AddAdditionalAppiumOption("appium:uiautomator2ServerInstallTimeout", 120000);
			desiredCaps.AddAdditionalAppiumOption("appium:uiautomator2ServerLaunchTimeout", 120000);
			desiredCaps.AddAdditionalAppiumOption("appium:androidInstallTimeout", 180000);
			desiredCaps.AddAdditionalAppiumOption("appium:newCommandTimeout", 180000);
			desiredCaps.AddAdditionalAppiumOption("appium:altUnityHost", TestContext.Parameters["altTesterIP"]);
			desiredCaps.AddAdditionalAppiumOption("appium:altUnityPort", 13000);
			desiredCaps.AddAdditionalAppiumOption("appium:sendKeyStrategy", "setValue");
			desiredCaps.AddAdditionalAppiumOption("appium:ignoreHiddenApiPolicyError" , true);
			if (platformAndroid)
			{
				// desiredCaps.AddAdditionalAppiumOption("appium:chromedriverAutodownload", true);
				desiredCaps.AddAdditionalAppiumOption("appium:chromedriverExecutable", (string)rootDir+TestContext.Parameters["appium:chromedriverExecutable"]);
				driverAndroid = new AndroidDriver(appiumServerUri, desiredCaps, initTimeoutSec);

				AltReversePortForwarding.ReversePortForwardingAndroid();
			}
			if (platformIOS)
				driverIOS = new IOSDriver(appiumServerUri, desiredCaps, initTimeoutSec);
			
        	altDriver = new AltDriver(host: TestContext.Parameters["altTesterIP"],enableLogging: false);

			webDriverWait = new WebDriverWait(driver, TimeSpan.FromSeconds(1200));
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
		public void SaveAndExitGDPRTest()
		{
			Console.WriteLine(">>>SaveAndExitTestGDPR");
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

		[Test]
		public void SaveAndExitCCPATest()
		{
			Console.WriteLine(">>>SaveAndExitTestCCPA");
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
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressCCPAPmLayer");
        	pages.nativeAppLayer.pressCCPAPmLayer();
			Console.WriteLine($"Check for webView open: PmLayerCCPA.webViewIsOpen");
			isOpen = pages.pmLayerCCPA.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Try to get: pmLayerCCPA.getCheckedSwitchesNum"); 
         	int num = pages.pmLayerCCPA.getCheckedSwitchesNum(); 
   			Console.WriteLine($"CheckedSwitches: {num}");
			Console.WriteLine($"Try to click: pmLayerCCPA.clickOnSwitches(2)"); 
			pages.pmLayerCCPA.clickOnSwitches(2);
			Console.WriteLine($"Current button for tap: pmLayerCCPA.pressSaveAndExit");
        	pages.pmLayerCCPA.pressSaveAndExit();
						
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressCCPAPmLayer");
        	pages.nativeAppLayer.pressCCPAPmLayer();
			Console.WriteLine($"Check for webView open: PmLayerCCPA.webViewIsOpen");
			isOpen = pages.pmLayerCCPA.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Try to get: pmLayerCCPA.getCheckedSwitchesNum"); 
         	num = pages.pmLayerCCPA.getCheckedSwitchesNum(); 
   			Console.WriteLine($"CheckedSwitches: {num}");
			Console.WriteLine($"Current button for tap: pmLayerCCPA.pressExit");
        	pages.pmLayerCCPA.pressExit();
			
    		Assert.That(num==1, Is.True);	
		}

		[Test]
		public void SaveAndExitUSNATTest()
		{
			Console.WriteLine(">>>SaveAndExitTestUSNAT");
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
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressUSNATPmLayer");
        	pages.nativeAppLayer.pressUSNATPmLayer();
			Console.WriteLine($"Check for webView open: pmLayerUSNAT.webViewIsOpen");
			isOpen = pages.pmLayerUSNAT.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Try to get: pmLayerUSNAT.getCheckedSwitchesNum"); 
         	int num = pages.pmLayerUSNAT.getCheckedSwitchesNum(); 
   			Console.WriteLine($"CheckedSwitches: {num}");
			Console.WriteLine($"Try to click: pmLayerUSNAT.clickOnSwitches(1)"); 
			pages.pmLayerUSNAT.clickOnSwitches(1);
			Console.WriteLine($"Current button for tap: pmLayerUSNAT.pressSaveAndExit");
        	pages.pmLayerUSNAT.pressSaveAndExit();
						
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressUSNATPmLayer");
        	pages.nativeAppLayer.pressUSNATPmLayer();
			Console.WriteLine($"Check for webView open: pmLayerUSNAT.webViewIsOpen");
			isOpen = pages.pmLayerUSNAT.webViewIsOpen();
    		Assert.That(isOpen, Is.True);
			Console.WriteLine($"Try to get: pmLayerUSNAT.getCheckedSwitchesNum"); 
         	num = pages.pmLayerUSNAT.getCheckedSwitchesNum(); 
   			Console.WriteLine($"CheckedSwitches: {num}");
			Console.WriteLine($"Current button for tap: pmLayerUSNAT.pressExit");
        	pages.pmLayerUSNAT.pressExit();
			
    		Assert.That(num==0, Is.True);	
		}

		[Test]
		public void ClearAllButtonTest()
		{
			Console.WriteLine(">>>ClearAllButtonTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			System.Threading.Thread.Sleep(2000);
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
    		Assert.That(data!="-", Is.True);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressClearAll");
			pages.nativeAppLayer.pressClearAll();
			System.Threading.Thread.Sleep(2000);
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
    		Assert.That(data=="-", Is.True);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressLoadMessage");
			pages.nativeAppLayer.pressLoadMessage();
			pages.firstLayerGO(true, true, true);
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
        	data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
    		Assert.That(data!="-", Is.True);
		}

		[Test]
		public void AuthIdTest()
		{
			Console.WriteLine(">>>AuthIdTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
        	var data = pages.nativeAppLayer.getAuthIdText();
			Console.WriteLine($"AuthIdText: \"{data}\" (just \"AuthId:\" means no authId was used)");
            Assert.That(data=="AuthId:", Is.True);

            if (platformIOS)
            {
	            // this is iOS-specific code which checks that iOS bridge handles conversion of c# string to obj-c string correctly
	            Console.WriteLine("String send: Test");
				data = altDriver.CallStaticMethod<string>("ConsentManagementProvider.CMP", "GetBridgeString", "Assembly-CSharp", new[] { "Test" });
				Console.WriteLine($"Got: {data}");
    			Assert.That(data=="Test", Is.True);
            }

			pages.nativeAppLayer.waitForSdkDone();
			pages.nativeAppLayer.pressClearAll();
			pages.nativeAppLayer.waitForSdkDone("SDK:Not Started");
			altDriver.CallStaticMethod<string>("ConsentManagementProvider.CMP", "ConcreteInstance.LoadMessage", "Assembly-CSharp", new[] { "AltTesterTest" });
			pages.nativeAppLayer.waitForSdkDone();
        	data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
			if(data=="-")
			{
				// it means it is the first time this property is called with specified authId
				// this part of code is meant to be executed only once in a lifetime
				Console.WriteLine("The very first time using this authId!");
				pages.firstLayerGO(true, true, true);
				pages.nativeAppLayer.waitForSdkDone();
			}
    		Assert.That(data!="-", Is.True);	
		}

		[Test]
		public void AcceptRejectAllUsnatInPMTest()
		{
			Console.WriteLine(">>>AcceptRejectAllUsnatInPMTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string data = "";
			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();
			Console.WriteLine("Call 'LoadPrivacyManager' with pmId 988851");
			altDriver.CallStaticMethod<int>("ConsentManagementProvider.CMPTestUtils", "LoadPrivacyManager", "Assembly-CSharp", new[] { "3", "988851" });
			Console.WriteLine($"Current button for tap: pmLayerUSNAT.pressAcceptAll");
			pages.pmLayerUSNAT.pressAcceptAll();
			System.Threading.Thread.Sleep(2000);

			Console.WriteLine("Call 'LoadPrivacyManager' with pmId 988851");
			altDriver.CallStaticMethod<int>("ConsentManagementProvider.CMPTestUtils", "LoadPrivacyManager", "Assembly-CSharp", new[] { "3", "988851" });
			Console.WriteLine($"Try to get: pmLayerUSNAT.getAcceptRejectState");
			data = pages.pmLayerUSNAT.getAcceptRejectState();
			if (platformAndroid)
				Assert.That(data=="accepted", Is.True);
			Console.WriteLine($"Current button for tap: pmLayerUSNAT.pressRejectAll");
			pages.pmLayerUSNAT.pressRejectAll();
			System.Threading.Thread.Sleep(2000);

			Console.WriteLine("Call 'LoadPrivacyManager' with pmId 988851");
			altDriver.CallStaticMethod<int>("ConsentManagementProvider.CMPTestUtils", "LoadPrivacyManager", "Assembly-CSharp", new[] { "3", "988851" });
			Console.WriteLine($"Try to get: pmLayerUSNAT.getAcceptRejectState");
			data = pages.pmLayerUSNAT.getAcceptRejectState();
			if (platformAndroid)
				Assert.That(data=="rejected", Is.True);
			System.Threading.Thread.Sleep(2000);	
			Assert.That(true, Is.True);
		}

		[Test]
		public void ProgramaticRejectAllGDPRTest()
		{
			Console.WriteLine(">>>ProgramaticRejectAllGDPRTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();

			var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, "Privacy Settings CMP");

			Console.WriteLine($"Try to get: statusCampaignGDPR");
			string status = altElement.GetComponentProperty<string>("PrivacySettings", "statusCampaignGDPR", "Assembly-CSharp");
			Console.WriteLine($"statusCampaignGDPR: {status}");
			Assert.That(status=="accepted", Is.True);
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
			var data = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {data}");
			
			Console.WriteLine("Call 'rejectAll' with campaign GDPR");
			altDriver.CallStaticMethod<int>("ConsentManagementProvider.CMP", "ConcreteInstance.RejectAll", "Assembly-CSharp", new object[] { 0 });
			System.Threading.Thread.Sleep(2000);

			Console.WriteLine($"Try to get: statusCampaignGDPR");
			status = altElement.GetComponentProperty<string>("PrivacySettings", "statusCampaignGDPR", "Assembly-CSharp");
			Console.WriteLine($"statusCampaignGDPR: {status}");	
			Assert.That(status=="rejected", Is.True);	
			Console.WriteLine($"Try to get: nativeAppLayer.getConsentValueText");
			var dataNew = pages.nativeAppLayer.getConsentValueText();
			Console.WriteLine($"ConsentValueText: {dataNew}");

			Assert.That(data!=dataNew, Is.True);	
		}

		[Test]
		public void ProgramaticRejectAllCCPATest()
		{
			Console.WriteLine(">>>ProgramaticRejectAllCCPATest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();

			var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, "Privacy Settings CMP");

			Console.WriteLine($"Try to get: statusCampaignCCPA");
			string status = altElement.GetComponentProperty<string>("PrivacySettings", "statusCampaignCCPA", "Assembly-CSharp");
			Console.WriteLine($"statusCampaignCCPA: {status}");
			Assert.That(status=="accepted", Is.True);
			
			Console.WriteLine("Call 'rejectAll' with campaign CCPA");
			altDriver.CallStaticMethod<int>("ConsentManagementProvider.CMP", "ConcreteInstance.RejectAll", "Assembly-CSharp", new object[] { 2 });
			System.Threading.Thread.Sleep(2000);

			Console.WriteLine($"Try to get: statusCampaignCCPA");
			status = altElement.GetComponentProperty<string>("PrivacySettings", "statusCampaignCCPA", "Assembly-CSharp");
			Console.WriteLine($"statusCampaignCCPA: {status}");	
			Assert.That(status=="rejected", Is.True);	
		}

		[Test]
		public void ProgramaticRejectAllUSNATTest()
		{
			Console.WriteLine(">>>ProgramaticRejectAllUSNATTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();

			var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, "Privacy Settings CMP");

			Console.WriteLine($"Try to get: statusCampaignUSNAT");
			string status = altElement.GetComponentProperty<string>("PrivacySettings", "statusCampaignUSNAT", "Assembly-CSharp");
			Console.WriteLine($"statusCampaignUSNAT: {status}");
			Assert.That(status=="accepted", Is.True);
			
			Console.WriteLine("Call 'rejectAll' with campaign USNAT");
			altDriver.CallStaticMethod<int>("ConsentManagementProvider.CMP", "ConcreteInstance.RejectAll", "Assembly-CSharp", new object[] { 3 });
			System.Threading.Thread.Sleep(2000);

			Console.WriteLine($"Try to get: statusCampaignUSNAT");
			status = altElement.GetComponentProperty<string>("PrivacySettings", "statusCampaignUSNAT", "Assembly-CSharp");
			Console.WriteLine($"statusCampaignUSNAT: {status}");	
			Assert.That(status=="rejected", Is.True);	
		}

		[Test]
		public void ProgramaticCustomConsentGDPRTest()
		{
			Console.WriteLine(">>>ProgramaticCustomConsentGDPRTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();

			Console.WriteLine("Call 'CustomConsentGDPR'");
			altDriver.CallStaticMethod<object>("ConsentManagementProvider.CMPTestUtils", "CustomConsentGDPR", "Assembly-CSharp", new[] { "" });
			System.Threading.Thread.Sleep(2000);
			Console.WriteLine($"Try to get: delegateCalled");
			bool delegateCalled = altDriver.GetStaticProperty<bool>("ConsentManagementProvider.CMPTestUtils", "delegateCalled", "Assembly-CSharp");
			Console.WriteLine($"delegateCalled: {delegateCalled}");

			Assert.That(delegateCalled, Is.True);

			Console.WriteLine("Call 'DeleteCustomConsentGDPR'");
			altDriver.CallStaticMethod<object>("ConsentManagementProvider.CMPTestUtils", "DeleteCustomConsentGDPR", "Assembly-CSharp", new[] { "" });
			System.Threading.Thread.Sleep(2000);
			Console.WriteLine($"Try to get: delegateCalled");
			delegateCalled = altDriver.GetStaticProperty<bool>("ConsentManagementProvider.CMPTestUtils", "delegateCalled", "Assembly-CSharp");
			Console.WriteLine($"delegateCalled: {delegateCalled}");

			Assert.That(delegateCalled, Is.True);
		}

		[Test]
		public void MessageLanguageTest()
		{
			Console.WriteLine(">>>MessageLanguageTest");
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}

			string firstLayerContext = pages.preFirstLayer.SelectFirstLayer();

			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();

			Console.WriteLine($"Current button for tap: nativeAppLayer.pressClearAll");
			pages.nativeAppLayer.pressClearAll();
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine("Call 'InitializeWithLanguage' with SPANISH language");
			altDriver.CallStaticMethod<object>("ConsentManagementProvider.CMPTestUtils", "InitializeWithLanguage", "Assembly-CSharp", new[] { "35" }); //SPANISH
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressLoadMessage");
			pages.nativeAppLayer.pressLoadMessage();
			Console.WriteLine($"Current button for tap: pages.firstLayerGDPR.acceptAllPath with SPANISH language");
			pages.firstLayerGDPR.driverHelper.pressButton(pages.firstLayerGDPR.acceptAllPath, pages.firstLayerGDPR.textViewPathES, true, true);
			Console.WriteLine($"Current button for tap: pages.firstLayerCCPA.acceptAllPath with SPANISH language");
			pages.firstLayerCCPA.driverHelper.pressButton(pages.firstLayerCCPA.acceptAllPath, pages.firstLayerCCPA.textViewPathES, false, true);
			Console.WriteLine($"Current button for tap: pages.firstLayerUSNAT.acceptAllPath with SPANISH language");
			pages.firstLayerUSNAT.driverHelper.pressButton(pages.firstLayerUSNAT.acceptAllPath, pages.firstLayerUSNAT.textViewPathES, true, true);
			pages.nativeAppLayer.waitForSdkDone();
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressGDPRPmLayer");
        	pages.nativeAppLayer.pressGDPRPmLayer();
			Console.WriteLine($"Check for webView open: pmLayerGDPR.webViewIsOpen with SPANISH language");
        	bool isOpen = pages.pmLayerGDPR.driverHelper.webViewIsOpen(pages.pmLayerGDPR.textViewPathES);
			Assert.That(isOpen, Is.True);
			Console.WriteLine($"Current button for tap: pmLayerGDPR.pressExit");
			pages.pmLayerGDPR.driverHelper.pressButton(pages.pmLayerGDPR.exitButtonPath, pages.pmLayerGDPR.textViewPathES, true, true);

			Console.WriteLine($"Current button for tap: nativeAppLayer.pressClearAll");
			pages.nativeAppLayer.pressClearAll();
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine("Call 'InitializeWithLanguage' with TAGALOG language");
			altDriver.CallStaticMethod<object>("ConsentManagementProvider.CMPTestUtils", "InitializeWithLanguage", "Assembly-CSharp", new[] { "37" }); //TAGALOG
			System.Threading.Thread.Sleep(1000);
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressLoadMessage");
			pages.nativeAppLayer.pressLoadMessage();
			pages.firstLayerGO(true, true, true);
			pages.nativeAppLayer.waitForSdkDone();
			Console.WriteLine($"Current button for tap: nativeAppLayer.pressGDPRPmLayer");
        	pages.nativeAppLayer.pressGDPRPmLayer();
			Console.WriteLine($"Check for webView open: pmLayerGDPR.webViewIsOpen with TAGALOG language");
        	isOpen = pages.pmLayerGDPR.driverHelper.webViewIsOpen(pages.pmLayerGDPR.textViewPathTL);
			Assert.That(isOpen, Is.True);
		}

        [TearDown]
        public void Teardown()
        {
        	altDriver.Stop();
			if(platformIOS)
        		driverIOS.Dispose();
			if(platformAndroid)
				driverAndroid.Dispose();
			if (platformAndroid)
        		AltReversePortForwarding.RemoveReversePortForwardingAndroid();
			//shellHelper.StopAltTester();
			//shellHelper.StopAppium();
        }
    }
}
