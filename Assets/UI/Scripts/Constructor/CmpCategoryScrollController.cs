using System.Collections.Generic;
using UnityEngine;

public class CmpCategoryScrollController : CmpScrollController
{
    [SerializeField] private CmpSwitch cmpSwitch;
    [SerializeField] private GameObject cmpDescritionCellPrefab;
    [SerializeField] private GameObject categoryDetailsPrefab;

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
                AddCategories(CmpLocalizationMapper.categories); // == Pusposes
                AddSpecialPurposes(CmpLocalizationMapper.specialPurposes);
                AddFeatures(CmpLocalizationMapper.features);
                AddSpecialFeatures(CmpLocalizationMapper.specialFeatures);
                RectTransform scrollRect = ((RectTransform)scrollContent.transform);
                scrollRect.SetPositionAndRotation(new Vector3(scrollRect.position.x, -400f, scrollRect.position.z), scrollRect.rotation);
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
        AddDescriptionCell("SpecialFeaturesHeader", "SpecialFeaturesDefinition");
        foreach (var specialFeature in specialFeatures)
        {
            var longController = AddCell(specialFeature.name, specialFeature.description);
            SetCellOnClickAction(longController, specialFeature);
        }
    }

    private void AddFeatures(List<CmpFeatureModel> features)
    {
        AddDescriptionCell("FeaturesHeader", "FeaturesDefinition");
        foreach (var feature in features)
        {
            var longController = AddCell(feature.name, feature.description);
            SetCellOnClickAction(longController, feature);
        }
    }

    private void AddSpecialPurposes(List<CmpSpecialPurposeModel> specialPurposes)
    {
        AddDescriptionCell("SpecialPurposesHeader", "SpecialPurposesDefinition");
        foreach (var spec in specialPurposes)
        {
            var longController = AddCell(spec.name, spec.description);
            longController.DisableOnOffGroup();
            //SetCellOnClickAction(longController, spec);
        }
    }

    private void AddCategories(List<CmpCategoryModel> categories)
    {
        AddDescriptionCell("PurposesHeader", "PurposesDefinition");
        foreach (CmpCategoryModel cat in categories)
        {
            var longController = AddCell(cat.name, cat.friendlyDescription);
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

        var categoryRelatedController = cell.AddComponent<CmpCategoryLongButtonUiController>();
        categoryRelatedController.SetChangingTextRef(changingText);
        categoryRelatedController.SetChangingTextString(description);
        categoryRelatedController.SetButtonRef(longController.GetButton());
        return longController;
    }

    private void AddDescriptionCell(string headerId, string definitionId)
    {
        var header = postponedElements[headerId];
        var def = postponedElements[definitionId];
        var desc = Instantiate(cmpDescritionCellPrefab, scrollContent.transform);
        var descriptionController = desc.GetComponent<CmpDescriptionCellUiController>();
        descriptionController.SetLocalization(header, def);
    }

    private void SetCellOnClickAction(CmpLongButtonUiController longController, CmpCategoryBaseModel model)
    {
        CmpCategoryLongButtonUiController catController = longController.gameObject.GetComponent<CmpCategoryLongButtonUiController>();
        catController.SetOnClickAction(delegate { InstantiateCategoriesDetailsPrefab(model); });
    }

    private void InstantiateCategoriesDetailsPrefab(CmpCategoryBaseModel model)
    {
        var canvas = GameObject.Find("Canvas").transform;
        GameObject go = Instantiate(categoryDetailsPrefab, canvas);
        CmpCategoryDetailsScrollController detailsController = go.GetComponent<CmpCategoryDetailsScrollController>();
        detailsController.SetInfo(model);
    }
}
