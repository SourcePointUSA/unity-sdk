namespace UnityAppiumTests
{
    public abstract class FirstLayerUSNAT
    {
        public abstract string textViewPath { get; }
        public abstract string showOptionsPath { get; }
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract WebDriverWait wait { get; }

        public void pressAcceptAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(showOptionsPath))); 
            IWebElement acceptAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(acceptAllPath))); 
			acceptAllButton.Click();
        }

        public void pressRejectAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(showOptionsPath))); 
            IWebElement rejectAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(rejectAllPath))); 
			rejectAllButton.Click();
        }
    } 

    public class FirstLayerUSNATAndroid: FirstLayerUSNAT
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='USNat Message']"; } }
        public override string showOptionsPath { get { return "//android.widget.Button[@text='Show Options']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerUSNATIOS: FirstLayerUSNAT
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='USNat Message']"; } }
        public override string showOptionsPath { get { return "//XCUIElementTypeButton[@name='Show Options']"; } }
        public override string rejectAllPath { get { return "//XCUIElementTypeButton[@name='Reject All']"; } }
        public override string acceptAllPath { get { return "//XCUIElementTypeButton[@name='Accept All']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}