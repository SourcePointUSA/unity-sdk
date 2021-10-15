using System.Collections.Generic;

public static class CmpPmSaveAndExitVariablesContext
{
    private static List<ConsentGdprSaveAndExitVariablesCategory> acceptedCategories = new List<ConsentGdprSaveAndExitVariablesCategory>();
    private static List<ConsentGdprSaveAndExitVariablesVendor> acceptedVendors = new List<ConsentGdprSaveAndExitVariablesVendor>();
    private static Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>> acceptedCategoryVendors = new Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>>();

    private static bool isAcceptedCategoriesChanged = false;
    private static bool isAcceptedVendorsChanged = false;
    public static bool IsAcceptedCategoriesChanged => isAcceptedCategoriesChanged;
    public static bool IsAcceptedVendorsChanged => isAcceptedVendorsChanged;

    #region Category
    public static void AcceptCategory(CmpCategoryModel model)
    {
        var cat = new ConsentGdprSaveAndExitVariablesCategory(model._id, model.iabId, model.type, true, false);
        acceptedCategories.Add(cat);
        foreach (var vendor in model.requiringConsentVendors)
        {
            int? iabId = null;
            foreach (var v in CmpLocalizationMapper.vendors)
            {
                if (v.vendorId.Equals(vendor.vendorId) && v.iabId.HasValue)
                    iabId = v.iabId.Value;
                acceptedCategoryVendors[model._id] ??= new List<ConsentGdprSaveAndExitVariablesVendor>();
                acceptedCategoryVendors[model._id].Add(new ConsentGdprSaveAndExitVariablesVendor(vendor.vendorId, iabId, vendor.vendorType, true, false));
                isAcceptedVendorsChanged = true;
            }
        }
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
            if (acceptedCategoryVendors[excluded._id] != null)
            {
                acceptedCategoryVendors.Remove(excluded._id);
                isAcceptedVendorsChanged = true;
            }
        }
        isAcceptedCategoriesChanged = true;
    }

    public static ConsentGdprSaveAndExitVariablesCategory[] GetAcceptedCategories()
    {
        return acceptedCategories.ToArray();
    }

    public static void SetAcceptedCategoriesChangedFalse()
    {
        isAcceptedCategoriesChanged = false;
    }
    #endregion
    
    #region Vendor
    public static void AcceptVendor (int? iabId, string id, string type)
    {
        var vend = new ConsentGdprSaveAndExitVariablesVendor(id, iabId, type, true, false);
        acceptedVendors.Add(vend);
        isAcceptedVendorsChanged = true;
    }

    public static void ExcludeVendor(string id)
    {
        ConsentGdprSaveAndExitVariablesVendor excluded = null;
        foreach (var vendor in acceptedVendors)
        {
            if (vendor._id.Equals(id))
                excluded = vendor;
        }
        if(excluded!=null)
            acceptedVendors.Remove(excluded);
        foreach (var kv in acceptedCategoryVendors)
        {
            foreach (var vendor in kv.Value)
            {
                if (vendor._id.Equals(id))
                {
                    excluded = vendor;
                }
                kv.Value.Remove(excluded);
            }
        }
        isAcceptedVendorsChanged = true;
    }
    
    public static ConsentGdprSaveAndExitVariablesVendor[] GetAcceptedVendors()
    {
        List<ConsentGdprSaveAndExitVariablesVendor> resultList = acceptedVendors;
        foreach (var kv in acceptedCategoryVendors)
        {
            foreach (var vendor in kv.Value)
            {
                if(!resultList.Exists(x => x._id.Equals(vendor._id)))
                    resultList.Add(vendor);
            }
        }
        return resultList.ToArray();
    }

    public static void SetAcceptedVendorsChangedFalse()
    {
        isAcceptedVendorsChanged = false;
    }
    #endregion
}
