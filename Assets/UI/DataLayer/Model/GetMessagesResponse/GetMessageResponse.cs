using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GetMessageResponse
{
    [JsonInclude] public int propertyId;
    [JsonInclude] public PropertyPriorityData propertyPriorityData;
    [JsonInclude] public LocalState localState;
    [JsonInclude] public List<BaseGetMessagesCampaign> campaigns;

    public GdprGetMessagesCampaign GetGdprCampaign()
    {
        return GetCampaign<GdprGetMessagesCampaign>();
    }
    
    public CcpaGetMessagesCampaign GetCcpaCampaign()
    {
        return GetCampaign<CcpaGetMessagesCampaign>();
    }

    private T GetCampaign<T>() where T : BaseGetMessagesCampaign
    {
        T result = null;
        if (campaigns != null && campaigns.Count > 0)
        {
            foreach (var camp in campaigns)
            {
                if (camp is T campaign)
                    result = campaign;
            }
        }
        return result;
    }
}