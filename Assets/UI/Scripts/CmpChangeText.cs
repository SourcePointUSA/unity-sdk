using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CmpChangeText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text buttonText;
    [SerializeField] private Text changingText;
    [SerializeField] private TextColorAnimationController changingTextController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.fontStyle = FontStyle.Bold;
        changingText.text = "Cookies, device identifiers, or other information can be stored or accessed on your device for the purposes presented to you.";
        changingTextController.SetActiveState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.fontStyle = FontStyle.Normal;
        changingTextController.SetIdleState();
        changingText.text = "";
    }
}