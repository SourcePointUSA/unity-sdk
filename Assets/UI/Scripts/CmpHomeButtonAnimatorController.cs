using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmpHomeButtonAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private Text text;
    private readonly string scaleUp = "SCALE_UP";
    private readonly string scaleDown = "SCALE_DOWN";

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.fontStyle = FontStyle.Bold;
        StartCoroutine(animator.TriggerAnimation(scaleUp));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.fontStyle = FontStyle.Normal;
        StartCoroutine(animator.TriggerAnimation(scaleDown));
    }
}