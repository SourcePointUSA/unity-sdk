using System.Text.Json.Serialization;

public class LocalState
{
    [JsonInclude] public Ios14LocalState ios14;
    [JsonInclude] public GdprLocalState gdpr;
    [JsonInclude] public CcpaLocalState ccpa;
}