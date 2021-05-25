using System.Collections;

namespace ConsentManagementProviderLib
{
    [System.Serializable]
    public class CCPAConsent
    {
        //TODO: investigate necessity of readonly
        public /*readonly*/ string status;
        public IList rejectedVendors;
        public IList rejectedCategories;
        //bool rejectedAll;
        public string uspstring;
        //string /*JSONObject*/ thisContent;

        /*
        public CCPAConsent(AndroidJavaObject nativeCcpa)
        {
            this.status = nativeCcpa.Call<string>("getStatus");
            this.rejectedVendors = nativeCcpa.Call<IList>("getRejectedVendors");
            this.rejectedCategories = nativeCcpa.Call<IList>("getRejectedCategories");
            //this.rejectedAll = nativeCcpa.Call<bool>("getRejectedAll");
            this.uspstring = nativeCcpa.Call<string>("getUspstring");
            //this.thisContent = nativeCcpa.Call<AndroidJavaObject>("getThisContent").Call<string>("toString");
            //Util.Log("thisContent -> " + thisContent);
        }
        */
    }
}