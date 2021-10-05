using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

public static class CmpLocalizationMapper
{
    private static Dictionary<string, List<CmpUiElementModel>> elements;
    public static List<CmpShortCategoryModel> shortCategories;
    public static Dictionary<string, string> popupBgColors;

    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;

    private static bool isExtraCallInitialized = false;
    public static bool IsExtraCallInitialized => isExtraCallInitialized;

    private static bool isConsented = false;
    public static bool IsConsented => isConsented;
    
    public static List<CmpCategoryModel> categories;
    public static List<CmpSpecialPurposeModel> specialPurposes;
    public static List<CmpFeatureModel> features;
    public static List<CmpSpecialFeatureModel> specialFeatures;
    public static List<CmpVendorModel> vendors;

    static CmpLocalizationMapper()
    {
        NetworkClient.Instance.GetMessages(22,
            "https://appletv.mobile.demo",
            new CampaignsPostGetMessagesRequest(
                new SingleCampaignPostGetMessagesRequest(new Dictionary<string, string>()),
                new SingleCampaignPostGetMessagesRequest(new Dictionary<string, string>())
            ),
            OnGetMessagesSuccessCallback, OnExceptionCallback, 3000);
    }

    public static void PrivacyManagerView()
    {
        isExtraCallInitialized = false;
        NetworkClient.Instance.PrivacyManagerViews(OnPrivacyManagerViewsSuccessCallback, OnExceptionCallback, 3000);
    }

    public static void MessageGdpr()
    {
        isInitialized = false;
        NetworkClient.Instance.MessageGdpr(OnMessageGdprSuccessCallback, OnExceptionCallback, 3000);
    }
    
    #region Success
    private static void OnGetMessagesSuccessCallback(string json)
    {
        GetMessageResponse messages = NativeUiJsonDeserializer.DeserializeGetMessages(json);
        SaveContext.SaveCampaigns(messages.campaigns);
        SaveContext.SaveLocalState(messages.localState);
        SaveContext.SavePropertyId(messages.propertyId);
        var gdprCamp = messages.GetGdprCampaign();
        if (gdprCamp?.message == null || gdprCamp.ui == null || gdprCamp.ui.Count == 0)
        {
            isConsented = true;
        }
        else
        {
            GdprMessage gdpr = gdprCamp?.message;
            shortCategories = gdpr?.categories;
            popupBgColors = gdprCamp?.popupBgColors;
            elements = gdprCamp?.ui;
        }
        isInitialized = true;
    }

    private static void OnPrivacyManagerViewsSuccessCallback(string json)
    {
        NativeUiJsonDeserializer.DeserializeExtraCall(json: json,
                                                      categoryModels: ref categories,
                                                      specialPurposeModels: ref specialPurposes,
                                                      featureModels: ref features,
                                                      specialFeatureModels: ref specialFeatures,
                                                      vendorModels: ref vendors);
        isExtraCallInitialized = true;
    }

    private static void OnMessageGdprSuccessCallback(string json)
    {
        elements?.Clear();
        shortCategories?.Clear();
        popupBgColors?.Clear();
        var messageGdpr = NativeUiJsonDeserializer.DeserializeMessageGdprGetResponse(json);
        elements = messageGdpr.ui;
        shortCategories = messageGdpr.message?.categories;
        popupBgColors = messageGdpr.popupBgColors;
        isInitialized = true;
    }

    public static void OnConsentGdprSuccessCallback(string json)
    {
        var consent = JsonSerializer.Deserialize<PostConsentResponse>(json);
        SaveContext.SaveLocalState(consent.localState);
        SaveContext.SaveUserConsent(consent.userConsent);
        //TODO: pass to SpUserConsent handler
        CmpPopupDestroyer.DestroyAllPopups();
    }
    #endregion

    public static void OnExceptionCallback(Exception ex)
    {
        //TODO: throw into SpClient.OnException
        UnityEngine.Debug.LogError(ex.Message);
        UnityEngine.Debug.LogError("All popups will be destroyed!!!");
        CmpPopupDestroyer.DestroyAllPopups();
    }
    
    public static CmpUiElementModel GetCmpUiElement(string viewId, string uiElementId)
    {
        CmpUiElementModel result = null;
        if(elements!=null && elements.ContainsKey(viewId))
            foreach (var uiElement in elements[viewId])
            {
                if (uiElement.id.Equals(uiElementId))
                {
                    result = uiElement;
                    break;
                }
            }
        return result;
    }
}