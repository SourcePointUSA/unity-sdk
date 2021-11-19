using System.Text.Json.Serialization;

public class SelectedPrivacyManager
{
    [JsonInclude] public int type;
    [JsonInclude] public SelectedPrivacyManagerData data;
}