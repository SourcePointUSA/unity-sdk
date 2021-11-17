using UnityEngine;

public class CmpOnOffButtonController : MonoBehaviour
{
    [SerializeField] private CmpScrollController scrollController;
    
    public void OnButtonClick()
    {
        if (scrollController is CmpCategoryDetailsScrollController catDetailsScroll)
        {
            var model = catDetailsScroll.GetModel();
            model.accepted = true;
            switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    CmpPmSaveAndExitVariablesContext.AcceptCategory(model);
                    break;
                case 2:
                    CmpPmSaveAndExitVariablesContext.ExcludeCategory(model._id, true);
                    break;
            }
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            model.accepted = true;
            switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    CmpPmSaveAndExitVariablesContext.AcceptVendor(model);
                    break;
                case 2:
                    CmpPmSaveAndExitVariablesContext.ExcludeVendor(model.vendorId, model.name);
                    break;
            }
        }
        Destroy(scrollController.gameObject);
    }

    public void OffButtonClick()
    {
        if (scrollController is CmpCategoryDetailsScrollController catDetailsScroll)
        {
            var model = catDetailsScroll.GetModel();
            model.accepted = false;
            switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    CmpPmSaveAndExitVariablesContext.ExcludeCategory(model._id);
                    break;
                case 2:
                    CmpPmSaveAndExitVariablesContext.AcceptCategory(model, false);
                    break;
            }
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            model.accepted = false;
            switch (CmpCampaignPopupQueue.CurrentCampaignToShow())
            {
                case 0:
                    CmpPmSaveAndExitVariablesContext.ExcludeVendor(model.vendorId, model.name);
                    break;
                case 2:
                    CmpPmSaveAndExitVariablesContext.AcceptVendor(model, false);
                    break;
            }
        }
        Destroy(scrollController.gameObject);
    }
}
