using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public enum CAMPAIGN_TYPE
    {
        GDPR,
        CCPA,
    }

    internal static class CAMPAIGN_TYPE_STRING_KEY
    {
        internal const string
        GDPR = "GDPR",
        CCPA = "CCPA";
    }
}