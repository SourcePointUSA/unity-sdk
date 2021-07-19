using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class TextColorAnimationController : MonoBehaviour
{
    [SerializeField] private Graphic graphic;
    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color activeTextColor;
    
    private Coroutine changeColCor = null;

    public virtual void SetIdleState()
    {
        if (changeColCor != null)
        {
            StopCoroutine(changeColCor);
        }
        changeColCor = StartCoroutine(graphic.ChangeColor(defaultTextColor));
    }

    public virtual void SetActiveState()
    {
        if (changeColCor != null)
        {
            StopCoroutine(changeColCor);
        }
        changeColCor = StartCoroutine(graphic.ChangeColor(activeTextColor));
    }
}