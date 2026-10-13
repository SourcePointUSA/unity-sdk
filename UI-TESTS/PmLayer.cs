namespace UnityAppiumTests
{
    public abstract class PmLayer
    {
        public abstract string rejectAllPath { get; }
        public abstract string acceptAllPath { get; }
        public abstract string textViewPath { get; }
        public abstract string saveAndExitPath { get; }
        public abstract string exitButtonPath { get; }
        public abstract string switchPrefix { get; }
        public abstract string switchPostfix { get; }
        public abstract string[] switches { get; }
        public abstract WebDriverWait wait { get; }
		#pragma warning disable CS8618
        public DriverHelper driverHelper;
		#pragma warning restore CS8618

        public void pressAcceptAll() => driverHelper.pressButton(acceptAllPath, textViewPath);
        public void pressRejectAll() => driverHelper.pressButton(rejectAllPath, textViewPath);
        public void pressSaveAndExit() => driverHelper.pressButton(saveAndExitPath, textViewPath);
        public void pressExit() => driverHelper.pressButton(exitButtonPath, textViewPath);
        public bool webViewIsOpen() => driverHelper.webViewIsOpen(textViewPath);

        protected virtual OpenQA.Selenium.By AndroidSwitchLocator(string switchName) =>
            OpenQA.Selenium.By.XPath(AndroidLocator.PrivacyManagerToggle(switchName));

        protected OpenQA.Selenium.By SwitchLocator(string switchName, string additionalPrefix = "", bool byIosClass = false)
        {
            if (driverHelper.platform == "Android")
                return AndroidSwitchLocator(switchName);
            if (byIosClass)
                return MobileBy.IosClassChain(switchPrefix + additionalPrefix + switchName + switchPostfix);
            return OpenQA.Selenium.By.XPath(switchPrefix + additionalPrefix + switchName + switchPostfix);
        }

        public void clickOnSwitches(int num = 1, bool needSwipe = false, string additionalPrefix = "", bool byIosClass = false)
        {
            if (needSwipe)
                driverHelper.SwipeUp();
            int clicks = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch;
                _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SwitchLocator(_switchName, additionalPrefix, byIosClass)));
                _switch.Click();
                clicks++;
                if (num <= clicks)
                    break;
            }
        }

        public int getCheckedSwitchesNum(bool needSwipe = false, string attributeName = "checked", string attributeValue = "true", string additionalPrefix = "")
        {
            if (needSwipe)
                driverHelper.SwipeUp();
            int num = 0;
            foreach (string _switchName in switches)
            {
                IWebElement _switch = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SwitchLocator(_switchName, additionalPrefix)));
                if(_switch.GetAttribute(attributeName) != null && _switch.GetAttribute(attributeName).Equals(attributeValue))
                    num++;
            }
            return num;
        }
    } 
}
