using System.Collections.Generic;

public class CmpHomeScrollController : CmpScrollController
{
    public override void FillView()
    {
        ClearScrollContent();
        foreach (var cat in CmpLocalizationMapper.shortCategories)
        {
            var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
            CmpLongButtonUiController longButtonController = cell.GetComponent<CmpLongButtonUiController>();
            var longElement = postponedElements["CategoryButtons"];
            longButtonController.SetLocalization(longElement);
            longButtonController.SetMainText(cat.name);
        }
        ScrollAppear(-800f);
    }

    public void FillShortCategories(List<CmpShortCategoryModel> shortCategories)
    {
        ClearScrollContent();
        foreach (var cat in shortCategories)
        {
            var cell = Instantiate(cmpCellPrefab, scrollContent.transform);
            CmpLongButtonUiController longButtonController = cell.GetComponent<CmpLongButtonUiController>();
            longButtonController.SetMainText(cat.name);
        }
        ScrollAppear(0);
    }
}