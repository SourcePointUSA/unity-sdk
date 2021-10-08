using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

public static class CmpLocalizationMapper
{
    private static Dictionary<string, List<CmpUiElementModel>> elements;
    public static List<CmpShortCategoryModel> shortCategories;
    public static Dictionary<string, string> popupBgColors;
    private static Canvas canvas;

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

    public static void SetCanvas(Canvas canvas)
    {
        CmpLocalizationMapper.canvas = canvas;
    }
    
    public static void GetMessages(int accountId,
                                   string propertyHref,
                                   SingleCampaignPostGetMessagesRequest gdpr,
                                   SingleCampaignPostGetMessagesRequest ccpa,
                                   int environment,
                                   int millisTimeout)
    {
        NetworkClient.Instance.GetMessages(accountId,
                                            propertyHref,
                                            new CampaignsPostGetMessagesRequest(gdpr, ccpa),
                                            OnGetMessagesSuccessCallback, OnExceptionCallback, 
                                            environment,
                                            millisTimeout);
    }

    public static void PrivacyManagerView(Action<string> OnSuccessCalback)
    {
        isExtraCallInitialized = false;
        NetworkClient.Instance.PrivacyManagerViews(OnSuccessCalback, OnExceptionCallback);
    }

    public static void MessageGdpr()
    {
        isInitialized = false;
        NetworkClient.Instance.MessageGdpr(OnMessageGdprSuccessCallback, OnExceptionCallback);
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

    public static void OnPrivacyManagerViewsSuccessCallback(string json, GameObject prefab)
    {
        NativeUiJsonDeserializer.DeserializeExtraCall(json: json,
                                                      categoryModels: ref categories,
                                                      specialPurposeModels: ref specialPurposes,
                                                      featureModels: ref features,
                                                      specialFeatureModels: ref specialFeatures,
                                                      vendorModels: ref vendors);
        isExtraCallInitialized = true;
        InstantiateOnCanvas(prefab);
    }

    public static void InstantiateOnCanvas(GameObject prefab)
    {
        if(prefab!=null && canvas != null)
            GameObject.Instantiate(prefab, canvas.transform);
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