namespace UnityAppiumTests
{
    public abstract class PmLayerUSNAT: PmLayer
    {
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
    } 

    public class PmLayerUSNATAndroid: PmLayerUSNAT
    {            
        public override string textViewPath => "//android.widget.TextView[@text='USNat Privacy Manager']";
        public override string saveAndExitPath => "//android.widget.Button[@text='Save & Exit']";
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
        public override string exitButtonPath => "//XCUIElementTypeButton[@name='Cancel']";
        public override string switchPrefix => "//XCUIElementTypeSwitch[@name='Off On";
        public override string switchPostfix => "']";
        public override string[] switches => new[] { "' and @value='1" }; 
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public PmLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}
