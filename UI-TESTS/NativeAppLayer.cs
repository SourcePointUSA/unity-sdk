namespace UnityAppiumTests
{
    public class NativeAppLayer
    {
        const string textComponentName = "UnityEngine.UI.Text";
    	const string textMethodName = "get_text";
    	const string textAssemblyName = "UnityEngine.UI";
    	const string elementText = "-";
        AltDriver altDriver;

        public NativeAppLayer(AltDriver driver) => altDriver = driver;

        public string getConsentValueText()
        {
    		var altElement = altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, "Consent String Value Text");
    		return altElement.CallComponentMethod<string>(textComponentName, textMethodName, textAssemblyName, new object[] { });
        }
    }
}