using System.Text.Json.Serialization;
namespace ConsentManagementProviderLib
{
    public class SpCustomConsentAndroid
    {
        [JsonInclude]
        public SpCustomGdprAndroid gdpr;
    }
}