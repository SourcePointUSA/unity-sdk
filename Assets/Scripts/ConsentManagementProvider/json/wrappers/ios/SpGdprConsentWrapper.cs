using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpGdprConsentWrapper
    {
        [JsonInclude] public object applies;
        [JsonInclude] public GdprConsentWrapper consents;
    }
}