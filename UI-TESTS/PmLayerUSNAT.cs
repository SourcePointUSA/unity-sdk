namespace UnityAppiumTests
{
    public abstract class PmLayerUSNAT
    {
        public abstract string textViewPath { get; }
        public abstract string saveAndExitPath { get; }
        public abstract string exitButtonPath { get; }
        public abstract string switchPrefix { get; }
        public abstract string switchPostfix { get; }
        public abstract string[] switches { get; }
        public abstract WebDriverWait wait { get; }
        public DriverHelper driverHelper;

        public void pressExit()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath))); 
			System.Threading.Thread.Sleep(1000);
            driverHelper.SwipeUp();
			System.Threading.Thread.Sleep(1000);
            IWebElement exitButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(exitButtonPath))); 
			exitButton.Click();
        }

        public bool webViewIsOpen()
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(textViewPath)));
            return true;
        }

        public int getCheckedSwitchesNum()
        {
            int num = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+_switchName+switchPostfix)));
                if(_switch.GetAttribute("checked") != null && _switch.GetAttribute("checked").Equals(true))
                    num++;
            }
            return num;
        }
    } 

    public class PmLayerUSNATAndroid: PmLayerUSNAT
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='USNat Privacy Manager']"; } }
        public override string saveAndExitPath { get { return "//android.widget.Button[@text='Save & Exit']"; } }
        public override string exitButtonPath { get { return "//android.widget.Button[@text='Cancel']"; } }
        public override string switchPrefix { get { return "(//android.widget.ToggleButton[@text='Off On'])["; } }
        public override string switchPostfix { get { return "]"; } }
        public override string[] switches { get { return ["1", "2"]; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;

        public PmLayerUSNATAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class PmLayerUSNATIOS: PmLayerUSNAT
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='GDPR Privacy Manager']"; } }
        public override string saveAndExitPath { get { return "//XCUIElementTypeButton[@name='Save & Exit']"; } }
        public override string exitButtonPath { get { return "//XCUIElementTypeStaticText[@name='Cancel']"; } }
        public override string switchPrefix { get { return "?"; } }
        public override string switchPostfix { get { return "']"; } }
        public override string[] switches { get { return ["1", "2"]; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public PmLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}
