using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CmpVendorDetailsScrollController : CmpScrollController
{
    [SerializeField] Text header;
    [SerializeField] Text description;
    [SerializeField] GameObject cmpDescriptionCellPrefab;
    [SerializeField] Image qrImage;
    CmpVendorModel model;

    internal void SetInfo(CmpVendorModel model)
    {
        header.text = model.name;
        description.text = model.cookieHeader;
        this.model = model;
        FillView();
        GenerateQR(model.policyUrl);
    }

    private void GenerateQR(string policyUrl)
    {
        if (!string.IsNullOrEmpty(policyUrl))
        {
            //TODO
        }
    }

    public override void FillView()
    {
        ClearScrollContent();
        if (model != null)
        {
            AddConsentCategories(model.consentCategories); // == Pusposes
            //TODO: legIntCategories //??
            AddSpecialPurposes(model.iabSpecialPurposes);
            AddFeatures(model.iabFeatures);
            AddSpecialFeatures(model.iabSpecialFeatures);
            RectTransform scrollRect = ((RectTransform)scrollContent.transform);
            scrollRect.SetPositionAndRotation(new Vector3(scrollRect.position.x, -400f, scrollRect.position.z), scrollRect.rotation);
        }
    }

    private void AddSpecialFeatures(List<string> iabSpecialFeatures)
    {
        if (iabSpecialFeatures.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "SpecialFeaturesText");
            AddStringList(iabSpecialFeatures);
        }
    }

    private void AddFeatures(List<string> iabFeatures)
    {
        if (iabFeatures.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "FeaturesText");
            AddStringList(iabFeatures);
        }
    }

    private void AddSpecialPurposes(List<string> iabSpecialPurposes)
    {
        if (iabSpecialPurposes.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "SpecialPurposesText");
            AddStringList(iabSpecialPurposes);
        }
    }

    private void AddStringList(List<string> list)
    {
        foreach (string str in list)
            AddCell(str);
    }

    private void AddConsentCategories(List<CmpVendorCategoryModel> consentCategories)
    {
        if (consentCategories.Count > 0)
        {
            AddHeaderCell(cmpDescriptionCellPrefab, "PurposesText");
            foreach (CmpVendorCategoryModel model in consentCategories)
                AddCell(model.name);
        }
    }

    private void AddCell(string txt)
    {
        var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
        CmpLongButtonUiController longButtonController = cell.GetComponent<CmpLongButtonUiController>();
        var longElement = postponedElements["CategoryLongButton"];
        longButtonController.SetLocalization(longElement);
        longButtonController.SetMainText(txt);
    }
}