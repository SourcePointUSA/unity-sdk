using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpConsentsWrapperAndroid
    {
        [JsonInclude] public SpCcpaConsentWrapperAndroid ccpa;
        [JsonInclude] public SpGdprConsentWrapperAndroid gdpr;
    }

}