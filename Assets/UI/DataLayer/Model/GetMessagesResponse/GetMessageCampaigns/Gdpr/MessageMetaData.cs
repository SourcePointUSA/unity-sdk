using System.Text.Json.Serialization;

public class MessageMetaData
{
    [JsonInclude] public int categoryId;
    [JsonInclude] public int subCategoryId;
    [JsonInclude] public int messageId;
    
    [JsonInclude] public string prtnUUID;
    [JsonInclude] public string msgDescription;
    [JsonInclude] public int bucket;
}