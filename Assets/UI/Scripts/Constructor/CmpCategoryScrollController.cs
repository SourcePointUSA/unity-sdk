using System.Collections.Generic;
using UnityEngine;

public class CmpCategoryScrollController : CmpScrollController
{
    [SerializeField] private CmpSwitch cmpSwitch;
    [SerializeField] private GameObject cmpDescriptionCellPrefab;
    [SerializeField] private GameObject categoryDetailsPrefab;

    private void Update()
    {
        if (CmpPmSaveAndExitVariablesContext.IsAcceptedCategoriesChanged)
        {
            FillCategoryView();
            CmpPmSaveAndExitVariablesContext.SetAcceptedCategoriesChangedFalse();
        }
    }
    
    public override void FillView()
    {
        FillCategoryView();
    }

    private void FillCategoryView()
    {
        ClearScrollContent();
        switch (cmpSwitch.currentBtn)
        {
            case CmpSwitch.BUTTON_SELECTED.LEFT:
                //Consent Tab
                AddCategories(CmpLocalizationMapper.categories); // == Purposes
                AddSpecialPurposes(CmpLocalizationMapper.specialPurposes);
                AddFeatures(CmpLocalizationMapper.features);
                AddSpecialFeatures(CmpLocalizationMapper.specialFeatures);
                ScrollAppear(-1200f);
                break;
            case CmpSwitch.BUTTON_SELECTED.RIGHT:
                //Legitimate Interest Tab
                List<CmpCategoryModel> legIntVendors = new List<CmpCategoryModel>();
                foreach (CmpCategoryModel model in CmpLocalizationMapper.categories)
                {
                    if (model.legIntVendors.Count > 0)
                        legIntVendors.Add(model);
                }
                AddCategories(legIntVendors);
                break;
        }
    }

    private void AddSpecialFeatures(List<CmpSpecialFeatureModel> specialFeatures)
    {
        if(specialFeatures.Count>0)
            AddDescriptionCell(cmpDescriptionCellPrefab, "SpecialFeaturesHeader", "SpecialFeaturesDefinition");
        foreach (var specialFeature in specialFeatures)
        {
            var longController = AddCell(specialFeature.name, specialFeature.description);
            SetCellOnClickAction(longController, specialFeature);
        }
    }

    private void AddFeatures(List<CmpFeatureModel> features)
    {
        if(features.Count>0)
            AddDescriptionCell(cmpDescriptionCellPrefab, "FeaturesHeader", "FeaturesDefinition");
        foreach (var feature in features)
        {
            var longController = AddCell(feature.name, feature.description);
            SetCellOnClickAction(longController, feature);
        }
    }

    private void AddSpecialPurposes(List<CmpSpecialPurposeModel> specialPurposes)
    {
        if(specialPurposes.Count>0)
            AddDescriptionCell(cmpDescriptionCellPrefab, "SpecialPurposesHeader", "SpecialPurposesDefinition");
        foreach (var spec in specialPurposes)
        {
            var longController = AddCell(spec.name, spec.description);
            longController.DisableOnOffGroup();
            //SetCellOnClickAction(longController, spec);
        }
    }

    private void AddCategories(List<CmpCategoryModel> categories)
    {
        if(categories.Count>0)
            AddDescriptionCell(cmpDescriptionCellPrefab, "PurposesHeader", "PurposesDefinition");
        foreach (CmpCategoryModel cat in categories)
        {
            var longController = AddCell(cat.name, cat.friendlyDescription);
            bool accepted = cat.accepted || CmpPmSaveAndExitVariablesContext.IsCategoryAcceptedAnywhere(cat._id);
            longController.SetGroupState(accepted);
            SetCellOnClickAction(longController, cat);
        }
    }

    private CmpLongButtonUiController AddCell(string mainText, string description)
    {
        var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
        var longElement = postponedElements["CategoryButton"];
        var longController = cell.GetComponent<CmpLongButtonUiController>();
        longController.SetLocalization(longElement);
        longController.SetMainText(mainText);
        longController.EnableCustomTextLabel(false); //??

        var categoryRelatedController = cell.AddComponent<CmpLongButtonTextChangeController>();
        categoryRelatedController.SetChangingTextRef(changingText);
        categoryRelatedController.SetChangingTextString(description);
        categoryRelatedController.SetButtonRef(longController.GetButton());
        return longController;
    }

    private void SetCellOnClickAction(CmpLongButtonUiController longController, CmpCategoryBaseModel model)
    {
        CmpLongButtonTextChangeController catController = longController.gameObject.GetComponent<CmpLongButtonTextChangeController>();
        catController.SetOnClickAction(delegate { InstantiateVendorDetailsPrefab(model); });
    }

    private void InstantiateVendorDetailsPrefab(CmpCategoryBaseModel model)
    {
        var canvas = GameObject.Find("Canvas").transform;
        GameObject go = Instantiate(categoryDetailsPrefab, canvas);
        CmpCategoryDetailsScrollController detailsController = go.GetComponent<CmpCategoryDetailsScrollController>();
        detailsController.SetInfo(model);
    }
}
