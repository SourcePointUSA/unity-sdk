using System.Text.Json.Serialization;

public class PostConsentGdprRequest : PostConsentRequest
{
    [JsonInclude] public ConsentGdprSaveAndExitVariables pmSaveAndExitVariables; 

    public PostConsentGdprRequest(string requestUUID, string idfaStatus, LocalState localState, IncludeDataPostGetMessagesRequest includeData, ConsentGdprSaveAndExitVariables pmSaveAndExitVariables)
    {
        this.requestUUID = requestUUID;
        this.idfaStatus = idfaStatus;
        this.localState = localState;
        this.includeData = includeData;
        this.pmSaveAndExitVariables = pmSaveAndExitVariables;
    }
}