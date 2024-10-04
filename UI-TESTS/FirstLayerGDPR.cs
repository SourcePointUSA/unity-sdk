namespace UnityAppiumTests
{
    public abstract class FirstLayerGDPR: FirstLayer
    {
        public abstract string exitButtonPath { get; }
        public abstract bool needSwipe { get; }

        public new void pressAcceptAll() 
        {
            if (needSwipe)
                driverHelper.pressButton(acceptAllPath, textViewPath, true, true);
            else
                base.pressAcceptAll();
        }
    } 

    public class FirstLayerGDPRAndroid: FirstLayerGDPR
    {            
        public override string textViewPath => "//android.widget.TextView[@text='GDPR Message']";
        public override string textViewPathES => "//android.widget.TextView[@text='GDPR Message']";
        public override string showOptionsPath => "//android.widget.Button[@text='Show Options']";
        public override string rejectAllPath => "//android.widget.Button[@text='Reject All']";
        public override string acceptAllPath => "//android.widget.Button[@text='Accept All']";
        public override string exitButtonPath => "//android.widget.TextView[@text='X']";
        public override bool needSwipe => false;
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;

        public FirstLayerGDPRAndroid(WebDriverWait wait) => webDriverWait = wait;
    }

    public class FirstLayerGDPRIOS: FirstLayerGDPR
    {
        public override string textViewPath => "//XCUIElementTypeStaticText[@name='GDPR Message']";
        public override string textViewPathES => "//XCUIElementTypeStaticText[@name='GDPR Message']";
        public override string showOptionsPath => "//XCUIElementTypeButton[@name='Show Options']";
        public override string rejectAllPath => "//XCUIElementTypeButton[@name='Reject All']";
        public override string acceptAllPath => "//XCUIElementTypeButton[@name='Accept All']";
        public override string exitButtonPath => "//XCUIElementTypeStaticText[@name='X']";
        public override bool needSwipe => true;
        public override WebDriverWait wait => webDriverWait;
        public WebDriverWait webDriverWait;
        
        public FirstLayerGDPRIOS(WebDriverWait wait) => webDriverWait = wait;
    }
}