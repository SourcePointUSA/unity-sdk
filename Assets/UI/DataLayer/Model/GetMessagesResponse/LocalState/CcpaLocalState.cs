using System.Text.Json.Serialization;

public class CcpaLocalState : GdprLocalState
{
    [JsonInclude] public bool dnsDisplayed;
}