using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CmpSpecialFeatureModel : CmpCategoryBaseModel
{
    [JsonInclude] public List<CmpSpecialFeatureVendorModel> vendors;
}
