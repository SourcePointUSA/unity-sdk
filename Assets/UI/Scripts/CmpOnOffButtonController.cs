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
            CmpPmSaveAndExitVariablesContext.AcceptCategory(model);
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            model.accepted = true;
            CmpPmSaveAndExitVariablesContext.AcceptVendor(model);
        }
        Destroy(scrollController.gameObject);
    }

    public void OffButtonClick()
    {
        if (scrollController is CmpCategoryDetailsScrollController catDetailsScroll)
        {
            var model = catDetailsScroll.GetModel();
            model.accepted = false;
            CmpPmSaveAndExitVariablesContext.ExcludeCategory(model._id);
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            model.accepted = false;
            CmpPmSaveAndExitVariablesContext.ExcludeVendor(model.vendorId);
        }
        Destroy(scrollController.gameObject);
    }
}
