using UnityEngine;
using UnityEngine.UI;

public class CmpCategoryDetailsScrollController : CmpScrollController
{
    [SerializeField] Text header;
    [SerializeField] Text description;
    [SerializeField] CmpPartnersCounter partnersCount;
    CmpCategoryBaseModel model;

    public void SetInfo(CmpCategoryBaseModel model)
    {
        header.text = model.name;
        description.text = model.description;
        this.model = model;
        FillView();
        partnersCount.OnScrollElementAmountChange();
    }

    public override void FillView()
    {
        ClearScrollContent();
        if (this.model is CmpCategoryModel)
        {
            CmpCategoryModel mod = model as CmpCategoryModel;
            foreach(CmpCategoryConsentVendorModel categoryConsentVendor in mod.requiringConsentVendors)
            {
                var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
                CmpLongButtonUiController longButtonController = cell.GetComponent<CmpLongButtonUiController>();
                var longElement = postponedElements["VendorLongButton"];
                longButtonController.SetLocalization(longElement);
                longButtonController.SetMainText(categoryConsentVendor.name);
            }
        }
        ScrollAppear();
        //TODO: 
        //CmpSpecialPurposeModel    //  ?
        //CmpSpecialFeatureModel    //  ?
    }
}