namespace UnityAppiumTests
{
    public abstract class PmLayerUSNAT: PmLayer
    {
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }

        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath);
        public void pressSaveAndExit() => driverHelper.pressButton(saveAndExitPath, textViewPath, true, true);
        public void pressExit() => driverHelper.pressButton(exitButtonPath, textViewPath, true, true);
        public void clickOnSwitches(int num = 1) => base.clickOnSwitches(num, true);
        public int getCheckedSwitchesNum() 
        {
            int num;
            if (driverHelper.platform == "iOS")
            {
			    System.Threading.Thread.Sleep(1000);
                num = driverHelper.driverIOS.FindElements(OpenQA.Selenium.By.XPath(switchPrefix+switches.First()+switchPostfix)).Count;
            }
            else
                num = base.getCheckedSwitchesNum(true);
            return num;
        }

        public string getAcceptRejectState()
        {
            if (driverHelper.platform == "iOS") //While 'platformIOS' we can`t get state of usnat
                return "custom";
            
            IWebElement _switchFirst = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+switches.First()+switchPostfix)));
            bool _switchFirstVal = false;
            if(_switchFirst.GetAttribute("checked") != null && _switchFirst.GetAttribute("checked").Equals("true"))
                _switchFirstVal = true;
            
            IWebElement _switchSecond = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+switches.Last()+switchPostfix)));
            bool _switchSecondVal = false;
            if(_switchSecond.GetAttribute("checked") != null && _switchSecond.GetAttribute("checked").Equals("true"))
                _switchSecondVal = true;

            if (_switchFirstVal && !_switchSecondVal)
                return "accepted";
            if (!_switchFirstVal && _switchSecondVal)
                return "rejected";
            return "custom";
        }
    } 

    public class PmLayerUSNATAndroid: PmLayerUSNAT
    {            
        public override string textViewPath => "//android.widget.TextView[@text='USNat Privacy Manager']";
        public override string saveAndExitPath => "//android.widget.Button[@text='Save & Exit']";
        public override string rejectAllPath => "//android.widget.Button[@text='Reject All']";
        public override string acceptAllPath => "//android.widget.Button[@text='Accept All']";
        public override string exitButtonPath => "//android.widget.Button[@text='Cancel']";
        public override string switchPrefix => "(//android.widget.ToggleButton[@text='Off On'])[";
        public override string switchPostfix => "]";
        public override string[] switches => new[] {"1", "2"};
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;

        public PmLayerUSNATAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class PmLayerUSNATIOS: PmLayerUSNAT
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='USNat Privacy Manager']";
        public override string saveAndExitPath => "//XCUIElementTypeButton[@name='Save & Exit']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override string exitButtonPath => "//XCUIElementTypeButton[@name='Cancel']";
        public override string switchPrefix => "//XCUIElementTypeSwitch[@name='Off On";
        public override string switchPostfix => "']";
        public override string[] switches => new[] { "' and @value='1" }; 
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public PmLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}
