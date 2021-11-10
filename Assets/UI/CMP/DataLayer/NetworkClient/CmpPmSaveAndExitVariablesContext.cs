using System.Collections.Generic;

public static class CmpPmSaveAndExitVariablesContext
{
    private static List<ConsentGdprSaveAndExitVariablesCategory> acceptedCategories = new List<ConsentGdprSaveAndExitVariablesCategory>();
    private static List<ConsentGdprSaveAndExitVariablesCategory> previouslyAcceptedCategories = new List<ConsentGdprSaveAndExitVariablesCategory>();
    private static List<ConsentGdprSaveAndExitVariablesVendor> acceptedVendors = new List<ConsentGdprSaveAndExitVariablesVendor>();
    private static Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>> acceptedCategoryVendors = new Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>>();
    private static Dictionary<string, List<ConsentGdprSaveAndExitVariablesSpecialFeature>> acceptedSpecFeatures = new Dictionary<string, List<ConsentGdprSaveAndExitVariablesSpecialFeature>>();
    
    private static bool isAcceptedCategoriesChanged = false;
    private static bool isAcceptedVendorsChanged = false;
    public static bool IsAcceptedCategoriesChanged => isAcceptedCategoriesChanged;
    public static bool IsAcceptedVendorsChanged => isAcceptedVendorsChanged;

    #region Category
    public static void AcceptCategory(CmpCategoryModel model)
    {
        var cat = new ConsentGdprSaveAndExitVariablesCategory(model._id, model.iabId, model.type, true, false);
        if(!acceptedCategories.Exists(x => (x._id!= null && cat._id!=null && cat._id.Equals(x._id))))
            acceptedCategories.Add(cat);
        List<CmpCategoryConsentVendorModel> vendors = new List<CmpCategoryConsentVendorModel>();
        foreach (var vendor in model.requiringConsentVendors)
        {
           vendors.Add(vendor);
        }
        vendors.AddRange(model.legIntVendors);
        foreach (var vendor in vendors)
        {
            int? iabId = null;
            foreach (var v in CmpLocalizationMapper.CurrentVendors)
            {
                if (v.vendorId!=null && v.vendorId.Equals(vendor.vendorId))
                {
                    if(v.iabId.HasValue)
                        iabId = v.iabId.Value;
                    v.accepted = true;
                }
                //List init
                if(!acceptedCategoryVendors.ContainsKey(model._id))
                    acceptedCategoryVendors[model._id] = new List<ConsentGdprSaveAndExitVariablesVendor>();
                //Duplicate check
                if (!acceptedCategoryVendors[model._id].Exists(x => (x._id != null && vendor.vendorId!=null && x._id.Equals(vendor.vendorId))||(x.name!=null && vendor.name!=null && vendor.name.Equals(x.name)) ) )
                {
                    acceptedCategoryVendors[model._id].Add(new ConsentGdprSaveAndExitVariablesVendor(vendor.vendorId, iabId, vendor.vendorType, true, false, vendor.name));
                    isAcceptedVendorsChanged = true;
                }
            }
        }
        foreach (var category in CmpLocalizationMapper.CurrentCategories)
            if (category._id.Equals(model._id))
                category.accepted = true;
        isAcceptedCategoriesChanged = true;
    }

    public static void ExcludeCategory(string id)
    {
        ConsentGdprSaveAndExitVariablesCategory excluded = null;
        foreach (var cat in acceptedCategories)
        {
            if (cat._id.Equals(id))
                excluded = cat;
        }
        if (excluded != null)
        {
            acceptedCategories.Remove(excluded);
            if (acceptedCategoryVendors.ContainsKey(excluded._id) && acceptedCategoryVendors[excluded._id] != null)
            {
                acceptedCategoryVendors.Remove(excluded._id);
                isAcceptedVendorsChanged = true;
            }
        }
        foreach (var category in CmpLocalizationMapper.CurrentCategories)
            if (category._id.Equals(id))
                category.accepted = false;
        isAcceptedCategoriesChanged = true;
    }

    public static ConsentGdprSaveAndExitVariablesCategory[] GetAcceptedCategories()
    {
        List<ConsentGdprSaveAndExitVariablesCategory> result = new List<ConsentGdprSaveAndExitVariablesCategory>();
        foreach (var accepted in acceptedCategories)
            result.Add(accepted);
        foreach (var accepted in previouslyAcceptedCategories)
            if(!result.Exists(x => x._id.Equals(accepted._id)))
                result.Add(accepted);
        return result.ToArray();
    }

    public static void SetAcceptedCategoriesChangedFalse()
    {
        isAcceptedCategoriesChanged = false;
    }

    public static bool IsCategoryAcceptedAnywhere(string categoryId)
    {
        bool result = false;
        foreach (var cat in acceptedCategories)
        {
            if (cat._id.Equals(categoryId) && cat.consent)
                result = true;
        }
        return result;
    }

    public static void AcceptCategoryFromPreviousSession(string categoryId)
    {
        if (!previouslyAcceptedCategories.Exists(x => x._id.Equals(categoryId)))
        {
            foreach (var cat in CmpLocalizationMapper.CurrentCategories)
            {
                if(cat._id.Equals(categoryId))
                    previouslyAcceptedCategories.Add(new ConsentGdprSaveAndExitVariablesCategory(categoryId, cat.iabId, cat.type, true,false));
            }
        }
    }
    #endregion
    
    #region Vendor
    public static void AcceptVendor(CmpVendorModel model)
    {
        var vend = new ConsentGdprSaveAndExitVariablesVendor(model.vendorId, model.iabId, model.vendorType, true, false, model.name);
        acceptedVendors.Add(vend);
        if(model.iabSpecialFeatures!=null)
            foreach (var specFeat in model.iabSpecialFeatures)
            {
                //List init
                if (!acceptedSpecFeatures.ContainsKey(model.vendorId))
                    acceptedSpecFeatures[model.vendorId] = new List<ConsentGdprSaveAndExitVariablesSpecialFeature>();
                //Duplicate check
                if (!acceptedSpecFeatures[model.vendorId].Exists(x => x._id.Equals(specFeat)))
                {
                    int? iabId = null;
                    foreach (var v in CmpLocalizationMapper.CurrentVendors)
                    {
                        if (v.vendorId.Equals(model.vendorId))
                        {
                            // v.accepted = true;
                            if (v.iabId.HasValue)
                                iabId = v.iabId.Value;
                        }
                    }
                    acceptedSpecFeatures[model.vendorId].Add(new ConsentGdprSaveAndExitVariablesSpecialFeature(specFeat, iabId));
                }
            }
        isAcceptedVendorsChanged = true;
    }

    public static void ExcludeVendor(string id, string name)
    {
        ConsentGdprSaveAndExitVariablesVendor excluded = null;
        foreach (var vendor in acceptedVendors)
        {
            if (vendor._id!=null && vendor._id.Equals(id))
                excluded = vendor;
            else if(vendor.name!=null && vendor.name.Equals(name))
                excluded = vendor;
        }
        if (excluded != null)
        {
            acceptedVendors.Remove(excluded);
            if (excluded._id!=null && acceptedSpecFeatures.ContainsKey(excluded._id))
                acceptedSpecFeatures.Remove(excluded._id);
        }
        foreach (var kv in acceptedCategoryVendors)
        {
            foreach (var vendor in kv.Value)
                if (vendor._id.Equals(id))
                    excluded = vendor;
            kv.Value.Remove(excluded);
        }
        // foreach (var v in CmpLocalizationMapper.vendors)
        //     if (v.vendorId.Equals(id))
        //         v.accepted = false;
        isAcceptedVendorsChanged = true;
    }
    
    public static ConsentGdprSaveAndExitVariablesVendor[] GetAcceptedVendors()
    {
        List<ConsentGdprSaveAndExitVariablesVendor> resultList = new List<ConsentGdprSaveAndExitVariablesVendor>();
        foreach (var vend in acceptedVendors)
        {
            resultList.Add(vend);
        }
        foreach (var kv in acceptedCategoryVendors)
        {
            foreach (var vendor in kv.Value)
            {
                if(vendor._id!=null && !resultList.Exists(x => x._id.Equals(vendor._id)))
                    resultList.Add(vendor);
            }
        }
        return resultList.ToArray();
    }

    public static void SetAcceptedVendorsChangedFalse()
    {
        isAcceptedVendorsChanged = false;
    }

    public static bool IsVendorAcceptedAnywhere(string vendorId)
    {
        foreach (var vendor in acceptedVendors)
        {
            if (vendorId!=null && vendorId.Equals(vendor._id))
                return true;
        }
        foreach (var kv in acceptedCategoryVendors)
        foreach (var vendor in kv.Value)
        {
            if (vendorId!=null && vendorId.Equals(vendor._id))
                return true;
        }
        return false;
    }
    #endregion

    #region Special Features
    public static ConsentGdprSaveAndExitVariablesSpecialFeature[] GetSpecialFeatures()
    {
        List<ConsentGdprSaveAndExitVariablesSpecialFeature> result = new List<ConsentGdprSaveAndExitVariablesSpecialFeature>();
        foreach (var kv in acceptedSpecFeatures)
            foreach (var specFeat in kv.Value)
            {
                if (!result.Exists(x => x._id.Equals(specFeat._id)))
                    result.Add(specFeat);
            }
        return result.ToArray();
    }
    #endregion
}
