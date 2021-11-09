using System.Text.Json.Serialization;

public class PostConsentCcpaRequest : PostConsentRequest
{
    [JsonInclude] public ConsentCcpaSaveAndExitVariables pmSaveAndExitVariables; 

    public PostConsentCcpaRequest(string requestUUID, string idfaStatus, LocalState localState, IncludeDataPostGetMessagesRequest includeData, ConsentCcpaSaveAndExitVariables pmSaveAndExitVariables)
    {
        this.requestUUID = requestUUID;
        this.idfaStatus = idfaStatus;
        this.localState = localState;
        this.includeData = includeData;
        this.pmSaveAndExitVariables = pmSaveAndExitVariables;
    }
}