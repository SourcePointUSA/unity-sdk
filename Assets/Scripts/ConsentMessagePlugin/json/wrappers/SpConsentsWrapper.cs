using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpConsentsWrapper
    {
        [JsonInclude] public SpCcpaConsentWrapper ccpa;
        [JsonInclude] public SpGdprConsentWrapper gdpr;
    }
}