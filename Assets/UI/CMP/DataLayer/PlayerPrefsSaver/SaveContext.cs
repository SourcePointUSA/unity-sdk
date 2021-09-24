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
}