using System.Text.Json.Serialization;

public class PostConsentResponse
{
    [JsonInclude] public string uuid;
    [JsonInclude] public LocalState localState;
    [JsonInclude] public PostConsentUserConsent userConsent;
}