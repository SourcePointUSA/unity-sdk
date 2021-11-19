using System.Collections.Generic;

public static class CmpPmSaveAndExitVariablesContext
{
    // GDPR categories/vendors are ACCEPTED, CCPA categories/vendors are REJECTED
    private static Dictionary<int, List<ConsentGdprSaveAndExitVariablesCategory>> categories = new Dictionary<int, List<ConsentGdprSaveAndExitVariablesCategory>>();
    private static Dictionary<int, List<ConsentGdprSaveAndExitVariablesCategory>> previousSessionCategories = new Dictionary<int, List<ConsentGdprSaveAndExitVariablesCategory>>();
    private static Dictionary<int, List<ConsentGdprSaveAndExitVariablesVendor>> vendors = new Dictionary<int, List<ConsentGdprSaveAndExitVariablesVendor>>();
    private static Dictionary<int, Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>>> categoryVendors = new Dictionary<int, Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>>>();
    private static Dictionary<int, Dictionary<string, List<ConsentGdprSaveAndExitVariablesSpecialFeature>>> specFeatures = new Dictionary<int, Dictionary<string, List<ConsentGdprSaveAndExitVariablesSpecialFeature>>>();

    private static bool isAcceptedCategoriesChanged = false;
    private static bool isAcceptedVendorsChanged = false;
    public static bool IsAcceptedCategoriesChanged => isAcceptedCategoriesChanged;
    public static bool IsAcceptedVendorsChanged => isAcceptedVendorsChanged;

    static CmpPmSaveAndExitVariablesContext()
    {
        categories.Add(0, new List<ConsentGdprSaveAndExitVariablesCategory>());
        categories.Add(2, new List<ConsentGdprSaveAndExitVariablesCategory>());
        previousSessionCategories.Add(0, new List<ConsentGdprSaveAndExitVariablesCategory>());
        previousSessionCategories.Add(2, new List<ConsentGdprSaveAndExitVariablesCategory>());
        vendors.Add(0, new List<ConsentGdprSaveAndExitVariablesVendor>());
        vendors.Add(2, new List<ConsentGdprSaveAndExitVariablesVendor>());
        categoryVendors.Add(0, new Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>>());
        categoryVendors.Add(2, new Dictionary<string, List<ConsentGdprSaveAndExitVariablesVendor>>());
        specFeatures.Add(0, new Dictionary<string, List<ConsentGdprSaveAndExitVariablesSpecialFeature>>());
        specFeatures.Add(2, new Dictionary<string, List<ConsentGdprSaveAndExitVariablesSpecialFeature>>());
    }
    
    #region Category
    public static void AcceptCategory(CmpCategoryModel model, bool consent = true)
    {
        var cat = new ConsentGdprSaveAndExitVariablesCategory(model._id, model.iabId, model.type, consent, false);
        if(!categories[CmpCampaignPopupQueue.CurrentCampaignToShow()].Exists(x => (x._id!= null && cat._id!=null && cat._id.Equals(x._id))))
            categories[CmpCampaignPopupQueue.CurrentCampaignToShow()].Add(cat);
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
                    v.accepted = consent;
                }
                //List init
                if(!categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].ContainsKey(model._id))
                    categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()][model._id] = new List<ConsentGdprSaveAndExitVariablesVendor>();
                //Duplicate check
                if (!categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()][model._id].Exists(x => (x._id != null && vendor.vendorId!=null && x._id.Equals(vendor.vendorId))||(x.name!=null && vendor.name!=null && vendor.name.Equals(x.name)) ) )
                {
                    categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()][model._id].Add(new ConsentGdprSaveAndExitVariablesVendor(vendor.vendorId, iabId, vendor.vendorType, true, false, vendor.name));
                    isAcceptedVendorsChanged = true;
                }
            }
        }
        foreach (var category in CmpLocalizationMapper.CurrentCategories)
            if (category._id.Equals(model._id))
                category.accepted = consent;
        isAcceptedCategoriesChanged = true;
    }

    public static void ExcludeCategory(string id, bool consented = false)
    {
        ConsentGdprSaveAndExitVariablesCategory excluded = null;
        foreach (var cat in categories[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            if (cat._id.Equals(id))
                excluded = cat;
        }
        if (excluded != null)
        {
            categories[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(excluded);
            if (categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].ContainsKey(excluded._id) && categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()][excluded._id] != null)
            {
                categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(excluded._id);
                isAcceptedVendorsChanged = true;
            }
        }
        foreach (var category in CmpLocalizationMapper.CurrentCategories)
            if (category._id.Equals(id))
            {
                category.accepted = consented;
                if (categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].ContainsKey(category._id) && categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()][category._id] != null)
                {
                    categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(category._id);
                    isAcceptedVendorsChanged = true;
                }
                List<CmpCategoryConsentVendorModel> vendoritos = new List<CmpCategoryConsentVendorModel>();
                vendoritos.AddRange(category.legIntVendors);
                vendoritos.AddRange(category.disclosureOnlyVendors);
                vendoritos.AddRange(category.requiringConsentVendors);
                ConsentGdprSaveAndExitVariablesVendor vendo = null;
                foreach (var vendorez in vendoritos)
                    if (vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Exists(x => x._id!=null && vendorez.vendorId!=null && vendorez.vendorId.Equals(x._id)))
                    {
                        vendo = vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Find(x =>
                            x._id != null && vendorez.vendorId != null && vendorez.vendorId.Equals(x._id));
                    }
                if (vendo != null)
                {
                    if(CmpLocalizationMapper.CurrentVendors!=null)
                        foreach (var vendor in CmpLocalizationMapper.CurrentVendors)
                            if (vendor.vendorId!= null && vendo._id!=null && vendo._id.Equals(vendor.vendorId)
                                ||vendor.name.Equals(vendo.name))
                            {
                                vendor.accepted = consented;
                            }
                    vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(vendo);
                    isAcceptedVendorsChanged = true;
                }
                break;
            }
        ConsentGdprSaveAndExitVariablesCategory excluded2 = null;
        foreach (var cat in previousSessionCategories[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            if (cat._id.Equals(id))
                excluded2 = cat;
        }
        if(excluded2!=null)
            previousSessionCategories[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(excluded2);
        isAcceptedCategoriesChanged = true;
    }

    public static ConsentGdprSaveAndExitVariablesCategory[] GetAcceptedCategories()
    {
        List<ConsentGdprSaveAndExitVariablesCategory> result = new List<ConsentGdprSaveAndExitVariablesCategory>();
        foreach (var accepted in categories[CmpCampaignPopupQueue.CurrentCampaignToShow()])
            result.Add(accepted);
        foreach (var accepted in previousSessionCategories[CmpCampaignPopupQueue.CurrentCampaignToShow()])
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
        foreach (var cat in categories[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            if (cat._id.Equals(categoryId) && cat.consent)
                result = true;
        }
        foreach (var cat in previousSessionCategories[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            if (cat._id.Equals(categoryId) && cat.consent)
                result = true;
        }
        return result;
    }

    public static void AcceptCategoryFromPreviousSession(string categoryId, bool consent = true)
    {
        if (!previousSessionCategories[CmpCampaignPopupQueue.CurrentCampaignToShow()].Exists(x => x._id.Equals(categoryId)))
        {
            foreach (var cat in CmpLocalizationMapper.CurrentCategories)
            {
                if (cat._id.Equals(categoryId))
                {
                    previousSessionCategories[CmpCampaignPopupQueue.CurrentCampaignToShow()].Add(new ConsentGdprSaveAndExitVariablesCategory(categoryId, cat.iabId, cat.type, consent, false));
                    break;
                }
            }
        }
    }
    #endregion
    
    #region Vendor
    public static void AcceptVendor(CmpVendorModel model, bool consent = true)
    {
        var vend = new ConsentGdprSaveAndExitVariablesVendor(model.vendorId, model.iabId, model.vendorType, consent, false, model.name);
        if(vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()]!=null 
           // && (vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Count>0
           && !vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Exists(x => 
               (vend._id!=null && x._id.Equals(vend._id)) || x.name.Equals(vend.name)) )
            vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Add(vend);
        if(model.iabSpecialFeatures!=null)
            foreach (var specFeat in model.iabSpecialFeatures)
            {
                //List init
                if (!specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()].ContainsKey(model.vendorId))
                    specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()][model.vendorId] = new List<ConsentGdprSaveAndExitVariablesSpecialFeature>();
                //Duplicate check
                if (!specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()][model.vendorId].Exists(x => x._id.Equals(specFeat)))
                {
                    int? iabId = null;
                    foreach (var v in CmpLocalizationMapper.CurrentVendors)
                    {
                        if (v.vendorId.Equals(model.vendorId))
                        {
                            // v.accepted = true;
                            if (v.iabId.HasValue)
                                iabId = v.iabId.Value;
                            break;
                        }
                    }
                    specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()][model.vendorId].Add(new ConsentGdprSaveAndExitVariablesSpecialFeature(specFeat, iabId));
                }
            }
        isAcceptedVendorsChanged = true;
    }

    public static void ExcludeVendor(string id, string name)
    {
        ConsentGdprSaveAndExitVariablesVendor excluded = null;
        foreach (var vendor in vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            if (vendor._id != null && vendor._id.Equals(id))
            {
                excluded = vendor;
                break;
            }
            else if (vendor.name != null && vendor.name.Equals(name))
            {
                excluded = vendor;
                break;
            }
        }
        if (excluded != null)
        {
            vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(excluded);
            if (excluded._id!=null && specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()].ContainsKey(excluded._id))
                specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()].Remove(excluded._id);
        }
        foreach (var kv in categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            foreach (var vendor in kv.Value)
                if (id!=null && vendor._id!=null && vendor._id.Equals(id))
                {
                    excluded = vendor;
                    break;
                }
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
        foreach (var vend in vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            resultList.Add(vend);
        }
        foreach (var kv in categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()])
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
        foreach (var vendor in vendors[CmpCampaignPopupQueue.CurrentCampaignToShow()])
        {
            if (vendorId!=null && vendorId.Equals(vendor._id))
                return true;
        }
        foreach (var kv in categoryVendors[CmpCampaignPopupQueue.CurrentCampaignToShow()])
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
        foreach (var kv in specFeatures[CmpCampaignPopupQueue.CurrentCampaignToShow()])
            foreach (var specFeat in kv.Value)
            {
                if (!result.Exists(x => x._id.Equals(specFeat._id)))
                    result.Add(specFeat);
            }
        return result.ToArray();
    }
    #endregion
}
