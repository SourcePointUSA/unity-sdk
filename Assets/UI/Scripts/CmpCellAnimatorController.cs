using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.EventSystems;

public class CmpCellAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    private readonly string scaleUp = "SCALE_UP";
    private readonly string scaleDown = "SCALE_DOWN";

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(animator.TriggerAnimation(scaleUp));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(animator.TriggerAnimation(scaleDown));
    }
}