namespace UnityAppiumTests
{
    public abstract class FirstLayerCCPA: FirstLayer
    {
        public abstract string exitButtonPath { get; }
    } 

    public class FirstLayerCCPAAndroid: FirstLayerCCPA
    {            
        public override string textViewPath => "//android.widget.TextView[@text='CCPA Message']";
        public override string showOptionsPath => "//android.widget.Button[@text='Show Options']";
        public override string rejectAllPath => "//android.widget.Button[@text='Reject All']";
        public override string acceptAllPath => "//android.widget.Button[@text='Accept All']";
        public override string exitButtonPath => "//android.widget.TextView[@text='X']";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerCCPAAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerCCPAIOS: FirstLayerCCPA
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='CCPA Message']";
        public override string showOptionsPath => "//XCUIElementTypeButton[@name='Show Options']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override string exitButtonPath => "//XCUIElementTypeStaticText[@name='X']";
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerCCPAIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}