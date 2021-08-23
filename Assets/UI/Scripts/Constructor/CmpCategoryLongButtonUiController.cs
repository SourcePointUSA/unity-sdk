using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmpCategoryLongButtonUiController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text changingText;
    private string onFocusString;

    public void SetChangingTextString(string text)
    {
        onFocusString = text;
    }

    public void SetChangingTextRef(Text changingText)
    {
        this.changingText = changingText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        changingText.text = onFocusString;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        changingText.text = "";
    }
}