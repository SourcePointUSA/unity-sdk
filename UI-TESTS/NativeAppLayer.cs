namespace UnityAppiumTests
{
    public class NativeAppLayer
    {
        const string textComponentName = "UnityEngine.UI.Text";
    	const string textMethodName = "get_text";
    	const string textAssemblyName = "UnityEngine.UI";

    	const string ConsentValueText = "Consent String Value Text";
    	const string AuthIdText = "AuthId Text";
        const string SdkStatusText = "Sdk Status";
        const string LoadMessageButton = "Load Message";
    	const string GDPRPmButton = "GDPR Privacy Settings Button";
    	const string CCPAPmButton = "CCPA Privacy Settings Button";
    	const string USNATPmButton = "USNAT Privacy Settings Button";
        const string ClearAllButton = "Clear Data Button";
        AltDriver altDriver;

        public NativeAppLayer(AltDriver driver) => altDriver = driver;

        public string getConsentValueText()
        {
    		var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, ConsentValueText);
    		return altElement.CallComponentMethod<string>(textComponentName, textMethodName, textAssemblyName, new object[] { });
        }

        public string getAuthIdText()
        {
    		var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, AuthIdText);
    		return altElement.CallComponentMethod<string>(textComponentName, textMethodName, textAssemblyName, new object[] { });
        }

        public string getSdkStatus()
        {
            var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, SdkStatusText);
            return altElement.CallComponentMethod<string>(textComponentName, textMethodName, textAssemblyName, new object[] { });
        }

        public void waitForSdkDone(string status = "SDK:Finished")
        {
			Console.WriteLine("Wait for sdk status = " + status);
            string _status = "";
            int iter = 0;
            do
            {
              _status = getSdkStatus();
              System.Threading.Thread.Sleep(500);
              iter++;
            } while (_status != status && iter<=10);
        }

        public void pressLoadMessage() => altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, LoadMessageButton).Click();
        public void pressGDPRPmLayer() => altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, GDPRPmButton).Click();
        public void pressCCPAPmLayer() => altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, CCPAPmButton).Click();
        public void pressUSNATPmLayer() => altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, USNATPmButton).Click();
        public void pressClearAll() => altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, ClearAllButton).Click();
    }
}