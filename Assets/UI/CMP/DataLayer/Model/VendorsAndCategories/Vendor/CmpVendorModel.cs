using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CmpVendorModel : CmpSpecialFeatureVendorModel
{
    [JsonInclude] public string vendorType;
    [JsonInclude] public string cookieHeader;
    [JsonInclude] public int? iabId;
    [JsonInclude] public List<CmpVendorCategoryModel> legIntCategories;
    [JsonInclude] public List<CmpVendorCategoryModel> consentCategories;
    [JsonInclude] public List<string> iabSpecialPurposes;
    [JsonInclude] public List<string> iabFeatures;
    [JsonInclude] public List<string> iabSpecialFeatures;
    [JsonInclude] public List<CmpVendorCategoryModel> disclosureOnlyCategories;
    public bool accepted = false;
    //[JsonInclude] public List<> cookies;       //TODO
}
