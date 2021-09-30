using System.Collections.Generic;

public static class CmpPmSaveAndExitVariablesContext
{
    private static List<ConsentGdprSaveAndExitVariablesCategory> acceptedCategories = new List<ConsentGdprSaveAndExitVariablesCategory>();
    private static List<ConsentGdprSaveAndExitVariablesVendor> acceptedVendors = new List<ConsentGdprSaveAndExitVariablesVendor>();

    public static void AcceptCategory (int? iabId, string id, string type)
    {
        var cat = new ConsentGdprSaveAndExitVariablesCategory();
        cat.consent = true;
        cat._id = id;
        cat.iabId = iabId;
        cat.type = type;
        // cat.legInt = false;  //TODO
        acceptedCategories.Add(cat);
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
    }

    public static ConsentGdprSaveAndExitVariablesCategory[] GetAcceptedCategories()
    {
        return acceptedCategories.ToArray();
    }

    public static void AcceptVendor (int? iabId, string id, string type)
    {
        var vend = new ConsentGdprSaveAndExitVariablesVendor();
        vend.consent = true;
        vend._id = id;
        vend.iabId = iabId;
        vend.vendorType = type;
        // vend.legInt = false; //TODO
        acceptedVendors.Add(vend);
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
    }
    
    public static ConsentGdprSaveAndExitVariablesVendor[] GetAcceptedVendors()
    {
        return acceptedVendors.ToArray();
    }
}
