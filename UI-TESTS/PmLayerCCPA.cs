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
        public override string textViewPath { get { return "//android.widget.TextView[@text='Centro de Privacidad CCPA']"; } }
        public override string saveAndExitPath { get { return "//android.widget.Button[@text='Save & Exit']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override string exitButtonPath { get { return "//android.widget.Button[@text='Cancel']"; } }
        public override string switchPrefix { get { return "//android.view.View[@text='"; } }
        public override string switchPostfix { get { return "']/android.view.View[1]/android.widget.ToggleButton/android.widget.TextView"; } }
        public override string[] switches { get { return ["Category 1 Freewheel Freewheel", "Category 2 Seedtag Advertising S.L Seedtag Advertising S.L", "Category 3 Freewheel Freewheel"]; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;

        public PmLayerCCPAAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class PmLayerCCPAIOS: PmLayerCCPA
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
        
        public PmLayerCCPAIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}