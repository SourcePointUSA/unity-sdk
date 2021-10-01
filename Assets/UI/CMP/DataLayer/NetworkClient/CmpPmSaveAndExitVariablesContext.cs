using System.Collections.Generic;

public static class CmpPmSaveAndExitVariablesContext
{
    private static List<ConsentGdprSaveAndExitVariablesCategory> acceptedCategories = new List<ConsentGdprSaveAndExitVariablesCategory>();
    private static List<ConsentGdprSaveAndExitVariablesVendor> acceptedVendors = new List<ConsentGdprSaveAndExitVariablesVendor>();

    private static bool isAcceptedCategoriesChanged = false;
    private static bool isAcceptedVendorsChanged = false;
    public static bool IsAcceptedCategoriesChanged => isAcceptedCategoriesChanged;
    public static bool IsAcceptedVendorsChanged => isAcceptedVendorsChanged;

    #region Category
    public static void AcceptCategory(CmpCategoryModel model)
    {
        var cat = new ConsentGdprSaveAndExitVariablesCategory();
        cat.consent = true;
        cat._id = model._id;
        cat.iabId = model.iabId;
        cat.type = model.type;
        // cat.legInt = false;  //TODO
        acceptedCategories.Add(cat);
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
        if(excluded!=null)
            acceptedCategories.Remove(excluded);
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
        var vend = new ConsentGdprSaveAndExitVariablesVendor();
        vend.consent = true;
        vend._id = id;
        vend.iabId = iabId;
        vend.vendorType = type;
        // vend.legInt = false; //TODO
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
        isAcceptedVendorsChanged = true;
    }
    
    public static ConsentGdprSaveAndExitVariablesVendor[] GetAcceptedVendors()
    {
        return acceptedVendors.ToArray();
    }

    public static void SetAcceptedVendorsChangedFalse()
    {
        isAcceptedVendorsChanged = false;
    }
    #endregion
}
