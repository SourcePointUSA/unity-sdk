using UnityEngine;
using UnityEngine.EventSystems;
using Assets.UI.Scripts.Util;
using UnityEngine.UI;

public class CmpButtonAnimatorController : TextColorAnimationController, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    private readonly string scaleUp = "SCALE_UP";
    private readonly string scaleDown = "SCALE_DOWN";

    public void OnPointerEnter(PointerEventData eventData)
    {
        base.SetActiveState();
        StartCoroutine(animator.TriggerAnimation(scaleUp));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        base.SetIdleState();
        StartCoroutine(animator.TriggerAnimation(scaleDown));
    }
}
