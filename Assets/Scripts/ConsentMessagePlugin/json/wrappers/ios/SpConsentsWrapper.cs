using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpConsentsWrapper
    {
        [JsonInclude] public SpCcpaConsentWrapper ccpa;
        [JsonInclude] public SpGdprConsentWrapper gdpr;
    }
}