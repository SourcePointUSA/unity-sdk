using System.Text.Json.Serialization;

public class PostConsentGdprRequest
{
    // [JsonInclude] public List<string> pubData = new List<string>();                //TODO
    [JsonInclude] public ConsentGdprSaveAndExitVariables pmSaveAndExitVariables; 
    [JsonInclude] public IncludeDataPostGetMessagesRequest includeData;
    [JsonInclude] public string requestUUID;
    [JsonInclude] public string idfaStatus;
    [JsonInclude] public LocalState localState;

    public PostConsentGdprRequest(string requestUUID, string idfaStatus, LocalState localState, IncludeDataPostGetMessagesRequest includeData, ConsentGdprSaveAndExitVariables pmSaveAndExitVariables)
    {
        this.requestUUID = requestUUID;
        this.idfaStatus = idfaStatus;
        this.localState = localState;
        this.includeData = includeData;
        this.pmSaveAndExitVariables = pmSaveAndExitVariables;
    }
}