namespace UnityAppiumTests
{
    public abstract class FirstLayerCCPA: FirstLayer
    {
        public abstract string exitButtonPath { get; }
    } 

    public class FirstLayerCCPAAndroid: FirstLayerCCPA
    {            
        public override string textViewPath { get { return "//android.widget.TextView[@text='CCPA Message']"; } }
        public override string showOptionsPath { get { return "//android.widget.Button[@text='Show Options']"; } }
        public override string rejectAllPath { get { return "//android.widget.Button[@text='Reject All']"; } }
        public override string acceptAllPath { get { return "//android.widget.Button[@text='Accept All']"; } }
        public override string exitButtonPath { get { return "//android.widget.TextView[@text='X']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerCCPAAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerCCPAIOS: FirstLayerCCPA
    {
        public override string textViewPath { get { return "//XCUIElementTypeStaticText[@name='CCPA Message']"; } }
        public override string showOptionsPath { get { return "//XCUIElementTypeButton[@name='Show Options']"; } }
        public override string rejectAllPath { get { return "//XCUIElementTypeButton[@name='Reject All']"; } }
        public override string acceptAllPath { get { return "//XCUIElementTypeButton[@name='Accept All']"; } }
        public override string exitButtonPath { get { return "//XCUIElementTypeStaticText[@name='X']"; } }
        public override WebDriverWait wait {get { return webDriverWait; } }
        public WebDriverWait webDriverWait;
        
        public FirstLayerCCPAIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}