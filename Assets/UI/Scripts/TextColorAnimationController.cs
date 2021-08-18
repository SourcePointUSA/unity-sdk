using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

public class TextColorAnimationController : MonoBehaviour
{
    [SerializeField] private Graphic graphic;
    [SerializeField] public Color defaultTextColor;
    [SerializeField] public Color activeTextColor;

    private Coroutine changeColCor = null;

    public virtual void SetIdleState(Color defaultTextColor)
    {
        if (changeColCor != null)
        {
            StopCoroutine(changeColCor);
        }
        changeColCor = StartCoroutine(graphic.ChangeColor(defaultTextColor));
    }

    public virtual void SetActiveState(Color activeTextColor)
    {
        if (changeColCor != null)
        {
            StopCoroutine(changeColCor);
        }
        changeColCor = StartCoroutine(graphic.ChangeColor(activeTextColor));
    }

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