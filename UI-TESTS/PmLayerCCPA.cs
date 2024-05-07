namespace UnityAppiumTests
{
    public abstract class PmLayerCCPA: PmLayer
    {
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }

        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath);
    } 

    public class PmLayerCCPAAndroid: PmLayerCCPA
    {            
        public override string textViewPath => "//android.widget.TextView[@text='Centro de Privacidad CCPA']";
        public override string saveAndExitPath => "//android.widget.Button[@text='Save & Exit']";
        public override string rejectAllPath => "//android.widget.Button[@text='Reject All']";
        public override string acceptAllPath => "//android.widget.Button[@text='Accept All']";
        public override string exitButtonPath => "//android.widget.Button[@text='Cancel']";
        public override string switchPrefix => "//android.view.View[@text='";
        public override string switchPostfix => "']/android.view.View[1]/android.widget.ToggleButton/android.widget.TextView";

        public override string[] switches => new[]
        {
            "Category 1 Freewheel Freewheel", 
            "Category 2 Seedtag Advertising S.L Seedtag Advertising S.L",
            "Category 3 Freewheel Freewheel"
        };
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;

        public PmLayerCCPAAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class PmLayerCCPAIOS: PmLayerCCPA
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='Centro de Privacidad CCPA']";
        public override string saveAndExitPath => "//XCUIElementTypeButton[@name='Save & Exit']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override string exitButtonPath => "//XCUIElementTypeButton[@name='Cancel']";
        public override string switchPrefix => "(//XCUIElementTypeSwitch[@value='0'])[";
        public override string switchPostfix => "]";
        public override string[] switches => new[] { "1", "2", "3" };
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public PmLayerCCPAIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}