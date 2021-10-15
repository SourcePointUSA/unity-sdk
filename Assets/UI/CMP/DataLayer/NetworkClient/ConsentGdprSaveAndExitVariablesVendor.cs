using System.Text.Json.Serialization;

public class ConsentGdprSaveAndExitVariablesVendor
{
    [JsonInclude] public string _id;
    [JsonInclude] public int? iabId;
    [JsonInclude] public string vendorType;
    [JsonInclude] public bool consent;
    [JsonInclude] public bool legInt;

    public ConsentGdprSaveAndExitVariablesVendor(string _id, int? iabId, string vendorType, bool consent, bool legInt)
    {
        this._id = _id;
        this.iabId = iabId;
        this.vendorType = vendorType;
        this.consent = consent;
        this.legInt = legInt;
    }
}