namespace UnityAppiumTests
{
    public abstract class PmLayer
    {
        public abstract string textViewPath { get; }
        public abstract string saveAndExitPath { get; }
        public abstract string exitButtonPath { get; }
        public abstract string switchPrefix { get; }
        public abstract string switchPostfix { get; }
        public abstract string[] switches { get; }
        public abstract WebDriverWait wait { get; }
        public DriverHelper driverHelper;

        public void pressSaveAndExit() => driverHelper.pressButton(saveAndExitPath, textViewPath);
        public void pressExit() => driverHelper.pressButton(exitButtonPath, textViewPath);
        public bool webViewIsOpen() => driverHelper.webViewIsOpen(textViewPath);

        public void clickOnSwitches(int num = 1, bool needSwipe = false)
        {
            if (needSwipe)
                driverHelper.SwipeUp();
            int clicks = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+_switchName+switchPostfix)));
                _switch.Click();
                clicks++;
                if (num <= clicks)
                    break;
            }
        }

        public int getCheckedSwitchesNum(bool needSwipe = false, string attributeName = "checked", string attributeValue = "true")
        {
            if (needSwipe)
                driverHelper.SwipeUp();
            int num = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(OpenQA.Selenium.By.XPath(switchPrefix+_switchName+switchPostfix)));
                if(_switch.GetAttribute(attributeName) != null && _switch.GetAttribute(attributeName).Equals(attributeValue))
                    num++;
            }
            return num;
        }
    } 
}
