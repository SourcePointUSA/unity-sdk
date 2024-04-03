using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers; // SeleniumExtras is used for WaitHelpers
using System;
using System.Collections.Generic;

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

		[SetUp]
		public void Setup()
		{
			var desiredCaps = new AppiumOptions();
			desiredCaps.AddAdditionalCapability("platformName", TestContext.Parameters["platformName"]);
			desiredCaps.AddAdditionalCapability("deviceName", TestContext.Parameters["deviceName"]);
			desiredCaps.AddAdditionalCapability("appium:app", TestContext.Parameters["appium:app"]);
			desiredCaps.AddAdditionalCapability("appium:automationName", TestContext.Parameters["appium:automationName"]);
			if (platformAndroid)
			{
				// desiredCaps.AddAdditionalCapability("appium:chromedriverAutodownload", true);
				desiredCaps.AddAdditionalCapability("appium:chromedriverExecutable", TestContext.Parameters["appium:chromedriverExecutable"]);
				driverAndroid = new AndroidDriver<AndroidElement>(appiumServerUri, desiredCaps, initTimeoutSec);
			}
			if (platformIOS)
				driverIOS = new IOSDriver<IOSElement>(appiumServerUri, desiredCaps, initTimeoutSec);
		}

		[Test]
		public void ClickAcceptAllButtonTest()
		{
			if (driver == null)
			{
				Assert.Fail("Driver has not been initialized.");
			}
			
			// Wait until more than one Context is being shown (NATIVE_APP & WEBVIEW)
			WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

			int contextToRun = 0;
			if(platformAndroid)
				contextToRun = 1;

			string contextNameToRun = "NATIVE_APP";
			if(platformAndroid)
				contextNameToRun = "WEBVIEW";

			if(platformAndroid)
				wait.Until(d => ((AndroidDriver<AndroidElement>)d).Contexts.Count > contextToRun);
			else
				wait.Until(d => ((IOSDriver<IOSElement>)d).Contexts.Count > contextToRun);
			
			// Finding WEBVIEW Context
			if(platformAndroid)
			{
				foreach (var context in ((AndroidDriver<AndroidElement>)driver).Contexts)
				{
					Console.WriteLine(context); // For debugging: print available Contexts
					if (context.StartsWith(contextNameToRun))
					{
					    ((AndroidDriver<AndroidElement>)driver).Context = context;
					    break;
					}
				}
			}
			else
			{
				foreach (var context in ((IOSDriver<IOSElement>)driver).Contexts)
				{
					Console.WriteLine(context);
					if (context.StartsWith(contextNameToRun))
					{
					    ((IOSDriver<IOSElement>)driver).Context = context;
					    break;
					}
				}
			}
            
			if(platformIOS)
			{
				IWebElement bringItOn = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//XCUIElementTypeButton[@name='Bring it on']"))); 
				bringItOn.Click();
            	driver.SwitchTo().Alert().Accept();
			}

			// Ensure the Context is changed
			if (platformAndroid)
				Console.WriteLine($"Current context: {driverAndroid.Context}");
			else
				Console.WriteLine($"Current context: {driverIOS.Context}");

			// Now we are in the  WebView's Context and can interact with web-elements
			IWebElement acceptAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(TestContext.Parameters["AcceptAllFirstLayerGDPR"]))); 
			acceptAllButton.Click();
			
			// Assert after interaction
			Assert.That(true, Is.True); // An example of usage
		}


        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
