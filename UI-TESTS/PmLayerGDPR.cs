namespace UnityAppiumTests
{
    public abstract class PmLayerGDPR
    {
        public abstract string textViewPath { get; }
        public abstract string saveAndExitPath { get; }
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract string exitButtonPath { get; }
        public abstract string switchPrefix { get; }
        public abstract string switchPostfix { get; }
        public abstract string[] switches { get; }
        public abstract WebDriverWait wait { get; }

        public void pressAcceptAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
            IWebElement acceptAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(acceptAllPath))); 
			acceptAllButton.Click();
        }

        public void pressRejectAll()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
            IWebElement rejectAllButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(rejectAllPath))); 
			rejectAllButton.Click();
        }

        public void pressSaveAndExit()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
            IWebElement saveAndExitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(saveAndExitPath))); 
			saveAndExitButton.Click();
        }

        public void pressExit()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
            IWebElement exitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(exitButtonPath))); 
			exitButton.Click();
        }

        public bool webViewIsOpen()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath)));
            return true;
        }

        public int clickOnSwitches(int num = 1)
        {
            int clicks = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+_switchName+switchPostfix)));
                _switch.Click();
                clicks++;
                if (num <= clicks)
                    break;
            }
            return num;
        }

        public int getCheckedSwitchesNum()
        {
            int num = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+_switchName+switchPostfix)));
                if(_switch.GetAttribute("checked") != null && _switch.GetAttribute("checked").Equals("true"))
                    num++;
            }
            return num;
        }
    } 

    public class PmLayerGDPRAndroid: PmLayerGDPR
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='GDPR Privacy Manager']"; } }
        public override string saveAndExitPath { get { return "//android.widget.Button[@text='Save & Exit']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override string exitButtonPath { get { return "//android.widget.Button[@text='Cancel']"; } }
        public override string switchPrefix { get { return "//android.widget.ToggleButton[@text='"; } }
        public override string switchPostfix { get { return "']"; } }
        public override string[] switches { get { return ["Store and/or access information on a device", "Use limited data to select advertising", "Create profiles for personalised advertising", "Use profiles to select personalised advertising"]; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;

        public PmLayerGDPRAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class PmLayerGDPRIOS: PmLayerGDPR
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='GDPR Privacy Manager']"; } }
        public override string saveAndExitPath { get { return "//XCUIElementTypeButton[@name='Save & Exit']"; } }
        public override string rejectAllPath { get { return "//XCUIElementTypeButton[@name='Reject All']"; } }
        public override string acceptAllPath { get { return "//XCUIElementTypeButton[@name='Accept All']"; } }
        public override string exitButtonPath { get { return "//XCUIElementTypeStaticText[@name='Cancel']"; } }
        public override string switchPrefix { get { return "?"; } }
        public override string switchPostfix { get { return "']"; } }
        public override string[] switches { get { return ["Store and/or access information on a device", "Use limited data to select advertising", "Create profiles for personalised advertising", "Use profiles to select personalised advertising"]; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public PmLayerGDPRIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}