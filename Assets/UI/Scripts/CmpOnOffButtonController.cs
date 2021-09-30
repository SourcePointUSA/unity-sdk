using UnityEngine;

public class CmpOnOffButtonController : MonoBehaviour
{
    [SerializeField] private CmpScrollController scrollController;
    
    public void OnButtonClick()
    {
        if (scrollController is CmpCategoryDetailsScrollController catDetailsScroll)
        {
            var model = catDetailsScroll.GetModel();
            CmpPmSaveAndExitVariablesContext.AcceptCategory(model.iabId, model._id, model.type);
            foreach (var vendor in model.requiringConsentVendors)
            {
                int? iabId = null;
                foreach (var v in CmpLocalizationMapper.vendors)
                {
                    if (v.vendorId.Equals(vendor.vendorId) && v.iabId.HasValue)
                        iabId = v.iabId.Value;
                }
                Debug.LogWarning(iabId.ToString());
                CmpPmSaveAndExitVariablesContext.AcceptVendor(iabId, vendor.vendorId, vendor.vendorType);
            }
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            CmpPmSaveAndExitVariablesContext.AcceptVendor(model.iabId, model.vendorId, model.vendorType);
        }
        Destroy(scrollController.gameObject);
    }

    public void OffButtonClick()
    {
        if (scrollController is CmpCategoryDetailsScrollController catDetailsScroll)
        {
            var model = catDetailsScroll.GetModel();
            CmpPmSaveAndExitVariablesContext.ExcludeCategory(model._id);
            foreach (var vendor in model.requiringConsentVendors)
            {
                CmpPmSaveAndExitVariablesContext.ExcludeVendor(vendor.vendorId);
            }
        }else if (scrollController is CmpVendorDetailsScrollController vendDetailsScroll)
        {
            var model = vendDetailsScroll.GetModel();
            CmpPmSaveAndExitVariablesContext.ExcludeVendor(model.vendorId);
        }
        Destroy(scrollController.gameObject);
    }
}
