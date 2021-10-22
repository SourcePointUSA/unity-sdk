using System.Collections.Generic;
using System.Text.Json;

public static class SaveContext
{
    private static PlayerPrefsSaver saver = new PlayerPrefsSaver();
    private static readonly string localStateKey = "localState";
    private static readonly string campaignsKey = "campaigns";
    private static readonly string propertyIdKey = "propertyId";
    private static readonly string userConsentKey = "userConsent";

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
    
    public static void SaveUserConsent(PostConsentUserConsent consentUserConsent)
    {
        string json = JsonSerializer.Serialize(consentUserConsent);
        SaveString(userConsentKey, json);
        UpdateUserConsentUIState();
    }

    public static void UpdateUserConsentUIState()
    {
        var userConsent = saver.GetUserConsent(userConsentKey);
        Dictionary<string, SpGetMessagesVendorGrant> grants = userConsent?.grants;
        if (grants != null)
            foreach (var kv in grants)
            {
                if (kv.Value.vendorGrant)
                {
                    var vendorId = kv.Key;
                    if(CmpLocalizationMapper.vendors!=null)
                        foreach (var vendor in CmpLocalizationMapper.vendors)
                        {
                            if (vendor.vendorId.Equals(vendorId))
                            {
                                vendor.accepted = true;
                            }
                        }
                }
                foreach (var kvPups in kv.Value.purposeGrants)
                {
                    if (kvPups.Value)
                    {
                        var categoryId = kvPups.Key;
                        if(CmpLocalizationMapper.categories!=null)
                            foreach (var cat in CmpLocalizationMapper.categories)
                            {
                                if (cat._id.Equals(categoryId))
                                {
                                    cat.accepted = true;
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