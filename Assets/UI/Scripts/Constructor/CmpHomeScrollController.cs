using System;
using System.Collections.Generic;
using UnityEditor;

public class CmpHomeScrollController : CmpScrollController
{
    public static bool isShown = false;
    public static bool isDestroyed = false;
    
    private new void Start()
    {
        base.Start();
        isShown = true;
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }

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