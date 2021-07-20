using Assets.UI.Scripts.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CmpCellAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<TextColorAnimationController> graphicsList;

    private readonly string scaleUp = "SCALE_UP";
    private readonly string scaleDown = "SCALE_DOWN";

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(animator.TriggerAnimation(scaleUp));
        SetActive();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(animator.TriggerAnimation(scaleDown));
        SetIdle();
    }

    private void SetIdle()
    {
        foreach(TextColorAnimationController graphic in graphicsList)
        {
            if(graphic.isActiveAndEnabled)
                graphic.SetIdleState();
        }
    }

    private void SetActive()
    {
        foreach (TextColorAnimationController graphic in graphicsList)
        {
            if (graphic.isActiveAndEnabled)
                graphic.SetActiveState();
        }
    }
}