using System.Collections;
using UnityEngine;

namespace ConsentManagementProviderLib
{
    internal static class CALLBACK_TYPE
    {
        internal const string 
        NotSet = "CallbackNotSet",
        System = "System",
        Default = "Default",
        OnConsentReady = "OnConsentReady",
        OnConsentUIReady = "OnConsentUIReady",
        OnConsentAction = "OnConsentAction",
        OnConsentUIFinished = "OnConsentUIFinished",
        OnErrorCallback = "OnErrorCallback",
        OnSPFinished = "OnSPFinished",
        OnSPUIFinished = "OnSPUIFinished",
        OnCustomConsent = "OnCustomConsent";
    }
}