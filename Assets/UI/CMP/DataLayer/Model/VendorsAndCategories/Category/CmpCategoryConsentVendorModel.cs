using System.Text.Json.Serialization;

public class CmpCategoryConsentVendorModel : CmpSpecialFeatureVendorModel
{
    [JsonInclude] public string vendorType;
    //[JsonInclude] public List<> cookies;       //TODO
}
