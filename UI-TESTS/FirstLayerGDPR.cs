namespace UnityAppiumTests
{
    public abstract class FirstLayerGDPR
    {
        public abstract string textViewPath { get; }
        public abstract string showOptionsPath { get; }
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract string exitButtonPath { get; }
        public abstract WebDriverWait wait { get; }

        public void pressAcceptAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
			System.Threading.Thread.Sleep(1000);
            IWebElement acceptAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(acceptAllPath))); 
			acceptAllButton.Click();
        }

        public void pressRejectAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
			System.Threading.Thread.Sleep(1000);
            IWebElement rejectAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(rejectAllPath))); 
			rejectAllButton.Click();
        }
    } 

    public class FirstLayerGDPRAndroid: FirstLayerGDPR
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='GDPR Message']"; } }
        public override string showOptionsPath { get { return "//android.widget.Button[@text='Show Options']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override string exitButtonPath { get { return "//android.widget.TextView[@text='X']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;

        public FirstLayerGDPRAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerGDPRIOS: FirstLayerGDPR
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='GDPR Message']"; } }
        public override string showOptionsPath { get { return "//XCUIElementTypeButton[@name='Show Options']"; } }
        public override string rejectAllPath { get { return "//XCUIElementTypeButton[@name='Reject All']"; } }
        public override string acceptAllPath { get { return "//XCUIElementTypeButton[@name='Accept All']"; } }
        public override string exitButtonPath { get { return "//XCUIElementTypeStaticText[@name='X']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerGDPRIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}