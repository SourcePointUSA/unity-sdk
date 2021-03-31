using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GdprConsentLib
{
    public class GDPRConsent
    {
        //TODO: investigate analogue of java.lang.List (List<object> ?)
        IList /*List<object>*/ acceptedCategories;
        IList acceptedVendors;
        IList specialFeatures;
        IList legIntCategories;
        string euconsent;
        Dictionary<string, object> tcData;
        Dictionary<string, object> vendorsGrants;
        string /*JSONObject*/ thisContent;

        public GDPRConsent(AndroidJavaObject nativeGdpr)
        {
            var cats = nativeGdpr.Call<IList>("getAcceptedCategories");
            Util.Log("cats.Count -> " + cats.Count);
            this.acceptedCategories = cats;
            this.acceptedVendors = nativeGdpr.Call<IList>("getAcceptedVendors");
            this.specialFeatures = nativeGdpr.Call<IList>("getSpecialFeatures");
            this.legIntCategories = nativeGdpr.Call<IList>("getLegIntCategories");
            this.euconsent = nativeGdpr.Call<string>("getEuconsent");
            this.tcData = nativeGdpr.Call<Dictionary<string, object>>("getTcData");
            this.vendorsGrants = nativeGdpr.Call<Dictionary<string, object>>("getVendorsGrants");
            this.thisContent = nativeGdpr.Call<AndroidJavaObject>("getThisContent").Call<string>("toString");
            Util.Log("thisContent -> " + thisContent);
        }
    }
}