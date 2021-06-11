using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    public enum CAMPAIGN_ENV
    {
        STAGE = 0,
        PUBLIC = 1,
    }

    internal static class CAMPAIGN_ENV_STRING_KEY
    {
        internal const string
        STAGE = "stage",
        PUBLIC = "prod";
    }
}