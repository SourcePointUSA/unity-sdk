using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpGdprConsentWrapper
    {
        [JsonInclude] public bool applies;
        [JsonInclude] public GdprConsentWrapper consents;
    }
}