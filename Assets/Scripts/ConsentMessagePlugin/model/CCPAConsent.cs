using System.Collections;
using UnityEngine;

namespace GdprConsentLib
{
    public class CCPAConsent
    {
        //TODO: investigate necessity of readonly
        /*readonly*/ string status;
        IList rejectedVendors;
        IList rejectedCategories;
        bool rejectedAll;
        string uspstring;
        string /*JSONObject*/ thisContent;

        public CCPAConsent(AndroidJavaObject nativeCcpa)
        {
            this.status = nativeCcpa.Call<string>("getStatus");
            this.rejectedVendors = nativeCcpa.Call<IList>("getRejectedVendors");
            this.rejectedCategories = nativeCcpa.Call<IList>("getRejectedCategories");
            this.rejectedAll = nativeCcpa.Call<bool>("getRejectedAll");
            this.uspstring = nativeCcpa.Call<string>("getUspstring");
            this.thisContent = nativeCcpa.Call<AndroidJavaObject>("getThisContent").Call<string>("toString");
            Util.Log("thisContent -> " + thisContent);
        }
    }
}