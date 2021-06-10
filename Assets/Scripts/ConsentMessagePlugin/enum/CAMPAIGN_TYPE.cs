using System.Collections;
using UnityEngine;

namespace GdprConsentLib
{
    public enum CAMPAIGN_TYPE
    {
        GDPR = 0,
        IOS14 = 1, //exist ios only
        CCPA = 2,
        // UNKNOWN = 3 //exist ios only
    }

    internal static class CAMPAIGN_TYPE_STRING_KEY
    {
        internal const string
        GDPR = "GDPR",
        CCPA = "CCPA";
    }
}