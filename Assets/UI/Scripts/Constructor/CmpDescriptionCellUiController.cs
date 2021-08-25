using UnityEngine;

public class CmpDescriptionCellUiController : MonoBehaviour
{
    [SerializeField] CmpTextUiController header;
    [SerializeField] CmpTextUiController description;

    public void SetLocalization(CmpUiElementModel headerElement, CmpUiElementModel descElement)
    {
        header.SetLocalization(headerElement);
        if(description.isActiveAndEnabled)
            description.SetLocalization(descElement);
    }
}