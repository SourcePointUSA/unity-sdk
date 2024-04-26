namespace UnityAppiumTests
{
    public abstract class FirstLayerCCPA
    {
        public abstract string textViewPath { get; }
        public abstract string showOptionsPath { get; }
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract string exitButtonPath { get; }
        public abstract WebDriverWait wait { get; }

        public void pressAcceptAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(showOptionsPath))); 
            IWebElement acceptAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(acceptAllPath))); 
			acceptAllButton.Click();
        }
    } 

    public class FirstLayerCCPAAndroid: FirstLayerCCPA
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='CCPA Message']"; } }
        public override string showOptionsPath { get { return "//android.widget.Button[@text='Show Options']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override string exitButtonPath { get { return "//android.widget.TextView[@text='X']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerCCPAAndroid(WebDriverWait wait)
        {
            webDriverWait = wait;
        }
    }

    public class FirstLayerCCPAIOS: FirstLayerCCPA
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='CCPA Message']"; } }
        public override string showOptionsPath { get { return "//XCUIElementTypeButton[@name='Show Options']"; } }
        public override string rejectAllPath { get { return "//XCUIElementTypeButton[@name='Reject All']"; } }
        public override string acceptAllPath { get { return "//XCUIElementTypeButton[@name='Accept All']"; } }
        public override string exitButtonPath { get { return "//XCUIElementTypeStaticText[@name='X']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerCCPAIOS(WebDriverWait wait)
        {
            webDriverWait = wait;
        }
    }
}