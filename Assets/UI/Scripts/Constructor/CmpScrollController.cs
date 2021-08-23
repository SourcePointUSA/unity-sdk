using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CmpScrollController : MonoBehaviour
{
    [SerializeField] protected GameObject cmpCellPrefab;
    [SerializeField] protected GameObject scrollContent;
    [SerializeField] protected Text changingText;

    protected Dictionary<string, CmpUiElementModel> postponedElements;

    public void SetPostponedElements(Dictionary<string, CmpUiElementModel> postponedElements)
    {
        this.postponedElements = postponedElements;
    }

    public virtual void FillView()
    {

    }
}
