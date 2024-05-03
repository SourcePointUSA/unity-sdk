namespace UnityAppiumTests
{
    public abstract class PmLayerGDPR: PmLayer
    {
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }

        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath);
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