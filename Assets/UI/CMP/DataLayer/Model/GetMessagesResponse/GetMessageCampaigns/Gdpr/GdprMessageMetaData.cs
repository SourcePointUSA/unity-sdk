using System.Text.Json.Serialization;

public class GdprMessageMetaData
{
    [JsonInclude] public int messageId;
    [JsonInclude] public string prtnUUID;
    [JsonInclude] public string msgDescription;
    [JsonInclude] public int bucket;
    [JsonInclude] public int categoryId;
    [JsonInclude] public int subCategoryId;
}