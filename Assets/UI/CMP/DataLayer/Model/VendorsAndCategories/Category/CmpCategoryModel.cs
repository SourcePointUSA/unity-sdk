using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CmpCategoryModel : CmpCategoryBaseModel
{
    [JsonInclude] public string type;
    [JsonInclude] public string friendlyDescription;
    [JsonInclude] public bool disclosureOnly;
    [JsonInclude] public List<CmpCategoryConsentVendorModel> requiringConsentVendors;
    [JsonInclude] public List<CmpCategoryConsentVendorModel> legIntVendors;
    [JsonInclude] public List<CmpCategoryConsentVendorModel> disclosureOnlyVendors;
    public bool accepted = false;
    //[JsonInclude] public List<CmpConsentVendorModel> doNotAllowVendors;       //TODO
}
