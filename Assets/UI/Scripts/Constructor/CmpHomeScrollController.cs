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
        ScrollAppear();
    }
}