using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;

public static class CmpLocalizationMapper
{
    private static NetworkClient netClient;

    private static Dictionary<string, List<CmpUiElementModel>> elements;
    public static List<CmpShortCategoryModel> shortCategories;
    public static Dictionary<string, string> popupBgColors;
    
    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;
    
    private static bool isExtraCallInitialized = false;
    public static bool IsExtraCallInitialized => isExtraCallInitialized;

    public static List<CmpCategoryModel> categories;
    public static List<CmpSpecialPurposeModel> specialPurposes;
    public static List<CmpFeatureModel> features;
    public static List<CmpSpecialFeatureModel> specialFeatures;
    public static List<CmpVendorModel> vendors;
    
    static CmpLocalizationMapper()
    {
        netClient = new NetworkClient();
        netClient.GetMessages(OnGetMessagesSuccessCallback, OnExceptionCallback, 3000);
    }

    public static void PrivacyManagerView()
    {
        isExtraCallInitialized = false;
        netClient.PrivacyManagerViews(OnPrivacyManagerViewsSuccessCallback, OnExceptionCallback, 3000);
    }

    public static void MessageGdpr()
    {
        isInitialized = false;
        netClient.MessageGdpr(OnMessageGdprSuccessCallback, OnExceptionCallback, 3000);
    }
    
    #region Success
    private static void OnGetMessagesSuccessCallback(string json)
    {
        GetMessageResponse messages = NativeUiJsonDeserializer.DeserializeGetMessages(json);
        string localState = JsonSerializer.Serialize(messages.localState);
        string campaigns = JsonSerializer.Serialize(messages.campaigns);
        SaveContext.SaveString("campaigns", campaigns);
        SaveContext.SaveString("localState", localState);
        SaveContext.SaveString("propertyId", messages.propertyId.ToString());
        var gdprCamp = messages.GetGdprCampaign();
        GdprMessage gdpr = gdprCamp?.message;
        shortCategories = gdpr?.categories;
        popupBgColors = gdprCamp?.popupBgColors;
        elements = gdprCamp?.ui;
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
    #endregion

    private static void OnExceptionCallback(Exception ex)
    {
        //TODO: throw into SpClient.OnException
        UnityEngine.Debug.LogError(ex.Message);
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