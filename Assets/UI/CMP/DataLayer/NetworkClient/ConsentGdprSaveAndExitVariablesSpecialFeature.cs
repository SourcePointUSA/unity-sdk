using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariablesSpecialFeature
{
    [JsonInclude] public string _id;
    [JsonInclude] public int? iabId;

    public ConsentGdprSaveAndExitVariablesSpecialFeature(string id, int? iabId)
    {
        this._id = id;
        this.iabId = iabId;
    }
}