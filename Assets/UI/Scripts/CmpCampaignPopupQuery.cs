using System.Collections.Generic;

public static class CmpCampaignPopupQuery
{
    private static Queue<int> campaignsToShow = new Queue<int>();
    public static bool IsCampaignAvailable => campaignsToShow.Count>0;

    public static void EnqueueCampaignId(int campId)
    {
        campaignsToShow.Enqueue(campId);
    }

    public static int CurrentCampaignToShow()
    {
        return campaignsToShow.Peek();
    }

    public static int DequeueCampaignId()
    {
        return campaignsToShow.Dequeue();
    }
}