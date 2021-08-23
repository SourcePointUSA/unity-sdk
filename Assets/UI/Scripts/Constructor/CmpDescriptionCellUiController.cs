using UnityEngine;

public class CmpDescriptionCellUiController : MonoBehaviour
{
    [SerializeField] CmpTextUiController header;
    [SerializeField] CmpTextUiController description;

    public void SetLocalization(CmpUiElementModel headerElement, CmpUiElementModel descElement)
    {
        header.SetLocalization(headerElement);
        description.SetLocalization(descElement);
    }
}