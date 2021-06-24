using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpConsentsWrapperAndroid
    {
        [JsonInclude] public SpCcpaConsentWrapperAndroid ccpa;
        [JsonInclude] public SpGdprConsentWrapperAndroid gdpr;
    }

}