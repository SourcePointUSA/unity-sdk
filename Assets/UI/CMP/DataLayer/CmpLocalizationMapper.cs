using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UnityEngine;

public static class CmpLocalizationMapper
{
    private static Dictionary<string, List<CmpUiElementModel>> gdprElements;
    private static Dictionary<string, List<CmpUiElementModel>> ccpaElements;
    public static List<CmpShortCategoryModel> shortCategories;
    public static Dictionary<string, string> popupBgColors;

    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;

    private static bool isExtraCallInitialized = false;
    public static bool IsExtraCallInitialized => isExtraCallInitialized;

    private static bool isGdprConsented = false;
    public static bool IsGdprConsented => isGdprConsented;
    private static bool isCcpaConsented = false;
    public static bool IsCcpaConsented => isCcpaConsented;
    public static bool IsConsented => isGdprConsented && isCcpaConsented;
    
    public static bool IsPmReadyForResurface = false;
    
    public static List<CmpCategoryModel> categories;
    public static List<CmpSpecialPurposeModel> specialPurposes;
    public static List<CmpFeatureModel> features;
    public static List<CmpSpecialFeatureModel> specialFeatures;
    public static List<CmpVendorModel> vendors;

    private static Canvas canvas;
    private static GameObject homePrefab;
    private static int environment;
    public static string language { get; set; }
    public static string propertyId { get; set; }
    public static string privacyManagerId { get; set; }
    public static Exception cmpException;
    public static int? lastActionCode;
    public static PostConsentUserConsent gdprUserConsent;
    public static PostConsentUserConsent ccpaUserConsent;
    
    public static void SetCanvas(GameObject homePrefab, Canvas canvas)
    {
        CmpLocalizationMapper.canvas = canvas;
        CmpLocalizationMapper.homePrefab = homePrefab;
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
            NetworkClient.Instance.PrivacyManagerViews(CmpCampaignPopupQueue.CurrentCampaignToShow(), propertyId, language, OnSuccessDeserializeCallback, OnSuccessInstantiateGOCallback, OnExceptionCallback);
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
        var ccpaCamp = messages.GetCcpaCampaign();
        if (gdprCamp != null)
        {
            if (gdprCamp?.message == null || gdprCamp.ui == null || gdprCamp.ui.Count == 0)
            {
                if (gdprCamp.userConsent == null)
                    UnityEngine.Debug.LogError("UserConsent is NULL");
                else
                {
                    gdprUserConsent = new PostConsentUserConsent()
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
                    // CmpPopupDestroyer.DestroyAllHelperGO();  //TODO: CHECK
                    SaveContext.SaveUserConsent(gdprUserConsent);
                    isGdprConsented = true;
                }
            }
            else
            {
                GdprMessage gdpr = gdprCamp?.message;
                shortCategories = gdpr?.categories;
                popupBgColors = gdprCamp?.popupBgColors;
                gdprElements = gdprCamp?.ui;
            }
        }
        if (ccpaCamp != null)
        {
            if (ccpaCamp?.message == null || ccpaCamp.ui == null || ccpaCamp.ui.Count == 0)
            {
                if (ccpaCamp.userConsent == null)
                    UnityEngine.Debug.LogError("UserConsent is NULL");
                else
                {
                    //TODO: user consented  
                    isCcpaConsented = true;
                }
            }
            else
            {
                BaseMessage ccpa = ccpaCamp?.message;
                popupBgColors = ccpaCamp.popupBgColors;
                ccpaElements = ccpaCamp.ui;
                // shortCategories ??
            }
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
        SaveContext.UpdateUserConsentUIState();
        isExtraCallInitialized = true;
    }

    public static void InstantiateOnCanvas(GameObject prefab)
    {
        if(prefab!=null && canvas != null)
            GameObject.Instantiate(prefab, canvas.transform);
    }
    
    private static void OnMessageGdprSuccessCallback(string json)
    {
        var messageGdpr = NativeUiJsonDeserializer.DeserializeMessageGdprGetResponse(json);
        if (messageGdpr.message?.categories != null && messageGdpr.message?.categories.Count > 0)
        {
            shortCategories?.Clear();
            shortCategories = messageGdpr.message?.categories;
        }
        if (messageGdpr.ui != null && messageGdpr.ui.Count > 0)
        {
            gdprElements?.Clear();
            gdprElements = messageGdpr.ui;
        }
        if (messageGdpr.popupBgColors != null && messageGdpr.popupBgColors.Count > 0)
        {
            popupBgColors?.Clear();
            popupBgColors = messageGdpr.popupBgColors;
        }
        SaveContext.UpdateUserConsentUIState();
        isInitialized = true;
        IsPmReadyForResurface = true;
    }

    public static void OnConsentGdprSuccessCallback(string json)
    {
        var consent = JsonSerializer.Deserialize<PostConsentResponse>(json);
        SaveContext.SaveLocalState(consent.localState);
        SaveContext.SaveUserConsent(consent.userConsent);
        CmpPopupDestroyer.DestroyAllPopups();
        // CmpPopupDestroyer.DestroyAllHelperGO(); //TODO
        switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
        {
            case 0:
                gdprUserConsent = consent.userConsent;
                gdprUserConsent.uuid = consent.uuid;
                isGdprConsented = true;
                CmpCampaignPopupQueue.DequeueCampaignId();
                break;
            case 2:
                ccpaUserConsent = consent.userConsent;
                ccpaUserConsent.uuid = consent.uuid;
                isCcpaConsented = true;
                CmpCampaignPopupQueue.DequeueCampaignId();
                break;
        }
        if (CmpCampaignPopupQueue.IsCampaignAvailable)
            InstantiateOnCanvas(homePrefab);
    }
    #endregion

    public static void OnExceptionCallback(Exception ex)
    {
        UnityEngine.Debug.LogError("All popups will be destroyed!!!");
        CmpPopupDestroyer.DestroyAllPopups();
        cmpException = ex;
        CmpCampaignPopupQueue.DequeueCampaignId();
        if (CmpCampaignPopupQueue.IsCampaignAvailable)
            InstantiateOnCanvas(homePrefab);
    }
    
    public static CmpUiElementModel GetCmpUiElement(string viewId, string uiElementId)
    {
        CmpUiElementModel result = null;
        // campaignType == GDPR = 0, CCPA = 2
         switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
        {
            case 0:
                if(gdprElements!=null && gdprElements.ContainsKey(viewId))
                    foreach (var uiElement in gdprElements[viewId])
                        if (uiElement.id.Equals(uiElementId))
                        {
                            result = uiElement;
                            break;
                        }
                break;
            case 2:
                if(ccpaElements!=null && ccpaElements.ContainsKey(viewId))
                    foreach (var uiElement in ccpaElements[viewId])
                        if (uiElement.id.Equals(uiElementId))
                        {
                            result = uiElement;
                            break;
                        }
                break;
        }
        return result;
    }
}