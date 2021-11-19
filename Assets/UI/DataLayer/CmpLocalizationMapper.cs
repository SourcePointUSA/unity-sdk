using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UnityEngine;

public static class CmpLocalizationMapper
{
    // TODO:
    // SaveContext.UpdateUserConsentUIState(); <- ACCEPT_ALL || REJECT_ALL
    // GDPR all off, CCPA all on.

    private static Dictionary<string, List<CmpUiElementModel>> gdprElements;
    private static Dictionary<string, List<CmpUiElementModel>> ccpaElements;
    public static List<CmpShortCategoryModel> shortCategories;
    public static Dictionary<string, string> popupBgColors;

    private static bool isInitialized = false;
    public static bool IsInitialized => isInitialized;  //UI is ready to be loaded
    private static bool isGdprPmInitialized = false;    //UI for GDPR PM is ready to be loaded
    private static bool isCcpaPmInitialized = false;    //UI for CCPA PM is ready to be loaded
    private static bool isGdprConsented = false;
    public static bool IsGdprConsented => isGdprConsented;  //gdpr campaign consented
    private static bool isCcpaConsented = false;
    public static bool IsCcpaConsented => isCcpaConsented;  //ccpa campaign consented
    public static bool IsConsented => isGdprConsented && isCcpaConsented;
    
    public static bool IsPmReadyForResurface = false;
    
    private static List<CmpCategoryModel> gdprCategories;
    private static List<CmpSpecialPurposeModel> gdprSpecialPurposes;
    private static List<CmpFeatureModel> gdprFeatures;
    private static List<CmpSpecialFeatureModel> gdprSpecialFeatures;
    private static List<CmpVendorModel> gdprVendors;
    
    private static List<CmpCategoryModel> ccpaCategories;
    private static List<CmpSpecialPurposeModel> ccpaSpecialPurposes;
    private static List<CmpFeatureModel> ccpaFeatures;
    private static List<CmpSpecialFeatureModel> ccpaSpecialFeatures;
    private static List<CmpVendorModel> ccpaVendors;

    public static List<CmpCategoryModel> CurrentCategories
    {
        get
        {
            switch(CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    return gdprCategories;
                case 2:
                    return ccpaCategories;
                default:
                    //wtf
                    return null;
            }
        }
    }
    public static List<CmpSpecialPurposeModel> CurrentSpecialPurposes
    {
        get
        {
            switch(CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    return gdprSpecialPurposes;
                case 2:
                    return ccpaSpecialPurposes;
                default:
                    //wtf
                    return null;
            }
        }
    }
    public static List<CmpFeatureModel> CurrentFeatures
    {
        get
        {
            switch(CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    return gdprFeatures;
                case 2:
                    return ccpaFeatures;
                default:
                    //wtf
                    return null;
            }
        }
    }
    public static List<CmpSpecialFeatureModel> CurrentSpecialFeatures
    {
        get
        {
            switch(CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    return gdprSpecialFeatures;
                case 2:
                    return ccpaSpecialFeatures;
                default:
                    //wtf
                    return null;
            }
        }
    }
    public static List<CmpVendorModel> CurrentVendors
    {
        get
        {
            switch(CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    return gdprVendors;
                case 2:
                    return ccpaVendors;
                default:
                    //wtf
                    return null;
            }
        }
    }
    
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

    public static void PrivacyManagerView(GameObject prefab)
    {
        var campaignId = CmpCampaignPopupQueue.CurrentCampaignToShow();
        switch (campaignId)
        {
            case 0:
                if (!isGdprPmInitialized)
                    NetworkClient.Instance.PrivacyManagerViews(campaignId, propertyId, language, OnPrivacyManagerViewsSuccessCallback,delegate { InstantiateOnCanvas(prefab); }, OnExceptionCallback);
                else
                    InstantiateOnCanvas(prefab);
                break;
            case 2:
                if (!isCcpaPmInitialized)
                    NetworkClient.Instance.PrivacyManagerViews(campaignId, propertyId, language, OnPrivacyManagerViewsSuccessCallback,delegate { InstantiateOnCanvas(prefab); }, OnExceptionCallback);
                else
                    InstantiateOnCanvas(prefab);
                break;
        }
    }

    public static void Message()
    {
        isInitialized = false;
        NetworkClient.Instance.MessageGdpr(environment,
                                           language,
                                           propertyId,
                                   privacyManagerId,
                                           OnMessageSuccessCallback, 
                                           OnExceptionCallback);
    }

    public static void Consent(int actionCode)
    {
        lastActionCode = actionCode;
        switch (actionCode)
        {

            case 1:
                ConsentSaveAndExitVariables saveAndExitVariables = null;
                switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
                {
                    case 0:
                        saveAndExitVariables = new ConsentGdprSaveAndExitVariables(
                            language: language,
                            privacyManagerId: privacyManagerId, 
                            categories: CmpPmSaveAndExitVariablesContext.GetAcceptedCategories(), 
                            vendors: CmpPmSaveAndExitVariablesContext.GetAcceptedVendors(),
                            specialFeatures: CmpPmSaveAndExitVariablesContext.GetSpecialFeatures()); 
                        break;
                    case 2:
                        saveAndExitVariables = new ConsentCcpaSaveAndExitVariables(
                            language: language,
                            privacyManagerId: privacyManagerId, 
                            rejectedCategories: CmpPmSaveAndExitVariablesContext.GetAcceptedCategories(), 
                            rejectedVendors: CmpPmSaveAndExitVariablesContext.GetAcceptedVendors(),
                            specialFeatures: CmpPmSaveAndExitVariablesContext.GetSpecialFeatures());
                        break;
                }
                NetworkClient.Instance.Consent(actionType: actionCode, 
                                                   environment: environment,
                                                   language: language,
                                                   privacyManagerId: privacyManagerId, 
                                                   onSuccessAction: OnConsentSuccessCallback, 
                                                   onErrorAction: OnExceptionCallback,
                                                   pmSaveAndExitVariables: saveAndExitVariables);
                break;
            default:
                NetworkClient.Instance.Consent(actionType: actionCode, 
                                                   environment: environment,
                                                   language: language,
                                                   privacyManagerId: privacyManagerId, 
                                                   onSuccessAction: OnConsentSuccessCallback, 
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
                    SaveContext.SaveGdprUserConsent(gdprUserConsent);
                    CmpCampaignPopupQueue.DequeueCampaignId();
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
            if (ccpaCamp.userConsent != null && (!ccpaCamp.userConsent.newUser || ccpaCamp.userConsent.rejectedAll || ccpaCamp.userConsent.status.Equals("consentedAll")))
            {
                ccpaUserConsent = new PostConsentUserConsent()
                {
                    uspstring = ccpaCamp.userConsent.uspstring,
                    status = ccpaCamp.userConsent.status,
                    rejectedVendors = ccpaCamp.userConsent.rejectedVendors.ToArray(),
                    rejectedCategories = ccpaCamp.userConsent.rejectedCategories.ToArray(),
                    signedLspa = ccpaCamp.userConsent.signedLspa,
                    rejectedAll = ccpaCamp.userConsent.rejectedAll,
                 };
                SaveContext.SaveCcpaUserConsent(ccpaUserConsent);
                CmpCampaignPopupQueue.DequeueCampaignId();
                isCcpaConsented = true;
            }
            else if(ccpaCamp?.message != null || ccpaCamp.ui != null || ccpaCamp.ui.Count != 0)
            {
                BaseMessage ccpa = ccpaCamp?.message;
                popupBgColors = ccpaCamp.popupBgColors;
                ccpaElements = ccpaCamp.ui;
                // shortCategories ??
            }
            else if (ccpaCamp.userConsent == null)
                UnityEngine.Debug.LogError("UserConsent is NULL");
        }
        isInitialized = true;
        if (!CmpCampaignPopupQueue.IsCampaignAvailable)
            CmpPopupDestroyer.DestroyAllHelperGO();
    }

    private static void OnPrivacyManagerViewsSuccessCallback(string json)
    {
        switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
        {
            case 0:
                NativeUiJsonDeserializer.DeserializeExtraCall(json: json,
                                                             categoryModels: ref gdprCategories,
                                                             specialPurposeModels: ref gdprSpecialPurposes,
                                                             featureModels: ref gdprFeatures,
                                                             specialFeatureModels: ref gdprSpecialFeatures,
                                                             vendorModels: ref gdprVendors);
                isGdprPmInitialized = true;
                break;
            case 2:
                NativeUiJsonDeserializer.DeserializeExtraCall(json: json,
                                                             categoryModels: ref ccpaCategories,
                                                             specialPurposeModels: ref ccpaSpecialPurposes,
                                                             featureModels: ref ccpaFeatures,
                                                             specialFeatureModels: ref ccpaSpecialFeatures,
                                                             vendorModels: ref ccpaVendors);
                isCcpaPmInitialized = true;
                break;
        }
        SaveContext.UpdateUserConsentUIState();
    }

    public static void InstantiateOnCanvas(GameObject prefab)
    {
        if(prefab!=null && canvas != null)
            GameObject.Instantiate(prefab, canvas.transform);
    }
    
    private static void OnMessageSuccessCallback(string json)
    {
        var message = NativeUiJsonDeserializer.DeserializeMessageGdprGetResponse(json);
        if (message.message?.categories != null && message.message?.categories.Count > 0)
        {
            shortCategories?.Clear();
            shortCategories = message.message?.categories;
        }
        if (message.ui != null && message.ui.Count > 0)
        {
            switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    gdprElements?.Clear();
                    gdprElements = message.ui;
                    break;
                case 2:
                    ccpaElements?.Clear();
                    ccpaElements = message.ui;
                    break;
            }
        }
        if (message.popupBgColors != null && message.popupBgColors.Count > 0)
        {
            popupBgColors?.Clear();
            popupBgColors = message.popupBgColors;
        }
        SaveContext.UpdateUserConsentUIState();
        isInitialized = true;
        IsPmReadyForResurface = true;
    }

    private static void OnConsentSuccessCallback(string json)
    {
        var consent = JsonSerializer.Deserialize<PostConsentResponse>(json);
        SaveContext.SaveLocalState(consent.localState);
        CmpPopupDestroyer.DestroyAllPopups();
        switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
        {
            case 0:
                gdprUserConsent = consent.userConsent;
                SaveContext.SaveGdprUserConsent(gdprUserConsent);
                gdprUserConsent.uuid = consent.uuid;
                isGdprConsented = true;
                CmpCampaignPopupQueue.DequeueCampaignId();
                break;
            case 2:
                ccpaUserConsent = consent.userConsent;
                SaveContext.SaveCcpaUserConsent(ccpaUserConsent);
                ccpaUserConsent.uuid = consent.uuid;
                isCcpaConsented = true;
                CmpCampaignPopupQueue.DequeueCampaignId();
                break;
        }
        if (CmpCampaignPopupQueue.IsCampaignAvailable)
            InstantiateOnCanvas(homePrefab);
        else
            CmpPopupDestroyer.DestroyAllHelperGO();
    }
    #endregion

    private static void OnExceptionCallback(Exception ex)
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

    public static bool IsCurrentCampaignPmInitialized()
    {
        var campaignId = CmpCampaignPopupQueue.CurrentCampaignToShow();
        switch (campaignId)
        {
            case 0:
                return isGdprPmInitialized;
            case 2:
                return isCcpaPmInitialized;
            default:
                return false;
        }
    }
}