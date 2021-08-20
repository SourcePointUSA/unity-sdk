using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CmpFeatureModel : CmpCategoryBaseModel
{ 
    [JsonInclude] public List<CmpBaseConsentVendorModel> vendors;
}
