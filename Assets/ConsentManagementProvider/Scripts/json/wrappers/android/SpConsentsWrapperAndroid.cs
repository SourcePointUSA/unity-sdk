using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpConsentsWrapperAndroid
    {
        [JsonInclude] public CcpaConsentWrapper? ccpa;
        [JsonInclude] public SpGdprConsentWrapperAndroid? gdpr;
    }
}