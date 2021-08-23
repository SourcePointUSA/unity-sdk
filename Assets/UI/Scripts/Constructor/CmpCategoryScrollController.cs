using System.Collections.Generic;
using UnityEngine;

public class CmpCategoryScrollController : CmpScrollController
{
    [SerializeField] private CmpSwitch cmpSwitch;
    [SerializeField] private GameObject cmpDescritionCellPrefab;

    public override void FillView()
    {
        FillCategoryView();
    }

    public void FillCategoryView()
    {
        foreach (Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }
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
                List<CmpCategoryModel> legIntCategories = new List<CmpCategoryModel>();
                foreach (CmpCategoryModel model in CmpLocalizationMapper.categories)
                {
                    if (model.legIntVendors.Count > 0)
                        legIntCategories.Add(model);
                }
                AddCategories(legIntCategories);
                break;
        }
    }

    private void AddSpecialFeatures(List<CmpSpecialFeatureModel> specialFeatures)
    {
        AddDescriptionCell("SpecialFeaturesHeader", "SpecialFeaturesDefinition");
        foreach (var specialFeature in specialFeatures)
        {
            AddCell(specialFeature.name, specialFeature.description);
        }
    }

    private void AddFeatures(List<CmpFeatureModel> features)
    {
        AddDescriptionCell("FeaturesHeader", "FeaturesDefinition");
        foreach (var feature in features)
        {
            AddCell(feature.name, feature.description);
        }
    }

    private void AddSpecialPurposes(List<CmpSpecialPurposeModel> specialPurposes)
    {
        AddDescriptionCell("SpecialPurposesHeader", "SpecialPurposesDefinition");
        foreach (var spec in specialPurposes)
        {
            AddCell(spec.name, spec.description);
        }
    }

    private void AddCategories(List<CmpCategoryModel> categories)
    {
        AddDescriptionCell("PurposesHeader", "PurposesDefinition");
        foreach (var cat in categories)
        {
            AddCell(cat.name, cat.friendlyDescription);
        }
    }

    private void AddCell(string mainText, string description)
    {
        var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
        var longController = cell.GetComponent<CmpLongButtonUiController>();
        var longElement = postponedElements["CategoryButton"];
        longController.SetLocalization(longElement);
        var categoryRelatedController = cell.AddComponent<CmpCategoryLongButtonUiController>();
        categoryRelatedController.SetChangingTextRef(changingText);
        longController.SetMainText(mainText);
        categoryRelatedController.SetChangingTextString(description);
    }

    private void AddDescriptionCell(string headerId, string definitionId)
    {
        var header = postponedElements[headerId];
        var def = postponedElements[definitionId];
        var desc = Instantiate(cmpDescritionCellPrefab, scrollContent.transform);
        var descriptionController = desc.GetComponent<CmpDescriptionCellUiController>();
        descriptionController.SetLocalization(header, def);
    }
}