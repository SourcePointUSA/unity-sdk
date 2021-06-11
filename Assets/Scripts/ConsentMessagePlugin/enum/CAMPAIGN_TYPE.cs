using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public enum CAMPAIGN_TYPE
    {
        GDPR = 0,
        IOS14 = 1, //exist ios only
        CCPA = 2,
        // UNKNOWN = 3 //exist ios only
    }

    public enum CAMPAIGN_TYPE_ANDROID
    {
        GDPR = 0,
        CCPA = 1
    }

    internal static class CAMPAIGN_TYPE_STRING_KEY
    {
        internal const string
        GDPR = "GDPR",
        CCPA = "CCPA";
    }
}