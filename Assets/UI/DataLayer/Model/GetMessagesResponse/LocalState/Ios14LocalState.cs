using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Ios14LocalState
{
    [JsonInclude] public int propertyId;
    [JsonInclude] public string[] mmsCookies;
}