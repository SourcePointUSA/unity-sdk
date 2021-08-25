using System;
using UnityEngine;
using UnityEngine.UI;

public class CmpLongButtonController : MonoBehaviour
{
    private Button button;
    private Action onClickAction;
    
    public void SetButtonRef(Button button)
    {
        this.button = button;
    }
    
    public void SetOnClickAction(Action onClickAction)
    {
        this.onClickAction = onClickAction;
        this.button.onClick.AddListener(delegate { this.onClickAction?.Invoke(); });
    }
}