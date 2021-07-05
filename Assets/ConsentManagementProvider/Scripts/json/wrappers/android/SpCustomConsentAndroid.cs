using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpCustomConsentAndroid
    {
        [JsonInclude] public SpGdprConsentWrapperAndroid gdpr;
    }
}