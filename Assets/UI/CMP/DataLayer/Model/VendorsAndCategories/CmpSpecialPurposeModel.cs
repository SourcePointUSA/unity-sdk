using System.Text.Json.Serialization;

public class CmpSpecialPurposeModel : CmpFeatureModel
{
    [JsonInclude] public string type;
}
