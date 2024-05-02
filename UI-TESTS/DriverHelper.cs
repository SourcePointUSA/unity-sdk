namespace UnityAppiumTests
{
    public class DriverHelper
    {
        public string platform;
        public AndroidDriver<AndroidElement> driverAndroid;
        public IOSDriver<IOSElement> driverIOS;
        public DriverHelper(string platform, AndroidDriver<AndroidElement> driverAndroid, IOSDriver<IOSElement> driverIOS)
        {
            this.platform = platform;
            this.driverAndroid = driverAndroid;
            this.driverIOS = driverIOS;
        }
        public void SwipeUp()
        {
            var finger = new PointerInputDevice(PointerKind.Touch);
            var start = new Point(5, 2106);
            var end = new Point(3, 305);
            var swipe = new ActionSequence(finger);
            swipe.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, start.X, start.Y, TimeSpan.Zero));
            swipe.AddAction(finger.CreatePointerDown(MouseButton.Left));
            swipe.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, end.X, end.Y, TimeSpan.FromMilliseconds(1000)));
            swipe.AddAction(finger.CreatePointerUp(MouseButton.Left));
            if(platform == "Android")
                driverAndroid.PerformActions(new List<ActionSequence> { swipe });
            else
                driverIOS.PerformActions(new List<ActionSequence> { swipe });
        }
    }
}