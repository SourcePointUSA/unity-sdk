namespace UnityAppiumTests
{
    public abstract class PmLayerUSNAT: PmLayer
    {
        public void pressSaveAndExit() => driverHelper.pressButton(saveAndExitPath, textViewPath, true, true);
        public void pressExit() => driverHelper.pressButton(exitButtonPath, textViewPath, true, true);
        public void clickOnSwitches(int num = 1) => base.clickOnSwitches(num, true);
        public int getCheckedSwitchesNum() => base.getCheckedSwitchesNum(true);
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
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='GDPR Privacy Manager']";
        public override string saveAndExitPath => "//XCUIElementTypeButton[@name='Save & Exit']";
        public override string exitButtonPath => "//XCUIElementTypeStaticText[@name='Cancel']";
        public override string switchPrefix => "?";
        public override string switchPostfix => "']";
        public override string[] switches => new[] {"1", "2"};
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public PmLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}
