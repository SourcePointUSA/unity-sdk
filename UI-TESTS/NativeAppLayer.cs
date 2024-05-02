namespace UnityAppiumTests
{
    public class NativeAppLayer
    {
        const string textComponentName = "UnityEngine.UI.Text";
    	const string textMethodName = "get_text";
    	const string textAssemblyName = "UnityEngine.UI";

    	const string ConsentValueText = "Consent String Value Text";
    	const string GDPRPmButton = "GDPR Privacy Settings Button";
    	const string CCPAPmButton = "CCPA Privacy Settings Button";
    	const string USNATPmButton = "USNAT Privacy Settings Button";
        AltDriver altDriver;

        public NativeAppLayer(AltDriver driver) => altDriver = driver;

        public string getConsentValueText()
        {
    		var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, ConsentValueText);
    		return altElement.CallComponentMethod<string>(textComponentName, textMethodName, textAssemblyName, new object[] { });
        }
    }
}