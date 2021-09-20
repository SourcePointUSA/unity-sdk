using Assets.UI.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CmpPopupController : MonoBehaviour
{
    [SerializeField] private string viewId;
    [SerializeField] private Image bg;
    [SerializeField] private List<CmpLocalizationUiElement> uiElements;
    [SerializeField] private List<string> postponedElementId;
    [SerializeField] CmpScrollController scrollController;
    private Dictionary<string, CmpUiElementModel> postponedElements;

    private void Awake()
    {
        postponedElements = new Dictionary<string, CmpUiElementModel>();
        StartCoroutine(WaitNetworkCoroutine());
    }

    private IEnumerator WaitNetworkCoroutine()
    {
        if (!viewId.Equals("HomeView") && !viewId.Equals("PrivacyPolicyView") && !CmpLocalizationMapper.IsExtraCallInitialized)
        {
            CmpLocalizationMapper.PrivacyManagerView();
            while (!CmpLocalizationMapper.IsExtraCallInitialized)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        while (!CmpLocalizationMapper.IsInitialized)
        {
            yield return new WaitForEndOfFrame();
        }
        SetBgColor();
        MapLocazliation();
        MapPostponedLocalization();
        FillPostponedData();
    }

    private void SetBgColor()
    {
        if(CmpLocalizationMapper.popupBgColors!=null && CmpLocalizationMapper.popupBgColors.ContainsKey(viewId))
            bg.color = GraphicExtension.HexToColor(CmpLocalizationMapper.popupBgColors[viewId]);
    }

    private void MapPostponedLocalization()
    {
        foreach (var elementId in postponedElementId)
        {
            CmpUiElementModel initializer = CmpLocalizationMapper.GetCmpUiElement(viewId, elementId);
            if (initializer != null)
            {
                postponedElements[elementId] = initializer;
            }
            else
            {
                Debug.LogError(">>>DAFUQ >:C " + elementId);
            }
        }
    }

    private void MapLocazliation()
    {
        foreach (var ui in uiElements)
        {
            CmpUiElementModel initializer = CmpLocalizationMapper.GetCmpUiElement(viewId, ui.ID);
            if (initializer != null)
            {
                if (initializer is CmpTextModel txt)
                {
                    string str = txt.Text;
                    if (str.Contains("<p>"))
                        str = str.Replace("<p>", "");
                    if (str.Contains("</p>"))
                        str = str.Replace("</p>", "");
                    txt.SetText(str);
                }
                ui.SetLocalization(initializer);
            }
            else
            {
                Debug.LogError(">>>DAFUQ >:C " + ui.ID);
            }
        }
    }

    private void FillPostponedData()
    {
        if (this.postponedElements != null && this.postponedElements.Count>0)
        {
            scrollController.SetPostponedElements(this.postponedElements);
            scrollController.FillView();
        }
        // else if (scrollController is CmpHomeScrollController home 
        //           && CmpLocalizationMapper.shortCategories != null 
        //           && CmpLocalizationMapper.shortCategories.Count>0)
        // {
        //     //if there is no postponedElements but short categories
        //     //then fill scroll with default cell 
        //     home.FillShortCategories(CmpLocalizationMapper.shortCategories);
        // }
        //else nothing to do here
    }
}
