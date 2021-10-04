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
            foreach (var vendor in model.requiringConsentVendors)
            {
                int? iabId = null;
                foreach (var v in CmpLocalizationMapper.vendors)
                {
                    if (v.vendorId.Equals(vendor.vendorId) && v.iabId.HasValue)
                        iabId = v.iabId.Value;
                }
                CmpPmSaveAndExitVariablesContext.AcceptVendor(iabId, vendor.vendorId, vendor.vendorType);
            }
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            model.accepted = true;
            CmpPmSaveAndExitVariablesContext.AcceptVendor(model.iabId, model.vendorId, model.vendorType);
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
            foreach (var vendor in model.requiringConsentVendors)
            {
                CmpPmSaveAndExitVariablesContext.ExcludeVendor(vendor.vendorId);
            }
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            model.accepted = false;
            CmpPmSaveAndExitVariablesContext.ExcludeVendor(model.vendorId);
        }
        Destroy(scrollController.gameObject);
    }
}
