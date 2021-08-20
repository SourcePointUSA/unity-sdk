using System.Text.Json.Serialization;

public class CmpBaseConsentVendorModel
{
    [JsonInclude] public string name;
    [JsonInclude] public string policyUrl;
}
