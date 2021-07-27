using Assets.UI.Scripts.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmpHomeButtonAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private Text buttonText;
    [SerializeField] private Text changingText;
    [SerializeField] private TextColorAnimationController changingTextController;
    private readonly string scaleUp = "SCALE_UP";
    private readonly string scaleDown = "SCALE_DOWN";

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.fontStyle = FontStyle.Bold;
        StartCoroutine(animator.TriggerAnimation(scaleUp));
        changingText.text = "Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you.";
        changingTextController.SetActiveState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.fontStyle = FontStyle.Normal;
        StartCoroutine(animator.TriggerAnimation(scaleDown));
        changingTextController.SetIdleState();
        changingText.text = "";
    }
}