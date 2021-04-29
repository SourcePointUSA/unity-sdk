using System.Collections;
using UnityEngine;

namespace GdprConsentLib
{
    public enum CAMPAIGN_ENV
    {
        STAGE,
        PUBLIC
    }

    internal static class CAMPAIGN_ENV_STRING_KEY
    {
        internal const string
        STAGE = "stage",
        PUBLIC = "prod";
    }
}