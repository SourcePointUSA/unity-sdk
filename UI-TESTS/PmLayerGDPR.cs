namespace UnityAppiumTests
{
    public abstract class PmLayerGDPR: PmLayer
    {
        public abstract string textViewPathES { get; }
        public abstract string textViewPathTL { get; }
        public abstract string attributeName { get; }
        public abstract string attributeValue { get; }

        public int getCheckedSwitchesNum() => base.getCheckedSwitchesNum(false, attributeName, attributeValue);
    } 

    public class PmLayerGDPRAndroid: PmLayerGDPR
    {            
        public override string textViewPath => AndroidLocator.ElementWithText("GDPR Privacy Manager");
        public override string textViewPathES => AndroidLocator.ElementWithText("Centro de Privacidad GDPR");
        public override string textViewPathTL => AndroidLocator.ElementWithText("Tagalog");
        public override string saveAndExitPath => AndroidLocator.ButtonWithLabel("Save & Exit");
        public override string rejectAllPath => AndroidLocator.ButtonWithLabel("Reject All");
        public override string acceptAllPath => AndroidLocator.ButtonWithLabel("Accept All");
        public override string exitButtonPath => AndroidLocator.ButtonWithLabel("Cancel");
        public override string switchPrefix => "(//android.widget.Switch)[";
        public override string switchPostfix => "]";
        public override string[] switches => new[]
        {
            "1",
            "2",
            "3",
            "4"
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
        public override string textViewPathES => "//XCUIElementTypeStaticText[@name='Centro de Privacidad GDPR']";
        public override string textViewPathTL => "//XCUIElementTypeStaticText[@name='Tagalog']";
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
