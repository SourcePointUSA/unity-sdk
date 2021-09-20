using System;
using System.Collections.Generic;
using System.Text.Json;

public static class CmpLocalizationMapper
{
    private static NetworkClient netClient;

    private static Dictionary<string, List<CmpUiElementModel>> elements;
    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;

    public static Dictionary<string, string> popupBgColors;
    public static List<CmpShortCategoryModel> shortCategories;

    public static List<CmpCategoryModel> categories;
    public static List<CmpSpecialPurposeModel> specialPurposes;
    public static List<CmpFeatureModel> features;
    public static List<CmpSpecialFeatureModel> specialFeatures;
    public static List<CmpVendorModel> vendors;
    
    static CmpLocalizationMapper()
    {
        netClient = new NetworkClient();
        netClient.GetMessages(OnGetMessagesSuccessCallback, OnExceptionCallback, 3000);
        /*
        NativeUiJsonDeserializer.DeserializeExtraCall(JSONSTUB.extraCall, 
                                                        ref categories,
                                                        ref specialPurposes,
                                                        ref features,
                                                        ref specialFeatures,
                                                        ref vendors);
        */
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