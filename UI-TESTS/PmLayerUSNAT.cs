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
