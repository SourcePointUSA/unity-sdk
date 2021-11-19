using System.Text.Json.Serialization;

public class CmpSpecialFeatureVendorModel : CmpBaseConsentVendorModel
{
    [JsonInclude] public string vendorId;
}
