using System;
using System.Collections.Generic;
using UnityEngine;

public class CmpVendorScrollController : CmpScrollController
{
    [SerializeField] private CmpSwitch cmpSwitch;
    [SerializeField] private GameObject vendorDetailsPrefab;

    public override void FillView()
    {
        FillVendorView();
    }

    private void FillVendorView()
    {
        ClearScrollContent();
        switch (cmpSwitch.currentBtn)
        {
            case CmpSwitch.BUTTON_SELECTED.LEFT:
                //Consent Tab
                AddVendors(CmpLocalizationMapper.vendors);
                ScrollAppear();
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
                ScrollAppear();
                break;
        }
    }

    private void AddVendors(List<CmpVendorModel> vendors)
    {
        foreach (CmpVendorModel vendor in vendors)
        {
            bool enableCustomTextLabel = vendor.vendorType.Equals("CUSTOM");
            CmpLongButtonUiController longController = AddCell(vendor.name, enableCustomTextLabel);
            CmpLongButtonController btn = longController.gameObject.AddComponent<CmpLongButtonController>();
            btn.SetButtonRef(longController.GetButton());
            btn.SetOnClickAction(delegate { InstantiateVendorDetailsPrefab(vendor); } );
        }
    }

    private CmpLongButtonUiController AddCell(string mainText, bool enableCustomTextLabel)
    {
        var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
        var longController = cell.GetComponent<CmpLongButtonUiController>();
        var longElement = postponedElements["VendorButton"];
        longController.SetLocalization(longElement);
        longController.SetMainText(mainText);
        longController.EnableCustomTextLabel(enableCustomTextLabel);
        return longController;
    }

    private void InstantiateVendorDetailsPrefab(CmpVendorModel model)
    {
        var canvas = GameObject.Find("Canvas").transform;
        GameObject go = Instantiate(vendorDetailsPrefab, canvas);
        CmpVendorDetailsScrollController detailsController = go.GetComponent<CmpVendorDetailsScrollController>();
        detailsController.SetInfo(model);
    }
}
