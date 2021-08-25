using System.Collections.Generic;
using UnityEngine;

public class CmpVendorScrollController : CmpScrollController
{
    [SerializeField] private CmpSwitch cmpSwitch;

    public override void FillView()
    {
        FillVendorView();
    }

    private void FillVendorView()
    {
        ClearScrollContent();
        RectTransform scrollRect = ((RectTransform)scrollContent.transform);
        switch (cmpSwitch.currentBtn)
        {
            case CmpSwitch.BUTTON_SELECTED.LEFT:
                //Consent Tab
                AddVendors(CmpLocalizationMapper.vendors);
                scrollRect.SetPositionAndRotation(new Vector3(scrollRect.position.x, -400f, scrollRect.position.z), scrollRect.rotation);
                break;
            case CmpSwitch.BUTTON_SELECTED.RIGHT:
                //Legitimate Interest Tab
                List<CmpVendorModel> legIntCategories = new List<CmpVendorModel>();
                foreach (CmpVendorModel model in CmpLocalizationMapper.vendors)
                {
                    if (model.legIntCategories.Count > 0)
                        legIntCategories.Add(model);
                }
                AddVendors(legIntCategories);
                scrollRect.SetPositionAndRotation(new Vector3(scrollRect.position.x, -400f, scrollRect.position.z), scrollRect.rotation);
                break;
        }
    }

    private void AddVendors(List<CmpVendorModel> vendors)
    {
        foreach (CmpVendorModel vendor in vendors)
        {
            bool enableCustomTextLabel = false;
            if (vendor.vendorType.Equals("CUSTOM")) //TODO: CHECK
                enableCustomTextLabel = true;
             AddCell(vendor.name, enableCustomTextLabel);
        }
    }

    private void AddCell(string mainText, bool enableCustomTextLabel)
    {
        var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
        var longController = cell.GetComponent<CmpLongButtonUiController>();
        var longElement = postponedElements["VendorButton"];
        longController.SetLocalization(longElement);
        longController.SetMainText(mainText);
        longController.EnableCustomTextLabel(enableCustomTextLabel);
    }
}