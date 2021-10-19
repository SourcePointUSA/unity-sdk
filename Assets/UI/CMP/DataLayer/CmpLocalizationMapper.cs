using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private static Canvas canvas;
    private static int environment;
    public static string language { get; set; }
    public static string propertyId { get; set; }
    public static string privacyManagerId { get; set; }
    public static Exception cmpException;
    public static int? lastActionCode;
    public static PostConsentUserConsent userConsent;
    
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
        CmpLocalizationMapper.environment = environment;
        NetworkClient.Instance.GetMessages(accountId,
                                           propertyHref,
                                           new CampaignsPostGetMessagesRequest(gdpr, ccpa),
                                           OnGetMessagesSuccessCallback, 
                                           OnExceptionCallback, 
                                           environment,
                                           millisTimeout);
    }

    public static void PrivacyManagerView(Action<string> OnSuccessDeserializeCallback, Action OnSuccessInstantiateGOCallback)
    {
        if (!isExtraCallInitialized)
        {
            NetworkClient.Instance.PrivacyManagerViews(propertyId, language, OnSuccessDeserializeCallback, OnSuccessInstantiateGOCallback, OnExceptionCallback);
        }
        else
        {
            OnSuccessInstantiateGOCallback.Invoke();
        }
    }

    public static void MessageGdpr()
    {
        isInitialized = false;
        NetworkClient.Instance.MessageGdpr(environment,
                                           language,
                                           propertyId,
                                  privacyManagerId,
                                           OnMessageGdprSuccessCallback, 
                                           OnExceptionCallback);
    }

    public static void ConsentGdpr(int actionCode)
    {
        lastActionCode = actionCode;
        switch (actionCode)
        {

            case 1:
                var saveAndExitVariables = new ConsentGdprSaveAndExitVariables(
                                            language: language,
                                            privacyManagerId: privacyManagerId, 
                                            categories: CmpPmSaveAndExitVariablesContext.GetAcceptedCategories(), 
                                            vendors: CmpPmSaveAndExitVariablesContext.GetAcceptedVendors(),
                                            specialFeatures: CmpPmSaveAndExitVariablesContext.GetSpecialFeatures()); 
                NetworkClient.Instance.ConsentGdpr(actionType: actionCode, 
                                                   environment: environment,
                                                   language: language,
                                                   privacyManagerId: privacyManagerId, 
                                                   onSuccessAction: OnConsentGdprSuccessCallback, 
                                                   onErrorAction: OnExceptionCallback,
                                                   pmSaveAndExitVariables: saveAndExitVariables);
                break;
            default:
                NetworkClient.Instance.ConsentGdpr(actionType: actionCode, 
                                                   environment: environment,
                                                   language: language,
                                                   privacyManagerId: privacyManagerId, 
                                                   onSuccessAction: OnConsentGdprSuccessCallback, 
                                                   onErrorAction: OnExceptionCallback);
                break;
        }
    }
    
    #region Success
    private static void OnGetMessagesSuccessCallback(string json)
    {
        GetMessageResponse messages = NativeUiJsonDeserializer.DeserializeGetMessages(json);
        SaveContext.SaveCampaigns(messages.campaigns);
        SaveContext.SaveLocalState(messages.localState);
        SaveContext.SavePropertyId(messages.propertyId);
        var gdprCamp = messages.GetGdprCampaign();
        //!!! TODO: CHECK NOT NULL
        if (gdprCamp?.message == null || gdprCamp.ui == null || gdprCamp.ui.Count == 0)
        {
            userConsent = new PostConsentUserConsent()
            {
                TCData = gdprCamp.userConsent.TCData,
                grants = gdprCamp.userConsent.grants,
                // specialFeatures = gdprCamp.userConsent.,
                // legIntCategories = gdprCamp.userConsent.,
                // acceptedVendors = gdprCamp.userConsent.,
                // acceptedCategories = gdprCamp.userConsent.,
                euconsent = gdprCamp.userConsent.euconsent,
                addtlConsent = gdprCamp.userConsent.addtlConsent,
                dateCreated = gdprCamp.userConsent.dateCreated,
                consentedToAll = gdprCamp.userConsent.consentedToAll.GetValueOrDefault(false) 
            };
            // SaveContext.SaveUserConsent(userConsent); //TODO: CHECK
            // CmpPopupDestroyer.DestroyAllHelperGO();  //TODO: CHECK
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

    public static void OnPrivacyManagerViewsSuccessCallback(string json)//, GameObject prefab)
    {
        NativeUiJsonDeserializer.DeserializeExtraCall(json: json,
                                                      categoryModels: ref categories,
                                                      specialPurposeModels: ref specialPurposes,
                                                      featureModels: ref features,
                                                      specialFeatureModels: ref specialFeatures,
                                                      vendorModels: ref vendors);
        isExtraCallInitialized = true;
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
        userConsent = consent.userConsent;
        CmpPopupDestroyer.DestroyAllPopups();
        // CmpPopupDestroyer.DestroyAllHelperGO(); //TODO
        isConsented = true;
    }
    #endregion

    public static void OnExceptionCallback(Exception ex)
    {
        UnityEngine.Debug.LogError("All popups will be destroyed!!!");
        CmpPopupDestroyer.DestroyAllPopups();
        cmpException = ex;
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