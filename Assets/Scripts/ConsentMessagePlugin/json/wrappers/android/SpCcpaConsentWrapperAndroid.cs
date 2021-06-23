using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib.Json
{
    internal class SpCcpaConsentWrapperAndroid : CcpaConsentWrapper
    {
        [JsonInclude] public object newUser;
        [JsonInclude] public object rejectedAll;
        [JsonInclude] public object signedLspa;
        [JsonInclude] public string dateCreated;
    }

}