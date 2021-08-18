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
    private Dictionary<string, CmpUiElementModel> postponedElements;

    private void Awake()
    {
        postponedElements = new Dictionary<string, CmpUiElementModel>();
        StartCoroutine(WaitNetworkCoroutine());
    }

    private IEnumerator WaitNetworkCoroutine()
    {
        while (!CmpLocalizationMapper.IsInitialized)
        {
            yield return new WaitForEndOfFrame();
        }
        SetBgColor();
        MapLocazliation();
        MapPostponedLocalization();
    }

    private void SetBgColor()
    {
        bg.color = GraphicExtension.HexToColor(NativeUiJsonDeserializer.popupBgColors[viewId]);
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
                ui.SetLocalization(initializer);
            }
            else
            {
                Debug.LogError(">>>DAFUQ >:C " + ui.ID);
            }
        }
    }
}