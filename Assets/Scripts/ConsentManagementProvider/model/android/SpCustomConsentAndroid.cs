using System.Text.Json.Serialization;
namespace ConsentManagementProviderLib.Android
{
    internal class SpCustomConsentAndroid
    {
        [JsonInclude] public CustomGdprAndroid gdpr;
    }
}