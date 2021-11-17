using System.Collections.Generic;
using System.Text.Json;

public static class SaveContext
{
    private static PlayerPrefsSaver saver = new PlayerPrefsSaver();
    private static readonly string localStateKey = "localState";
    private static readonly string campaignsKey = "campaigns";
    private static readonly string propertyIdKey = "propertyId";
    private static readonly string userConsentKey = "userConsent";
    private static readonly string gdprKey = "GDPR";
    private static readonly string ccpaKey = "CCPA";

    #region Save
    public static void SavePropertyId(int propertyId)
    {
        saver.SaveInt(propertyIdKey, propertyId);
    }
    
    public static void SaveLocalState(LocalState localState)
    {
        string json = JsonSerializer.Serialize(localState);
        SaveString(localStateKey, json);
    }

    public static void SaveCampaigns(List<BaseGetMessagesCampaign> campaigns)
    {
        string json = JsonSerializer.Serialize(campaigns);
        SaveString(campaignsKey, json);
    }
    
    public static void SaveGdprUserConsent(PostConsentUserConsent consentUserConsent)
    {
        string json = JsonSerializer.Serialize(consentUserConsent);
        SaveString(userConsentKey+gdprKey, json);
        UpdateUserConsentUIState();
    }
    
    public static void SaveCcpaUserConsent(PostConsentUserConsent consentUserConsent)
    {
        string json = JsonSerializer.Serialize(consentUserConsent);
        SaveString(userConsentKey+ccpaKey, json);
        UpdateUserConsentUIState();
    }

    public static void UpdateUserConsentUIState()
    {
        string key = userConsentKey;
        switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
        {
            case 0:
                key += gdprKey;
                break;
            case 2:
                key += ccpaKey;
                break;
        }
        var userConsent = saver.GetUserConsent(key);
        Dictionary<string, SpGetMessagesVendorGrant> grants = userConsent?.grants;
        if (key.Equals(userConsentKey + gdprKey) && grants == null) return;
        if (key.Equals(userConsentKey + ccpaKey))
        {
            //CCPA
            var rejectedC = userConsent.rejectedCategories;
            var rejectedV = userConsent.rejectedVendors;
            if (rejectedC != null && CmpLocalizationMapper.CurrentCategories != null)
            {
                foreach (var categoryId in rejectedC)
                {
                    //accepting category
                    foreach (var cat in CmpLocalizationMapper.CurrentCategories)
                        if (cat._id.Equals(categoryId))
                        {
                            cat.accepted = false;
                            CmpPmSaveAndExitVariablesContext.AcceptCategoryFromPreviousSession(cat._id, false);
                            break;
                        }
                    //accepting vendor if at least 1 category is accepted
                    // if (CmpLocalizationMapper.CurrentVendors != null)
                    // {
                    //     foreach (var vendor in CmpLocalizationMapper.CurrentVendors)
                    //     {
                    //         vendor.
                    //         if (vendor.vendorId != null && vendor.vendorId.Equals(vendorId))
                    //         {
                    //             // vendor.accepted = true; //TODO
                    //             // CmpPmSaveAndExitVariablesContext.AcceptVendor(vendor);
                    //             break;
                    //         }
                    //     }
                    // }
                }
            }
            if (rejectedV != null && CmpLocalizationMapper.CurrentVendors != null)
            {
                foreach (var vendorId in rejectedV)
                {
                    foreach (var vendor in CmpLocalizationMapper.CurrentVendors)
                        if (vendor.vendorId != null && vendor.vendorId.Equals(vendorId))
                        {
                            vendor.accepted = false;  
                            CmpPmSaveAndExitVariablesContext.AcceptVendor(vendor, false);
                            break;
                        }
                }
            }
        }
        else
        {
            //GDPR
            foreach (var kv in grants)
            {
                if (CmpLocalizationMapper.CurrentVendors != null && kv.Value.vendorGrant)
                {
                    var vendorId = kv.Key;
                    foreach (var vendor in CmpLocalizationMapper.CurrentVendors)
                        if (vendor.vendorId != null && vendor.vendorId.Equals(vendorId))
                        {
                            vendor.accepted = true;
                            CmpPmSaveAndExitVariablesContext.AcceptVendor(vendor);
                            break;
                        }
                }
                foreach (var kvPups in kv.Value.purposeGrants)
                {
                    if (CmpLocalizationMapper.CurrentCategories != null && kvPups.Value)
                    {
                        //accepting category
                        var categoryId = kvPups.Key;
                        foreach (var cat in CmpLocalizationMapper.CurrentCategories)
                            if (cat._id.Equals(categoryId))
                            {
                                cat.accepted = true;
                                CmpPmSaveAndExitVariablesContext.AcceptCategoryFromPreviousSession(cat._id);
                                break;
                            }
                        //accepting vendor if at least 1 category is accepted
                        if (CmpLocalizationMapper.CurrentVendors != null)
                        {
                            var vendorId = kv.Key;
                            foreach (var vendor in CmpLocalizationMapper.CurrentVendors)
                                if (vendor.vendorId != null && vendor.vendorId.Equals(vendorId))
                                {
                                    vendor.accepted = true;
                                    CmpPmSaveAndExitVariablesContext.AcceptVendor(vendor);
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }

    private static void SaveString(string key, string value)
    {
        saver.SaveString(key, value);
    }
    #endregion

    public static LocalState GetLocalState()
    {
        return saver.GetLocalState(localStateKey);
    }

    public static int GetPropertyId()
    {
        return saver.GetInt(propertyIdKey);
    }
}