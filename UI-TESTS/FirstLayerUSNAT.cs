namespace UnityAppiumTests
{
    public abstract class FirstLayerUSNAT: FirstLayer
    {
        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath, true, true);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath, true, true);
    } 

    public class FirstLayerUSNATAndroid: FirstLayerUSNAT
    {            
        public override string textViewPath => "//android.widget.TextView[@text='USNat Message']";
        public override string showOptionsPath => "//android.widget.Button[@text='Show Options']";
        public override string rejectAllPath => "//android.widget.Button[@text='Reject All']";
        public override string acceptAllPath => "//android.widget.Button[@text='Accept All']";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerUSNATIOS: FirstLayerUSNAT
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='USNat Message']";
        public override string showOptionsPath => "//XCUIElementTypeButton[@name='Show Options']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}