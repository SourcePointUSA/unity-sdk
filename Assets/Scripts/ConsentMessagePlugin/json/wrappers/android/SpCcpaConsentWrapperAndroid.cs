using System.Text.Json.Serialization;

namespace ConsentManagementProviderLib
{
    public class SpCcpaConsentWrapperAndroid : CcpaConsentWrapper
    {
        [JsonInclude] public object newUser;
        [JsonInclude] public object rejectedAll;
        [JsonInclude] public object signedLspa;
        [JsonInclude] public string dateCreated;
    }

}