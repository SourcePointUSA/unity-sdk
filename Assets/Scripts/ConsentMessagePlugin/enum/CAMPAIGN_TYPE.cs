using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public enum CAMPAIGN_TYPE
    {
        GDPR,
        CCPA,
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