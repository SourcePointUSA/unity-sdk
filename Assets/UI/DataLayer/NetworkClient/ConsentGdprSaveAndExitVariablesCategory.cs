using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariablesCategory
{
    [JsonInclude] public string _id;
    [JsonInclude] public int? iabId;
    [JsonInclude] public string type;
    [JsonInclude] public bool consent;
    [JsonInclude] public bool legInt;

    public ConsentGdprSaveAndExitVariablesCategory(string _id, int? iabId, string type, bool consent, bool legInt)
    {
        this._id = _id;
        this.iabId = iabId;
        this.type = type;
        this.consent = consent;
        this.legInt = legInt;
    }
}