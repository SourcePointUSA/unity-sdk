namespace UnityAppiumTests
{
    public abstract class FirstLayerUSNAT: FirstLayer
    {
        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath, true, true);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath, true, true);
    } 

    public class FirstLayerUSNATAndroid: FirstLayerUSNAT
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='USNat Message']"; } }
        public override string showOptionsPath { get { return "//android.widget.Button[@text='Show Options']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerUSNATIOS: FirstLayerUSNAT
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='USNat Message']"; } }
        public override string showOptionsPath { get { return "//XCUIElementTypeButton[@name='Show Options']"; } }
        public override string rejectAllPath { get { return "//XCUIElementTypeButton[@name='Reject All']"; } }
        public override string acceptAllPath { get { return "//XCUIElementTypeButton[@name='Accept All']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}