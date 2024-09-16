namespace UnityAppiumTests
{
    public class DriverHelper
    {
        public string platform;
        public AndroidDriver? driverAndroid;
        public IOSDriver? driverIOS;
        public WebDriverWait webDriverWait;

        public DriverHelper(string platform, AndroidDriver? driverAndroid, IOSDriver? driverIOS, WebDriverWait webDriverWait)
        {
            this.platform = platform;
            this.driverAndroid = driverAndroid;
            this.driverIOS = driverIOS;
            this.webDriverWait = webDriverWait;
        }
        public void SwipeUp()
        {
            var finger = new PointerInputDevice(PointerKind.Touch);
            var start = new Point(5, 2106);
            var end = new Point(3, 305);
            if (platform == "iOS")
            {
                start = new Point(14, 580);
                end = new Point(18, 50);
            }
            var swipe = new ActionSequence(finger);
            swipe.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, start.X, start.Y, TimeSpan.Zero));
            swipe.AddAction(finger.CreatePointerDown(MouseButton.Left));
            swipe.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, end.X, end.Y, TimeSpan.FromMilliseconds(1000)));
            swipe.AddAction(finger.CreatePointerUp(MouseButton.Left));
            if(platform == "Android")
                driverAndroid!.PerformActions(new List<ActionSequence> { swipe });
            else
                driverIOS!.PerformActions(new List<ActionSequence> { swipe });
        }

        public void pressButton(string buttonPath, string? expectedWebViewName = null, bool needSwipe = false, bool needWait = false)
        {
            if (expectedWebViewName != null)
                webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(expectedWebViewName)));
            if (needWait)
			    System.Threading.Thread.Sleep(1000);
            if (needSwipe)
                SwipeUp();
            if (needWait)
			    System.Threading.Thread.Sleep(1000);
            IWebElement button = webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(buttonPath))); 
			button.Click();
        }

        public bool webViewIsOpen(string expectedWebViewName)
        {
            webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(OpenQA.Selenium.By.XPath(expectedWebViewName)));
            return true;
        }
    }
}