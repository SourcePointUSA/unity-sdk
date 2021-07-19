using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.UI.Scripts.Util;

public class CmpButtonAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Graphic graphic;
    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color activeTextColor;
    [SerializeField] private Animator animator;
    private readonly string scaleUp = "SCALE_UP";
    private readonly string scaleDown = "SCALE_DOWN";

    private Coroutine changeColCor = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetActiveState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetIdleState();
    }

    private void SetIdleState()
    {
        if(changeColCor!=null)
        {
            StopCoroutine(changeColCor);
        }
        changeColCor = StartCoroutine(graphic.ChangeColor(defaultTextColor));
        StartCoroutine(animator.TriggerAnimation(scaleDown));
    }

    private void SetActiveState()
    {
        if (changeColCor != null)
        {
            StopCoroutine(changeColCor);
        }
        changeColCor = StartCoroutine(graphic.ChangeColor(activeTextColor));
        StartCoroutine(animator.TriggerAnimation(scaleUp));
    }
}
