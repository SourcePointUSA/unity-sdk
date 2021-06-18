using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpGdprConsentWrapper
    {
        [JsonInclude] public object applies;
        [JsonInclude] public GdprConsentWrapper consents;
    }
}