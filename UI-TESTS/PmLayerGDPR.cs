namespace UnityAppiumTests
{
    public abstract class PmLayerGDPR: PmLayer
    {
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract string attributeName { get; }
        public abstract string attributeValue { get; }

        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath);
        public int getCheckedSwitchesNum() => base.getCheckedSwitchesNum(false, attributeName, attributeValue);
    } 

    public class PmLayerGDPRAndroid: PmLayerGDPR
    {            
        public override string textViewPath => "//android.widget.TextView[@text='GDPR Privacy Manager']";
        public override string saveAndExitPath => "//android.widget.Button[@text='Save & Exit']";
        public override string rejectAllPath => "//android.widget.Button[@text='Reject All']";
        public override string acceptAllPath => "//android.widget.Button[@text='Accept All']";
        public override string exitButtonPath => "//android.widget.Button[@text='Cancel']";
        public override string switchPrefix => "//android.widget.ToggleButton[@text='";
        public override string switchPostfix => "']";
        public override string[] switches => new[]
        {
            "Store and/or access information on a device",
            "Use limited data to select advertising",
            "Create profiles for personalised advertising",
            "Use profiles to select personalised advertising"
        };
        public override string attributeName => "checked";
        public override string attributeValue => "true";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;

        public PmLayerGDPRAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class PmLayerGDPRIOS: PmLayerGDPR
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='GDPR Privacy Manager']";
        public override string saveAndExitPath => "//XCUIElementTypeButton[@name='Save & Exit']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override string exitButtonPath => "//XCUIElementTypeButton[@name='Cancel']";
        public override string switchPrefix => "//XCUIElementTypeSwitch[@name='";
        public override string switchPostfix => "']";
        public override string[] switches => new[]
        {
            "Store and/or access information on a device",
            "Use limited data to select advertising",
            "Create profiles for personalised advertising",
            "Use profiles to select personalised advertising"
        };
        public override string attributeName => "value";
        public override string attributeValue => "1";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public PmLayerGDPRIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}