using System.Collections.ObjectModel;

namespace UnityAppiumTests
{
    public abstract class PreFirstLayer
    {
        public abstract int contextToRun { get; }
        public abstract string contextNameToRun { get; }
        public abstract WebDriverWait wait { get; }

        public string SelectFirstLayerGen<T>(T driver, Func<T, ReadOnlyCollection<string>> contexts)
        {
            wait.Until(d => contexts(driver).Count > contextToRun);

            ReadOnlyCollection<string> contextPress = contexts(driver);
            foreach (var context in contextPress)
			{
				Console.WriteLine(context);
				if (context.StartsWith(contextNameToRun))
				{
				    return context;
				    break;
				}
			}
			return "";
        }

        public abstract string SelectFirstLayer();
        public abstract void SetContex(string contex);
    }
    
    public class PreFirstLayerAndroid: PreFirstLayer
    {
        public override int contextToRun { get { return 1; } }
        public override string contextNameToRun { get { return "WEBVIEW"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        public AndroidDriver<AndroidElement> driver;
        
        public PreFirstLayerAndroid(WebDriverWait wait, AndroidDriver<AndroidElement> driverAndroid)
        {
            webDriverWait = wait;
            driver = driverAndroid;
        }

        public override string SelectFirstLayer()
        {
            return SelectFirstLayerGen<AndroidDriver<AndroidElement>>(driver, x => driver.Contexts);
        }

        public override void SetContex(string contex)
        {
            ((AndroidDriver<AndroidElement>)driver).Context = contex;
        }
    }
    
    public class PreFirstLayerIOS: PreFirstLayer
    {
        public override int contextToRun { get { return 0; } }
        public override string contextNameToRun { get { return "NATIVE_APP"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        public IOSDriver<IOSElement> driver;
        
        public PreFirstLayerIOS(WebDriverWait wait, IOSDriver<IOSElement> driverIOS)
        {
            webDriverWait = wait;
            driver = driverIOS;
        }

        public override string SelectFirstLayer()
        {
            string layer = SelectFirstLayerGen<IOSDriver<IOSElement>>(driver, x => driver.Contexts);
			IWebElement bringItOn = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath("//XCUIElementTypeButton[@name='Bring it on']"))); 
			bringItOn.Click();
            driver.SwitchTo().Alert().Accept();
            return layer;
        }
        
        public override void SetContex(string contex)
        {
            ((IOSDriver<IOSElement>)driver).Context = contex;
        }
    }
}