using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    public class SpActionWrapper
    {
        [JsonInclude] public string type;
        [JsonInclude] public string customActionId;
    }
}   
