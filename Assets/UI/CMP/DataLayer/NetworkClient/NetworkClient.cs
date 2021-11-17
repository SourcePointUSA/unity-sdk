using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

public class NetworkClient
{
    // HttpClient is intended to be instantiated once per application, rather than per-use
    readonly HttpClient client = new HttpClient();
    private static GameObject dispatcherGO;
    private static NetworkCallbackEventDispatcher dispatcher;
    public static int msTimeout;
    private static NetworkClient instance;
    public static NetworkClient Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetworkClient();
                msTimeout = 3000;
                instance.client.Timeout = TimeSpan.FromMilliseconds(3000);
            }
            if (dispatcherGO == null)
            {
                dispatcherGO = new GameObject();
                dispatcher = dispatcherGO.AddComponent<NetworkCallbackEventDispatcher>();
            }
            return instance;
        }
    }
    
    #region Public
    public void GetMessages(int accountId, string propertyHref, CampaignsPostGetMessagesRequest campaigns, Action<string> onSuccessAction, Action<Exception> onErrorAction, int environment, int millisTimeout)
    {
        msTimeout = millisTimeout;
        instance.client.Timeout = TimeSpan.FromMilliseconds(millisTimeout);
        string idfaStatus = "unknown";
        var dict = new Dictionary<string, string> {{"type", "RecordString"}};
        var includeData = new IncludeDataPostGetMessagesRequest()
        {
            localState = dict,
            messageMetaData = dict,
            TCData = dict
        };
        var requestBody = new PostGetMessagesRequest(accountId, propertyHref, idfaStatus, GUID.Value, campaigns, 
            SaveContext.GetLocalState(), 
            // new LocalState(), // TODO: remove & uncomment line above
            includeData);
        Task.Factory.StartNew(async delegate { await PostGetMessages(requestBody, environment, onSuccessAction, onErrorAction); });
    }

    public void PrivacyManagerViews(int campaignId, string siteId, string consentLanguage, Action<string> onSuccessAction, Action onSuccessInstantiateGOCallback, Action<Exception> onErrorAction)
    {
        switch (campaignId)
        {
            case 0:
                Task.Factory.StartNew(async delegate { await GetPrivacyManagerView(GetGdprPrivacyManagerViewUriWithQueryParams(siteId, consentLanguage), onSuccessAction, onSuccessInstantiateGOCallback, onErrorAction); });
                break;
            case 2:
                Task.Factory.StartNew(async delegate { await GetPrivacyManagerView(GetCcpaPrivacyManagerViewUriWithQueryParams(siteId, consentLanguage), onSuccessAction, onSuccessInstantiateGOCallback, onErrorAction); });
                break;
        }
    }

    public void MessageGdpr(int environment, string consentLanguage, string propertyId, string messageId, Action<string> onSuccessAction, Action<Exception> onErrorAction)
    {
        Task.Factory.StartNew(async delegate { await GetMessage(environment, consentLanguage, propertyId, messageId, onSuccessAction, onErrorAction); });
    }

    public void Consent(/*CONSENT_ACTION_TYPE == */ int actionType, 
        int environment,
        string language, 
        string privacyManagerId,
        Action<string> onSuccessAction,
        Action<Exception> onErrorAction,
        ConsentSaveAndExitVariables pmSaveAndExitVariables = null)
    {
        var dict = new Dictionary<string, string> {{"type", "RecordString"}};
        var includeData = new IncludeDataPostGetMessagesRequest()
        {
            localState = dict,
            TCData = dict
            // messageMetaData = dict,
        };
        ConsentSaveAndExitVariables concretePmSaveAndExitVariables;
        PostConsentRequest body = null;
        switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
        {
            case 0:
                if (pmSaveAndExitVariables != null)
                    concretePmSaveAndExitVariables = pmSaveAndExitVariables as ConsentGdprSaveAndExitVariables;
                else
                    concretePmSaveAndExitVariables = new ConsentGdprSaveAndExitVariables(
                        language: language, 
                        privacyManagerId: privacyManagerId,
                        categories: new ConsentGdprSaveAndExitVariablesCategory[] { },
                        vendors: new ConsentGdprSaveAndExitVariablesVendor[] { },
                        specialFeatures: new ConsentGdprSaveAndExitVariablesSpecialFeature[] { }
                        );
                body = new PostConsentGdprRequest(
                    requestUUID: GUID.Value,
                    idfaStatus: "accepted",
                    localState: SaveContext.GetLocalState(),
                    includeData: includeData,
                    pmSaveAndExitVariables: (ConsentGdprSaveAndExitVariables) concretePmSaveAndExitVariables
                    );
                break;
            case 2:
                if (pmSaveAndExitVariables != null)
                {
                    //remove if vendor._id == null
                    concretePmSaveAndExitVariables = pmSaveAndExitVariables as ConsentCcpaSaveAndExitVariables;
                    List<ConsentGdprSaveAndExitVariablesVendor> rej = new List<ConsentGdprSaveAndExitVariablesVendor>();
                    foreach (var rejected in (pmSaveAndExitVariables as ConsentCcpaSaveAndExitVariables).rejectedVendors)
                        if (!string.IsNullOrEmpty(rejected._id))
                            rej.Add(rejected);
                    (concretePmSaveAndExitVariables as ConsentCcpaSaveAndExitVariables).rejectedVendors = rej.ToArray();
                }
                else
                    concretePmSaveAndExitVariables = new ConsentCcpaSaveAndExitVariables(
                        language: language, 
                        privacyManagerId: privacyManagerId,
                        rejectedCategories: new ConsentGdprSaveAndExitVariablesCategory[] { },
                        rejectedVendors: new ConsentGdprSaveAndExitVariablesVendor[] { },
                        specialFeatures: new ConsentGdprSaveAndExitVariablesSpecialFeature[] { }
                        );
                body = new PostConsentCcpaRequest(
                    requestUUID: GUID.Value,
                    idfaStatus: "accepted",
                    localState: SaveContext.GetLocalState(),
                    includeData: includeData,
                    pmSaveAndExitVariables: (ConsentCcpaSaveAndExitVariables) concretePmSaveAndExitVariables
                    );
                break;
        }

        if (body == null)
        {
            onErrorAction?.Invoke(new Exception("Message body is null!!!"));
        }
        else
            Task.Factory.StartNew(async delegate { await PostConsent(actionType, environment, body, onSuccessAction, onErrorAction); });
    }
    #endregion
    
    #region Query Parameters
    private static string GetMessageUriWithQueryParams(int environment, string consentLanguage, string propertyId, string messageId)
    {
        // https://cdn.sp-stage.net/wrapper/v2/message/gdpr?env=stage&consentLanguage=en&propertyId=4933&messageId=16434
        string env = environment == 0 ? "stage" : "prod";
        string campaignName = CmpCampaignPopupQueue.CurrentCampaignToShow() == 0 ? "gdpr" : "ccpa";
        return BuildUriWithQuery(baseAdr: "https://cdn.sp-stage.net/wrapper/v2/",
                                path: "wrapper/v2/message/" + campaignName,
                                qParams: new Dictionary<string, string>()
                                {
                                    {"env", env},
                                    {"consentLanguage", consentLanguage},
                                    {"propertyId", propertyId},
                                    {"messageId", messageId},
                                });
    }

    private static string GetGdprPrivacyManagerViewUriWithQueryParams(string siteId, string consentLanguage)
    {
        // https://cdn.privacy-mgmt.com/consent/tcfv2/privacy-manager/privacy-manager-view?siteId=17935&consentLanguage=EN
        // return BuildUriWithQuery(baseAdr: "https://cdn.privacy-mgmt.com/",
        return BuildUriWithQuery(baseAdr: "https://cdn.sp-stage.net/",
                                path: "consent/tcfv2/privacy-manager/privacy-manager-view/",
                                qParams: new Dictionary<string, string>()
                                {
                                    { "siteId", siteId },
                                    { "consentLanguage", consentLanguage },
                                });
    }
    
    private static string GetCcpaPrivacyManagerViewUriWithQueryParams(string siteId, string consentLanguage)
    {
        // https://cdn.privacy-mgmt.com/ccpa/privacy-manager/privacy-manager-view/?siteId=17935&consentLanguage=EN
        return BuildUriWithQuery(baseAdr: "https://cdn.privacy-mgmt.com/",
            path: "ccpa/privacy-manager/privacy-manager-view/",
            qParams: new Dictionary<string, string>()
            {
                { "siteId", siteId },
                { "consentLanguage", consentLanguage },
            });
    }

    private static string GetGetMessagesUriWithQueryParams(int environment)
    {
        // https://cdn.sp-stage.net/wrapper/v2/get_messages/?env=stage
        string env = environment == 0 ? "stage" : "prod";
        return BuildUriWithQuery(baseAdr: "https://cdn.sp-stage.net/",
            path: "wrapper/v2/get_messages/",
            qParams: new Dictionary<string, string>()
            {
                { "env", env },
            });
    }
    
    private static string GetConsentGdprQueryParams(int action, int environment)
    {
        // https://cdn.privacy-mgmt.com/wrapper/v2/messages/choice/gdpr/11?env=prod
        // return BuildUriWithQuery(baseAdr: "https://cdn.privacy-mgmt.com/",
        string env = environment == 0 ? "stage" : "prod";
        string campaignName = CmpCampaignPopupQueue.CurrentCampaignToShow() == 0 ? "gdpr/" : "ccpa/";
        return BuildUriWithQuery(baseAdr: "https://cdn.sp-stage.net/",
            path: "wrapper/v2/messages/choice/" + campaignName + action.ToString(),
            qParams: new Dictionary<string, string>()
            {
                { "env", env },
            });
    }
    
    private static string BuildUriWithQuery(string baseAdr, string path, Dictionary<string, string> qParams)
    {
        var builder = new UriBuilder(baseAdr) {Port = -1};
        builder.Path = path;
        var query = HttpUtility.ParseQueryString(builder.Query);
        foreach (KeyValuePair<string, string> kv in qParams)
        {
            query[kv.Key] = kv.Value;
        }                             
        builder.Query = query.ToString();
        return builder.ToString();
    }
    #endregion

    #region Network Requests
    async Task GetMessage(int environment, string consentLanguage, string propertyId, string messageId, Action<string> onSuccessAction, Action<Exception> onErrorAction)
    {
        try
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(GetMessageUriWithQueryParams(environment, consentLanguage, propertyId, messageId));
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            dispatcher.Enqueue(delegate { onSuccessAction?.Invoke(responseBody); });
        }
        catch(HttpRequestException ex)
        {
            dispatcher.Enqueue(delegate { onErrorAction?.Invoke(ex); });
        }
    }

    async Task GetPrivacyManagerView(string requestUri, Action<string> onSuccessAction, Action onSuccessInstantiateGOCallback, Action<Exception> onErrorAction)
    {
        try
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            dispatcher.Enqueue(delegate { onSuccessAction?.Invoke(responseBody); });
            dispatcher.Enqueue(delegate { onSuccessInstantiateGOCallback?.Invoke(); });
        }
        catch (Exception ex)
        {            
            dispatcher.Enqueue(delegate { onErrorAction?.Invoke(ex); });
        }
    }

    async Task PostGetMessages(PostGetMessagesRequest body, int environment, Action<string> onSuccessAction, Action<Exception> onErrorAction)
    {
        try
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var options = new JsonSerializerOptions { IgnoreNullValues = true };
            string json = JsonSerializer.Serialize(body, options);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(GetGetMessagesUriWithQueryParams(environment), data);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            dispatcher.Enqueue(delegate { onSuccessAction?.Invoke(responseBody); });
        }
        catch (Exception ex)
        {            
            dispatcher.Enqueue(delegate { onErrorAction?.Invoke(ex); });
        }
    }

    async Task PostConsent(int actionType, int environment, PostConsentRequest body, Action<string> onSuccessAction, Action<Exception> onErrorAction)
    {
        try
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var options = new JsonSerializerOptions { IgnoreNullValues = true };
            string json = null;
            switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    json = JsonSerializer.Serialize(body as PostConsentGdprRequest, options);
                    break;
                case 2:
                    json = JsonSerializer.Serialize(body as PostConsentCcpaRequest, options);
                    break;
            }
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            string link = GetConsentGdprQueryParams(actionType, environment);
            HttpResponseMessage response = await client.PostAsync(link, data);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            dispatcher.Enqueue(delegate { onSuccessAction?.Invoke(responseBody); });
        }
        catch (Exception ex)
        {          
            dispatcher.Enqueue(delegate { onErrorAction?.Invoke(ex); });
        }
    }
    #endregion
}