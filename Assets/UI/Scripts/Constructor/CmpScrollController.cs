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

    protected void ClearScrollContent()
    {
        if(scrollContent!=null)
            foreach (Transform child in scrollContent.transform)
                Destroy(child.gameObject);
    }

    protected void AddHeaderCell(GameObject cmpHeaderCellPrefab, string headerId)
    {
        var header = postponedElements[headerId];
        var desc = Instantiate(cmpHeaderCellPrefab, scrollContent.transform);
        var descriptionController = desc.GetComponent<CmpDescriptionCellUiController>();
        descriptionController.SetLocalization(header, null);
    }

    protected void AddDescriptionCell(GameObject cmpDescriptionCellPrefab, string headerId, string definitionId)
    {
        var header = postponedElements[headerId];
        var def = postponedElements[definitionId];
        var desc = Instantiate(cmpDescriptionCellPrefab, scrollContent.transform);
        var descriptionController = desc.GetComponent<CmpDescriptionCellUiController>();
        descriptionController.SetLocalization(header, def);
    }

    public virtual void FillView()
    {

    }
}
