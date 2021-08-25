using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmpCategoryLongButtonUiController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text changingText;
    private string onFocusString;
    private Button button;
    private Action onClickAction;

    public void SetChangingTextString(string text)
    {
        onFocusString = text;
    }

    public void SetChangingTextRef(Text changingText)
    {
        this.changingText = changingText;
    }

    public void SetButtonRef(Button button)
    {
        this.button = button;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        changingText.text = onFocusString;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        changingText.text = "";
    }

    public void SetOnClickAction(Action onClickAction)
    {
        this.onClickAction = onClickAction;
        this.button.onClick.AddListener(delegate { this.onClickAction?.Invoke(); });
    }
}