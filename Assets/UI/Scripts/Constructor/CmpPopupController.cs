using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmpPopupController : MonoBehaviour
{
    [SerializeField] private string viewId;
    [SerializeField] private List<CmpLocalizationUiElement> uiElements;

    private void Awake()
    {
        StartCoroutine(WaitNetworkCoroutine());
    }

    private IEnumerator WaitNetworkCoroutine()
    {
        while (!CmpLocalizationMapper.IsInitialized)
        {
            yield return new WaitForEndOfFrame();
        }
        MapLocazliation();
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
                Debug.LogError(">>>DAFUQ >:C");
            }
        }
    }
}