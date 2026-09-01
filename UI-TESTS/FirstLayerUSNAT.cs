namespace UnityAppiumTests
{
    public abstract class FirstLayerUSNAT: FirstLayer
    {
        public new void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath, true, true);
        public new void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath, true, true);
    } 

    public class FirstLayerUSNATAndroid: FirstLayerUSNAT
    {            
        public override string textViewPath => "//android.widget.TextView[@text='USNat Message']";
        public override string textViewPathES => "//android.widget.TextView[@text='USNat Message']";
        public override string showOptionsPath => AndroidLocator.ButtonWithLabel("Show Options");
        public override string rejectAllPath => AndroidLocator.ButtonWithLabel("Reject All");
        public override string acceptAllPath => AndroidLocator.ButtonWithLabel("Accept All");
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerUSNATIOS: FirstLayerUSNAT
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='USNat Message']";
        public override string textViewPathES => "//XCUIElementTypeStaticText[@name='USNat Message']";
        public override string showOptionsPath => "//XCUIElementTypeButton[@name='Show Options']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerUSNATIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}
