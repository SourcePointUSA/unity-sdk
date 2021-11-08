using System;
using System.Collections.Generic;
using UnityEngine;

public class CmpVendorScrollController : CmpScrollController
{
    [SerializeField] private CmpSwitch cmpSwitch;
    [SerializeField] private GameObject vendorDetailsPrefab;

    private void Update()
    {
        if (CmpPmSaveAndExitVariablesContext.IsAcceptedVendorsChanged)
        {
            FillVendorView();
            CmpPmSaveAndExitVariablesContext.SetAcceptedVendorsChangedFalse();
        }
    }

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
                if(CmpLocalizationMapper.vendors!=null)
                    AddVendors(CmpLocalizationMapper.vendors);
                ScrollAppear();
                break;
            case CmpSwitch.BUTTON_SELECTED.RIGHT:
                //Legitimate Interest Tab
                List<CmpVendorModel> legIntCategories = new List<CmpVendorModel>();
                if(CmpLocalizationMapper.vendors!=null)
                    foreach (CmpVendorModel model in CmpLocalizationMapper.vendors)
                        if (model.legIntCategories!=null && model.legIntCategories.Count > 0)
                            legIntCategories.Add(model);
                AddVendors(legIntCategories);
                ScrollAppear();
                break;
        }
    }

    private void AddVendors(List<CmpVendorModel> vendors)
    {
        foreach (CmpVendorModel vendor in vendors)
        {
            bool isAccepted = CmpPmSaveAndExitVariablesContext.IsVendorAcceptedAnywhere(vendor.vendorId) || vendor.accepted;
            bool enableCustomTextLabel = vendor.vendorType!= null && vendor.vendorType.Equals("CUSTOM");
            CmpLongButtonUiController longController = AddCell(vendor.name, enableCustomTextLabel);
            longController.SetGroupState(isAccepted);
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
